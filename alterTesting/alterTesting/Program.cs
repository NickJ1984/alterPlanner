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
            linkManager linkManager = new linkManager(new Identity(e_Entity.Task));
            linkManager.Vault Vault = new linkManager.Vault(linkManager);
            linkManager.Watcher Watcher = new linkManager.Watcher();
            linkManager.activeLinkManager activeLink = new linkManager.activeLinkManager(linkManager, Vault.getSlaveDependences);
            

            #region default
            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine(); 
            #endregion
        }

        
    }
}
