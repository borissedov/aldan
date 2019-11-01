namespace Aldan.Core.Configuration
{
    /// <summary>
    /// Represents startup Aldan configuration parameters
    /// </summary>
    public partial class AldanConfig
    {
        public string ScheduleTaskUrl { get; set; }
        public SecuritySettings Security { get; set; }

        public DataSettings Data { get; set; }
    }

    public class DataSettings
    {
        public string ConnectionString { get; set; }
    }

    public class SecuritySettings
    {
        public string EncryptionKey { get; set; }
    }
}