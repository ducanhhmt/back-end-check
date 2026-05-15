using CleanArchitecture.Domain.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Model
{
    public class UploadImageRequest
    {
        public List<IFormFile> Files { get; set; } = new();
        public UploadType Category { get; set; }  // "User" hoặc "Product"
    }
}
