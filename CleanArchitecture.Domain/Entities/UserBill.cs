using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Entities
{
    public class UserBill
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string UserName { get; private set; }
        public string Address { get; private set; }
        public string Phone { get; private set; }
        public int TotalPrice { get; private set; }
        public int State { get; private set; }
        public int ShippingPrice { get; private set; }
        public int DiscountPrice { get; private set; }
        public DateTime DateCreated { get; private set; } = DateTime.Now;
        protected UserBill() { }
        public UserBill(Guid userId, string userName, string Address, string Phone, int TotalPrice, int State, int ShippingPrice, int DiscountPrice, DateTime DateCreated)
        {
            UpdateInfo(userId, userName, Address, Phone, TotalPrice, State, ShippingPrice, DiscountPrice, DateCreated);
        }

        public void UpdateInfo(Guid userId, string userName, string address, string phone, int totalPrice, int state, int shippingPrice, int discountPrice, DateTime dateCreated)
        {
            if (string.IsNullOrWhiteSpace(address.ToString()))
                throw new Exception("Không có địa chỉ giao hàng");
            if (string.IsNullOrWhiteSpace(phone.ToString()))
                throw new Exception("Không có địa chỉ giao hàng");
            UserId = userId;
            UserName = userName;
            Address = address;
            Phone = phone;
            TotalPrice = totalPrice;
            State = state;
            ShippingPrice = shippingPrice;
            DiscountPrice = discountPrice;
            DateCreated = dateCreated;
        }

        public void UpdateState(int state)
        {
            State = state;
        }
    }
}
