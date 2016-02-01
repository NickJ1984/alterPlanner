﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
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
            public alter.classes.iEvent<EA_value<int>> someEvent;
            public event EventHandler<EA_value<int>> event_test
            {
                add { someEvent.add(value); }
                remove { someEvent.remove(value); }
            }

            public tiEv()
            {
                someEvent = new alter.classes.iEvent<EA_value<int>>();
                someEvent.iEventHandler += (s, e) => Console.WriteLine(string.Format("Parent handler {0}",e.Value));
            }

            public Action unsuscribe(EventHandler<EA_value<int>> method)
            {
                return () => someEvent.iEventHandler -= method;
            }

            internal bool invokeEvent(object sender, EA_value<int> e)
            { return someEvent.invokeEvent(sender, e); }

            internal void clear()
            {
                someEvent.clear();
            }
            public void print()
            {
                Console.WriteLine(new string('-',30));
                for (int i1 = 0; i1 < someEvent.count; i1++) Console.WriteLine("{0}. {1}",i1, someEvent[i1].Method.Name);
                Console.WriteLine(new string('-', 30));
                int[] i = new int[0];
            }
        }

        public class ievntSubscriber
        {
            private Action unsss;

            private void handler(object sender, EA_value<int> e)
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

        static void Main(string[] args)
        {
            EventHandler<EventArgs> ehTest = new EventHandler<EventArgs>((s, e) => Console.WriteLine("Handler for tests"));
            EventHandler<EventArgs>[] fDelegates = new EventHandler<EventArgs>[10];
            alter.classes.iEventObservable<EventArgs> evntClass = new alter.classes.iEventObservable<EventArgs>();
            for (int i = 0; i < fDelegates.Length; i++)
            {
                int j = i + 1;
                fDelegates[i] = new EventHandler<EventArgs>((o, e) => Console.WriteLine("#{0}. Object type: {1}", j, o.GetType()));
            }


            for (int i = 0; i < fDelegates.Length; i++)
                evntClass.event_noSubscribers += fDelegates[i];

            Console.WriteLine("event_noSubscribers run#1:");
            evntClass.iEventHandler += ehTest;
            evntClass.iEventHandler -= ehTest;
            evntClass.clear();
            Console.WriteLine("event_noSubscribers run#2:");
            evntClass.iEventHandler += ehTest;
            evntClass.iEventHandler -= ehTest;




            #region default
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine(); 
            #endregion
        }
    }
}