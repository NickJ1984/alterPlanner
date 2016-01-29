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
        double getDuration();
        void setDuration(double days);

        event EventHandler<EA_valueChange<double>> event_durationChanged;
    }
}
