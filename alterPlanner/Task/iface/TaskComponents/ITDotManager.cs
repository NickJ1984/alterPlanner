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
        IDot getActive();
        IDot getPassive();

        IDot getDot(eDot type);
        void changeActiveDot(eDot type);

        event EventHandler<EA_valueChange<eDot>> event_activeChanged;
    }
}
