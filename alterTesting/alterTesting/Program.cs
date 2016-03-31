using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.classes;
using alter.Function.classes;
using alter.iface;
using alter.Link.classes;
using alter.Link.iface;
using alter.Service.classes;
using alter.Service.debugHelpers;
using alter.types;
using alterTesting.Emulators;
using alterTesting.TestClasses;

namespace alterTesting
{
    class Program
    {
        public static void testParam(params int[] iValue)
        {

            for (int i = 0; i < iValue.Length; i++)
            {
                Console.WriteLine("{0}. {1}", i + 1, iValue[i]);
            }
        }

        static void Main(string[] args)
        {
            HashSet<string> test = new HashSet<string>();

            Console.WriteLine(test.Add("ttt"));
            Console.WriteLine(test.Add("ttt"));

            #region default

            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();

            #endregion
        }
    }
}
