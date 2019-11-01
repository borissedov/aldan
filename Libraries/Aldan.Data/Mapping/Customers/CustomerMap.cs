using Aldan.Core.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldan.Data.Mapping.Customers
{
    /// <summary>
    /// Represents a customer mapping configuration
    /// </summary>
    public partial class CustomerMap : AldanEntityTypeConfiguration<Customer>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable(nameof(Customer));
            builder.HasKey(customer => customer.Id);

            builder.Property(customer => customer.Email).HasMaxLength(1000);

            base.Configure(builder);
        }

        #endregion
    }
}