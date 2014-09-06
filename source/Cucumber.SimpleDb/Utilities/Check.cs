// Portions copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See MicrosoftOpenTechnologiesLicense.txt in the project root for license information.
// This class has been modified from the original version at http://goo.gl/hgZp7i.

using System;

namespace Cucumber.SimpleDb.Utilities
{
    internal static class Check
    {
        public static T NotNull<T>(T value, string parameterName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        public static T? NotNull<T>(T? value, string parameterName) where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        public static string NotEmpty(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(String.Format("{0} cannot be null or whitespace.", parameterName));
            }

            return value;
        }
    }
}