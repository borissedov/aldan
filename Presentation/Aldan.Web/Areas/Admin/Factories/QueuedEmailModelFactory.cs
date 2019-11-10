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
    /// Represents the queued email model factory implementation
    /// </summary>
    public partial class QueuedEmailModelFactory : IQueuedEmailModelFactory
    {
        #region Fields

        private readonly IQueuedEmailService _queuedEmailService;

        #endregion

        #region Ctor

        public QueuedEmailModelFactory(
            IQueuedEmailService queuedEmailService)
        {
            _queuedEmailService = queuedEmailService;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Prepare queued email search model
        /// </summary>
        /// <param name="searchModel">Queued email search model</param>
        /// <returns>Queued email search model</returns>
        public virtual QueuedEmailSearchModel PrepareQueuedEmailSearchModel(QueuedEmailSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare default search values
            searchModel.SearchMaxSentTries = 10;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged queued email list model
        /// </summary>
        /// <param name="searchModel">Queued email search model</param>
        /// <returns>Queued email list model</returns>
        public virtual QueuedEmailListModel PrepareQueuedEmailListModel(QueuedEmailSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter emails
            var startDateValue = searchModel.SearchStartDate?.ToUniversalTime();
            var endDateValue = searchModel.SearchEndDate?.ToUniversalTime().AddDays(1);

            //get queued emails
            var queuedEmails = _queuedEmailService.SearchEmails(fromEmail: searchModel.SearchFromEmail,
                toEmail: searchModel.SearchToEmail,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                loadNotSentItemsOnly: searchModel.SearchLoadNotSent,
                maxSendTries: searchModel.SearchMaxSentTries,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new QueuedEmailListModel().PrepareToGrid(searchModel, queuedEmails, () =>
            {
                return queuedEmails.Select(queuedEmail =>
                {
                    //fill in model values from the entity
                    var queuedEmailModel = queuedEmail.ToModel<QueuedEmailModel>();

                    //little performance optimization: ensure that "Body" is not returned
                    queuedEmailModel.Body = string.Empty;

                    //convert dates to the user time
                    queuedEmailModel.CreatedOn = queuedEmail.CreatedOnUtc.ToLocalTime();

                    if (queuedEmail.SentOnUtc.HasValue)
                        queuedEmailModel.SentOn = queuedEmail.SentOnUtc.Value.ToLocalTime();

                    return queuedEmailModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare queued email model
        /// </summary>
        /// <param name="model">Queued email model</param>
        /// <param name="queuedEmail">Queued email</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Queued email model</returns>
        public virtual QueuedEmailModel PrepareQueuedEmailModel(QueuedEmailModel model, QueuedEmail queuedEmail, bool excludeProperties = false)
        {
            if (queuedEmail == null)
                return model;

            //fill in model values from the entity
            model = model ?? queuedEmail.ToModel<QueuedEmailModel>();

            model.CreatedOn = queuedEmail.CreatedOnUtc.ToLocalTime();

            if (queuedEmail.SentOnUtc.HasValue)
                model.SentOn = queuedEmail.SentOnUtc.Value.ToLocalTime();

            return model;
        }

        #endregion
    }
}