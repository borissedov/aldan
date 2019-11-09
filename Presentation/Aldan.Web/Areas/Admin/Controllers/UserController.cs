using System;
using Aldan.Core;
using Aldan.Core.Configuration;
using Aldan.Core.Domain.Messages;
using Aldan.Core.Domain.Users;
using Aldan.Services.Common;
using Aldan.Services.Messages;
using Aldan.Services.Users;
using Aldan.Web.Areas.Admin.Factories;
using Aldan.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Aldan.Web.Areas.Admin.Models.Users;
using Aldan.Web.Framework.Controllers;
using Aldan.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aldan.Web.Areas.Admin.Controllers
{
    public partial class UserController : BaseAdminController
    {
        private readonly IUserModelFactory _userModelFactory;
        private readonly IWorkContext _workContext;
        private readonly IUserService _userService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly INotificationService _notificationService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly AldanConfig _config;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IGenericAttributeService _genericAttributeService;

        #region Fields


        #endregion

        #region Ctor

        public UserController(
            IUserModelFactory userModelFactory,
            IWorkContext workContext, 
            IUserService userService, 
            IUserRegistrationService userRegistrationService,
            INotificationService notificationService, 
            IQueuedEmailService queuedEmailService, 
            AldanConfig config, 
            IWorkflowMessageService workflowMessageService, 
            IGenericAttributeService genericAttributeService)
        {
            _userModelFactory = userModelFactory;
            _workContext = workContext;
            _userService = userService;
            _userRegistrationService = userRegistrationService;
            _notificationService = notificationService;
            _queuedEmailService = queuedEmailService;
            _config = config;
            _workflowMessageService = workflowMessageService;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        #region Users

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            //prepare model
            var model = _userModelFactory.PrepareUserSearchModel(new UserSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult UserList(UserSearchModel searchModel)
        {
            //prepare model
            var model = _userModelFactory.PrepareUserListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Create()
        {
            //prepare model
            var model = _userModelFactory.PrepareUserModel(new UserModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Create(UserModel model, bool continueEditing, IFormCollection form)
        {
            if (!string.IsNullOrWhiteSpace(model.Email) && _userService.GetUserByEmail(model.Email) != null)
                ModelState.AddModelError(string.Empty, "Email is already registered");

            // Ensure that valid email address is entered to avoid registered users with empty email address
            if (!CommonHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError(string.Empty, "Valid Email is required for user");

                _notificationService.ErrorNotification("Valid Email is required for user");
            }
            
            if (ModelState.IsValid)
            {
                //fill entity from model
                var user = model.ToEntity<User>();

                user.UserGuid = Guid.NewGuid();
                user.CreatedOnUtc = DateTime.UtcNow;
                user.LastActivityDateUtc = DateTime.UtcNow;
                
                _userService.InsertUser(user);
                
                _genericAttributeService.SaveAttribute(user, AldanUserDefaults.FirstNameAttribute, model.FirstName);
                _genericAttributeService.SaveAttribute(user, AldanUserDefaults.LastNameAttribute, model.LastName);

                //password
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    var changePassRequest = new ChangePasswordRequest(model.Email, false, model.Password);
                    var changePassResult = _userRegistrationService.ChangePassword(changePassRequest);
                    if (!changePassResult.Success)
                    {
                        foreach (var changePassError in changePassResult.Errors)
                            _notificationService.ErrorNotification(changePassError);
                    }
                }

                _userService.UpdateUser(user);

                _notificationService.SuccessNotification("The new user has been added successfully.");

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = user.Id });
            }

            //prepare model
            model = _userModelFactory.PrepareUserModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            //try to get a user with the specified id
            var user = _userService.GetUserById(id);
            if (user == null || user.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = _userModelFactory.PrepareUserModel(null, user);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(UserModel model, bool continueEditing, IFormCollection form)
        {
            //try to get a user with the specified id
            var user = _userService.GetUserById(model.Id);
            if (user == null || user.Deleted)
                return RedirectToAction("List");

            // Ensure that valid email address is entered to avoid registered users with empty email address
            if (!CommonHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError(string.Empty, "Valid Email is required for user");
                _notificationService.ErrorNotification("Valid Email is required for user");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //email
                    if (!string.IsNullOrWhiteSpace(model.Email))
                        _userRegistrationService.SetEmail(user, model.Email, false);
                    else
                        user.Email = model.Email;
                    
                    _genericAttributeService.SaveAttribute(user, AldanUserDefaults.FirstNameAttribute, model.FirstName);
                    _genericAttributeService.SaveAttribute(user, AldanUserDefaults.LastNameAttribute, model.LastName);

                    user.Role = (Role) model.SelectedUserRoleId;

                    _userService.UpdateUser(user);

                    _notificationService.SuccessNotification("The user has been updated successfully.");

                    if (!continueEditing)
                        return RedirectToAction("List");

                    return RedirectToAction("Edit", new { id = user.Id });
                }
                catch (Exception exc)
                {
                    _notificationService.ErrorNotification(exc.Message);
                }
            }

            //prepare model
            model = _userModelFactory.PrepareUserModel(model, user, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("changepassword")]
        public virtual IActionResult ChangePassword(UserModel model)
        {
            //try to get a user with the specified id
            var user = _userService.GetUserById(model.Id);
            if (user == null)
                return RedirectToAction("List");

            if (!ModelState.IsValid)
                return RedirectToAction("Edit", new { id = user.Id });

            var changePassRequest = new ChangePasswordRequest(model.Email,
                false, model.Password);
            var changePassResult = _userRegistrationService.ChangePassword(changePassRequest);
            if (changePassResult.Success)
                _notificationService.SuccessNotification("The password has been changed successfully.");
            else
                foreach (var error in changePassResult.Errors)
                    _notificationService.ErrorNotification(error);

            return RedirectToAction("Edit", new { id = user.Id });
        }
        
        #endregion
        
        public virtual IActionResult SendEmail(UserModel model)
        {
            //try to get a user with the specified id
            var user = _userService.GetUserById(model.Id);
            if (user == null)
                return RedirectToAction("List");

            try
            {
                if (string.IsNullOrWhiteSpace(user.Email))
                    throw new AldanException("User email is empty");
                if (!CommonHelper.IsValidEmail(user.Email))
                    throw new AldanException("User email is not valid");
                if (string.IsNullOrWhiteSpace(model.SendEmail.Subject))
                    throw new AldanException("Email subject is empty");
                if (string.IsNullOrWhiteSpace(model.SendEmail.Body))
                    throw new AldanException("Email body is empty");

                var email = new QueuedEmail
                {
                    FromName = _config.PlatformSettings.Name,
                    From = _config.PlatformSettings.Email,
                    //ToName = _userService.GetUserFullName(user),
                    ToName = user.Email,
                    To = user.Email,
                    Subject = model.SendEmail.Subject,
                    Body = model.SendEmail.Body,
                    CreatedOnUtc = DateTime.UtcNow
                };
                _queuedEmailService.InsertQueuedEmail(email);

                _notificationService.SuccessNotification("The email has been queued successfully.");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc.Message);
            }

            return RedirectToAction("Edit", new { id = user.Id });
        }
        
        [HttpPost, ActionName("Edit")]
        [FormValueRequired("impersonate")]
        public virtual IActionResult Impersonate(int id)
        {
            //try to get a user with the specified id
            var user = _userService.GetUserById(id);
            if (user == null)
                return RedirectToAction("List");

            _genericAttributeService.SaveAttribute<int?>(_workContext.CurrentUser, AldanUserDefaults.ImpersonatedUserIdAttribute, user.Id);

            return RedirectToAction("Index", "Home", new { area = string.Empty });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("send-welcome-message")]
        public virtual IActionResult SendWelcomeMessage(UserModel model)
        {
            //try to get a user with the specified id
            var user = _userService.GetUserById(model.Id);
            if (user == null)
                return RedirectToAction("List");

            _workflowMessageService.SendUserWelcomeMessage(user);

            _notificationService.SuccessNotification("Welcome email has been successfully sent.");

            return RedirectToAction("Edit", new { id = user.Id });
        }

        #endregion
    }
}