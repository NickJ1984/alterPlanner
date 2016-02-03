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
        private class CDot : IDot
        {
            #region vars
            private readonly e_Dot _type;
            private DateTime _date;
            private Func<DateTime, DateTime> _fncCheck;
            #endregion
            #region props
            public DateTime Date
            {
                get { return _date; }
                private set
                {
                    if(_date != value)
                    {
                        DateTime old = _date;
                        _date = value;
                        OnDateChanged(new ea_ValueChange<DateTime>(old, _date));
                    }
                }
            }
            #endregion
            #region events
            public event EventHandler<ea_ValueChange<DateTime>> event_DateChanged;
            #endregion
            #region constr
            public CDot(e_Dot type)
            {
                _fncCheck = (date) => date;
                _type = type;
            }
            #endregion
            #region handlers
            private void OnDateChanged(ea_ValueChange<DateTime> args)
            {
                EventHandler<ea_ValueChange<DateTime>> handler = event_DateChanged;
                if (handler != null) handler(this, args);
            }
            #endregion
            #region methods
            public DateTime GetDate()
            { return Date; }

            public e_Dot GetDotType()
            { return _type; }
            public void SetDate(object sender, DateTime date)
            { this.Date = date; }
            #endregion
        }
    }
    
}
