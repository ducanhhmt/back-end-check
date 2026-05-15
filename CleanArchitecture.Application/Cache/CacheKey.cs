using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Cache
{
    public class CacheKey
    {
        public const string News = "LstNews";
        public const string Users = "LstUsers";
        public const string Category = "Category";
    }
}
