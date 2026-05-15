using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Users
{
    public class UpdateUserAddressCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string Address { get; set; }
    }
}
