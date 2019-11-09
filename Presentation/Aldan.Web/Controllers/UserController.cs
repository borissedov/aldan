using System;
using System.Linq;
using Aldan.Core;
using Aldan.Core.Domain.Users;
using Aldan.Services.Authentication;
using Aldan.Services.Common;
using Aldan.Services.Events;
using Aldan.Services.Messages;
using Aldan.Services.Users;
using Aldan.Web.Factories;
using Aldan.Web.Framework;
using Aldan.Web.Framework.Controllers;
using Aldan.Web.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace Aldan.Web.Controllers
{
    public partial class UserController : BaseController
    {
        #region Fields

        private readonly IAuthenticationService _authenticationService;
        private readonly IUserModelFactory _userModelFactory;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserService _userService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public UserController(
            IAuthenticationService authenticationService,
            IUserRegistrationService userRegistrationService,
            IUserService userService,
            IEventPublisher eventPublisher,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService, 
            IUserModelFactory userModelFactory, IGenericAttributeService genericAttributeService)
        {
            _authenticationService = authenticationService;
            _userRegistrationService = userRegistrationService;
            _userService = userService;
            _eventPublisher = eventPublisher;
            _webHelper = webHelper;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _userModelFactory = userModelFactory;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        #region Login / logout

        public virtual IActionResult Login()
        {
            var model = _userModelFactory.PrepareLoginModel();
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var loginResult =
                    _userRegistrationService.ValidateUser(
                        model.Email, model.Password);
                switch (loginResult)
                {
                    case UserLoginResults.Successful:
                    {
                        var user = _userService.GetUserByEmail(model.Email);

                        //sign in new user
                        _authenticationService.SignIn(user, model.RememberMe);

                        //raise event       
                        _eventPublisher.Publish(new UserLoggedinEvent(user));

                        if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                            return RedirectToRoute("Homepage");

                        return Redirect(returnUrl);
                    }
                    case UserLoginResults.UserNotExist:
                        ModelState.AddModelError("",
                            "No user account found");
                        break;
                    case UserLoginResults.Deleted:
                        ModelState.AddModelError("",
                            "User is deleted");
                        break;
                    case UserLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("",
                            "The credentials provided are incorrect");
                        break;
                }
            }

            //If we got this far, something failed, redisplay form
            model = _userModelFactory.PrepareLoginModel();
            return View(model);
        }

        public virtual IActionResult Logout()
        {
            if (_workContext.OriginalUserIfImpersonated != null)
            {
                //logout impersonated user
                _genericAttributeService
                    .SaveAttribute<int?>(_workContext.OriginalUserIfImpersonated, AldanUserDefaults.ImpersonatedUserIdAttribute, null);

                //redirect back to user details page (admin area)
                return RedirectToAction("Edit", "User", new {id = _workContext.CurrentUser.Id, area = AreaNames.Admin});
            }

            //standard logout 
            _authenticationService.SignOut();

            //raise logged out event       
            _eventPublisher.Publish(new UserLoggedOutEvent(_workContext.CurrentUser));

            return RedirectToRoute("Homepage");
        }

        #endregion

        #region Password recovery

        public virtual IActionResult PasswordRecovery()
        {
            var model = _userModelFactory.PreparePasswordRecoveryModel();
            return View(model);
        }

        [HttpPost, ActionName("PasswordRecovery")]
        [FormValueRequired("send-email")]
        public virtual IActionResult PasswordRecoverySend(PasswordRecoveryModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetUserByEmail(model.Email);
                if (user != null && !user.Deleted)
                {
                    //save token and current date
                    var passwordRecoveryToken = Guid.NewGuid();
                    _genericAttributeService.SaveAttribute(user, AldanUserDefaults.PasswordRecoveryTokenAttribute,
                        passwordRecoveryToken.ToString());
                    DateTime? generatedDateTime = DateTime.UtcNow;
                    _genericAttributeService.SaveAttribute(user,
                        AldanUserDefaults.PasswordRecoveryTokenDateGeneratedAttribute, generatedDateTime);

                    //send email
                    _workflowMessageService.SendUserPasswordRecoveryMessage(user);

                    model.Result = "Email with instructions has been sent to you.";
                }
                else
                {
                    model.Result = "Email not found.";
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult PasswordRecoveryConfirm(string token, string email)
        {
            var user = _userService.GetUserByEmail(email);
            if (user == null)
                return RedirectToRoute("Homepage");

            if (string.IsNullOrEmpty(_genericAttributeService.GetAttribute<string>(user, AldanUserDefaults.PasswordRecoveryTokenAttribute)))
            {
                return View(new PasswordRecoveryConfirmModel
                {
                    DisablePasswordChanging = true,
                    Result = "Your password already has been changed. For changing it once more, you need to again recover the password."
                });
            }

            var model = _userModelFactory.PreparePasswordRecoveryConfirmModel();

            //validate token
            if (!_userService.IsPasswordRecoveryTokenValid(user, token))
            {
                model.DisablePasswordChanging = true;
                model.Result = "Wrong password recovery token";
            }

            //validate token expiration date
            if (_userService.IsPasswordRecoveryLinkExpired(user))
            {
                model.DisablePasswordChanging = true;
                model.Result = "Your password recovery link is expired";
            }

            return View(model);
        }

        [HttpPost, ActionName("PasswordRecoveryConfirm")]
        [FormValueRequired("set-password")]
        public virtual IActionResult PasswordRecoveryConfirmPOST(string token, string email,
            PasswordRecoveryConfirmModel model)
        {
            var user = _userService.GetUserByEmail(email);
            if (user == null)
                return RedirectToRoute("Homepage");

            //validate token
            if (!_userService.IsPasswordRecoveryTokenValid(user, token))
            {
                model.DisablePasswordChanging = true;
                model.Result = "Wrong password recovery token";
                return View(model);
            }

            //validate token expiration date
            if (_userService.IsPasswordRecoveryLinkExpired(user))
            {
                model.DisablePasswordChanging = true;
                model.Result = "Your password recovery link is expired";
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var response = _userRegistrationService.ChangePassword(new ChangePasswordRequest(email,
                    false, model.NewPassword));
                if (response.Success)
                {
                    _genericAttributeService.SaveAttribute(user, AldanUserDefaults.PasswordRecoveryTokenAttribute, "");
                    model.DisablePasswordChanging = true;
                    model.Result = "Your password has been changed";
                }
                else
                {
                    model.Result = response.Errors.FirstOrDefault();
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Register

        public virtual IActionResult Register()
        {
            var model = new RegisterModel();
            model = _userModelFactory.PrepareRegisterModel(model);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Register(RegisterModel model, string returnUrl)
        {
            if (_workContext.CurrentUser != null)
            {
                //Already registered user. 
                _authenticationService.SignOut();

                //raise logged out event       
                _eventPublisher.Publish(new UserLoggedOutEvent(_workContext.CurrentUser));
            }
            
            //Save a new record
            _workContext.CurrentUser = new User
            {
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow
            };

            var user = _workContext.CurrentUser;

            if (ModelState.IsValid)
            {
                var registrationRequest = new UserRegistrationRequest(user,
                    model.Email,
                    model.Password);
                var registrationResult = _userRegistrationService.RegisterUser(registrationRequest);
                if (registrationResult.Success)
                {
                    _genericAttributeService.SaveAttribute(user, AldanUserDefaults.FirstNameAttribute, model.FirstName);
                    _genericAttributeService.SaveAttribute(user, AldanUserDefaults.LastNameAttribute, model.LastName);
                    
                    _authenticationService.SignIn(user, true);

                    _workflowMessageService.SendUserRegisteredNotificationMessage(user);

                    //raise event       
                    _eventPublisher.Publish(new UserRegisteredEvent(user));

                    //send user welcome message
                    _workflowMessageService.SendUserWelcomeMessage(user);

                    var redirectUrl = Url.RouteUrl("RegisterResult",
                        new {returnUrl},
                        _webHelper.CurrentRequestProtocol);
                    return Redirect(redirectUrl);
                }

                //errors
                foreach (var error in registrationResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            model = _userModelFactory.PrepareRegisterModel(model);
            return View(model);
        }

        public virtual IActionResult RegisterResult()
        {
            return View();
        }

        [HttpPost]
        public virtual IActionResult RegisterResult(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                return RedirectToRoute("Homepage");

            return Redirect(returnUrl);
        }

        #endregion

        #region My account / Change password

        public virtual IActionResult ChangePassword()
        {
            if (_workContext.CurrentUser == null)
                return Challenge();

            var model = _userModelFactory.PrepareChangePasswordModel();
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ChangePassword(ChangePasswordModel model)
        {
            if (_workContext.CurrentUser == null)
                return Challenge();

            var user = _workContext.CurrentUser;

            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(user.Email,
                    true, model.NewPassword, model.OldPassword);
                var changePasswordResult = _userRegistrationService.ChangePassword(changePasswordRequest);
                if (changePasswordResult.Success)
                {
                    model.Result = "Password was changed";
                    return View(model);
                }

                //errors
                foreach (var error in changePasswordResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #endregion
    }
}