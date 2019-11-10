using System;
using System.Collections.Generic;
using Aldan.Core.Domain.Messages;
using Aldan.Services.Messages;
using Aldan.Web.Areas.Admin.Factories;
using Aldan.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Aldan.Web.Areas.Admin.Models.Messages;
using Aldan.Web.Framework.Controllers;
using Aldan.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aldan.Web.Areas.Admin.Controllers
{
    public partial class MessageTemplateController : BaseAdminController
    {
        #region Fields

        
        private readonly IMessageTemplateModelFactory _messageTemplateModelFactory;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly INotificationService _notificationService;
        private readonly IWorkflowMessageService _workflowMessageService;

        #endregion Fields

        #region Ctor

        public MessageTemplateController(
            IMessageTemplateModelFactory messageTemplateModelFactory,
            IMessageTemplateService messageTemplateService,
            INotificationService notificationService,
            IWorkflowMessageService workflowMessageService)
        {
            _messageTemplateModelFactory = messageTemplateModelFactory;
            _messageTemplateService = messageTemplateService;
            _notificationService = notificationService;
            _workflowMessageService = workflowMessageService;
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
            var model = _messageTemplateModelFactory.PrepareMessageTemplateSearchModel(new MessageTemplateSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(MessageTemplateSearchModel searchModel)
        {
            //prepare model
            var model = _messageTemplateModelFactory.PrepareMessageTemplateListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Edit(int id)
        {
            //try to get a message template with the specified id
            var messageTemplate = _messageTemplateService.GetMessageTemplateById(id);
            if (messageTemplate == null)
                return RedirectToAction("List");

            //prepare model
            var model = _messageTemplateModelFactory.PrepareMessageTemplateModel(null, messageTemplate);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(MessageTemplateModel model, bool continueEditing)
        {
            //try to get a message template with the specified id
            var messageTemplate = _messageTemplateService.GetMessageTemplateById(model.Id);
            if (messageTemplate == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                messageTemplate = model.ToEntity(messageTemplate);

                _messageTemplateService.UpdateMessageTemplate(messageTemplate);

                _notificationService.SuccessNotification("The message template has been updated successfully.");

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = messageTemplate.Id });
            }

            //prepare model
            model = _messageTemplateModelFactory.PrepareMessageTemplateModel(model, messageTemplate);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            //try to get a message template with the specified id
            var messageTemplate = _messageTemplateService.GetMessageTemplateById(id);
            if (messageTemplate == null)
                return RedirectToAction("List");

            _messageTemplateService.DeleteMessageTemplate(messageTemplate);

            _notificationService.SuccessNotification("The message template has been deleted successfully.");

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("message-template-copy")]
        public virtual IActionResult CopyTemplate(MessageTemplateModel model)
        {
            //try to get a message template with the specified id
            var messageTemplate = _messageTemplateService.GetMessageTemplateById(model.Id);
            if (messageTemplate == null)
                return RedirectToAction("List");

            try
            {
                var newMessageTemplate = _messageTemplateService.CopyMessageTemplate(messageTemplate);

                _notificationService.SuccessNotification("The message template has been copied successfully");

                return RedirectToAction("Edit", new { id = newMessageTemplate.Id });
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = model.Id });
            }
        }

        public virtual IActionResult TestTemplate(int id)
        {
            //try to get a message template with the specified id
            var messageTemplate = _messageTemplateService.GetMessageTemplateById(id);
            if (messageTemplate == null)
                return RedirectToAction("List");

            //prepare model
            var model = _messageTemplateModelFactory
                .PrepareTestMessageTemplateModel(new TestMessageTemplateModel(), messageTemplate);

            return View(model);
        }

        [HttpPost, ActionName("TestTemplate")]
        [FormValueRequired("send-test")]
        public virtual IActionResult TestTemplate(TestMessageTemplateModel model, IFormCollection form)
        {
            //try to get a message template with the specified id
            var messageTemplate = _messageTemplateService.GetMessageTemplateById(model.Id);
            if (messageTemplate == null)
                return RedirectToAction("List");

            var tokens = new List<Token>();
            foreach (var formKey in form.Keys)
                if (formKey.StartsWith("token_", StringComparison.InvariantCultureIgnoreCase))
                {
                    var tokenKey = formKey.Substring("token_".Length).Replace("%", string.Empty);
                    var stringValue = form[formKey].ToString();

                    //try get non-string value
                    object tokenValue;
                    if (bool.TryParse(stringValue, out var boolValue))
                        tokenValue = boolValue;
                    else if (int.TryParse(stringValue, out var intValue))
                        tokenValue = intValue;
                    else if (decimal.TryParse(stringValue, out var decimalValue))
                        tokenValue = decimalValue;
                    else
                        tokenValue = stringValue;

                    tokens.Add(new Token(tokenKey, tokenValue));
                }

            _workflowMessageService.SendTestEmail(messageTemplate.Id, model.SendTo, tokens);

            if (ModelState.IsValid)
            {
                _notificationService.SuccessNotification("Email has been successfully queued.");
            }

            return RedirectToAction("Edit", new { id = messageTemplate.Id });
        }

        #endregion
    }
}