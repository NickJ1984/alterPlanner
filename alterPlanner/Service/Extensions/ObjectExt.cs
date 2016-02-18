using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alter.Service.Extensions
{
    internal static class ObjectExt
    {
        internal static bool hasInterface(this object target, params Type[] interfaces)
        {
            if (target == null) return false;
            bool result = false;

            Type[] iTarget = target.GetType().GetInterfaces();

            for (int i = 0; i < interfaces.Length; i++)
            {
                result = Array.Exists(iTarget, val => val == interfaces[i]);
                if (!result) return false;
            }

            return result;
        }
    }
}
