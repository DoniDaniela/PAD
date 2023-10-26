using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Basket.API.Infrastructure.EntityConfigurations
{
    public class BasketItemTypeConfiguration : IEntityTypeConfiguration<BasketItem>
    {
        public void Configure(EntityTypeBuilder<BasketItem> builder)
        {
            builder.ToTable("BasketItem");
            builder.HasIndex(e => e.BasketId, "IX_BasketItem_BasketId");

            builder.Property(e => e.OldUnitPrice).HasColumnType("decimal(18, 2)");
            builder.Property(e => e.PictureUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            builder.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(255);
            builder.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            builder.HasOne(d => d.Basket).WithMany(p => p.Items).HasForeignKey(d => d.BasketId);
        }
    }
}
