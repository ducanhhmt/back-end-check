using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.Bill;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Repository_Interface;
using MediatR;

namespace CleanArchitecture.Application.Handlers.BillHandlers
{
    public class GetAllUserBillInfoHandlers : IRequestHandler<GetAllUserBillInfoQueries, UserBillRespone>
    {
        private readonly IProductRepository _productRepository;
        private readonly IBillInfoRepository _billInfoRepository;
        private readonly IUserBillRepository _userBillRepository;
        private readonly IMapper _mapper;

        public GetAllUserBillInfoHandlers(IProductRepository productRepository, IBillInfoRepository billInfoRepository,
            IUserBillRepository userBillRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _billInfoRepository = billInfoRepository;
            _userBillRepository = userBillRepository;
            _mapper = mapper;
        }

        public async Task<UserBillRespone> Handle(GetAllUserBillInfoQueries request, CancellationToken ct)
        {
            var state = request.state ?? -1;  // -1 = tất cả state
            var cacheKey = $"userBill:{request.userId}:state:{state}:page:{request.pageIndex}:";
            // ── 1. Đọc cache MongoDB ──────────────────────────────
            var cached = await _userBillRepository.GetCachedUserBillListingPerState(request.userId!.Value, state, request.pageIndex);
            if (cached.Count > 0)
            {
                // Cache hit → tính pageCount từ cache (tổng bill / pagesize)
                // pageCount được lưu trong TotalPages (JsonIgnore nên không expose ra FE)
                var totalPages = cached.FirstOrDefault()?.TotalPages ?? 1;
                return new UserBillRespone { items = cached, pageCount = totalPages };
            }

            // ── 2. Cache miss → query SQL ─────────────────────────
            var userBills = await _userBillRepository.GetByAccount(request.userId!.Value);
            if (userBills == null || !userBills.Any())
                return new UserBillRespone();
            // Filter state
            if (request.state != null)
                userBills = userBills.Where(s => s.State == request.state).ToList();
            // Tính pageCount trước khi paging
            var pageCount = (int)Math.Ceiling(userBills.Count / (double)request.pagesize);
            // Paging
            var pagedBills = userBills
                .OrderByDescending(x => x.DateCreated)
                .Skip((request.pageIndex - 1) * request.pagesize)
                .Take(request.pagesize)
                .ToList();
            // Map
            var result = _mapper.Map<List<UserBillModel>>(pagedBills);
            // Load item preview cho từng bill
            foreach (var bill in result)
            {
                var items = await _billInfoRepository.GetById(bill.Id);
                bill.Items = new ProductPreviewItems
                {
                    Name = items.FirstOrDefault()?.Name ?? "",
                    Thumbnail = items.FirstOrDefault()?.Thumbnail ?? ""
                };
                bill.itemsCount = items.Count();
                bill.TotalPages = pageCount;  // lưu vào TotalPages để cache biết
            }
            // ── 3. Write cache MongoDB ────────────────────────────
            await _userBillRepository.WriteUserBillPerStateListCache(cacheKey, result);
            return new UserBillRespone { items = result, pageCount = pageCount };
        }
    }
}
