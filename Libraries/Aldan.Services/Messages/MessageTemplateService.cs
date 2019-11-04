using System;
using System.Collections.Generic;
using System.Linq;
using Aldan.Core.Caching;
using Aldan.Core.Data;
using Aldan.Core.Domain.Messages;
using Aldan.Services.Events;

namespace Aldan.Services.Messages
{
    /// <summary>
    /// Message template service
    /// </summary>
    public partial class MessageTemplateService : IMessageTemplateService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<MessageTemplate> _messageTemplateRepository;

        #endregion

        #region Ctor

        public MessageTemplateService(
            ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IRepository<MessageTemplate> messageTemplateRepository)
        {
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _messageTemplateRepository = messageTemplateRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        public virtual void DeleteMessageTemplate(MessageTemplate messageTemplate)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException(nameof(messageTemplate));

            _messageTemplateRepository.Delete(messageTemplate);

            _cacheManager.RemoveByPrefix(AldanMessageDefaults.MessageTemplatesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(messageTemplate);
        }

        /// <summary>
        /// Inserts a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        public virtual void InsertMessageTemplate(MessageTemplate messageTemplate)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException(nameof(messageTemplate));

            _messageTemplateRepository.Insert(messageTemplate);

            _cacheManager.RemoveByPrefix(AldanMessageDefaults.MessageTemplatesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(messageTemplate);
        }

        /// <summary>
        /// Updates a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        public virtual void UpdateMessageTemplate(MessageTemplate messageTemplate)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException(nameof(messageTemplate));

            _messageTemplateRepository.Update(messageTemplate);

            _cacheManager.RemoveByPrefix(AldanMessageDefaults.MessageTemplatesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(messageTemplate);
        }

        /// <summary>
        /// Gets a message template
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <returns>Message template</returns>
        public virtual MessageTemplate GetMessageTemplateById(int messageTemplateId)
        {
            if (messageTemplateId == 0)
                return null;

            return _messageTemplateRepository.GetById(messageTemplateId);
        }

        /// <summary>
        /// Gets message templates by the name
        /// </summary>
        /// <param name="messageTemplateName">Message template name</param>
        /// <returns>List of message templates</returns>
        public virtual IList<MessageTemplate> GetMessageTemplatesByName(string messageTemplateName)
        {
            if (string.IsNullOrWhiteSpace(messageTemplateName))
                throw new ArgumentException(nameof(messageTemplateName));

            var key = string.Format(AldanMessageDefaults.MessageTemplatesByNameCacheKey, messageTemplateName);
            return _cacheManager.Get(key, () =>
            {
                //get message templates with the passed name
                var templates = _messageTemplateRepository.Table
                    .Where(messageTemplate => messageTemplate.Name.Equals(messageTemplateName))
                    .OrderBy(messageTemplate => messageTemplate.Id).ToList();

                return templates;
            });
        }

        /// <summary>
        /// Gets all message templates
        /// </summary>
        /// <returns>Message template list</returns>
        public virtual IList<MessageTemplate> GetAllMessageTemplates()
        {
            return _cacheManager.Get(AldanMessageDefaults.MessageTemplatesAllCacheKey, () =>
            {
                var query = _messageTemplateRepository.Table;
                query = query.OrderBy(t => t.Name);
                
                    return query.ToList();
            });
        }

        /// <summary>
        /// Create a copy of message template with all depended data
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>Message template copy</returns>
        public virtual MessageTemplate CopyMessageTemplate(MessageTemplate messageTemplate)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException(nameof(messageTemplate));

            var mtCopy = new MessageTemplate
            {
                Name = messageTemplate.Name,
                BccEmailAddresses = messageTemplate.BccEmailAddresses,
                Subject = messageTemplate.Subject,
                Body = messageTemplate.Body,
                IsActive = messageTemplate.IsActive
            };

            InsertMessageTemplate(mtCopy);

            return mtCopy;
        }

        #endregion
    }
}