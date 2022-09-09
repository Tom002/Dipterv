using System.Globalization;
using System.Text;

namespace Dipterv.Shared.Exceptions
{
    /// <summary>
    /// Extensions for <see cref="Exception">Exception</see>.
    /// </summary>
    public static class ExceptionFormatterExtensions
    {
        /// <summary>
        /// Converts the exception and its stack trace to readable string.
        /// </summary>
        public static string ExceptionToString(this Exception ex, Action<StringBuilder> customFieldsFormatterAction)
        {
            StringBuilder stringBuilder = new();

            stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}: {1}{2}", ex.GetType().FullName, ex.Message, Environment.NewLine);
            customFieldsFormatterAction?.Invoke(stringBuilder);

            if (ex.InnerException != null)
            {
                stringBuilder
                    .AppendFormat(CultureInfo.InvariantCulture, " ---> {0}{1}", ex.InnerException, Environment.NewLine)
                    .AppendLine("   --- End of inner exception stack trace ---");
            }

            stringBuilder.AppendLine(ex.StackTrace);
            return stringBuilder.ToString();
        }
    }
}
