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
        bool isInGroup { get; }
        IGroup getGroup { get; }

        bool registerGroup(IGroup group);
        bool unregisterGroup();

        event EventHandler<ea_ValueChange<IGroup>> event_groupChanged;
    }
}
