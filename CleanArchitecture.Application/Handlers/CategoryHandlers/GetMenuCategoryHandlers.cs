using AutoMapper;
using CleanArchitecture.Application.Cache;
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
    public class GetMenuCategoryHandlers : IRequestHandler<GetMenuCategoryQueries, List<MenuItem>>
    {
        private readonly ICategoryRepository _repository;
        private readonly IMemCacheData _cacheData;
        private string _categoryMenu = "CategoryMenu";

        public GetMenuCategoryHandlers(ICategoryRepository repository, IMemCacheData cacheData)
        {
            _repository = repository;
            _cacheData = cacheData;
        }

        public async Task<List<MenuItem>> Handle(GetMenuCategoryQueries request, CancellationToken ct)
        {
            var cached = _cacheData.Get<List<MenuItem>>(_categoryMenu);
            if (cached != null && cached.Count > 0)
            {
                return cached;
            }
            var categories = await Task.FromResult(_repository.GetAllCategories());
            var result = await TransformToMenuStructure(categories);
            _cacheData.Set(_categoryMenu, result,TimeSpan.FromDays(30));
            return  result;
        }

        // Hàm chính
        private async Task<List<MenuItem>> TransformToMenuStructure(List<Category> categories)
        {
            if (categories == null || !categories.Any())
                return new List<MenuItem>();
            var lookup = categories.ToLookup(c => c.Parent_ID);
            var result = new List<MenuItem>();
            // Lấy các danh mục cha (Parent_ID == 0)
            var rootCategories = categories
                .Where(c => c.Parent_ID == null)
                .OrderBy(c => c.Id)
                .ToList();
            foreach (var root in rootCategories)
            {
                var menuItem = new MenuItem
                {
                    Id = root.Id,
                    Name = root.Name?.Trim() ?? string.Empty,
                };

                var children = lookup[root.Id]
                    .OrderBy(c => c.Id)
                    .ToList();

                if (children.Any())
                {
                    // Mỗi category con = 1 cột trong mega menu (giống hình ảnh bạn gửi)
                    foreach (var child in children)
                    {
                        menuItem.Groups.Add(new MenuGroup
                        {
                            Title = child.Name?.Trim().ToUpper() ?? string.Empty,
                            Items = lookup[child.Id].OrderBy(c => c.Id).Select(c => c.Name).ToList(),
                        });
                    }
                }
                result.Add(menuItem);
            }
            return result;
        }
    }
}

