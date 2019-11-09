using System.Collections.Generic;
using System.Linq;
using Aldan.Services.Logging;
using Aldan.Services.Messages;
using Aldan.Web.Areas.Admin.Factories;
using Aldan.Web.Areas.Admin.Models.Logging;
using Aldan.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Aldan.Web.Areas.Admin.Controllers
{
    public partial class LogController : BaseAdminController
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly ILogModelFactory _logModelFactory;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor

        public LogController(
            ILogger logger,
            ILogModelFactory logModelFactory,
            INotificationService notificationService)
        {
            _logger = logger;
            _logModelFactory = logModelFactory;
            _notificationService = notificationService;
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
            var model = _logModelFactory.PrepareLogSearchModel(new LogSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult LogList(LogSearchModel searchModel)
        {

            //prepare model
            var model = _logModelFactory.PrepareLogListModel(searchModel);

            return Json(model);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("clearall")]
        public virtual IActionResult ClearAll()
        {
            _logger.ClearLog();

            _notificationService.SuccessNotification("The log has been cleared successfully.");

            return RedirectToAction("List");
        }

        public virtual IActionResult View(int id)
        {
            //try to get a log with the specified id
            var log = _logger.GetLogById(id);
            if (log == null)
                return RedirectToAction("List");

            //prepare model
            var model = _logModelFactory.PrepareLogModel(null, log);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            //try to get a log with the specified id
            var log = _logger.GetLogById(id);
            if (log == null)
                return RedirectToAction("List");

            _logger.DeleteLog(log);

            _notificationService.SuccessNotification("The log entry has been deleted successfully.");

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (selectedIds != null)
                _logger.DeleteLogs(_logger.GetLogByIds(selectedIds.ToArray()).ToList());

            return Json(new { Result = true });
        }

        #endregion
    }
}