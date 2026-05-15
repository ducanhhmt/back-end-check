using System.Text.Json.Serialization;

namespace CleanArchitecture.Application.Model
{
    public class ProductAdminFilterRespone
    {
        public List<ProductAdminListingDto> products { get; set; }
        public int TotalRecords { get; set; }
        public int pageCount { get; set; }
    }

    public class ProductUserFilterRespone
    {
        public List<ProductUserListingDto> products { get; set; }
        public int TotalRecords { get; set; }
        public int pageCount { get; set; }
    }

    // ================================================================
    // ADMIN DTOs
    // ================================================================
    /// <summary>
    /// Admin listing — bảng quản lý sản phẩm.
    /// Cache MongoDB TTL ngắn (5 phút) vì admin hay thay đổi data.
    /// </summary>
    public class ProductAdminListingDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public int CategoriesId { get; set; }
        public string CategoriesName { get; set; }
        public int NxbId { get; set; }
        public string PublisherName { get; set; }
        public int Quantity { get; set; }
        [JsonIgnore]
        public string? ThumbnailIMG { get; set; }
        public List<string> Images =>
         ThumbnailIMG?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .ToList() ?? [];
        [JsonIgnore]
        public int TotalRecords { get; set; }
        [JsonIgnore]
        public int TotalPages { get; set; }
    }

    /// <summary>
    /// Admin detail — xem/edit đầy đủ thông tin kể cả giá vốn.
    /// Không cache — admin luôn cần data mới nhất, traffic thấp.
    /// </summary>
    public class ProductAdminDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Series { get; set; }
        public int NxbId { get; set; }
        public int CategoriesId { get; set; }
        public int Weight { get; set; }
        public int Price { get; set; }
        public int ImportPrice { get; set; }   // nhạy cảm — chỉ admin
        public int PublisherPrice { get; set; }
        public int Quantity { get; set; }
        public int Discount { get; set; }
        public DateTime? DateCreated { get; set; }   // chỉ admin
        public string? Description { get; set; }
        [JsonIgnore]
        public string? ThumbnailIMG { get; set; }
        public List<string> Images =>
         ThumbnailIMG?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .ToList() ?? [];
    }
    /// <summary>
    /// AdminFiltered — danh sách sản phẩm, dùng để nhập kho.
    /// Không cache — admin luôn cần data mới nhất, traffic thấp.
    /// </summary>
    public class ProductAdminFiltered
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public int ImportPrice { get; set; }
        public int Quantity { get; set; }
        [JsonIgnore]
        public string? ThumbnailIMG { get; set; }
        public string Images => ThumbnailIMG?.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
    }

    // ================================================================
    // USER DTOs
    // ================================================================
    /// <summary>
    /// User listing — trang danh sách sản phẩm hiển thị ra ngoài.
    /// Cache MongoDB TTL 15 phút.
    /// </summary>
    public class ProductUserListingDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public int Price { get; set; }
        public int PublisherPrice { get; set; }
        public int Discount { get; set; }
        public int Quantity { get; set; }
        [JsonIgnore]
        public string? ThumbnailIMG { get; set; }
        public List<string> Images =>
         ThumbnailIMG?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .ToList() ?? [];
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
    }

    /// <summary>
    /// User detail — trang chi tiết sản phẩm.
    /// Không có ImportPrice, DateCreated.
    /// Cache MongoDB TTL 30 phút + sliding TTL.
    /// </summary>
    public class ProductUserDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Series { get; set; }
        public int NxbId { get; set; }
        public int CategoriesId { get; set; }
        public int Weight { get; set; }
        public int Price { get; set; }
        public int PublisherPrice { get; set; }
        public int Quantity { get; set; }
        public int Discount { get; set; }
        public string? Description { get; set; }
        [JsonIgnore]
        public string? ThumbnailIMG { get; set; }
        public List<string> Images =>
         ThumbnailIMG?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .ToList() ?? [];
    }
}
