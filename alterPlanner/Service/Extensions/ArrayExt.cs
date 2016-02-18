using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alter.Service.Extensions
{
    internal static class ArrayExt
    {
        internal static bool isNullOrEmpty<T>(this T[] instance)
        {
            return instance?.Length == 0;
        }
    }
}
