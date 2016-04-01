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
        public class cInt : IComparable<cInt>
        {
            public int Integer = 0;

            public cInt(int Value)
            {
                Integer = Value;
            }
            public cInt()
            { }

            public override string ToString()
            {
                return String.Format("integer class value {0}", Integer);
            }

            public int CompareTo(cInt other)
            {
                if (Integer > other.Integer) return 1;
                else if (Integer == other.Integer) return 0;
                else return -1;
            }
        } 

        static void Main(string[] args)
        {
            int[] ARR = new[] {3, 0, 2, 10, 18, 200, 101, 78, 43};
            List<cInt> vals = new List<cInt>();
            for (int i = 0; i < 10; i++)
            {
                vals.Add(new cInt(i + 1));
            }
            Console.WriteLine(ARR.Max());
            Console.WriteLine(vals.Max());

            #region default

            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();

            #endregion
        }
    }
}
