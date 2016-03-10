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
            Action chapter = () => Console.WriteLine(new string('*', 50));
            dFunction func = new dFunction(new DateTime(2000,1,1), e_direction.Right);
            dFunction func2 = new dFunction(new DateTime(2005, 1, 1), e_direction.Fixed);
            
            Action<DateTime> check = date => Console.WriteLine("{0:dd.MM.yyyy} => check => {1:dd.MM.yyyy}", date, func.check(date));
            Console.WriteLine(func);
            Console.WriteLine("Func is binded {0} with {1}", func.isBinded, func.binded != null ? func.binded.ToString() : "Null");
            chapter();
            check(new DateTime(2010,1,1));
            check(new DateTime(2000, 1, 1));
            check(new DateTime(1910, 1, 1));
            chapter();
            func.setDiapason(func2);
            Console.WriteLine("Func is binded {0} with {1}", func.isBinded, func.binded != null ? func.binded.ToString() : "Null");
            chapter();
            check(new DateTime(1910, 1, 1));
            check(new DateTime(2000, 1, 1));
            check(new DateTime(2003, 1, 1));
            check(new DateTime(2005, 1, 1));
            check(new DateTime(2006, 1, 1));
            chapter();
            func.setDiapason(null);
            Console.WriteLine("Func is binded {0} with {1}", func.isBinded, func.binded != null ? func.binded.ToString() : "Null");
            chapter();
            check(new DateTime(2010, 1, 1));
            check(new DateTime(2000, 1, 1));
            check(new DateTime(1910, 1, 1));
            chapter();
            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
