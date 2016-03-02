using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.types;

namespace alter.Service.Extensions
{
    internal static class EnumsExt
    {
        internal static bool isEqual(this e_Entity instance, params e_Entity[] type)
        {
            if (type.isNullOrEmpty()) throw new ArgumentNullException();

            for (int i = 0; i < type.Length; i++)
                if (instance == type[i]) return true;
            return false;
        }
    }
}
