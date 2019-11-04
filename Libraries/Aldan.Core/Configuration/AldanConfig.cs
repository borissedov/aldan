namespace Aldan.Core.Configuration
{
    /// <summary>
    /// Represents startup Aldan configuration parameters
    /// </summary>
    public partial class AldanConfig
    {
        public SecuritySettings Security { get; set; }
        public DataSettings Data { get; set; }
        public EmailSettings EmailSettings { get; set; }
        public PlatformSettings PlatformSettings { get; set; }
    }

    public class PlatformSettings
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string CompanyVat { get; set; }
        public string FacebookLink { get; set; }
        public string TwitterLink { get; set; }
        public string YoutubeLink { get; set; }
    }

    public class EmailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
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