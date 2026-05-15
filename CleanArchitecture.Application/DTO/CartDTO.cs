using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.DTO
{
    public class CartDTO
    {
        public int Id { get; set; }
        public Guid ProductId { get;  set; }
        public string? Name { get;  set; }
        public int Price { get;  set; }
        public int PublisherPrice { get; set; }
        public int Quantity { get;  set; } = 1;
        public string Image { get;  set; }
        public bool selected { get;  set; } = false;
    }
}
