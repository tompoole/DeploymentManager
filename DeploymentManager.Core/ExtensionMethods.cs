using System;

namespace DeploymentManager.Core
{
    public static class ExtensionMethods
    {
        public static string ToReadableString(this DateTime date)
        {
            return date.ToString("dd MMM yyyy HH:mm");
        }

        public static string ToReadableString(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToReadableString();
            }

            return "Unknown";
        }

        public static string ToReadableString(this TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Hours > 0 ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Minutes > 0 ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Seconds > 0 ? string.Format("{0:0} second{1}", span.Seconds, span.Seconds == 1 ? String.Empty : "s") : string.Empty);

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

            return formatted;
        }

        public static string ToReadableString(this TimeSpan? span)
        {
            if (span.HasValue)
            {
                return span.Value.ToReadableString();
            }

            return "Unknown";
        }
    }
}