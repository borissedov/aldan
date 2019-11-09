using Aldan.Core.Domain.Logging;
using Aldan.Core.Domain.Tasks;
using Aldan.Core.Domain.Users;
using Aldan.Core.Infrastructure.Mapper;
using Aldan.Web.Areas.Admin.Models.Logging;
using Aldan.Web.Areas.Admin.Models.Tasks;
using Aldan.Web.Areas.Admin.Models.Users;
using AutoMapper;

namespace Aldan.Web.Areas.Admin.Infrastructure.Mapper
{
    /// <summary>
    /// AutoMapper configuration for admin area models
    /// </summary>
    public class AdminMapperConfiguration : Profile, IOrderedMapperProfile
    {
        #region Ctor

        public AdminMapperConfiguration()
        {

            CreateLoggingMaps();
            CreateUsersMaps();
            CreateTasksMaps();
        }



        #endregion

        #region Utilities


        /// <summary>
        /// Create users maps 
        /// </summary>
        protected virtual void CreateUsersMaps()
        {
            CreateMap<User, UserModel>()
                .ForMember(model => model.Email, options => options.Ignore())
                .ForMember(model => model.FullName, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.LastActivityDate, options => options.Ignore())
                .ForMember(model => model.Password, options => options.Ignore())
                .ForMember(model => model.LastVisitedPage, options => options.Ignore())
                .ForMember(model => model.SendEmail, options => options.Ignore())
                .ForMember(model => model.FirstName, options => options.Ignore())
                .ForMember(model => model.LastName, options => options.Ignore());

            CreateMap<UserModel, User>()
                .ForMember(entity => entity.UserGuid, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.LastActivityDateUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.LastLoginDateUtc, options => options.Ignore());

            CreateMap<User, OnlineUserModel>()
                .ForMember(model => model.LastActivityDate, options => options.Ignore())
                .ForMember(model => model.UserInfo, options => options.Ignore())
                .ForMember(model => model.LastIpAddress, options => options.Ignore())
                .ForMember(model => model.Location, options => options.Ignore())
                .ForMember(model => model.LastVisitedPage, options => options.Ignore());
        }
        
        /// <summary>
        /// Create logging maps 
        /// </summary>
        protected virtual void CreateLoggingMaps()
        {
            CreateMap<Log, LogModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.FullMessage, options => options.Ignore())
                .ForMember(model => model.UserEmail, options => options.Ignore());
            CreateMap<LogModel, Log>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.User, options => options.Ignore())
                .ForMember(entity => entity.LogLevelId, options => options.Ignore());
        }

        
        /// <summary>
        /// Create tasks maps 
        /// </summary>
        protected virtual void CreateTasksMaps()
        {
            CreateMap<ScheduleTask, ScheduleTaskModel>();
            CreateMap<ScheduleTaskModel, ScheduleTask>()
                .ForMember(entity => entity.Type, options => options.Ignore())
                .ForMember(entity => entity.LastStartUtc, options => options.Ignore())
                .ForMember(entity => entity.LastEndUtc, options => options.Ignore())
                .ForMember(entity => entity.LastSuccessUtc, options => options.Ignore());
        }

        
        #endregion

        #region Properties

        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order => 0;

        #endregion
    }
}