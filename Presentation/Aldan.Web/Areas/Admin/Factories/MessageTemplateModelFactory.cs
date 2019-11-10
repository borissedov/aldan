using System;
using System.Linq;
using Aldan.Core.Domain.Messages;
using Aldan.Services.Messages;
using Aldan.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Aldan.Web.Areas.Admin.Models.Messages;
using Aldan.Web.Framework.Models.Extensions;

namespace Aldan.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the message template model factory implementation
    /// </summary>
    public partial class MessageTemplateModelFactory : IMessageTemplateModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IMessageTokenProvider _messageTokenProvider;

        #endregion

        #region Ctor

        public MessageTemplateModelFactory(
            IBaseAdminModelFactory baseAdminModelFactory,
            IMessageTemplateService messageTemplateService,
            IMessageTokenProvider messageTokenProvider)
        {
            _baseAdminModelFactory = baseAdminModelFactory;
            _messageTemplateService = messageTemplateService;
            _messageTokenProvider = messageTokenProvider;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Prepare message template search model
        /// </summary>
        /// <param name="searchModel">Message template search model</param>
        /// <returns>Message template search model</returns>
        public virtual MessageTemplateSearchModel PrepareMessageTemplateSearchModel(MessageTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged message template list model
        /// </summary>
        /// <param name="searchModel">Message template search model</param>
        /// <returns>Message template list model</returns>
        public virtual MessageTemplateListModel PrepareMessageTemplateListModel(MessageTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get message templates
            var messageTemplates = _messageTemplateService
                .GetAllMessageTemplates().ToPagedList(searchModel);

            //prepare list model
            var model = new MessageTemplateListModel().PrepareToGrid(searchModel, messageTemplates, () =>
            {
                return messageTemplates.Select(messageTemplate =>
                {
                    //fill in model values from the entity
                    var messageTemplateModel = messageTemplate.ToModel<MessageTemplateModel>();

                    return messageTemplateModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare message template model
        /// </summary>
        /// <param name="model">Message template model</param>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>Message template model</returns>
        public virtual MessageTemplateModel PrepareMessageTemplateModel(MessageTemplateModel model,
            MessageTemplate messageTemplate)
        {
            if (messageTemplate != null)
            {
                //fill in model values from the entity
                model = model ?? messageTemplate.ToModel<MessageTemplateModel>();
            }

            var allowedTokens = string.Join(", ", _messageTokenProvider.GetListOfAllowedTokens(_messageTokenProvider.GetTokenGroups(messageTemplate)));
            model.AllowedTokens = $"{allowedTokens}{Environment.NewLine}{Environment.NewLine}For conditional expressions use the token %if (your conditions) ... endif%{Environment.NewLine}";

            return model;
        }

        /// <summary>
        /// Prepare test message template model
        /// </summary>
        /// <param name="model">Test message template model</param>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>Test message template model</returns>
        public virtual TestMessageTemplateModel PrepareTestMessageTemplateModel(TestMessageTemplateModel model,
            MessageTemplate messageTemplate)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (messageTemplate == null)
                throw new ArgumentNullException(nameof(messageTemplate));

            model.Id = messageTemplate.Id;

            //filter tokens to the current template
            var subject = messageTemplate.Subject;
            var body = messageTemplate.Body;
            model.Tokens = _messageTokenProvider.GetListOfAllowedTokens()
                .Where(token => subject.Contains(token) || body.Contains(token)).ToList();

            return model;
        }

        #endregion
    }
}