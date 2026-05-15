using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Users;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Handlers.CategoryHandlers
{
    public class GetAllCategoryHandlers : IRequestHandler<GetAllCategoryQueries, List<Category>>
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public GetAllCategoryHandlers(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public Task<List<Category>> Handle(GetAllCategoryQueries request, CancellationToken ct)
        {
            var categories = _repository.GetAllCategories();
            return Task.FromResult(categories);
        }
    }
}

