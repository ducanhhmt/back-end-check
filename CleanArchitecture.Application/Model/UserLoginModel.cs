using CleanArchitecture.Application.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Model
{
    public class UserLoginModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Name => FirstName + " " + LastName;  // Sử dụng property expression body
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        [JsonIgnore]
        public string Address { get; set; }
        // ✅ Tự động convert từ Address JSON string
        [NotMapped]
        public List<UserAddress> LstAddress =>
            string.IsNullOrWhiteSpace(Address)
            ? new List<UserAddress>()
            : JsonSerializer.Deserialize<List<UserAddress>>(Address,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) // ✅
              ?? new List<UserAddress>();
        public bool Gender { get; set; } = true;
        public DateOnly Birthday { get; set; }
    }
    public class UserAddress
    {
        [JsonPropertyName("name")]      // ✅ khớp với key trong DB
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;
    }
}
