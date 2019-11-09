using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Aldan.Core.Infrastructure;
using Aldan.Web.Areas.Admin.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Aldan.Web.Areas.Admin.Factories
{
    public class CommonModelFactory : ICommonModelFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAldanFileProvider _fileProvider;

        public CommonModelFactory(
            IHttpContextAccessor httpContextAccessor, 
            IAldanFileProvider fileProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _fileProvider = fileProvider;
        }

        public SystemInfoModel PrepareSystemInfoModel(SystemInfoModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.ServerTimeZone = TimeZoneInfo.Local.StandardName;
            model.ServerLocalTime = DateTime.Now;
            model.UtcTime = DateTime.UtcNow;
            model.HttpHost = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host];

            //ensure no exception is thrown
            try
            {
                model.OperatingSystem = Environment.OSVersion.VersionString;
                model.AspNetInfo = RuntimeEnvironment.GetSystemVersion();
                model.IsFullTrust = AppDomain.CurrentDomain.IsFullyTrusted.ToString();
            }
            catch { }

            foreach (var header in _httpContextAccessor.HttpContext.Request.Headers)
            {
                model.Headers.Add(new SystemInfoModel.HeaderModel
                {
                    Name = header.Key,
                    Value = header.Value
                });
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var loadedAssemblyModel = new SystemInfoModel.LoadedAssembly
                {
                    FullName = assembly.FullName
                };

                //ensure no exception is thrown
                try
                {
                    loadedAssemblyModel.Location = assembly.IsDynamic ? null : assembly.Location;
                    loadedAssemblyModel.IsDebug = assembly.GetCustomAttributes(typeof(DebuggableAttribute), false)
                        .FirstOrDefault() is DebuggableAttribute attribute && attribute.IsJITOptimizerDisabled;

                    //https://stackoverflow.com/questions/2050396/getting-the-date-of-a-net-assembly
                    //we use a simple method because the more Jeff Atwood's solution doesn't work anymore 
                    //more info at https://blog.codinghorror.com/determining-build-date-the-hard-way/
                    loadedAssemblyModel.BuildDate = assembly.IsDynamic ? null : (DateTime?)TimeZoneInfo.ConvertTimeFromUtc(_fileProvider.GetLastWriteTimeUtc(assembly.Location), TimeZoneInfo.Local);

                }
                catch { }
                model.LoadedAssemblies.Add(loadedAssemblyModel);
            }
            
            return model;
        }
    }
}