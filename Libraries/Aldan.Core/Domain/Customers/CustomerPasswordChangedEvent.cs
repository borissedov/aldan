namespace Aldan.Core.Domain.Customers
{
    /// <summary>
    /// Customer password changed event
    /// </summary>
    public class CustomerPasswordChangedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <param name="password">Password</param>
        public CustomerPasswordChangedEvent(int customerId, string password)
        {
            CustomerId = customerId;
            Password = password;
        }

        /// <summary>
        /// Customer Id
        /// </summary>
        public int CustomerId { get; }
        
        /// <summary>
        /// Customer password
        /// </summary>
        public string Password { get; }
    }
}