using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.types;
using alter.args;
using alter.iface;

namespace alter.Task.iface.TaskComponents
{
    public interface ITDotManager
    {
        IDot GetActive();
        IDot GetPassive();

        IDot GetDot(e_Dot type);
        void ChangeActiveDot(e_Dot type);

        event EventHandler<ea_ValueChange<e_Dot>> event_ActiveChanged;
    }
}
