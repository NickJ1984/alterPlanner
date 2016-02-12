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

       
        
        static void Main(string[] args)
        {
            lineClass master = new lineClass()
            {
                start = new DateTime(2000, 1, 1),
                finish = new DateTime(2002, 1, 1)
            };
            lineClass slave = new lineClass()
            {
                start = new DateTime(2010, 2, 2),
                duration = 30
            };
            link Link = new link(master, slave, e_TskLim.StartFinish);
            #region actions

            Action newTest = () => Console.WriteLine(new string('+', 50));
            Action printSlave = () =>
            {
                Console.WriteLine(new string('*', 50));
                Console.WriteLine("Slave information:");
                Console.WriteLine("Start: {0:dd.MM.yyyy}   Finish: {1:dd.MM.yyyy}   Duration: {2}", slave.start, slave.finish, slave.duration);
                Console.WriteLine(new string('*', 50));
            };
            Action printMaster = () =>
            {
                Console.WriteLine(new string('*', 50));
                Console.WriteLine("Master information");
                Console.WriteLine("Start: {0:dd.MM.yyyy}   Finish: {1:dd.MM.yyyy}   Duration: {2}", master.start, master.finish, master.duration);
                Console.WriteLine(new string('*', 50));
            };
            Action printLink = () =>
            {
                Console.WriteLine(new string('*', 50));
                Console.WriteLine("Link information: ");
                Console.WriteLine("Limit: {0}    Delay: {1}", Link.limit, Link.delay);
                Console.WriteLine(new string('*', 50));
            };
            #endregion
            printSlave();
            printMaster();
            printLink();
            slave.setDependence(Link.depend);
            printSlave();
            newTest();
            Link.delay = 10;
            printLink();
            printSlave();
            newTest();
            Link.limit = e_TskLim.FinishStart;
            printLink();
            printMaster();
            printSlave();
            newTest();
            Link.delay = 20;
            Link.limit = e_TskLim.StartStart;
            printLink();
            printMaster();
            printSlave();
            #region default
            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine(); 
            #endregion
        }

        
    }
}
