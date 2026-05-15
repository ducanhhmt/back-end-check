using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Services_Interface
{
    public interface IPasswordHasher
    {
        ///khai báo hàm với kiểu ValueTuple :nhóm nhiều giá trị đơn giản lại, KHÔNG cần tạo class
        (string hash, string salt) HashPassword(string password);
        bool Verify(string password, string hash, string salt);
    }
}
