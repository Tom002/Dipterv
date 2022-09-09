using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Shared.Exceptions
{
    /// <summary>
    /// Business exception.
    /// </summary>
    [Serializable]
    public class BusinessException : Exception
    {
        /// <summary>
        /// Title.
        /// </summary>
        public string Title => Message;

        /// <summary>
        /// Details.
        /// </summary>
        public string Details { get; }

        /// <summary>
        /// Technical details.
        /// </summary>
        public string TechnicalDetails { get; }

        protected BusinessException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        public BusinessException()
        {
        }

        public BusinessException(string message)
            : base(message)
        {
        }

        public BusinessException(string title, string details)
        : base(title)
        {
            Details = details;
        }

        public BusinessException(string title, string details, string technicalDetails)
            : base(title)
        {
            Details = details;
            TechnicalDetails = technicalDetails;
        }

        public BusinessException(string title, Exception innerException)
            : base(title, innerException)
        {
        }

        public BusinessException(string title, string details, Exception innerException)
        : base(title, innerException)
        {
            Details = details;
        }

        /// <inheritdoc />
        public override string ToString() => this.ExceptionToString(ToStringCore);

        /// <summary>
        /// When overridden the <see cref="ToString">ToString</see> method can be extended with custom logic.
        /// </summary>
        protected virtual void ToStringCore(StringBuilder builder)
        {
            builder
                .AppendLine(Details)
                .AppendLine(TechnicalDetails);
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(Details), Details, typeof(string));
            info.AddValue(nameof(TechnicalDetails), TechnicalDetails, typeof(string));
        }
    }
}
