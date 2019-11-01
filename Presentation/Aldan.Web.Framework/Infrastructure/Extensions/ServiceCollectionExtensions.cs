using System;
using System.Linq;
using System.Net;
using Aldan.Core;
using Aldan.Core.Configuration;
using Aldan.Core.Http;
using Aldan.Core.Infrastructure;
using Aldan.Data;
using Aldan.Services.Authentication;
using Aldan.Services.Logging;
using Aldan.Services.Tasks;
using EasyCaching.Core;
using EasyCaching.InMemory;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using StackExchange.Profiling.Storage;

namespace Aldan.Web.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        /// <param name="hostingEnvironment">Hosting environment</param>
        /// <returns>Configured service provider</returns>
        public static IServiceProvider ConfigureApplicationServices(this IServiceCollection services,
            IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            //most of API providers require TLS 1.2 nowadays
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //add AldanConfig configuration parameters
            var aldanConfig = services.ConfigureStartupConfig<AldanConfig>(configuration.GetSection("Aldan"));

            //add accessor to HttpContext
            services.AddHttpContextAccessor();

            //create default file provider
            CommonHelper.DefaultFileProvider = new AldanFileProvider(hostingEnvironment);

            //initialize plugins
            var mvcCoreBuilder = services.AddMvcCore();

            //create engine and configure service provider
            var engine = EngineContext.Create();
            var serviceProvider = engine.ConfigureServices(services, configuration, aldanConfig);

            //initialize and start schedule tasks
            TaskManager.Instance.Initialize();
            TaskManager.Instance.Start();

            //log application start
            engine.Resolve<ILogger>().Information("Application started");

            return serviceProvider;
        }

        /// <summary>
        /// Create, bind and register as service the specified configuration parameters 
        /// </summary>
        /// <typeparam name="TConfig">Configuration parameters</typeparam>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Set of key/value application configuration properties</param>
        /// <returns>Instance of configuration parameters</returns>
        public static TConfig ConfigureStartupConfig<TConfig>(this IServiceCollection services, IConfiguration configuration) where TConfig : class, new()
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            //create instance of config
            var config = new TConfig();

            //bind it to the appropriate section of configuration
            configuration.Bind(config);

            //and register it as a service
            services.AddSingleton(config);

            return config;
        }

        /// <summary>
        /// Register HttpContextAccessor
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        /// <summary>
        /// Adds services required for anti-forgery support
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddAntiForgery(this IServiceCollection services)
        {
            //override cookie name
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = $"{AldanCookieDefaults.Prefix}{AldanCookieDefaults.AntiforgeryCookie}";

                //whether to allow the use of anti-forgery cookies from SSL protected page on the other store pages which are not
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            });
        }

        /// <summary>
        /// Adds services required for application session state
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddHttpSession(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.Cookie.Name = $"{AldanCookieDefaults.Prefix}{AldanCookieDefaults.SessionCookie}";
                options.Cookie.HttpOnly = true;

                //whether to allow the use of session values from SSL protected page on the other store pages which are not
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            });
        }

        /// <summary>
        /// Adds data protection services
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddAldanDataProtection(this IServiceCollection services)
        {
            var dataProtectionKeysPath = CommonHelper.DefaultFileProvider.MapPath("~/App_Data/DataProtectionKeys");
            var dataProtectionKeysFolder = new System.IO.DirectoryInfo(dataProtectionKeysPath);

            //configure the data protection system to persist keys to the specified directory
            services.AddDataProtection().PersistKeysToFileSystem(dataProtectionKeysFolder);
        }

        /// <summary>
        /// Adds authentication service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddAldanAuthentication(this IServiceCollection services)
        {
            //set default authentication schemes
            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = AldanAuthenticationDefaults.AuthenticationScheme;
                options.DefaultScheme = AldanAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = AldanAuthenticationDefaults.ExternalAuthenticationScheme;
            });

            //add main cookie authentication
            authenticationBuilder.AddCookie(AldanAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = $"{AldanCookieDefaults.Prefix}{AldanCookieDefaults.AuthenticationCookie}";
                options.Cookie.HttpOnly = true;
                options.LoginPath = AldanAuthenticationDefaults.LoginPath;
                options.AccessDeniedPath = AldanAuthenticationDefaults.AccessDeniedPath;

                //whether to allow the use of authentication cookies from SSL protected page on the other store pages which are not
                options.Cookie.SecurePolicy =  CookieSecurePolicy.None;
            });

            //add external authentication
            authenticationBuilder.AddCookie(AldanAuthenticationDefaults.ExternalAuthenticationScheme, options =>
            {
                options.Cookie.Name = $"{AldanCookieDefaults.Prefix}{AldanCookieDefaults.ExternalAuthenticationCookie}";
                options.Cookie.HttpOnly = true;
                options.LoginPath = AldanAuthenticationDefaults.LoginPath;
                options.AccessDeniedPath = AldanAuthenticationDefaults.AccessDeniedPath;

                //whether to allow the use of authentication cookies from SSL protected page on the other store pages which are not
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            });

