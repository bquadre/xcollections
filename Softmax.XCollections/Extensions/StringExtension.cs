using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Extensions
{
    public static class StringExtension
    {
        public static bool Contains(this string input, string value, StringComparison comparisonType)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return input.IndexOf(value, comparisonType) >= 0;
        }
    }
}
