using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;

namespace alter.Task.iface.TaskComponents
{
    public interface ITGroupManager : IDependence
    {
        IId CurrentGroup();
        void SetGroup(IId group);

        event EventHandler<IId> event_GroupJoined;
    }
}
