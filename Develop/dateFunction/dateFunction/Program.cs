using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace dateFunction
{
    class Program
    {
        static void Main(string[] args)
        {
            dfData data = new dfData(new DateTime(2001, 1, 1), e_direction.Left);
            dfData data2 = new dfData(new DateTime(1990, 1, 1), e_direction.Right);

            Console.WriteLine(dfData.isIntersectST(data, data2));
            
            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
