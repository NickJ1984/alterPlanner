using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;
using alter.args;
using alter.types;
using alter.Link.iface;

namespace alter.Link.iface
{
    public interface ILinkManager
    {
        bool addLink(eDependType type, ILink newLink);

        bool delLink(string linkID);
        bool delLink(eDependType dependType);
        bool delLink();

        string[] getLinks(eDependType dependType);
        string[] getLinks();

        bool linkExist(string linkID);

        int getLinksCount(eDependType dependType);
        
        event EventHandler<EA_value<ILink>> event_linkAdded;
        event EventHandler<EA_value<ILink>> event_linkDeleted;
    }
}
