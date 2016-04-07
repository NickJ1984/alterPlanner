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
        public interface ITest1
        {
            void test_print();
        }
        public interface ITest2
        {
            void test_print2();
        }
        public interface ITesting : ITest1, ITest2
        { }

        public class ex : ITesting
        {
            public void test_print()
            {
                Console.WriteLine("Метод интерфейса ITest1 запущен.");
            }

            public void test_print2()
            {
                Console.WriteLine("Метод интерфейса ITest2 запущен.");
            }
        }

        public class ex2
        {
            public void method(ITesting entity)
            {
                Console.WriteLine("*****Запущено из класса ex2*****");
                entity.test_print();
                entity.test_print2();
                Console.WriteLine("********************************");
            }
        }

        static void Main(string[] args)
        {
            ex test = new ex();
            ex2 test2 = new ex2();
            test2.method(((ITesting)test));

            #region default

            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();

            #endregion
        }
    }
}
