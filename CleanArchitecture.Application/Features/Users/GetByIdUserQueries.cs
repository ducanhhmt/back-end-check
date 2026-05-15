using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Users
{
    public class GetByIdUserQueries : IRequest<UserLoginModel>
    {
        public Guid UserId { get; set; }
    }
}
