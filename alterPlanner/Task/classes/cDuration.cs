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
        private class CDuration : ITDuration
        {
            #region vars
            public double MinValue = 0;
            private ITDotManager _dManager;
            private double _duration;
            #endregion
            #region events
            public event EventHandler<ea_ValueChange<double>> event_DurationChanged;
            #endregion
            #region constructors
            public CDuration(ITDotManager dotManager)
            {
                _dManager = dotManager;
                _duration = MinValue;
            }
            #endregion
            #region handlers
            private void OnDurationChange(ea_ValueChange<double> args)
            {
                EventHandler<ea_ValueChange<double>> handler = event_DurationChanged;
                if (handler != null) handler(this, args);
            }
            #endregion
            #region methods
            public double GetDuration()
            {
                throw new NotImplementedException();
            }
            public void SetDuration(double days)
            {
                if (_duration == days || days < MinValue) return;

                double temp = _duration;
                _duration = days;

                OnDurationChange(new ea_ValueChange<double>(temp, _duration));
            }
            #endregion
        }
    }
    
}
