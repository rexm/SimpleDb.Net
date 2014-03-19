using System;
using System.Text;
using System.Globalization;

namespace Cucumber.SimpleDb.Utilities
{
    internal static class StringUtilities
    {
        private const string rfc3986validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        public static string ToRfc3986(this string input)
        {
            StringBuilder encoded = new StringBuilder(input.Length * 2);
            foreach (char symbol in System.Text.Encoding.UTF8.GetBytes(input))
            {
                if (rfc3986validChars.IndexOf(symbol) == -1)
                {
                    encoded.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)symbol));

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

