using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Model
{
    public class UserCartModel
    {
        public int Id { get; set; }
        public Guid ProductId { get; set; }
        public string? Name { get; set; }
        public int Price { get; set; }
        public int PublisherPrice { get; set; }
        public int CartQuantity { get; set; } = 1;
        public string Image { get; set; }
        public bool SoldOut { get; set; } = false;
        public bool LowStock { get; set; } = false; 
        public int StockQuantity { get; set; } = 1;
        public bool Selected { get; set; } = false;
    }
}
