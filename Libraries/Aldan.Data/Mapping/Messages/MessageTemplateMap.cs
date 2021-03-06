using System;
using Aldan.Core.Domain.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldan.Data.Mapping.Messages
{
    /// <summary>
    /// Represents a message template mapping configuration
    /// </summary>
    public partial class MessageTemplateMap : AldanEntityTypeConfiguration<MessageTemplate>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<MessageTemplate> builder)
        {
            builder.ToTable(nameof(MessageTemplate));
            builder.HasKey(template => template.Id);

            builder.Property(template => template.Name).HasMaxLength(200).IsRequired();
            builder.Property(template => template.BccEmailAddresses).HasMaxLength(200);
            builder.Property(template => template.Subject).HasMaxLength(1000);

            builder.HasData(new MessageTemplate
                {
                    Id = 1,
                    Name = MessageTemplateSystemNames.UserPasswordRecoveryMessage,
                    Subject = "%Platform.Name%. Password recovery",
                    Body =
                        $"<a href=\"%Platform.URL%\">%Platform.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}To change your password <a href=\"%User.PasswordRecoveryURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Platform.Name%{Environment.NewLine}",
                    IsActive = true
                },
                new MessageTemplate
                {
                    Id = 2,
                    Name = MessageTemplateSystemNames.UserWelcomeMessage,
                    Subject = "Welcome to %Platform.Name%",
                    Body =
                        $"We welcome you to <a href=\"%Platform.URL%\"> %Platform.Name%</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}For help with any of our online services, please email the platform-owner: <a href=\"mailto:%Platform.Email%\">%Platform.Email%</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Note: This email address was provided on our registration page. If you own the email and did not register on our site, please send an email to <a href=\"mailto:%Platform.Email%\">%Platform.Email%</a>.{Environment.NewLine}",
                    IsActive = true
                },
                new MessageTemplate
                {
                    Id = 3,
                    Name = MessageTemplateSystemNames.UserRegisteredNotification,
                    Subject = "%Platform.Name%. New user registration",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Platform.URL%\">%Platform.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new user registered with your platform. Below are the user's details:{Environment.NewLine}<br />{Environment.NewLine}Full name: %User.FullName%{Environment.NewLine}<br />{Environment.NewLine}Email: %User.Email%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true
                },
                new MessageTemplate
                {
                    Id = 4,
                    Name = MessageTemplateSystemNames.ContactUsMessage,
                    Subject = "%Platform.Name%. Contact us",
                    Body = $"<p>{Environment.NewLine}%ContactUs.Body%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true
                });
            
            base.Configure(builder);
        }

        #endregion
    }
}