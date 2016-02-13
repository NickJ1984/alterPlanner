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
using alter.types;
using alterTesting.Emulators;

namespace alterTesting
{


    class Program
    {
        public interface IFctry
        {
            void create();
            void delete();
            string getType();
        }
        public class tiEv
        {
            public alter.classes.IEvent<ea_Value<int>> someEvent;
            public event EventHandler<ea_Value<int>> event_test
            {
                add { someEvent.add(value); }
                remove { someEvent.Remove(value); }
            }
            public tiEv()
            {
                someEvent = new alter.classes.IEvent<ea_Value<int>>();
                someEvent.iEventHandler += (s, e) => Console.WriteLine(string.Format("Parent handler {0}",e.Value));
                someEvent.setSender(this);
            }
            public Action unsuscribe(EventHandler<ea_Value<int>> method)
            {
                return () => someEvent.iEventHandler -= method;
            }
            internal bool invokeEvent(object sender, ea_Value<int> e)
            {
                return someEvent.InvokeEvent(e); 
            }
            internal void clear()
            {
                someEvent.Clear();
            }
            public void print()
            {
                Console.WriteLine(new string('-',30));
                for (int i1 = 0; i1 < someEvent.Count; i1++) Console.WriteLine("{0}. {1}",i1, someEvent[i1].Method.Name);
                Console.WriteLine(new string('-', 30));
                int[] i = new int[0];
            }
        }

        public class ievntSubscriber
        {
            private Action unsss;

            private void handler(object sender, ea_Value<int> e)
            {
                Console.WriteLine(string.Format("Subscriber handler {0}", e.Value));
                //unsss();
            }
            public void subscribe(tiEv testEventClass)
            {
                testEventClass.event_test += handler;
                unsss = testEventClass.unsuscribe(handler);
            }

        }

        public class tstEvent
        {
            public void subscribe(ref EventHandler<EventArgs> Event)
            {
                Event += handler;
            }

            private void handler(object sender, EventArgs e)
            {
                Console.WriteLine("OKOKOKOK");
            }
        }

        public class test
        {
            public readonly string text;
            public string addOn;

            public test(string text)
            {
                this.text = text;
                addOn = "";
            }

            public void print()
            {
                if(string.IsNullOrEmpty(addOn)) Console.WriteLine(text);
                else Console.WriteLine("{0}; AddOn: {1}", text, addOn);
            }
        }
        public class MyStructComparer : IEqualityComparer<MyStruct>
        {
            public bool Equals(MyStruct x, MyStruct y)
            {
                return x.Check == y.Check || x.Check == 50 || y.Check == 50;
            }

            public int GetHashCode(MyStruct obj)
            {
                return obj.Check.GetHashCode();
            }
        }
        public struct MyStruct
        {
            public test reference;
            public string addOn;
            public int Check;

            
        }

        public class testCMP : IComparable<testCMP>
        {
            public readonly int index = -1;
            private static Random rnd = new Random();
            protected readonly int cmpValue = rnd.Next(1, 100);

            public testCMP(int index)
            {
                this.index = index;
            }

            public int CompareTo(testCMP other)
            {
                int result = cmpValue == other.cmpValue ? 0 : -1;
                return cmpValue > other.cmpValue ? 1 : result;
            }
        }

        static void Main(string[] args)
        {
            testCMP[] array = new testCMP[20];
            for(int i = 0; i < 20; i++) array[i] = new testCMP(i);
            testCMP max = array.Max();
            Console.WriteLine(max?.index);

            #region default
            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine(); 
            #endregion
        }

        
    }
}
