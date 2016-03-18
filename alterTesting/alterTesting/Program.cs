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
        static void Main(string[] args)
        {
            Dictionary<e_Entity, int> dict = new Dictionary<e_Entity, int>()
            {
                {e_Entity.Project, 0},
                {e_Entity.Group, 1},
                {e_Entity.Link, 2},
                {e_Entity.Task, 3}
            };

            foreach (var mbr in dict)
            {
                Console.WriteLine("{0,-8}{1}", mbr.Key, mbr.Value);
            }

            Action<e_Entity> printNext =
                entity => Console.WriteLine("Current: {0,-8} Next: {1}", entity, nextPriority(entity));
            printNext(e_Entity.Project);
            printNext(e_Entity.Link);
            printNext(e_Entity.Task);
            Console.WriteLine("First: {0,-8}", nextPriority());

            #region default
            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine(); 
            #endregion
        }

        public static e_Entity nextPriority(e_Entity entity)
        {
            Dictionary<e_Entity, int> priority = new Dictionary<e_Entity, int>()
            {
                {e_Entity.Project, 0},
                {e_Entity.Group, 1},
                {e_Entity.Link, 2},
                {e_Entity.Task, 3}
            };


            int current = priority[entity];
            int[] expected = priority.Values.Where(v => v < current).ToArray();

            if (expected.Length == 0) return e_Entity.None;
            else
            {
                int next = expected.Max();
                return priority.Where(v => v.Value == next).First().Key;
            }
        }
        public static e_Entity nextPriority()
        {
            Dictionary<e_Entity, int> priority = new Dictionary<e_Entity, int>()
            {
                {e_Entity.Project, 0},
                {e_Entity.Group, 1},
                {e_Entity.Link, 2},
                {e_Entity.Task, 3}
            };

            int max = priority.Values.Max();
            return priority.Where(v => v.Value == max).First().Key;
        }
    }
}
