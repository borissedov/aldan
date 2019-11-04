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
                    Name = MessageTemplateSystemNames.CustomerPasswordRecoveryMessage,
                    Subject = "%Platform.Name%. Password recovery",
                    Body =
                        $"<a href=\"%Platform.URL%\">%Platform.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}To change your password <a href=\"%Customer.PasswordRecoveryURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Platform.Name%{Environment.NewLine}",
                    IsActive = true
                },
                new MessageTemplate
                {
                    Id = 2,
                    Name = MessageTemplateSystemNames.CustomerWelcomeMessage,
                    Subject = "Welcome to %Platform.Name%",
                    Body =
                        $"We welcome you to <a href=\"%Platform.URL%\"> %Platform.Name%</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}You can now take part in the various services we have to offer you. Some of these services include:{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Permanent Cart - Any products added to your online cart remain there until you remove them, or check them out.{Environment.NewLine}<br />{Environment.NewLine}Address Book - We can now deliver your products to another address other than yours! This is perfect to send birthday gifts direct to the birthday-person themselves.{Environment.NewLine}<br />{Environment.NewLine}Order History - View your history of purchases that you have made with us.{Environment.NewLine}<br />{Environment.NewLine}Products Reviews - Share your opinions on products with our other customers.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}For help with any of our online services, please email the platform-owner: <a href=\"mailto:%Platform.Email%\">%Platform.Email%</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Note: This email address was provided on our registration page. If you own the email and did not register on our site, please send an email to <a href=\"mailto:%Platform.Email%\">%Platform.Email%</a>.{Environment.NewLine}",
                    IsActive = true
                },
                new MessageTemplate
                {
                    Id = 3,
                    Name = MessageTemplateSystemNames.CustomerRegisteredNotification,
                    Subject = "%Platform.Name%. New customer registration",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Platform.URL%\">%Platform.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new customer registered with your platform. Below are the customer's details:{Environment.NewLine}<br />{Environment.NewLine}Full name: %Customer.FullName%{Environment.NewLine}<br />{Environment.NewLine}Email: %Customer.Email%{Environment.NewLine}</p>{Environment.NewLine}",
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