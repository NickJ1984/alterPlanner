using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;
using alter.Link.iface;
using alter.args;

namespace alter.Task.iface.TaskComponents
{
    public interface ITActiveLink : IDependence
    {
        string getActiveLinkID();

        void handler_newSubscriber(object sender,  ILink newLink);
        void handler_unsuscribe(object sender, string stringID);
        void findNewActiveLink(string[] linksID);

        event EventHandler<EA_valueChange<string>> event_newActiveLink;
    }
}
