
namespace Aldan.Web.Framework.Models
{
    /// <summary>
    /// Represents base Aldan entity model
    /// </summary>
    public partial class BaseAldanEntityModel : BaseAldanModel
    {
        /// <summary>
        /// Gets or sets model identifier
        /// </summary>
        public virtual int Id { get; set; }
    }
}