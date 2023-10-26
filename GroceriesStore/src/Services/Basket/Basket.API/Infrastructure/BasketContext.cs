using Basket.API.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Basket.API.Infrastructure
{
    public class BasketContext : DbContext
    {
        public BasketContext(DbContextOptions<BasketContext> options) : base(options)
        {
        }

        public virtual DbSet<CustomerBasket> Baskets { get; set; }

        public virtual DbSet<BasketItem> BasketItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerBasketTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BasketItemTypeConfiguration());
        }
    }
}
