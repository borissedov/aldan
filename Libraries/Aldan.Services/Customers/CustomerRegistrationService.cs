using System;
using Aldan.Core;
using Aldan.Core.Domain.Customers;
using Aldan.Services.Events;
using Aldan.Services.Security;

namespace Aldan.Services.Customers
{
    public class CustomerRegistrationService : ICustomerRegistrationService
    {
        private readonly ICustomerService _customerService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IEncryptionService _encryptionService;

        public CustomerRegistrationService(
            ICustomerService customerService,
            IEventPublisher eventPublisher, 
            IEncryptionService encryptionService)
        {
            _customerService = customerService;
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

        public CustomerLoginResults ValidateCustomer(string usernameOrEmail, string password)
        {
            var customer = _customerService.GetCustomerByEmail(usernameOrEmail);

            if (customer == null)
                return CustomerLoginResults.CustomerNotExist;
            if (customer.Deleted)
                return CustomerLoginResults.Deleted;

            if (!PasswordsMatch(customer.Password, customer.PasswordSalt, password))
            {
                return CustomerLoginResults.WrongPassword;
            }

            customer.LastLoginDateUtc = DateTime.UtcNow;
            _customerService.UpdateCustomer(customer);

            return CustomerLoginResults.Successful;
        }

        public CustomerRegistrationResult RegisterCustomer(CustomerRegistrationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Customer == null)
                throw new ArgumentException("Can't load current customer");

            var result = new CustomerRegistrationResult();

            if (string.IsNullOrEmpty(request.Email))
            {
                result.AddError("Account.Register.Errors.EmailIsNotProvided");
                return result;
            }

            if (!CommonHelper.IsValidEmail(request.Email))
            {
                result.AddError("Common.WrongEmail");
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                result.AddError("Account.Register.Errors.PasswordIsNotProvided");
                return result;
            }

            //validate unique user
            if (_customerService.GetCustomerByEmail(request.Email) != null)
            {
                result.AddError("Account.Register.Errors.EmailAlreadyExists");
                return result;
            }

            //at this point request is valid
            request.Customer.Email = request.Email;

            var saltKey = _encryptionService.CreateSaltKey(AldanCustomerServiceDefaults.PasswordSaltKeySize);
            request.Customer.PasswordSalt = saltKey;
            request.Customer.Password = _encryptionService.CreatePasswordHash(request.Password, saltKey);

            request.Customer.Role = Role.User;

            _customerService.UpdateCustomer(request.Customer);

            return result;
        }

        public ChangePasswordResult ChangePassword(ChangePasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var result = new ChangePasswordResult();
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                result.AddError("Account.ChangePassword.Errors.EmailIsNotProvided");
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError("Account.ChangePassword.Errors.PasswordIsNotProvided");
                return result;
            }

            var customer = _customerService.GetCustomerByEmail(request.Email);
            if (customer == null)
            {
                result.AddError("Account.ChangePassword.Errors.EmailNotFound");
                return result;
            }

            //request isn't valid
            if (request.ValidateRequest &&
                !PasswordsMatch(customer.Password, customer.PasswordSalt, request.OldPassword))
            {
                result.AddError("Account.ChangePassword.Errors.OldPasswordDoesntMatch");
                return result;
            }

            var saltKey = _encryptionService.CreateSaltKey(AldanCustomerServiceDefaults.PasswordSaltKeySize);
            customer.PasswordSalt = saltKey;
            customer.Password = _encryptionService.CreatePasswordHash(request.NewPassword, saltKey);

            _customerService.UpdateCustomer(customer);

            //publish event
            _eventPublisher.Publish(new CustomerPasswordChangedEvent(customer.Id, request.NewPassword));

            return result;
        }

        public void SetEmail(Customer customer, string newEmail, bool requireValidation)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (newEmail == null)
                throw new AldanException("Email cannot be null");

            newEmail = newEmail.Trim();
            var oldEmail = customer.Email;

            if (!CommonHelper.IsValidEmail(newEmail))
                throw new AldanException("Account.EmailUsernameErrors.NewEmailIsNotValid");

            if (newEmail.Length > 100)
                throw new AldanException("Account.EmailUsernameErrors.EmailTooLong");

            var customer2 = _customerService.GetCustomerByEmail(newEmail);
            if (customer2 != null && customer.Id != customer2.Id)
                throw new AldanException("Account.EmailUsernameErrors.EmailAlreadyExists");

            customer.Email = newEmail;
            _customerService.UpdateCustomer(customer);
        }
    }
}