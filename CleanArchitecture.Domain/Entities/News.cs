using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Entities
{
    public class News
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public DateTime DateCreated { get; private set; }
        public string UserCreated { get; private set; }
        public string? Description { get; private set; }
        public string? Thumbnail { get; private set; }
        protected News() { }
        public News(string title, string content, DateTime dateCreated, string userCreated, string? description, string? thumbnail)
        {
            UpdateInfo(title, content, dateCreated, userCreated, description, thumbnail);
        }

        public void UpdateInfo(string title, string content, DateTime dateCreated, string userCreated, string description, string thumbnail)
        {
            if (dateCreated > DateTime.UtcNow)
                throw new Exception("Ngày sinh không hợp lệ");
            if (string.IsNullOrWhiteSpace(userCreated))
                throw new Exception("Tên người tạo chưa hợp lí");

            Title = title;
            Content = content;
            DateCreated = dateCreated;
            UserCreated = userCreated;
            Description = description;
            Thumbnail = thumbnail;
        }
    }
}
