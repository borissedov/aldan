using System;
using System.Linq;
using Aldan.Services.Tasks;
using Aldan.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Aldan.Web.Areas.Admin.Models.Tasks;
using Aldan.Web.Framework.Models.Extensions;

namespace Aldan.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the schedule task model factory implementation
    /// </summary>
    public partial class ScheduleTaskModelFactory : IScheduleTaskModelFactory
    {
        #region Fields

        private readonly IScheduleTaskService _scheduleTaskService;

        #endregion

        #region Ctor

        public ScheduleTaskModelFactory(
            IScheduleTaskService scheduleTaskService)
        {
            _scheduleTaskService = scheduleTaskService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare schedule task search model
        /// </summary>
        /// <param name="searchModel">Schedule task search model</param>
        /// <returns>Schedule task search model</returns>
        public virtual ScheduleTaskSearchModel PrepareScheduleTaskSearchModel(ScheduleTaskSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged schedule task list model
        /// </summary>
        /// <param name="searchModel">Schedule task search model</param>
        /// <returns>Schedule task list model</returns>
        public virtual ScheduleTaskListModel PrepareScheduleTaskListModel(ScheduleTaskSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get schedule tasks
            var scheduleTasks = _scheduleTaskService.GetAllTasks(true).ToPagedList(searchModel);

            //prepare list model
            var model = new ScheduleTaskListModel().PrepareToGrid(searchModel, scheduleTasks, () =>
            {
                return scheduleTasks.Select(scheduleTask =>
                {
                    //fill in model values from the entity
                    var scheduleTaskModel = scheduleTask.ToModel<ScheduleTaskModel>();

                    //convert dates to the user time
                    if (scheduleTask.LastStartUtc.HasValue)
                    {
                        scheduleTaskModel.LastStartUtc = scheduleTask.LastStartUtc?.ToLocalTime().ToString("G");
                    }

                    if (scheduleTask.LastEndUtc.HasValue)
                    {
                        scheduleTaskModel.LastEndUtc = scheduleTask.LastEndUtc?.ToLocalTime().ToString("G");
                    }

                    if (scheduleTask.LastSuccessUtc.HasValue)
                    {
                        scheduleTaskModel.LastSuccessUtc = scheduleTask.LastSuccessUtc?.ToLocalTime().ToString("G");
                    }

                    return scheduleTaskModel;
                });
            });
            return model;
        }

        #endregion
    }
}