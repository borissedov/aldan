using System.ComponentModel;
using Aldan.Web.Framework.Models;

namespace Aldan.Web.Areas.Admin.Models.Tasks
{
    /// <summary>
    /// Represents a schedule task model
    /// </summary>
    public partial class ScheduleTaskModel : BaseAldanEntityModel
    {
        #region Properties

        [DisplayName("Admin.System.ScheduleTasks.Name")]
        public string Name { get; set; }

        [DisplayName("Admin.System.ScheduleTasks.Seconds")]
        public int Seconds { get; set; }

        [DisplayName("Admin.System.ScheduleTasks.Enabled")]
        public bool Enabled { get; set; }

        [DisplayName("Admin.System.ScheduleTasks.StopOnError")]
        public bool StopOnError { get; set; }

        [DisplayName("Admin.System.ScheduleTasks.LastStart")]
        public string LastStartUtc { get; set; }

        [DisplayName("Admin.System.ScheduleTasks.LastEnd")]
        public string LastEndUtc { get; set; }

        [DisplayName("Admin.System.ScheduleTasks.LastSuccess")]
        public string LastSuccessUtc { get; set; }

        #endregion
    }
}