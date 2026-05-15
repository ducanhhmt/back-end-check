using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Users
{
    public class ChangeUserPasswordCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string currentPassword { get; set; }
        public string newPassword { get; set; }
    }
}
