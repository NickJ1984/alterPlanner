using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.iface;

namespace alter.Group.iface
{
    public interface IGroupManager : ISender
    {
        bool inGroup { get; }
        IGroup getGroup { get; }

        bool addToGroup(IGroup group);
        bool removeFromGroup();

        event EventHandler<ea_ValueChange<IGroup>> event_groupChanged;
    }
}
