using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<News> news { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<Cart> carts { get; set; }
        public DbSet<UserBill> userBill { get; set; }
        public DbSet<BillInfo> billInfo { get; set; }
        public DbSet<Supplier> supplier { get; set; }
        public DbSet<SupplierPurchase> supplierPurchase { get; set; }
        public DbSet<Purchase> purchase { get; set; }
        public DbSet<PurchaseOrder> purchaseOrder { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<News>()
               .ToTable("News")            // Xác định tên bảng
               .HasKey(so => so.Id);       // Đặt khóa chính
            modelBuilder.Entity<User>()
               .ToTable("Nguoi_dung")            // Xác định tên bảng
               .HasKey(so => so.Id);       // Đặt khóa chính
            modelBuilder.Entity<Category>()
              .ToTable("Categories")            // Xác định tên bảng
              .HasKey(so => so.Id);       // Đặt khóa chính
            modelBuilder.Entity<Product>()
              .ToTable("Product")            // Xác định tên bảng
              .HasKey(so => so.Id);       // Đặt khóa chính
            modelBuilder.Entity<Cart>()
             .ToTable("Cart")            // Xác định tên bảng
             .HasKey(so => so.Id);       // Đặt khóa chính
            modelBuilder.Entity<UserBill>()
             .ToTable("UserBill")            // Xác định tên bảng
             .HasKey(so => so.Id);       // Đặt khóa chính
            modelBuilder.Entity<BillInfo>()
             .ToTable("BillInfo")            // Xác định tên bảng
             .HasKey(so => so.Id);       // Đặt khóa chính
            modelBuilder.Entity<Supplier>()
             .ToTable("Supplier")            // Xác định tên bảng
             .HasKey(so => so.Id);       // Đặt khóa chính
            modelBuilder.Entity<SupplierPurchase>()
             .ToTable("Supplier-Purchase")            // Xác định tên bảng
             .HasKey(so => so.Id);       // Đặt khóa chính
            modelBuilder.Entity<Purchase>()
             .ToTable("Purchase")            // Xác định tên bảng
             .HasKey(so => so.Id);       // Đặt khóa chính
            modelBuilder.Entity<PurchaseOrder>()
             .ToTable("Purchase-Order")            // Xác định tên bảng
             .HasKey(so => so.Id);       // Đặt khóa chính
        }
    }
}
