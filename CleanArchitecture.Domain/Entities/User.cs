using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        public string? Email { get; private set; }
        public string? Phone { get; private set; }
        public string? Address { get; private set; }
        public string Account { get; private set; }
        public string PasswordHash { get; private set; }
        public string PasswordSalt { get; private set; }
        public string? Role { get; private set; }
        public DateTime? Birthday { get; set; }
        public string? Avatar { get; private set; }
        protected User() { }
        public User(string? firstName, string? lastName, string? email, string? phone, string? address, string account,
            string passwordHash, string passwordSalt, string? role, DateTime? birthday, string? avatar)
        {
            UpdateInfo(firstName, lastName, email, phone, address, account, passwordHash, passwordSalt, role, birthday, avatar);           
        }

        public void UpdateInfo(string? firstName, string? lastName, string? email, string? phone, string? address, string account,
            string passwordHash, string passwordSalt, string? role, DateTime? birthday, string? avatar)
        {
            if (string.IsNullOrWhiteSpace(account))
                throw new Exception("Tên người tạo chưa hợp lí");
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new Exception("Pass chưa hợp lí");
            if (string.IsNullOrWhiteSpace(passwordSalt))
                throw new Exception("Pass chưa hợp lí");

            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            Address = address;
            Account = account;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            Role = role;
            Birthday = birthday;
            Avatar = avatar;
        }
    }
    public class LoginRespone
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public bool isLogin { get; set; }

    }
}
