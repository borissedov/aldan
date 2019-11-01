using Aldan.Core.Domain.Customers;

namespace Aldan.Services.Customers
{
    /// <summary>
    /// Customer registration request
    /// </summary>
    public class CustomerRegistrationRequest
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="email">Email</param>
        /// <param name="password">Password</param>
        public CustomerRegistrationRequest(Customer customer, string email, 
            string password)
        {
            Customer = customer;
            Email = email;
            Password = password;
        }

        /// <summary>
        /// Customer
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }
}
