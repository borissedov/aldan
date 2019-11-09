using Aldan.Core.Infrastructure;
using Aldan.Web.Framework;
using Aldan.Web.Framework.Controllers;
using Aldan.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aldan.Web.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    //[SaveSelectedTab]
    public abstract partial class BaseAdminController : BaseController
    {
        /// <summary>
        /// Creates an object that serializes the specified object to JSON.
        /// </summary>
        /// <param name="data">The object to serialize.</param>
        /// <returns>The created object that serializes the specified data to JSON format for the response.</returns>
        public override JsonResult Json(object data)
        {
            //use IsoDateFormat on writing JSON text to fix issue with dates in grid
            var serializerSettings = EngineContext.Current.Resolve<IOptions<MvcJsonOptions>>()?.Value?.SerializerSettings
                ?? new JsonSerializerSettings();


            serializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;

            return base.Json(data, serializerSettings);
        }

    }
}