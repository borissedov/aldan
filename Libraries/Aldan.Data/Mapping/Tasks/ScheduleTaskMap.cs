using Aldan.Core.Domain.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldan.Data.Mapping.Tasks
{
    /// <summary>
    /// Represents a schedule task mapping configuration
    /// </summary>
    public partial class ScheduleTaskMap : AldanEntityTypeConfiguration<ScheduleTask>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ScheduleTask> builder)
        {
            builder.ToTable(nameof(ScheduleTask));
            builder.HasKey(task => task.Id);

            builder.Property(task => task.Name).IsRequired();
            builder.Property(task => task.Type).IsRequired();

            builder.HasData(new ScheduleTask
                {
                    Id = 1,
                    Name = "Send emails",
                    Seconds = 60,
                    Type = "Aldan.Services.Messages.QueuedMessagesSendTask, Aldan.Services",
                    Enabled = true,
                    StopOnError = false
                });
            base.Configure(builder);
        }

        #endregion
    }
}