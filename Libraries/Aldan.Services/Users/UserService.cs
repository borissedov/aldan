using System;
using System.Collections.Generic;
using System.Linq;
using Aldan.Core;
using Aldan.Core.Data;
using Aldan.Core.Domain.Users;
using Aldan.Services.Events;

namespace Aldan.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IEventPublisher _eventPublisher;

        public UserService(
            IRepository<User> userRepository, IEventPublisher eventPublisher)
        {
            _userRepository = userRepository;
            _eventPublisher = eventPublisher;
        }

        public IPagedList<User> GetAllUsers(DateTime? createdFromUtc = null, DateTime? createdToUtc = null, Role[] userRoles = null,
            string email = null, string ipAddress = null, int pageIndex = 0, int pageSize = Int32.MaxValue,
            bool getOnlyTotalCount = false)
        {
            var query = _userRepository.Table;
            if (createdFromUtc.HasValue)
                query = query.Where(c => createdFromUtc.Value <= c.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(c => createdToUtc.Value >= c.CreatedOnUtc);
            
            query = query.Where(c => !c.Deleted);

            if (userRoles != null && userRoles.Length > 0)
                query = query.Where(z => userRoles.Contains(z.Role));

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(c => c.Email.Contains(email));
            
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
    }
}