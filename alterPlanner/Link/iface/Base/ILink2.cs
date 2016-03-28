using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;
using alter.types;
using alter.args;
using alter.Task.iface;

namespace alter.Link.iface.Base
{
    public interface ILink_2 : IId, IRemovable, ILimit<e_TskLim>
    {
        DateTime date { get; }
        double delay { get; set; }
        
        event EventHandler<ea_ValueChange<double>> event_delayChanged;
        event EventHandler<ea_ValueChange<DateTime>> event_dateChanged;

        ILine getMember(e_DependType dependType);
        ILine getMember(string memberID);

        IId getMemberID(e_DependType dependType);
        IId getMemberID(string memberID);

        e_DependType getDependType(string memberID);

        void unsubscribe(string memberID);
    }
}
