using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.Task.iface.TaskComponents;
using alter.types;
using alter.iface;

namespace alter.Task.classes
{
    public partial class task
    {
        private class cDot : IDot
        {
            #region vars
            private readonly eDot _type;
            private DateTime _date;
            private Func<DateTime, DateTime> _fncCheck;
            #endregion
            #region props
            public DateTime date
            {
                get { return _date; }
                private set
                {
                    if(_date != value)
                    {
                        DateTime old = _date;
                        _date = value;
                        onDateChanged(new EA_valueChange<DateTime>(old, _date));
                    }
                }
            }
            #endregion
            #region events
            public event EventHandler<EA_valueChange<DateTime>> event_dateChanged;
            #endregion
            #region constr
            public cDot(eDot type)
            {
                _fncCheck = (date) => date;
                _type = type;
            }
            #endregion
            #region handlers
            private void onDateChanged(EA_valueChange<DateTime> args)
            {
                EventHandler<EA_valueChange<DateTime>> handler = event_dateChanged;
                if (handler != null) handler(this, args);
            }
            #endregion
            #region methods
            public DateTime getDate()
            { return date; }

            public eDot getDotType()
            { return _type; }
            public void setDate(object sender, DateTime date)
            { this.date = date; }
            #endregion
        }
    }
    
}
