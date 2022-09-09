using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Shared.Exceptions
{
    /// <summary>
    /// Display error.
    /// </summary>
    public class DisplayError
    {
        /// <summary>
        /// Title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Details.
        /// </summary>
        public string Details { get; }

        /// <summary>
        /// Technical details.
        /// </summary>
        public string TechnicalDetails { get; }

        /// <summary>
        /// Validation errors.
        /// </summary>
        public Dictionary<string, List<string>> ValidationErrors { get; }

        /// <summary>
        /// Validation errors in displayable format.
        /// </summary>
        public string DisplayValidationErrors
            => (ValidationErrors?.Any() ?? false) ? string.Join('\n', ValidationErrors.SelectMany(v => v.Value)) : null;

        public DisplayError(string title, string details = null, string technicalDetails = null, Dictionary<string, List<string>> validationErrors = null)
        {
            Title = title;
            Details = details;
            TechnicalDetails = technicalDetails;
            ValidationErrors = validationErrors ?? new Dictionary<string, List<string>>();
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1197:Optimize StringBuilder.Append/AppendLine call.", Justification = "Not too many validation errors.")]
        public override string ToString()
        {
            StringBuilder builder = new();

            if (!string.IsNullOrEmpty(Title))
            {
                builder.AppendLine(Title);
            }

            if (!string.IsNullOrEmpty(Details))
            {
                builder.AppendLine(Details);
            }

            if (ValidationErrors?.Any() == true)
            {
                builder.AppendLine(string.Join("\n", ValidationErrors.Select(c => $"{c.Key}: {string.Join(",", c.Value)}")));
            }

            if (!string.IsNullOrEmpty(TechnicalDetails))
            {
                builder.AppendLine(TechnicalDetails);
            }

            return builder.ToString();
        }
    }
}
