using Microsoft.EntityFrameworkCore;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Security.Principal;

namespace SanGiaoDich_BrotherHood.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.Birthday).IsRequired(false);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging(); // Bật ghi nhớ dữ liệu nhạy cảm
            base.OnConfiguring(optionsBuilder);
        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<AddressDetail> AddressDetails { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillDetail> BillDetails { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<ImageProduct> ImageProducts { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
        public DbSet<PaymentResponseModel> PaymentResponses { get; set; }
        public DbSet<PaymentRequestModel> PaymentRequests { get; set; }
        public DbSet<Withdrawal_Infomation> withdrawal_Infomations { get; set; }
    }
}
