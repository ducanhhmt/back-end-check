using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Entities
{
    public class Purchase
    {
        public Guid Id { get; private set; }
        public string UserCreated { get; private set; }
        public int SupplierId { get; private set; }
        public int ImportPrice { get; private set; }
        public string Description { get; private set; }
        public DateTime DateCreated { get; private set; }
        public int State { get; private set; }

        protected Purchase() { }
        public Purchase(string UserCreated, int SupplierId, int ImportPrice, string Description, DateTime DateCreated, int State)
        {
            CreateInfo(UserCreated, SupplierId, ImportPrice, Description, DateCreated, State);           
        }

        public void CreateInfo(string userCreated, int supplierId, int importPrice, string description, DateTime dateCreated, int state)
        {
            if (string.IsNullOrWhiteSpace(userCreated))
                throw new Exception("Không có Thông tin người tạo");
            if (string.IsNullOrWhiteSpace(supplierId.ToString()))
                throw new Exception("Không có thông tin nhà cung cấp");
            UserCreated = userCreated;
            SupplierId = supplierId;
            ImportPrice = importPrice;
            Description = description;
            DateCreated = dateCreated;
            State = state;
        }
        public void UpdateInfo(string description, int state)
        {
            Description = description;
            State = state; 
        }
    }  
}
