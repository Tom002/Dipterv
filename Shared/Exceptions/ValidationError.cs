using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Shared.Exceptions
{
    /// <summary>
    /// Validation error.
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Error message.
        /// </summary>
        public string Error { get; }

        public ValidationError(string key, string error)
        {
            Key = key;
            Error = error;
        }
    }
}
