namespace Aldan.Services.Messages
{
    /// <summary>
    /// Represents default values related to messages services
    /// </summary>
    public static partial class AldanMessageDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string MessageTemplatesAllCacheKey => "Aldan.messagetemplate.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : template name
        /// </remarks>
        public static string MessageTemplatesByNameCacheKey => "Aldan.messagetemplate.name-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string MessageTemplatesPrefixCacheKey => "Aldan.messagetemplate.";

        /// <summary>
        /// Gets a key for notifications list from TempDataDictionary
        /// </summary>
        public static string NotificationListKey => "NotificationList";
    }
}