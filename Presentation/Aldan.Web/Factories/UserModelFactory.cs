using Aldan.Web.Models.User;

namespace Aldan.Web.Factories
{
    public class UserModelFactory : IUserModelFactory
    {
        public RegisterModel PrepareRegisterModel(RegisterModel model)
        {
            return model;
        }

        public LoginModel PrepareLoginModel()
        {
            return new LoginModel();
        }

        public PasswordRecoveryModel PreparePasswordRecoveryModel()
        {
            return new PasswordRecoveryModel();
        }

        public PasswordRecoveryConfirmModel PreparePasswordRecoveryConfirmModel()
        {
            return new PasswordRecoveryConfirmModel();
        }

        public ChangePasswordModel PrepareChangePasswordModel()
        {
            return new ChangePasswordModel();
        }
    }
}