using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;
using alter.types;
using alter.Function.iface;


namespace alter.Task.iface.TaskComponents
{
    public interface ITLimit
    {
        DateTime activeDotLimit(DateTime date);
        DateTime passiveDotLimit(DateTime date);

        void setLimit(eEntity Object, IDependence depend);
        

        event EventHandler event_activeUpdate;
        event EventHandler event_passiveUpdate;
    }
}
