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
        string GetActiveLinkId();

        void handler_newSubscriber(object sender,  ILink newLink);
        void handler_unsuscribe(object sender, string stringId);
        void FindNewActiveLink(string[] linksId);

        event EventHandler<ea_ValueChange<string>> event_NewActiveLink;
    }
}
