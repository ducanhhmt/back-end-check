using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Model
{
    public class RemoveCartbyUserId
    {
        public Guid productId { get; set; }
        public Guid userId { get; set; }
    }
}
