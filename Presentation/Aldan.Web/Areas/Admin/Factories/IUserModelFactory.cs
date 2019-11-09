using Aldan.Core.Domain.Users;
using Aldan.Web.Areas.Admin.Models.Users;

namespace Aldan.Web.Areas.Admin.Factories
{
    public interface IUserModelFactory
    {
        UserSearchModel PrepareUserSearchModel(UserSearchModel searchModel);
        UserListModel PrepareUserListModel(UserSearchModel searchModel);
        UserModel PrepareUserModel(UserModel model, User user, bool excludeProperties = false);
    }
}