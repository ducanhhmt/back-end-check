using CleanArchitecture.Application.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.News
{
    public class AddNewsCommand : IRequest<bool> 
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
        public string UserCreated { get; set; }
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
    }
}
