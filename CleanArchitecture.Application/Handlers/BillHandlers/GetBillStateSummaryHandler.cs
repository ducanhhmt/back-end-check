using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.Bill;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Repository_Interface;
using MediatR;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace CleanArchitecture.Application.Handlers.BillHandlers
{
    public class GetBillStateSummaryHandler : IRequestHandler<GetBillStateSummaryQueries, BillStateSummary>
    {
        private readonly IBillInfoRepository _billInfoRepository;
        private readonly IUserBillRepository _userBillRepository;
        private readonly IMapper _mapper;

        public GetBillStateSummaryHandler( IBillInfoRepository billInfoRepository,
            IUserBillRepository userBillRepository, IMapper mapper)
        {
            _billInfoRepository = billInfoRepository;
            _userBillRepository = userBillRepository;
            _mapper = mapper;
        }

        public async Task<BillStateSummary> Handle(GetBillStateSummaryQueries request, CancellationToken ct)
        {
            // 1. Lấy toàn bộ bill theo account
            var userBills = await _userBillRepository.GetByAccount(request.userId);
            if (userBills == null && userBills.Count == 0)
            {
                return new BillStateSummary();
            }
            // 2. Sắp xếp theo state và count
            var userBillsPerState = userBills.GroupBy(u => u.State).Select(g => new { State = g.Key, Count = g.Count() });
            // 3. Lấy cặp key-value dictionary theo state và count
            var dict = userBillsPerState.ToDictionary(x => x.State, x => x.Count);
            return new BillStateSummary
            {
                All = dict.Values.Sum(),
                PaymentWaiting = dict.GetValueOrDefault(4,0),
                Processing = dict.GetValueOrDefault(0, 0),
                Shipping = dict.GetValueOrDefault(1, 0),
                Completed = dict.GetValueOrDefault(2, 0),
                Cancelled = dict.GetValueOrDefault(3, 0)
            };
        }
    }
}
