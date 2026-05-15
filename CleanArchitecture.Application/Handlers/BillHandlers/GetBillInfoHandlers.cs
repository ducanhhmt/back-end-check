using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.Bill;
using CleanArchitecture.Application.Features.Carts;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Products;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Handlers.BillHandlers
{
    public class GetBillInfoHandlers : IRequestHandler<GetBillInfoQueries, UserBillDTO>
    {
        private readonly IProductRepository _productRepository;
        private readonly IBillInfoRepository _billInfoRepository;
        private readonly IUserBillRepository _userBillRepository;
        private readonly IMapper _mapper;

        public GetBillInfoHandlers(IProductRepository productRepository, IBillInfoRepository billInfoRepository,
            IUserBillRepository userBillRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _billInfoRepository = billInfoRepository;
            _userBillRepository = userBillRepository;
            _mapper = mapper;
        }

        public async Task<UserBillDTO> Handle(GetBillInfoQueries request, CancellationToken ct)
        {
            var bill = _mapper.Map<UserBillDTO>(await _userBillRepository.GetById(request.BillId));
            bill.Items = await _billInfoRepository.GetById(request.BillId);
            return bill;
        }
    }
}
