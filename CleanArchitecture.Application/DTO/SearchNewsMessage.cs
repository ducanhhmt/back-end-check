using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.DTO
{
    public class SearchNewsMessage
    {
        public string RequestId { get; set; }
        public string Keyword { get; set; }
    }
}
