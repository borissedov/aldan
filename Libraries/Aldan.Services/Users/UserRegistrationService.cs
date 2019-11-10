using System;
using Aldan.Core;
using Aldan.Core.Domain.Users;
using Aldan.Services.Events;
using Aldan.Services.Security;

namespace Aldan.Services.Users
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly IUserService _userService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IEncryptionService _encryptionService;

        public UserRegistrationService(
            IUserService userService,
            IEventPublisher eventPublisher, 
            IEncryptionService encryptionService)
        {
            _userService = userService;
            _eventPublisher = eventPublisher;
            _encryptionService = encryptionService;
        }

        #region Utilities

        /// <summary>
        /// Check whether the entered password matches with a saved one
        /// </summary>
        /// <param name="salt"></param>
        /// <param name="enteredPassword">The entered password</param>
        /// <param name="password"></param>
        /// <returns>True if passwords match; otherwise false</returns>
        protected bool PasswordsMatch(string password, string salt, string enteredPassword)
        {
            if (string.IsNullOrEmpty(enteredPassword))
                return false;

            var savedPassword = _encryptionService.CreatePasswordHash(enteredPassword, salt);

            if (string.IsNullOrEmpty(password))
                return false;

            return password.Equals(savedPassword);
        }

        #endregion

        public UserLoginResults ValidateUser(string usernameOrEmail, string password)
        {
            var user = _userService.GetUserByEmail(usernameOrEmail);

            if (user == null)
                return UserLoginResults.UserNotExist;
            if (user.Deleted)
                return UserLoginResults.Deleted;

            if (!PasswordsMatch(user.Password, user.PasswordSalt, password))
            {
                return UserLoginResults.WrongPassword;
            }

            user.LastLoginDateUtc = DateTime.UtcNow;
            _userService.UpdateUser(user);

            return UserLoginResults.Successful;
        }

        public UserRegistrationResult RegisterUser(UserRegistrationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.User == null)
                throw new ArgumentException("Can't load current user");

            var result = new UserRegistrationResult();

            if (string.IsNullOrEmpty(request.Email))
            {
                result.AddError("Email is required.");
                return result;
            }

            if (!CommonHelper.IsValidEmail(request.Email))
            {
                result.AddError("Wrong email");
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                result.AddError("Password is not provided");
                return result;
            }

            //validate unique user
            if (_userService.GetUserByEmail(request.Email) != null)
            {
                result.AddError("The specified email already exists");
                return result;
            }

            //at this point request is valid
            request.User.Email = request.Email;

            var saltKey = _encryptionService.CreateSaltKey(AldanUserServiceDefaults.PasswordSaltKeySize);
            request.User.PasswordSalt = saltKey;
            request.User.Password = _encryptionService.CreatePasswordHash(request.Password, saltKey);

            request.User.Role = Role.User;

            _userService.UpdateUser(request.User);

            return result;
        }

        public ChangePasswordResult ChangePassword(ChangePasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var result = new ChangePasswordResult();
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                result.AddError("Email is not entered");
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError("Password is not entered");
                return result;
            }

            var user = _userService.GetUserByEmail(request.Email);
            if (user == null)
            {
                result.AddError("The specified email could not be found");
                return result;
            }

            //request isn't valid
            if (request.ValidateRequest &&
                !PasswordsMatch(user.Password, user.PasswordSalt, request.OldPassword))
            {
                result.AddError("Old password doesn't match");
                return result;
            }

            var saltKey = _encryptionService.CreateSaltKey(AldanUserServiceDefaults.PasswordSaltKeySize);
            user.PasswordSalt = saltKey;
            user.Password = _encryptionService.CreatePasswordHash(request.NewPassword, saltKey);

            _userService.UpdateUser(user);

            //publish event
            _eventPublisher.Publish(new UserPasswordChangedEvent(user.Id, request.NewPassword));

            return result;
        }

        public void SetEmail(User user, string newEmail, bool requireValidation)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (newEmail == null)
                throw new AldanException("Email cannot be null");

            newEmail = newEmail.Trim();
            var oldEmail = user.Email;

            if (!CommonHelper.IsValidEmail(newEmail))
                throw new AldanException("New email is not valid");

            if (newEmail.Length > 100)
                throw new AldanException("E-mail address is too long");

            var user2 = _userService.GetUserByEmail(newEmail);
            if (user2 != null && user.Id != user2.Id)
                throw new AldanException("The e-mail address is already in use");

            user.Email = newEmail;
            _userService.UpdateUser(user);
        }
    }
}