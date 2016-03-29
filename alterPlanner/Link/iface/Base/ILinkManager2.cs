using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.types;

namespace alter.Link.iface.Base
{
    public interface ILinkManager2
    {
        ILink_2 activeLink { get; }
        int linkCount { get; }

        ILink_2[] getLinks(e_DependType depend);
        ILink_2[] getLinks();
        ILink_2 getLink(string linkID);

        bool addToLink(e_DependType role, ILink_2 link);

        bool isLinkExist(string linkID);

        void clear();
        
        event EventHandler<ea_ValueChange<ILink_2>> event_activeLinkChanged;
        event EventHandler<ILink_2> event_linkAdded;
        event EventHandler<ILink_2> event_linkUnsubscribed;
    }
}
