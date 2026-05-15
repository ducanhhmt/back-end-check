using CleanArchitecture.Application.Cache;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Infrastructure.Consumers;
using CleanArchitecture.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            #region nếu không dùng mediatR thì sẽ cần phải auto dependency repository
            //services.Scan(scan => scan
            //    .FromAssemblies(typeof(DependencyInjection).Assembly)
            //    .AddClasses(classes => classes.Where(type =>
            //        type.Name.EndsWith("Repository") || type.Name.EndsWith("Service")))
            //    .AsImplementedInterfaces()
            //    .WithScopedLifetime()
            //);
            #endregion

            #region Đăng kí server Redis
            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = configuration["Redis:ConnectionString"];
            //});
            //builder.Services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
            //    options.InstanceName = "ProductApi_";
            //});

            //builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            //    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379"));
            #endregion

            #region đăng kí nhận xử lý queue RabbitMQ , nếu có nhiều consumer nhận xử lý sẽ khai báo ở đây cho gọn programs
            //services.AddHostedService<UserCreateConsumer>();
            #endregion
            return services;
        }
    }

}
