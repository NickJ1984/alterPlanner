using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dateFunction
{
    public enum e_direction
    {
        Left = 1,
        Right = 2,
        Fixed = 3
    }



    public struct valueChange<T>
    {
        public readonly T Old;
        public readonly T New;

        public valueChange(T Old, T New)
        {
            this.Old = Old;
            this.New = New;
        }
    }    
}
