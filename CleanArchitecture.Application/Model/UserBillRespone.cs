using CleanArchitecture.Application.DTO;
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
    public class UserBillRespone
    {
        public List<UserBillModel> items { get; set; }
        public int pageCount{ get; set; }  
    }
}
