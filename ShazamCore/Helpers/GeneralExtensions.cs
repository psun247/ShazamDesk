using System.Collections;

namespace ShazamCore.Helpers
{
    public static class GeneralExtensions
    {
        /// <summary>
        /// Detect if a string is null / empty.  
        /// Can even handle a null string.  e.g.
        /// string x = null;
        /// if (x.IsBlank())...
        /// </summary>        
        public static bool IsBlank(this string? str) => string.IsNullOrWhiteSpace(str);

        /// <summary>
        /// Detect if a string is not null / empty.
        /// </summary>        
        public static bool IsNotBlank(this string? str) => !IsBlank(str);

        public static bool IsEmpty(this ICollection c)
        {
            return (c == null) || (c.Count == 0);
        }

        public static bool IsNotEmpty(this ICollection c)
        {
            return !IsEmpty(c);
        }

        public static bool In<T>(this T item, params T[] testValues)
        {
            if (item != null)
            {
                foreach (T test_value in testValues)
                {
                    if (test_value != null && test_value.Equals(item))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool NotIn<T>(this T item, params T[] testValues)
        {
            return !In(item, testValues);
        }
    }
}
