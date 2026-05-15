using CleanArchitecture.Application.Services_Interface;
using CleanArchitecture.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure
{

    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;
        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> Generate(string id, string username, string role)
        {
            var key = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            if (string.IsNullOrEmpty(key))
                return "";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expireDate = DateTime.UtcNow.AddDays(30);
            var claims = new List<Claim>() {
                new Claim("Id", id.ToString()),
                new Claim("Name", username),
                new Claim("ExpireDate", expireDate.ToString("yyyy-MM-dd")),
                new Claim("Role",role),
            };
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer, // Audience có thể là tổ chức của bạn hoặc ứng dụng của bạn
                claims: claims,
                expires: expireDate,
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
