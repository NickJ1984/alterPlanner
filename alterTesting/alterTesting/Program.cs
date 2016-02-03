using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.classes;
using alter.Function.classes;
using alter.iface;
using alter.Link.iface;
using alter.types;

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
            }

            public Action unsuscribe(EventHandler<ea_Value<int>> method)
            {
                return () => someEvent.iEventHandler -= method;
            }

            internal bool invokeEvent(object sender, ea_Value<int> e)
            { return someEvent.InvokeEvent(sender, e); }

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
            function fnc = new function(DateTime.Now, e_Direction.Fixed);
            Dependence dpn = new Dependence(DateTime.Now, e_Direction.Fixed, e_Dot.Start);
            dpn.SetDirection(fnc.GetDirection);
            dpn.SetDate(fnc.GetDate);
            #region Вспомогательные функции
            Action<string, string, string> dOut = (Obj, Old, New) =>
            {
                Console.WriteLine
                ("Dependance changed: {0}\noldValue: {1}\nnewValue: {2}", Obj, Old, New);
            };
            #endregion
            #region Функции для подписки
            Func<EventHandler<ea_ValueChange<e_Direction>>, Action> fncDir =
                (eHnd) =>
                {
                    fnc.event_DirectionChanged += eHnd;
                    return () => fnc.event_DirectionChanged -= eHnd;
                };
            Func<EventHandler<ea_ValueChange<DateTime>>, Action> fncDate =
                (eHnd) =>
                {
                    fnc.event_DateChanged += eHnd;
                    return () => fnc.event_DateChanged -= eHnd;
                };
            #endregion
            #region Автообновление зависимости
            dpn.setAutoupdate(fncDate, fncDir);
            #endregion
            #region отслеживание
            dpn.event_DependDotChanged += (s, e) =>
            { dOut("dependDot", e.OldValue.ToString(), e.NewValue.ToString()); };
            dpn.event_DateChanged += (s, e) =>
            { dOut("manageDate", e.OldValue.ToString(), e.NewValue.ToString()); };
            dpn.event_DirectionChanged += (s, e) =>
            { dOut("Direction", e.OldValue.ToString(), e.NewValue.ToString()); };
            #endregion



            #region testing
            fnc.SetDate(new DateTime(2000,1,1));
            fnc.SetDirection(e_Direction.Left);
            dpn.SetDependDot(e_Dot.Finish);
            Console.WriteLine("Unsuscribe...");
            fnc.SetDate(new DateTime(2001,10,12));
            #endregion

            #region default
            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine(); 
            #endregion
        }
    }
}
