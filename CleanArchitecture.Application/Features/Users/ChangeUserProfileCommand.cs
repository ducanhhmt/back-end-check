using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Users
{
    public class ChangeUserProfileCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; } = "";
        public string phone { get; set; }
        public bool gender { get; set; }
        public DateTime birthday { get; set; }
    }
}
