using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Entities
{
    public class Supplier
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Phone { get; private set; }
        public string Email { get; private set; }
        public string Address { get; private set; }
        protected Supplier() { }
        public Supplier(string Name, string Phone, string Email, string Address)
        {
            UpdateInfo(Name, Phone, Email, Address);           
        }

        public void UpdateInfo(string name, string phone, string email, string address)
        {
            if (string.IsNullOrWhiteSpace(name.ToString()))
                throw new Exception("Không có Tên nhà cung cấp");
            Name = name;
            Phone = phone;
            Email = email;
            Address = address;
        }
    }  
}
