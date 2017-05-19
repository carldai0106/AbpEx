using System;
using System.Text;

namespace Abp.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveLastString(this StringBuilder sb, string remove)
        {
            var source = sb.ToString();
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }
            var i = source.LastIndexOf(remove, StringComparison.Ordinal);
            return i > 0 ? source.Substring(0, i) : sb.ToString();
        }

        public static string RemoveLastString(this string source, string remove)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }
            var i = source.LastIndexOf(remove, StringComparison.Ordinal);
            return i > 0 ? source.Substring(0, i) : source;
        }
    }
}
