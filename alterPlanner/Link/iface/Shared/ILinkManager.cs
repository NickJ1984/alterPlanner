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
        bool DelLink(string linkId);
        bool DelLink(e_DependType dependType);
        bool DelLink();

        string[] GetLinks(e_DependType dependType);
        string[] GetLinks();

        bool LinkExist(string linkId);

        int GetLinksCount(e_DependType dependType);
        
        event EventHandler<ea_Value<ILink>> event_LinkAdded;
        event EventHandler<ea_Value<ILink>> event_LinkDeleted;
    }
    public interface ILinkConnector : ILinkManager
    {
        bool AddLink(e_DependType type, ILink newLink);
    }
}
