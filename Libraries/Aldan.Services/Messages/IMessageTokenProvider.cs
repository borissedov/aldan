using System.Collections.Generic;
using Aldan.Core.Domain.Customers;
using Aldan.Core.Domain.Messages;

namespace Aldan.Services.Messages
{
    /// <summary>
    /// Message token provider
    /// </summary>
    public partial interface IMessageTokenProvider
    {
        /// <summary>
        /// Add platform tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        void AddPlatformTokens(IList<Token> tokens);

        /// <summary>
        /// Add customer tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="customer">Customer</param>
        void AddCustomerTokens(IList<Token> tokens, Customer customer);

        /// <summary>
        /// Get collection of allowed (supported) message tokens
        /// </summary>
        /// <param name="tokenGroups">Collection of token groups; pass null to get all available tokens</param>
        /// <returns>Collection of allowed message tokens</returns>
        IEnumerable<string> GetListOfAllowedTokens(IEnumerable<string> tokenGroups = null);

        /// <summary>
        /// Get token groups of message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>Collection of token group names</returns>
        IEnumerable<string> GetTokenGroups(MessageTemplate messageTemplate);
    }
}