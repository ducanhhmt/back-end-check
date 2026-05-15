using Amazon.Auth.AccessControlPolicy;
using CleanArchitecture.Application;
using CleanArchitecture.Application.Cache;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Handlers.UserHandlers;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Application.Services_Interface;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Consumers;
using CleanArchitecture.Infrastructure.HubRealTime;
using CleanArchitecture.Infrastructure.RabbitMQ;
//using CleanArchitecture.Infrastructure.RabbitMQEvent.UserEventHandler;
using CleanArchitecture.Infrastructure.Repository;
using MassTransit;
using MassTransit.MultiBus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Text;
using static MassTransit.Monitoring.Performance.BuiltInCounters;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
using CleanArchitecture.Infrastructure.Services;
using CleanArchitecture.Application.Model.PaymentModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var mongoConnection = builder.Configuration.GetConnectionString("MongoDb") ?? "mongodb://localhost:27017";
builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoConnection));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase("ProductDb"));
BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard));

builder.Services.Scan(scan => scan
    .FromAssemblyOf<NewsRepository>()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
        .AsImplementedInterfaces()
        .WithScopedLifetime());

//builder.Services.AddMediatR(cfg =>
//{
//    cfg.RegisterServicesFromAssemblies(
//        typeof(GetAllNewsQueries).Assembly    // Application
//        typeof(UserCreatedIntegrationEventHandler).Assembly  // Infrastructure
//    );
//});

// Momo API Payment (copy từ doc)
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
builder.Services.AddHttpClient<MomoService>();
builder.Services.AddScoped<IMomoService, MomoService>();
// ZaloPay Payment Config
builder.Services.Configure<ZaloPayModel>(builder.Configuration.GetSection("ZaloPay"));
builder.Services.AddHttpClient<ZaloPayService>();
builder.Services.AddScoped<IZaloPayService, ZaloPayService>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllNewsQueries).Assembly));
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IMemCacheData, CacheData>();
builder.Services.AddScoped<ISharedCacheData, MongoCacheService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
//SignalR
builder.Services.AddSignalR();
builder.Services.AddScoped<IMessageBus, RabbitMqService>();
//RabbitMQ
builder.Services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
builder.Services.AddSingleton<IRabbitMqQueueInitializer, RabbitMqQueueInitializer>();
//builder.Services.AddHostedService<UserCreateConsumer>();
builder.Services.AddHostedService<SearchNewsConsumer>();
//RABBITMQ MASSTRANSIT NẾU SỬ DỤNG
//builder.Services.AddMassTransit(x =>
//{
//    x.AddConsumer<ActivateUserConsumer>();
//    x.AddConsumer<DeleteUserConsumer>();
//    x.UsingRabbitMq((context, cfg) =>
//    {
//        cfg.Host("localhost", "/", h =>
//        {
//            h.Username("guest");
//            h.Password("guest");
//        });
//        cfg.UseDelayedMessageScheduler();
//        // TỰ ĐỘNG PURGE (xóa message cũ) khi khởi động – khuyến nghị
//        cfg.ReceiveEndpoint(e =>
//        {
//            e.PurgeOnStartup = true; // Xóa hết message cũ trong queue
//        });
//        cfg.ConfigureEndpoints(context);
//    });
//});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50MB tổng
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024;
});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below. Example: Bearer abc123"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", corBuilder =>
{
    //corBuilder.AllowAnyOrigin()
    //          .AllowAnyMethod()
    //          .AllowAnyHeader();
    corBuilder.WithOrigins("http://localhost:4200") // 👈 fix ở đây
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // 👈 bắt buộc
}));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
        ValidateIssuerSigningKey = true
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("MyPolicy");
app.UseHttpsRedirection();
app.UseAuthorization();
// ── 3. Tạo thư mục upload + serve static files không cần wwwroot ───────
var rootFolder = builder.Configuration["UploadSettings:RootFolder"]!;

var uploadRootPath = Path.Combine(
    builder.Environment.ContentRootPath,
    rootFolder);

Directory.CreateDirectory(uploadRootPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadRootPath),
    RequestPath = "/files"
});
app.MapControllers().RequireAuthorization();
app.MapHub<NewsHub>("/newsHub");

app.Run();
