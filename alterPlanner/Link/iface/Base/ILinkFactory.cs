using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;
using alter.types;
using alter.args;
using alter.Task.iface;
using alter.Project.iface;

namespace alter.Link.iface
{
    public interface ILinkFactory : IId, IChild
    {
        ILink getLink(string IDlink);
        ILink createLink(eLnkType type, IDock master, IDock slave);
        bool deleteLink(string IDlink);
        bool deleteLink();
    }
}
    
