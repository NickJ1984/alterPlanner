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
        
        static void Main(string[] args)
        {
            completeLink cLink = new completeLink(
                new DateTime(2000, 1, 1), 30,
                new DateTime(2010,1,1), 30,
                e_TskLim.FinishStart);
            completeLink cLink2 = new completeLink();
            cLink2.master = cLink.slave;
            cLink2.slave = cLink.master;
            cLink2.reinitLink(e_TskLim.FinishFinish);


            linkManager linkManager = new linkManager(cLink.slave);
            linkManager.Vault Vault = new linkManager.Vault(linkManager);
            linkManager.Watcher cWatcher = new linkManager.Watcher();
            linkManager.activeLinkManager activeLink = new linkManager.activeLinkManager(linkManager, Vault.getSlaveDependences);
            
           
            Console.WriteLine("1. Добавление связи в хранилище: {0}", Vault.Add(cLink.link));
            Console.WriteLine("\n2. Добавление связи с теми же участниками в хранилище: {0}", Vault.Add(cLink2.link));

            cWatcher.watchSlaveDependence(cLink.link);
            activeLink.setNewLink(cLink.link);
            Console.WriteLine("\n3. Подключение связи к классу activeLink: {0}", activeLink.activeLink.GetId() == cLink.idLink);
            Console.WriteLine("\n4. Подписка activeLink на тестовую связь в классе Watcher: {0}", cWatcher.subscribe(cLink.link.GetId(), activeLink));
            Console.WriteLine("\n5. Дата активной связи: {0}", activeLink.LDate);
            {
                double temp = cLink.master.duration;
                cLink.master.duration = 50;
                Console.WriteLine("-- Длительность управляющей задачи изменена c {1} на {0}", cLink.master.duration, temp);
            }
            Console.WriteLine("-- Дата активной связи: {0}", activeLink.LDate);
            Console.WriteLine("\n6. Отписка activeLink в классе Watcher: {0}", cWatcher.unsubscribe(cLink.idLink, activeLink));
            Console.WriteLine("-- Дата активной связи: {0}", activeLink.LDate);
            {
                double temp = cLink.master.duration;
                cLink.master.duration = 80;
                Console.WriteLine("-- Длительность управляющей задачи изменена c {1} на {0}", cLink.master.duration, temp);
            }
            Console.WriteLine("-- Дата активной связи: {0}", activeLink.LDate);


            #region default
            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine(); 
            #endregion
        }

        
    }
}
