using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alter.Service.Extensions
{
    internal static class ObjectExt
    {
        internal static bool isNull<TIn>(this TIn instance)
            where TIn : class
        {
            return instance == null;
        }
        internal static bool hasInterface<TIn>(this TIn self, params Type[] interfaces)
            where TIn : class
        {
            if (self == null) return false;
            bool result = false;

            Type[] iTarget = self.GetType().GetInterfaces();

            for (int i = 0; i < interfaces.Length; i++)
            {
                result = Array.Exists(iTarget, val => val == interfaces[i]);
                if (!result) return false;
            }

            return result;
        }

        internal static bool isType(this object instance, Type checkType)
        {
            return instance.GetType() == checkType;
        }
    }
}
