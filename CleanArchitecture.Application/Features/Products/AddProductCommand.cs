using CleanArchitecture.Application.DTO;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Products
{
    public class AddProductCommand : IRequest<Product>
    {
        public string Name { get; set; }
        public string Series { get; set; }
        public int NxbId { get; set; }
        public int CategoriesId { get; set; }
        public int Weight {get; set; }
        public int ImportPrice { get; set; }
        public int Price { get; set; }
        public int PublisherPrice { get; set; }
        public int Quantity { get; set; }
        public int Discount { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.Now;
        public string? Description { get; set; }
        public string? ThumbnailIMG { get; set; }
    }
}
