using CleanArchitecture.Application.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.News
{
    public class DeleteNewsCommand : IRequest<bool> 
    {
        public string Id { get; set; } = new Guid().ToString();
    }
}
