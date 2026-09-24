using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities.Order_Aggregate;

namespace ECommerce.Repository._Data.Configurations.Order_Configurations
{
    public class OrderItemConfigurations : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(orderItem => orderItem.Product, Product => Product.WithOwner());

            builder.Property(orderItem => orderItem.Price)
                .HasColumnType("decimal(12,2)");
        }
    }
}

