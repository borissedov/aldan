using Aldan.Core.Domain.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldan.Data.Mapping.Logging
{
    /// <summary>
    /// Represents a log mapping configuration
    /// </summary>
    public partial class LogMap : AldanEntityTypeConfiguration<Log>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.ToTable(nameof(Log));
            builder.HasKey(logItem => logItem.Id);

            builder.Property(logItem => logItem.ShortMessage).IsRequired();
            builder.Property(logItem => logItem.IpAddress).HasMaxLength(200);

            builder.Ignore(logItem => logItem.LogLevel);

            builder.HasOne(logItem => logItem.User)
                .WithMany()
                .HasForeignKey(logItem => logItem.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            base.Configure(builder);
        }

        #endregion
    }
}