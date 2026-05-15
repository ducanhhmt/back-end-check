using CleanArchitecture.Application.DTO;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.News
{
    public class GetMenuCategoryQueries : IRequest<List<MenuItem>> {}
}
