using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Entities
{
    public class Cart
    {
        public int Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid ProductId { get; private set; }
        public string? Name { get; private set; }
        public int Price { get; private set; }
        public int PublisherPrice { get; private set; }
        public int Quantity { get; private set; } = 1;
        public string Image { get; private set; }
        protected Cart() { }
        public Cart(Guid productId ,Guid userId, string? name, int price, int publisherPrice, int Quantity, string img)
        {
            UpdateInfo(productId, userId, name, price, publisherPrice, Quantity, img);           
        }

        public void UpdateInfo(Guid productId, Guid userId, string? name, int price, int publisherPrice, int quantity, string img)
        {
            if (string.IsNullOrWhiteSpace(productId.ToString()))
                throw new Exception("Không có Id sản phẩm");
            if (string.IsNullOrWhiteSpace(userId.ToString()))
                throw new Exception("Không có Id người mua");
            if (Quantity == 0)
                throw new Exception("Không có số lượng");
            ProductId = productId;
            UserId = userId;
            Name = name;
            Price = price;
            PublisherPrice = publisherPrice;
            Quantity = quantity;
            Image = img;
        }
    }  
}
