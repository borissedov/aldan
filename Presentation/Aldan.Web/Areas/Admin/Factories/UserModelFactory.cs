using System;
using System.Linq;
using Aldan.Core.Domain.Users;
using Aldan.Services.Common;
using Aldan.Services.Users;
using Aldan.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Aldan.Web.Areas.Admin.Models.Users;
using Aldan.Web.Framework.Models.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Aldan.Web.Areas.Admin.Factories
{
    public class UserModelFactory : IUserModelFactory
    {
        private readonly IUserService _userService;
        private readonly IGenericAttributeService _genericAttributeService;

        public UserModelFactory(IUserService userService, IGenericAttributeService genericAttributeService)
        {
            _userService = userService;
            _genericAttributeService = genericAttributeService;
        }

        #region Methods

        /// <summary>
        /// Prepare user search model
        /// </summary>
        /// <param name="searchModel">User search model</param>
        /// <returns>User search model</returns>
        public virtual UserSearchModel PrepareUserSearchModel(UserSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //search registered users by default
            searchModel.SelectedUserRoleIds.Add((int) Role.User);

            //prepare available user roles
            var availableRoles = new[] {Role.User, Role.Admin};
            searchModel.AvailableUserRoles = availableRoles.Select(role => new SelectListItem
            {
                Text = role.ToString(),
                Value = ((int) role).ToString(),
                Selected = searchModel.SelectedUserRoleIds.Contains((int) role)
            }).ToList();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged user list model
        /// </summary>
        /// <param name="searchModel">User search model</param>
        /// <returns>User list model</returns>
        public virtual UserListModel PrepareUserListModel(UserSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get users
            var users = _userService.GetAllUsers(userRoleIds: searchModel.SelectedUserRoleIds.ToArray(),
                email: searchModel.SearchEmail,
                firstName: searchModel.SearchFirstName,
                lastName: searchModel.SearchLastName,
                ipAddress: searchModel.SearchIpAddress,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new UserListModel().PrepareToGrid(searchModel, users, () =>
            {
                return users.Select(user =>
                {
                    //fill in model values from the entity
                    var userModel = user.ToModel<UserModel>();

                    //convert dates to the user time
                    userModel.Email = user.Email;
                    userModel.FullName = _userService.GetUserFullName(user);

                    userModel.CreatedOn = user.CreatedOnUtc;
                    userModel.LastActivityDate = user.LastActivityDateUtc;

                    userModel.RoleName = user.Role.ToString();

                    return userModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare user model
        /// </summary>
        /// <param name="model">User model</param>
        /// <param name="user">User</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>User model</returns>
        public virtual UserModel PrepareUserModel(UserModel model, User user, bool excludeProperties = false)
        {
            if (user != null)
            {
                //fill in model values from the entity
                model = model ?? new UserModel();

                model.Id = user.Id;


                //whether to fill in some of properties
                if (!excludeProperties)
                {
                    model.Email = user.Email;

                    model.FirstName =
                        _genericAttributeService.GetAttribute<string>(user, AldanUserDefaults.FirstNameAttribute);
                    model.LastName =
                        _genericAttributeService.GetAttribute<string>(user, AldanUserDefaults.LastNameAttribute);

                    model.CreatedOn = user.CreatedOnUtc;
                    model.LastActivityDate = user.LastActivityDateUtc;
                    model.LastIpAddress = user.LastIpAddress;
                    model.LastVisitedPage =
                        _genericAttributeService.GetAttribute<string>(user, AldanUserDefaults.LastVisitedPageAttribute);

                    model.SelectedUserRoleId = (int) user.Role;
                }
            }
            else
            {
                //whether to fill in some of properties
                if (!excludeProperties)
                {
                    //precheck Registered Role as a default role while creating a new user through admin
                    model.SelectedUserRoleId = (int) Role.User;
                }
            }

            //prepare available user roles
            var availableRoles = new[] {Role.User, Role.Admin};
            model.AvailableUserRoles = availableRoles.Select(role => new SelectListItem
            {
                Text = role.ToString(),
                Value = ((int) role).ToString(),
                Selected = model.SelectedUserRoleId == (int) role
            }).ToList();
            return model;
        }

        #endregion
    }
}