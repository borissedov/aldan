using Aldan.Web.Framework.Models;

namespace Aldan.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a setting mode model
    /// </summary>
    public partial class SettingModeModel : BaseAldanModel
    {
        #region Properties

        public string ModeName { get; set; }

        public bool Enabled { get; set; }

        #endregion
    }
}