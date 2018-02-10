using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    /// <summary>
    /// A class responsible for extension methods used in Core class.
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// Determine whether IEnumerable is null or empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T> (this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }
    }
}
