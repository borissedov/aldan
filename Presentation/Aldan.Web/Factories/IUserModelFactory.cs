using Aldan.Web.Models.User;

namespace Aldan.Web.Factories
{
    public interface IUserModelFactory
    {
        /// <summary>
        /// Prepare the user register model
        /// </summary>
        /// <param name="model">User register model</param>
        /// <returns>User register model</returns>
        RegisterModel PrepareRegisterModel(RegisterModel model);

        /// <summary>
        /// Prepare the login model
        /// </summary>
        /// <returns>Login model</returns>
        LoginModel PrepareLoginModel();

        /// <summary>
        /// Prepare the password recovery model
        /// </summary>
        /// <returns>Password recovery model</returns>
        PasswordRecoveryModel PreparePasswordRecoveryModel();

        /// <summary>
        /// Prepare the password recovery confirm model
        /// </summary>
        /// <returns>Password recovery confirm model</returns>
        PasswordRecoveryConfirmModel PreparePasswordRecoveryConfirmModel();

        /// <summary>
        /// Prepare the change password model
        /// </summary>
        /// <returns>Change password model</returns>
        ChangePasswordModel PrepareChangePasswordModel();
    }
}