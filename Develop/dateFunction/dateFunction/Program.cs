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
            dfData data2 = new dfData(new DateTime(1990, 1, 1), e_direction.Left);

            Console.WriteLine(dfData.isIntersectST(data, data2));
            KeyValuePair<DateTime, DateTime>? result = dfData.getIntersectionST(data, data2);
            Console.WriteLine("date#1: {0:dd.MM.yyyy}, date#2: {1:dd.MM.yyyy}", result.Value.Key, result.Value.Value);


            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
