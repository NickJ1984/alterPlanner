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
        bool delLink(string linkId);
        bool delLink(ILink Link);
        bool delLink(e_DependType dependType);
        bool delLinks();

        ILink[] getLinks(e_DependType dependType);
        ILink[] getLinks();
        ILink getLink(string linkID);

        ILink getActiveLink();

        bool LinkExist(string linkID);

        int GetLinksCount(e_DependType dependType);

        bool connect(ILink link);

        event EventHandler<ea_ValueChange<ILink>> event_newActiveLink;
        event EventHandler<ea_ValueChange<DateTime>> event_activeLinkDateChanged;
        event EventHandler<ea_Value<ILink>> event_linkAdded;
        event EventHandler<ea_Value<ILink>> event_linkDeleted;
    }
}
