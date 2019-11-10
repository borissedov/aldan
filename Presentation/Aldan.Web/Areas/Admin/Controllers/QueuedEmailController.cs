using System;
using System.Collections.Generic;
using System.Linq;
using Aldan.Core.Domain.Messages;
using Aldan.Services.Messages;
using Aldan.Web.Areas.Admin.Factories;
using Aldan.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Aldan.Web.Areas.Admin.Models.Messages;
using Aldan.Web.Framework.Controllers;
using Aldan.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Aldan.Web.Areas.Admin.Controllers
{
    public partial class QueuedEmailController : BaseAdminController
    {
        #region Fields
        private readonly INotificationService _notificationService;
        private readonly IQueuedEmailModelFactory _queuedEmailModelFactory;
        private readonly IQueuedEmailService _queuedEmailService;

        #endregion

        #region Ctor

        public QueuedEmailController(
            INotificationService notificationService,
            IQueuedEmailModelFactory queuedEmailModelFactory,
            IQueuedEmailService queuedEmailService)
        {
            _notificationService = notificationService;
            _queuedEmailModelFactory = queuedEmailModelFactory;
            _queuedEmailService = queuedEmailService;
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
            var model = _queuedEmailModelFactory.PrepareQueuedEmailSearchModel(new QueuedEmailSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult QueuedEmailList(QueuedEmailSearchModel searchModel)
        {
            //prepare model
            var model = _queuedEmailModelFactory.PrepareQueuedEmailListModel(searchModel);

            return Json(model);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-email-by-number")]
        public virtual IActionResult GoToEmailByNumber(QueuedEmailSearchModel model)
        {
            //try to get a queued email with the specified id
            var queuedEmail = _queuedEmailService.GetQueuedEmailById(model.GoDirectlyToNumber);
            if (queuedEmail == null)
                return List();

            return RedirectToAction("Edit", "QueuedEmail", new { id = queuedEmail.Id });
        }

        public virtual IActionResult Edit(int id)
        {
            //try to get a queued email with the specified id
            var email = _queuedEmailService.GetQueuedEmailById(id);
            if (email == null)
                return RedirectToAction("List");

            //prepare model
            var model = _queuedEmailModelFactory.PrepareQueuedEmailModel(null, email);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(QueuedEmailModel model, bool continueEditing)
        {
            //try to get a queued email with the specified id
            var email = _queuedEmailService.GetQueuedEmailById(model.Id);
            if (email == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                email = model.ToEntity(email);
                _queuedEmailService.UpdateQueuedEmail(email);

                _notificationService.SuccessNotification("The queued email has been updated successfully.");

                return continueEditing ? RedirectToAction("Edit", new { id = email.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = _queuedEmailModelFactory.PrepareQueuedEmailModel(model, email, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit"), FormValueRequired("requeue")]
        public virtual IActionResult Requeue(QueuedEmailModel queuedEmailModel)
        {
            //try to get a queued email with the specified id
            var queuedEmail = _queuedEmailService.GetQueuedEmailById(queuedEmailModel.Id);
            if (queuedEmail == null)
                return RedirectToAction("List");

            var requeuedEmail = new QueuedEmail
            {
                From = queuedEmail.From,
                FromName = queuedEmail.FromName,
                To = queuedEmail.To,
                ToName = queuedEmail.ToName,
                ReplyTo = queuedEmail.ReplyTo,
                ReplyToName = queuedEmail.ReplyToName,
                CC = queuedEmail.CC,
                Bcc = queuedEmail.Bcc,
                Subject = queuedEmail.Subject,
                Body = queuedEmail.Body,
                CreatedOnUtc = DateTime.UtcNow
            };
            _queuedEmailService.InsertQueuedEmail(requeuedEmail);

            _notificationService.SuccessNotification("The queued email has been requeued successfully.");

            return RedirectToAction("Edit", new { id = requeuedEmail.Id });
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            //try to get a queued email with the specified id
            var email = _queuedEmailService.GetQueuedEmailById(id);
            if (email == null)
                return RedirectToAction("List");

            _queuedEmailService.DeleteQueuedEmail(email);

            _notificationService.SuccessNotification("The queued email has been deleted successfully.");

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (selectedIds != null)
                _queuedEmailService.DeleteQueuedEmails(_queuedEmailService.GetQueuedEmailsByIds(selectedIds.ToArray()));

            return Json(new { Result = true });
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("delete-all")]
        public virtual IActionResult DeleteAll()
        {
            _queuedEmailService.DeleteAllEmails();

            _notificationService.SuccessNotification("All queued emails have been deleted successfully.");

            return RedirectToAction("List");
        }

        #endregion
    }
}