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
            lineDebug(master, e_DependType.Master);
            lineClass slave = new lineClass()
            {
                start = new DateTime(2010, 2, 2),
                duration = 30
            };
            lineDebug(slave, e_DependType.Slave);
            linkClass link = new linkClass(master, master, slave, slave, e_TskLim.StartFinish);
            linkDebug(link);
            dependDebug(link.depend);
            memberDebug(link.lmMaster, e_DependType.Master);
            memberDebug(link.lmSlave, e_DependType.Slave);
            link.delay = 10;
            #region actions
            Action printSlave = () =>
            {
                Console.WriteLine(new string('*', 50));
                Console.WriteLine("Slave task start at {0:dd.MM.yyyy} finish at {1:dd.MM.yyyy}", slave.start, slave.finish);
                Console.WriteLine(new string('*', 50));
            };
            Action printMaster = () =>
            {
                Console.WriteLine(new string('*', 50));
                Console.WriteLine("Master task start at {0:dd.MM.yyyy} finish at {1:dd.MM.yyyy}", master.start, master.finish);
                Console.WriteLine(new string('*', 50));
            };
            Action print = () =>
            {
                Console.WriteLine(new string('*', 50));
                Console.WriteLine("Link:");
                Console.WriteLine("Limit: [{0}]    Delay: {1}", link.limit.ToString(), link.delay);
                Console.WriteLine("Master:");
                Console.WriteLine("Depend dot: [{0}]    Depend dot date: {1:dd.MM.yyyy}",
                    link.GetInfoMember(e_DependType.Master).getObjectDependDotInfo().GetDotType(),
                    link.GetInfoMember(e_DependType.Master).getObjectDependDotInfo().GetDate());
                Console.WriteLine("Function:");
                Console.WriteLine("Depend date: {0:dd.MM.yyyy}    Depend dot: {1}",
                    link.GetSlaveDependence().GetDate(),
                    link.GetSlaveDependence().GetDependDot());
                Console.WriteLine(new string('*', 50));
            }; 
            #endregion

            printMaster();
            printSlave();
            print();
            link.SetLimit(e_TskLim.FinishFinish);
            link.SetLimit(e_TskLim.FinishStart);
            master.finish = new DateTime(2010, 2, 2);
            link.delay = 0;
            #region default
            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine(); 
            #endregion
        }

        public static void lineDebug(ILine obj, e_DependType dType)
        {
            string name = string.Format("{0} task => ", dType);
            obj.GetDot(e_Dot.Start).event_DateChanged += (o, v) =>
                Console.WriteLine("{0}[Start] changed from {1:dd.MM.yyyy} to {2:dd.MM.yyyy}",
                name, v.OldValue, v.NewValue);
            obj.GetDot(e_Dot.Finish).event_DateChanged += (o, v) =>
                Console.WriteLine("{0}[Finish] changed from {1:dd.MM.yyyy} to {2:dd.MM.yyyy}",
                name, v.OldValue, v.NewValue);
        }
        public static void linkDebug(linkClass link)
        {
            string name = "Link => ";
            link.event_DelayChanged += (o, v) =>
                Console.WriteLine("{0}Delay changed from {1} to {2}", name, v.OldValue, v.NewValue);
            link.event_LimitChanged += (o, v) =>
                Console.WriteLine("{0}Limit changed from {1} to {2}", name, v.OldValue, v.NewValue);
        }

        public static void memberDebug(linkMember member, e_DependType type)
        {
            string name = string.Format("{0} member => ", type);
            member.event_dependDateChanged += (o, v) =>
                Console.WriteLine("{0}Depend date changed from {1:dd.MM.yyyy} to {2:dd.MM.yyyy}", name, v.OldValue, v.NewValue);
            member.event_neighboursDeltaChanged += (o, v) =>
                Console.WriteLine("{0}Delta changed from {1} to {2}", name, v.OldValue, v.NewValue);
        }

        public static void dependDebug(Dependence depend)
        {
            string name = "Depend slave => ";
            depend.event_DependDotChanged += (o, v) =>
                Console.WriteLine("{0}Dot changed from {1} to {2}", name, v.OldValue, v.NewValue);
            depend.event_DateChanged += (o, v) =>
                Console.WriteLine("{0}Date changed from {1:dd.MM.yyyy} to {2:dd.MM.yyyy}", name, v.OldValue, v.NewValue);
        }
    }
}
