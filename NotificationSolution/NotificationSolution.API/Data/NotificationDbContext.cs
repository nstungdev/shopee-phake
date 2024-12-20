using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace NotificationSolution.API.Data
{
    public class NotificationDbContext(DbContextOptions options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
        }
    }
}
