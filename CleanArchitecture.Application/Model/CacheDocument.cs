using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Model
{
    public class CacheDocument<T>
    {
        public string Id { get; set; } = default!;   // key
        public T Data { get; set; } = default!;
        public DateTime ExpiredAt { get; set; }
    }
}
