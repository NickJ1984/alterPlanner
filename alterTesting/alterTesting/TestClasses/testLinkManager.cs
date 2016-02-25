using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.classes;
using alter.Link.classes;
using alter.Link.iface;
using alter.Service.debugHelpers;
using alter.types;
using alterTesting.Emulators;
using Microsoft.Win32;

namespace alterTesting.TestClasses
{
    public class testLinkManager
    {
        private int _counter = 0;
        private colorOut color;
        public int counter => ++_counter;

        public linkManager lManager;
        public lineClass myTask;

        public testLinkManager(lineClass myTask = null)
        {
            if (myTask == null)
                myTask = new lineClass(new DateTime(2010, 5, 1), 30);
            else this.myTask = myTask;

            color = new colorOut(); 
            color.addObject(1, ConsoleColor.Red);

            lManager = new linkManager(myTask);

            lManager.event_linkAdded += handler_linkAdded;
            lManager.event_linkDeleted += handler_linkDeleted;
            lManager.event_activeLinkDateChanged += handler_activeLinkDateChanged;
            lManager.event_newActiveLink += handler_newActiveLink;
        }

        protected void handler_linkAdded(object sender, ea_Value<ILink> e)
        {
            color.current = 1;
            Console.WriteLine("{0}. Link added. ID: {1}", counter, e.Value.GetId());
            color.resetCurrentColor();
        }
        protected void handler_linkDeleted(object sender, ea_Value<ILink> e)
        {
            color.current = 1;
            Console.WriteLine("{0}. Link deleted. ID: {1}", counter, e.Value.GetId());
            color.resetCurrentColor();
        }

        protected void handler_newActiveLink(object sender, ea_ValueChange<ILink> e)
        {
            color.current = 1;
            string Old = e.OldValue == null ? "Null" : e.OldValue.GetId();
            string New = e.NewValue == null ? "Null" : e.NewValue.GetId();
            Console.WriteLine("{0}. New active link. oldID: {1} newID: {2}", counter, Old, New);
            color.resetCurrentColor();
        }
        protected void handler_activeLinkDateChanged(object sender, ea_ValueChange<DateTime> e)
        {
            color.current = 1;
            Console.WriteLine("{0}. Active date changed. oldDate: {1:dd.MM.yyyy} newDate: {2:dd.MM.yyyy}", counter, e.OldValue, e.NewValue);
            color.resetCurrentColor();
        }

        
    }
}
