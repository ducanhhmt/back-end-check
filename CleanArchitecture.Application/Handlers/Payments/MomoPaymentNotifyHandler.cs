using CleanArchitecture.Application.Features.Payments;
using CleanArchitecture.Application.Repository_Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Handlers.Payments
{
    public class MomoPaymentNotifyHandler : IRequestHandler<MomoPaymentNotifyCommand, bool>
    {
        private readonly IUserBillRepository _billRepo;

        public MomoPaymentNotifyHandler(IUserBillRepository billRepo)
        {
            _billRepo = billRepo;
        }
        public async Task<bool> Handle(MomoPaymentNotifyCommand request, CancellationToken ct)
        {
            // Chỉ xử lý khi thanh toán thành công
            if (request.ResultCode != 0)
                return false;

            // Cắt billId sau dấu # trong OrderInfo
            // VD: "Khách hàng: ABC. Nội dung: Thanh toán đơn hàng #9eb7eae4-ff93-..."
            var billIdStr = request.OrderInfo.Split('#').LastOrDefault()?.Trim();

            if (string.IsNullOrEmpty(billIdStr) || !Guid.TryParse(billIdStr, out var billId))
            {
                Console.WriteLine($"[MomoNotify] Không parse được BillId từ OrderInfo: {request.OrderInfo}");
                return false;
            }
            var bill = await _billRepo.GetById(billId);
            if (bill == null)
            {
                Console.WriteLine($"[MomoNotify] Không tìm thấy bill: {billId}");
                return false;
            }

            // Cập nhật trạng thái bill → 0 = đang xử lý (đã thanh toán thành công)
            bill.UpdateState(0);
            await _billRepo.Update(bill);
            // Invalidate cache detail + list state=4 (chờ thanh toán) page 1
            // (invalidate rộng hơn nếu cần — xóa tất cả page của state 4)
            await _billRepo.InvalidateUserBilllCache(billId);
            await _billRepo.InvalidateUserBillListCache(bill.UserId, 4);
            await _billRepo.InvalidateUserBillListCache(bill.UserId, 0);
            Console.WriteLine($"[MomoNotify] Cập nhật bill {billId} thành công - TransId: {request.TransId}");
            return true;
        }
    }
}