//            //register and configure external authentication plugins now
//            var typeFinder = new WebAppTypeFinder();
//            var externalAuthConfigurations = typeFinder.FindClassesOfType<IExternalAuthenticationRegistrar>();
//            var externalAuthInstances = externalAuthConfigurations
//                .Select(x => (IExternalAuthenticationRegistrar)Activator.CreateInstance(x));
//
//            foreach (var instance in externalAuthInstances)
//                instance.Configure(authenticationBuilder);
        }

        /// <summary>
        /// Add and configure MVC for the application
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <returns>A builder for configuring MVC services</returns>
        public static IMvcBuilder AddAldanMvc(this IServiceCollection services)
        {
            //add basic MVC feature
            var mvcBuilder = services.AddMvc();

            //we use legacy (from previous versions) routing logic
            mvcBuilder.AddMvcOptions(options => options.EnableEndpointRouting = false);

            //sets the default value of settings on MvcOptions to match the behavior of asp.net core mvc 2.2
            mvcBuilder.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            //use cookie-based temp data provider
            mvcBuilder.AddCookieTempDataProvider(options =>
            {
                options.Cookie.Name = $"{AldanCookieDefaults.Prefix}{AldanCookieDefaults.TempDataCookie}";

                //whether to allow the use of cookies from SSL protected page on the other store pages which are not
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            });
            
            //MVC now serializes JSON with camel case names by default, use this code to avoid it
            mvcBuilder.AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //add fluent validation
            mvcBuilder.AddFluentValidation(configuration =>
            {
                //register all available validators from Aldan assemblies
                var assemblies = mvcBuilder.PartManager.ApplicationParts
                    .OfType<AssemblyPart>()
                    .Where(part => part.Name.StartsWith("Aldan", StringComparison.InvariantCultureIgnoreCase))
                    .Select(part => part.Assembly);
                configuration.RegisterValidatorsFromAssemblies(assemblies);

                //implicit/automatic validation of child properties
                configuration.ImplicitlyValidateChildProperties = true;
            });

            //register controllers as services, it'll allow to override them
            mvcBuilder.AddControllersAsServices();

            return mvcBuilder;
        }

        /// <summary>
        /// Register base object context
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddAldanObjectContext(this IServiceCollection services)
        {
            services.AddDbContextPool<AldanObjectContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServerWithLazyLoading(services);
            });
        }

        /// <summary>
        /// Add and configure MiniProfiler service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddAldanMiniProfiler(this IServiceCollection services)
        {
            services.AddMiniProfiler(miniProfilerOptions =>
            {
                //use memory cache provider for storing each result
                ((MemoryCacheStorage)miniProfilerOptions.Storage).CacheDuration = TimeSpan.FromMinutes(60);
            }).AddEntityFramework();
        }

        /// <summary>
        /// Add and configure EasyCaching service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddEasyCaching(this IServiceCollection services)
        {
            services.AddEasyCaching(option =>
            {
                //use memory cache
                option.UseInMemory("aldan_memory_cache");
            });
        }
    }
}