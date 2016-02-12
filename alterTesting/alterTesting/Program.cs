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
       
        public struct MyStruct
        {
            public test reference;
            public string addOn;
        }
        static void Main(string[] args)
        {
            test ttt = new test("First entry");
            MyStruct str = new MyStruct()
            {
                addOn = ttt.addOn,
                reference = ttt
            };
            ttt.addOn = "this is second added";
            str.reference.print();
            str.reference.addOn = str.addOn;
            str.reference.print();

            #region default
            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine(); 
            #endregion
        }

        
    }
}
