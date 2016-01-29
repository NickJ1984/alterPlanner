using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.Task.iface.TaskComponents;
using alter.types;


namespace alter.Task.classes
{
    public partial class task
    {
        private class cDuration : ITDuration
        {
            #region vars
            public double minValue = 0;
            private ITDotManager dManager;
            private double _duration;
            #endregion
            #region events
            public event EventHandler<EA_valueChange<double>> event_durationChanged;
            #endregion
            #region constructors
            public cDuration(ITDotManager dotManager)
            {
                dManager = dotManager;
                _duration = minValue;
            }
            #endregion
            #region handlers
            private void onDurationChange(EA_valueChange<double> args)
            {
                EventHandler<EA_valueChange<double>> handler = event_durationChanged;
                if (handler != null) handler(this, args);
            }
            #endregion
            #region methods
            public double getDuration()
            {
                throw new NotImplementedException();
            }
            public void setDuration(double days)
            {
                if (_duration == days || days < minValue) return;

                double temp = _duration;
                _duration = days;

                onDurationChange(new EA_valueChange<double>(temp, _duration));
            }
            #endregion
        }
    }
    
}
