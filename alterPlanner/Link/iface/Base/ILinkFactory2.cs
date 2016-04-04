using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;
using alter.Link.classes;
using alter.types;
using alter.Task.classes;
using alter.Task.iface;

namespace alter.Link.iface.Base
{
    public interface ILinkFactory2 : IId, IChild
    {
        int count { get; }
        ILink_2 createLink(IConnectible precursor, IConnectible follower, e_TskLim limit, double delay);
        bool removeLink(string linkID);

        ILink_2 getLink(string linkID);
        ILink_2[] getInvolved(string memberID);
        ILink_2[] getInvolved(string memberID, e_DependType dependType);
        ILink_2[] getLinks();

        event EventHandler<ILink_2> event_linkCreated;
        event EventHandler<ILink_2> event_linkRemoved;
    }
}
