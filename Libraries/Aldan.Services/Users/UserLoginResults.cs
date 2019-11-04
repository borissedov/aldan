namespace Aldan.Services.Users
{
    /// <summary>
    /// Represents the user login result enumeration
    /// </summary>
    public enum UserLoginResults
    {
        /// <summary>
        /// Login successful
        /// </summary>
        Successful = 1,

        /// <summary>
        /// User does not exist (email or username)
        /// </summary>
        UserNotExist = 2,

        /// <summary>
        /// Wrong password
        /// </summary>
        WrongPassword = 3,
        
        /// <summary>
        /// Deleted
        /// </summary>
        Deleted = 4
    }
}
