using Aldan.Web.Framework.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Aldan.Web.Infrastructure
{
    /// <summary>
    /// Represents provider that provided basic routes
    /// </summary>
    public partial class RouteProvider : IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //areas
            routeBuilder.MapRoute(name: "areaRoute", template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            //and default one
            routeBuilder.MapRoute("Default", "{controller}/{action}/{id?}");
            
            //home page
            routeBuilder.MapRoute("Homepage", "",
				new { controller = "Home", action = "Index" });

            //login
            routeBuilder.MapRoute("Login", "login/",
				new { controller = "User", action = "Login" });

            //register
            routeBuilder.MapRoute("Register", "register/",
				new { controller = "User", action = "Register" });

            //logout
            routeBuilder.MapRoute("Logout", "logout/",
				new { controller = "User", action = "Logout" });

            //register result page
            routeBuilder.MapRoute("RegisterResult", "registerresult/",
				new { controller = "User", action = "RegisterResult" });

            //check username availability
            routeBuilder.MapRoute("CheckUsernameAvailability", "user/checkusernameavailability",
				new { controller = "User", action = "CheckUsernameAvailability" });

            //passwordrecovery
            routeBuilder.MapRoute("PasswordRecovery", "passwordrecovery",
				new { controller = "User", action = "PasswordRecovery" });

            //password recovery confirmation
            routeBuilder.MapRoute("PasswordRecoveryConfirm", "passwordrecovery/confirm",
				new { controller = "User", action = "PasswordRecoveryConfirm" });

            //error page
            routeBuilder.MapRoute("Error", "error",
                new { controller = "Common", action = "Error" });

            //page not found
            routeBuilder.MapRoute("PageNotFound", "page-not-found", 
                new { controller = "Common", action = "PageNotFound" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority
        {
            get { return 0; }
        }

        #endregion
    }
}
