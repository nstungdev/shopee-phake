using Microsoft.EntityFrameworkCore;
using OrderSolution.API.Enums;

namespace OrderSolution.API.Data
{
    public class OrderDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.CustomerId)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.OrderStatus)
                      .IsRequired()
                      .HasMaxLength(150)
                      .HasConversion<string>()
                      .HasDefaultValue(OrderStatus.Pending);

                entity.Property(e => e.OrderDate)
                      .IsRequired();

                entity.Property(e => e.ShippingAddress)
                      .IsRequired()
                      .HasMaxLength(1000);

                entity.Property(e => e.PaymentMethod)
                      .IsRequired()
                      .HasMaxLength(150)
                      .HasConversion<string>();

                entity.Property(e => e.CreateAt)
                      .IsRequired()
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdateAt)
                      .IsRequired()
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Order has many OrderItems
                entity.HasMany(e => e.OrderItems)
                      .WithOne(e => e.Order)
                      .HasForeignKey(e => e.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.OrderId)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.ProductId)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.Quantity)
                      .IsRequired();

                entity.Property(e => e.UnitPrice)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");
            });
            #endregion

            #region Enums
            modelBuilder.HasPostgresEnum<OrderStatus>();
            modelBuilder.HasPostgresEnum<PaymentMethod>();
            #endregion
        }
    }
}
