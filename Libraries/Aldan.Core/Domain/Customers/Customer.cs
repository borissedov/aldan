using System;

namespace Aldan.Core.Domain.Customers
{
    /// <summary>
    /// Represents a customer
    /// </summary>
    public partial class Customer : BaseEntity
    {
//        private ICollection<ExternalAuthenticationRecord> _externalAuthenticationRecords;

        public Customer()
        {
            CustomerGuid = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the customer GUID
        /// </summary>
        public Guid CustomerGuid { get; set; }

        /// <summary>
        /// Gets or sets the email
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the password salt
        /// </summary>
        public string PasswordSalt { get; set; }
      
        /// <summary>
        /// Gets or sets the last IP address
        /// </summary>
        public string LastIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last login
        /// </summary>
        public DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public DateTime LastActivityDateUtc { get; set; }

        public bool Deleted { get; set; }
        
        public Role Role { get; set; }
        public int? ImpersonatedCustomerId { get; set; }
        public string PasswordRecoveryToken { get; set; }

        #region Navigation properties

//        /// <summary>
//        /// Gets or sets customer generated content
//        /// </summary>
//        public virtual ICollection<ExternalAuthenticationRecord> ExternalAuthenticationRecords
//        {
//            get => _externalAuthenticationRecords ?? (_externalAuthenticationRecords = new List<ExternalAuthenticationRecord>());
//            protected set => _externalAuthenticationRecords = value;
//        }

        #endregion
    }
}