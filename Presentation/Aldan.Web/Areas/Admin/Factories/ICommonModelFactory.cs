using Aldan.Web.Areas.Admin.Models.Common;

namespace Aldan.Web.Areas.Admin.Factories
{
    public interface ICommonModelFactory
    {
        /// <summary>
        /// Prepare system info model
        /// </summary>
        /// <param name="model">System info model</param>
        /// <returns>System info model</returns>
        SystemInfoModel PrepareSystemInfoModel(SystemInfoModel model);
    }
}