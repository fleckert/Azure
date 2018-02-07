using System;

namespace Microsoft.Azure.Helper
{
    public static class ConnectionStringExtensions
    {
        /// <summary>
        /// Gets a value for a given key within the connectionString.
        /// </summary>
        /// <param name="connectionString">A non null, non empty <see cref="string"/>.</param>
        /// <param name="key">A non null, non empty <see cref="string"/>.</param>
        /// <param name="funcProcessValue">A <see cref="Func{T, TResult}"/> that takes <see cref="string"/> and returns a <see cref="string"/>.</param>
        /// <param name="seperatorParts">A <see cref="string"/> that seperates the key-value pairs.</param>
        /// <param name="seperatorKeyValue">A <see cref="string"/> that seperates key and value.</param>
        public static string GetValue(this string connectionString, string key, Func<string, string> funcProcessValue = null, string seperatorParts = ";", string seperatorKeyValue = "=")
        {
            var parts = connectionString.Split(new[] { seperatorParts }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)
            {
                if (string.IsNullOrWhiteSpace(part))
                {
                    continue;
                }

                string[] keyValueParts = part.Split(new[] { seperatorKeyValue }, StringSplitOptions.RemoveEmptyEntries);

                if (keyValueParts.Length != 2)
                {
                    continue;
                }

                if (string.Equals(keyValueParts[0], key, StringComparison.OrdinalIgnoreCase))
                {
                    return (funcProcessValue ?? (_ => _))(keyValueParts[1]);
                }
            }

            return default(string);
        }
    }
}
