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
        DateTime ActiveDotLimit(DateTime date);
        DateTime PassiveDotLimit(DateTime date);

        void SetLimit(e_Entity Object, IDependence depend);
        

        event EventHandler event_ActiveUpdate;
        event EventHandler event_PassiveUpdate;
    }
}
