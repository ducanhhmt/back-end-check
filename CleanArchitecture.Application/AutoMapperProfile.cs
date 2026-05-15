using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Products;
using CleanArchitecture.Application.Features.Purchase;
using CleanArchitecture.Application.Features.Users;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Đăng ký global converter (dùng cho mọi nơi cần)
            var stringToListConverter = new StringToListConverter(';');

            CreateMap<News, NewsDTO>().ReverseMap();
            CreateMap<AddNewsCommand, News>().ReverseMap();
            CreateMap<UpdateNewsCommand, News>().ReverseMap();

            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<UserDto, UserLoginModel>().ReverseMap();
            CreateMap<AddUserCommand, User>().ReverseMap();

            CreateMap<Product, ProductDTO>()
            .ForMember(dest => dest.ThumbnailIMG,
                opt => opt.ConvertUsing(new StringToListConverter(','), src => src.ThumbnailIMG))
            .ReverseMap();
            CreateMap<AddProductCommand, Product>().ReverseMap();
            CreateMap<UpdateProductCommand, Product>().ReverseMap();
            CreateMap<Product, ProductUserDetailDto>().ReverseMap();
            CreateMap<Product,ProductAdminDetailDto>().ReverseMap();
            CreateMap<Cart, CartDTO>().ReverseMap();


            CreateMap<UserBillDTO,UserBill>().ReverseMap();
            CreateMap<UserBillModel, UserBill>().ReverseMap();

            CreateMap<AddNewPurchaseCommand, Purchase>().ReverseMap();
            CreateMap<Purchase, PurchaseDetailModel>().ReverseMap();          
        }
    }

    public class StringToListConverter : IValueConverter<string?, List<string>>
    {
        private readonly char _separator;

        public StringToListConverter(char separator = ';')
        {
            _separator = separator;
        }

        public List<string> Convert(string? source, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source))
                return new List<string>();

            return source.Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                         .Select(x => x.Trim())
                         .Where(x => !string.IsNullOrEmpty(x))
                         .ToList();
        }
    }
}
