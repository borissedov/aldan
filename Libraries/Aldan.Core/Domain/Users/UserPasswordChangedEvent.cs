namespace Aldan.Core.Domain.Users
{
    /// <summary>
    /// User password changed event
    /// </summary>
    public class UserPasswordChangedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="password">Password</param>
        public UserPasswordChangedEvent(int userId, string password)
        {
            UserId = userId;
            Password = password;
        }

        /// <summary>
        /// User Id
        /// </summary>
        public int UserId { get; }
        
        /// <summary>
        /// User password
        /// </summary>
        public string Password { get; }
    }
}