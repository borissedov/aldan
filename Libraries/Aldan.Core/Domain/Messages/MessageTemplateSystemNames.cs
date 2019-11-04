namespace Aldan.Core.Domain.Messages
{
    /// <summary>
    /// Represents message template system names
    /// </summary>
    public static partial class MessageTemplateSystemNames
    {
        #region Customer

        /// <summary>
        /// Represents system name of notification about new registration
        /// </summary>
        public const string CustomerRegisteredNotification = "NewCustomer.Notification";

        /// <summary>
        /// Represents system name of customer welcome message
        /// </summary>
        public const string CustomerWelcomeMessage = "Customer.WelcomeMessage";

        /// <summary>
        /// Represents system name of password recovery message
        /// </summary>
        public const string CustomerPasswordRecoveryMessage = "Customer.PasswordRecovery";

        #endregion

        #region Misc
        
        /// <summary>
        /// Represents system name of 'Contact us' message
        /// </summary>
        public const string ContactUsMessage = "Service.ContactUs";

        #endregion
    }
}