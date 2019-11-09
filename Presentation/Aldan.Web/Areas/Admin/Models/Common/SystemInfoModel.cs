using System;
using System.Collections.Generic;
using System.ComponentModel;
using Aldan.Web.Framework.Models;

namespace Aldan.Web.Areas.Admin.Models.Common
{
    public partial class SystemInfoModel : BaseAldanModel
    {
        public SystemInfoModel()
        {
            Headers = new List<HeaderModel>();
            LoadedAssemblies = new List<LoadedAssembly>();
        }

        [DisplayName("ASP.NET info")]
        public string AspNetInfo { get; set; }

        [DisplayName("Is full trust level")]
        public string IsFullTrust { get; set; }

        [DisplayName("Operating system")]
        public string OperatingSystem { get; set; }

        [DisplayName("Server local time")]
        public DateTime ServerLocalTime { get; set; }

        [DisplayName("Server time zone")]
        public string ServerTimeZone { get; set; }

        [DisplayName("Coordinated Universal Time (UTC)")]
        public DateTime UtcTime { get; set; }

        [DisplayName("HTTP_HOST")]
        public string HttpHost { get; set; }

        [DisplayName("Headers")]
        public IList<HeaderModel> Headers { get; set; }

        [DisplayName("Loaded assemblies")]
        public IList<LoadedAssembly> LoadedAssemblies { get; set; }

        public partial class HeaderModel : BaseAldanModel
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public partial class LoadedAssembly : BaseAldanModel
        {
            public string FullName { get; set; }
            public string Location { get; set; }
            public bool IsDebug { get; set; }
            public DateTime? BuildDate { get; set; }
        }
    }
}