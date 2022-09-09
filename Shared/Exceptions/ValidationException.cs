using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Shared.Exceptions
{
    public class ValidationException : BusinessException
    {
        /// <summary>
        /// Validation errors.
        /// </summary>
        public List<ValidationError> Errors { get; }

        public ValidationException(List<ValidationError> errors, string title)
            : base(title)
        {
            Errors = errors;
        }
    }
}
