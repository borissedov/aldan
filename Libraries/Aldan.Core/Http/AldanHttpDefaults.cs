
namespace Aldan.Core.Http
{
    /// <summary>
    /// Represents default values related to HTTP features
    /// </summary>
    public static partial class AldanHttpDefaults
    {

        /// <summary>
        /// Gets the name of a request item that stores the value that indicates whether the client is being redirected to a new location using POST
        /// </summary>
        public static string IsPostBeingDoneRequestItem => "aldan.IsPOSTBeingDone";
        
        /// <summary>
        /// Gets the name of X-FORWARDED-FOR header
        /// </summary>
        public static string XForwardedForHeader => "X-FORWARDED-FOR";
    }
}