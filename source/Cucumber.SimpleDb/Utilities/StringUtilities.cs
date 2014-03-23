using System.Globalization;
using System.Text;

namespace Cucumber.SimpleDb.Utilities
{
    internal static class StringUtilities
    {
        private const string Rfc3986ValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        public static string ToRfc3986(this string input)
        {
            var encoded = new StringBuilder(input.Length*2);
            foreach (char symbol in Encoding.UTF8.GetBytes(input))
            {
                if (Rfc3986ValidChars.IndexOf(symbol) == -1)
                {
                    encoded.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int) symbol));
                }
                else
                {
                    encoded.Append(symbol);
                }
            }

            return encoded.ToString();
        }
    }
}