using Aldan.Core.Domain.Users;

namespace Aldan.Core
{
    /// <summary>
    /// Represents work context
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        User CurrentUser { get; set; }

        /// <summary>
        /// Gets the original user (in case the current one is impersonated)
        /// </summary>
        User OriginalUserIfImpersonated { get; }
    }
}
