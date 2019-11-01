namespace Aldan.Services.Customers
{
    /// <summary>
    /// Represents the customer login result enumeration
    /// </summary>
    public enum CustomerLoginResults
    {
        /// <summary>
        /// Login successful
        /// </summary>
        Successful = 1,

        /// <summary>
        /// Customer does not exist (email or username)
        /// </summary>
        CustomerNotExist = 2,

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
