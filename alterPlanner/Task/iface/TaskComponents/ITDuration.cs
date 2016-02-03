using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;

namespace alter.Task.iface.TaskComponents
{
    public interface ITDuration
    {
        double GetDuration();
        void SetDuration(double days);

        event EventHandler<ea_ValueChange<double>> event_DurationChanged;
    }
}
