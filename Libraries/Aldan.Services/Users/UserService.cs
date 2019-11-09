using System;
using System.Collections.Generic;
using System.Linq;
using Aldan.Core;
using Aldan.Core.Data;
using Aldan.Core.Domain.Common;
using Aldan.Core.Domain.Users;
using Aldan.Services.Common;
using Aldan.Services.Events;

namespace Aldan.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IRepository<GenericAttribute> _gaRepository;

        private const int PasswordRecoveryLinkDaysValid = 7;

        public UserService(
            IRepository<User> userRepository, 
            IEventPublisher eventPublisher, 
            IGenericAttributeService genericAttributeService, 
            IRepository<GenericAttribute> gaRepository)
        {
            _userRepository = userRepository;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _gaRepository = gaRepository;
        }

        public IPagedList<User> GetAllUsers(DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int[] userRoleIds = null, string email = null,  string firstName = null, string lastName = null,
            string ipAddress = null, int pageIndex = 0, int pageSize = int.MaxValue,
            bool getOnlyTotalCount = false)
        {
            var query = _userRepository.Table;
            if (createdFromUtc.HasValue)
                query = query.Where(c => createdFromUtc.Value <= c.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(c => createdToUtc.Value >= c.CreatedOnUtc);
            
            query = query.Where(c => !c.Deleted);

            if (userRoleIds != null && userRoleIds.Length > 0)
                query = query.Where(z => userRoleIds.Contains((int)z.Role));

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(c => c.Email.Contains(email));
            
            if (!string.IsNullOrWhiteSpace(firstName))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == nameof(User) &&
                                z.Attribute.Key == AldanUserDefaults.FirstNameAttribute &&
                                z.Attribute.Value.Contains(firstName))
                    .Select(z => z.User);
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == nameof(User) &&
                                z.Attribute.Key == AldanUserDefaults.LastNameAttribute &&
                                z.Attribute.Value.Contains(lastName))
                    .Select(z => z.User);
            }
            
            //search by IpAddress
            if (!string.IsNullOrWhiteSpace(ipAddress) && CommonHelper.IsValidIpAddress(ipAddress))
            {
                query = query.Where(w => w.LastIpAddress == ipAddress);
            }

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            var users = new PagedList<User>(query, pageIndex, pageSize, getOnlyTotalCount);
            return users;
        }

        public IPagedList<User> GetOnlineUsers(DateTime lastActivityFromUtc, Role[] userRoles, int pageIndex = 0,
            int pageSize = Int32.MaxValue)
        {
            var query = _userRepository.Table;
            query = query.Where(c => lastActivityFromUtc <= c.LastActivityDateUtc);
            query = query.Where(c => !c.Deleted);
            if (userRoles != null && userRoles.Length > 0)
                query = query.Where(c => userRoles.Contains(c.Role));

            query = query.OrderByDescending(c => c.LastActivityDateUtc);
            var users = new PagedList<User>(query, pageIndex, pageSize);
            return users;
        }

        public void DeleteUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.Deleted = true;

            UpdateUser(user);

            //event notification
            _eventPublisher.EntityDeleted(user);
        }

        public User GetUserById(int userId)
        {
            if (userId == 0)
                return null;

            return _userRepository.GetById(userId);
        }

        public IList<User> GetUsersByIds(int[] userIds)
        {
            if (userIds == null || userIds.Length == 0)
                return new List<User>();

            var query = from c in _userRepository.Table
                where userIds.Contains(c.Id) && !c.Deleted
                select c;
            var users = query.ToList();
            //sort by passed identifiers
            var sortedUsers = new List<User>();
            foreach (var id in userIds)
            {
                var user = users.Find(x => x.Id == id);
                if (user != null)
                    sortedUsers.Add(user);
            }

            return sortedUsers;
        }

        public User GetUserByGuid(Guid userGuid)
        {
            if (userGuid == Guid.Empty)
                return null;

            var query = from c in _userRepository.Table
                where c.UserGuid == userGuid
                orderby c.Id
                select c;
            var user = query.FirstOrDefault();
            return user;
        }

        public User GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var query = from c in _userRepository.Table
                orderby c.Id
                where c.Email == email
                select c;
            var user = query.FirstOrDefault();
            return user;
        }

        public void InsertUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _userRepository.Insert(user);

            //event notification
            _eventPublisher.EntityInserted(user);
        }

        public void UpdateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _userRepository.Update(user);

            //event notification
            _eventPublisher.EntityUpdated(user);
        }

        public bool IsPasswordRecoveryTokenValid(User user, string token)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var cPrt = _genericAttributeService.GetAttribute<string>(user, AldanUserDefaults.PasswordRecoveryTokenAttribute);
            if (string.IsNullOrEmpty(cPrt))
                return false;

            if (!cPrt.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;
            
        }

        public bool IsPasswordRecoveryLinkExpired(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var generatedDate =  _genericAttributeService.GetAttribute<DateTime?>(user, AldanUserDefaults.PasswordRecoveryTokenDateGeneratedAttribute);
            if (!generatedDate.HasValue)
                return false;

            var daysPassed = (DateTime.UtcNow - generatedDate.Value).TotalDays;
            if (daysPassed > PasswordRecoveryLinkDaysValid)
                return true;

            return false;        
        }
        
        /// <summary>
        /// Get full name
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>User full name</returns>
        public virtual string GetUserFullName(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var firstName = _genericAttributeService.GetAttribute<string>(user, AldanUserDefaults.FirstNameAttribute);
            var lastName = _genericAttributeService.GetAttribute<string>(user, AldanUserDefaults.LastNameAttribute);

            var fullName = string.Empty;
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
                fullName = $"{firstName} {lastName}";
            else
            {
                if (!string.IsNullOrWhiteSpace(firstName))
                    fullName = firstName;

                if (!string.IsNullOrWhiteSpace(lastName))
                    fullName = lastName;
            }

            return fullName;
        }
    }
}