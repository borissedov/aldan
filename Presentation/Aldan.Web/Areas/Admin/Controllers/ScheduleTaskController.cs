using System;
using Aldan.Services.Messages;
using Aldan.Services.Tasks;
using Aldan.Web.Areas.Admin.Factories;
using Aldan.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Aldan.Web.Areas.Admin.Models.Tasks;
using Aldan.Web.Framework.Mvc;
using Aldan.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace Aldan.Web.Areas.Admin.Controllers
{
    public partial class ScheduleTaskController : BaseAdminController
    {
        #region Fields

        private readonly INotificationService _notificationService;
        private readonly IScheduleTaskModelFactory _scheduleTaskModelFactory;
        private readonly IScheduleTaskService _scheduleTaskService;

        #endregion

        #region Ctor

        public ScheduleTaskController(
            INotificationService notificationService,
            IScheduleTaskModelFactory scheduleTaskModelFactory,
            IScheduleTaskService scheduleTaskService)
        {
            _notificationService = notificationService;
            _scheduleTaskModelFactory = scheduleTaskModelFactory;
            _scheduleTaskService = scheduleTaskService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            //prepare model
            var model = _scheduleTaskModelFactory.PrepareScheduleTaskSearchModel(new ScheduleTaskSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(ScheduleTaskSearchModel searchModel)
        {
            //prepare model
            var model = _scheduleTaskModelFactory.PrepareScheduleTaskListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult TaskUpdate(ScheduleTaskModel model)
        {
            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //try to get a schedule task with the specified id
            var scheduleTask = _scheduleTaskService.GetTaskById(model.Id)
                               ?? throw new ArgumentException("Schedule task cannot be loaded");

            scheduleTask = model.ToEntity(scheduleTask);

            _scheduleTaskService.UpdateTask(scheduleTask);

            return new NullJsonResult();
        }

        public virtual IActionResult RunNow(int id)
        {
            try
            {
                //try to get a schedule task with the specified id
                var scheduleTask = _scheduleTaskService.GetTaskById(id)
                                   ?? throw new ArgumentException("Schedule task cannot be loaded", nameof(id));

                //ensure that the task is enabled
                var task = new Task(scheduleTask) { Enabled = true };
                task.Execute(true, false);

                _notificationService.SuccessNotification("Schedule task was run");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
            }

            return RedirectToAction("List");
        }

        #endregion
    }
}