using Aldan.Core.Configuration;
using Aldan.Core.Infrastructure;
using Aldan.Core.Infrastructure.DependencyManagement;
using Aldan.Web.Areas.Admin.Factories;
using Autofac;

namespace Aldan.Web.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, AldanConfig config)
        {
            builder.RegisterType<Aldan.Web.Factories.UserModelFactory>().As<Aldan.Web.Factories.IUserModelFactory>().InstancePerLifetimeScope();
            
            builder.RegisterType<Aldan.Web.Areas.Admin.Factories.CommonModelFactory>().As<Aldan.Web.Areas.Admin.Factories.ICommonModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Aldan.Web.Areas.Admin.Factories.UserModelFactory>().As<Aldan.Web.Areas.Admin.Factories.IUserModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<QueuedEmailModelFactory>().As<IQueuedEmailModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<MessageTemplateModelFactory>().As<IMessageTemplateModelFactory>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<Aldan.Web.Factories.CommonModelFactory>().As<Aldan.Web.Factories.ICommonModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<LogModelFactory>().As<ILogModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ScheduleTaskModelFactory>().As<IScheduleTaskModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<BaseAdminModelFactory>().As<IBaseAdminModelFactory>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order => 2;
    }
}
