using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.DTO
{
    public class CategoriesDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? Parent_ID { get; set; }
    }
    public class MenuGroup
    {
        public string Title { get; set; } = string.Empty;
        public List<string> Items { get; set; } = new();
    }

    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<MenuGroup> Groups { get; set; } = new();
    }
}
