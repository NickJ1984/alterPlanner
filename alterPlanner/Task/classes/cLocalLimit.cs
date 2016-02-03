using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.Task.iface.TaskComponents;
using alter.types;
using alter.Function.iface;
using alter.Function.classes;


namespace alter.Task.classes
{
    public partial class task
    {
        private class CLocalLimit : ITLocalLimit
        {
            #region vars
            private e_Direction _dir;
            private DateTime _date;
            private e_Dot _dot;
            private e_TlLim _type;
            private IFunction _func;
            #endregion
            #region props

            #endregion
            #region events
            public event EventHandler<ea_ValueChange<DateTime>> event_DateChanged;
            public event EventHandler<ea_ValueChange<e_Direction>> event_DirectionChanged;
            public event EventHandler<ea_ValueChange<e_Dot>> event_DependDotChanged;
            #endregion
            #region constructors
            public CLocalLimit(DateTime date, e_Direction direction)
            {
                _date = date;
                _dir = direction;

                _func = new function(_date, _dir);
            }
            #endregion
            #region handlers
            private void OnDateChange(ea_ValueChange<DateTime> args)
            {
                EventHandler<ea_ValueChange<DateTime>> handler = event_DateChanged;
                if (handler != null) handler(this, args);
            }
            private void OnDirectionChange(ea_ValueChange<e_Direction> args)
            {
                EventHandler<ea_ValueChange<e_Direction>> handler = event_DirectionChanged;
                if (handler != null) handler(this, args);
            }
            private void OnDotTypeChanged(ea_ValueChange<e_Dot> args)
            {
                EventHandler<ea_ValueChange<e_Dot>> handler = event_DependDotChanged;
                if (handler != null) handler(this, args);
            }
            #endregion
            #region methods
            public DateTime CheckDate(DateTime date)
            { return _func.CheckDate(date); }

            public DateTime GetDate()
            { return _date; }
            public void SetDate(DateTime date)
            {
                if (date == _date) return;

                DateTime temp = _date;
                _date = date;

                _func.SetDate(_date);

                OnDateChange(new ea_ValueChange<DateTime>(temp, _date));
            }

            public e_Direction GetDirection()
            { return _dir; }
            public void SetDirection(e_Direction direction)
            {
                if (_dir == direction) return;

                e_Direction temp = _dir;
                _dir = direction;

                _func.SetDirection(_dir);
                
                OnDirectionChange(new ea_ValueChange<e_Direction>(temp, _dir));
            }

            public e_Dot GetDotType()
            { return _dot; }
            public void SetDotType(e_Dot type)
            {
                if (_dot == type) return;

                e_Dot temp = _dot;
                _dot = type;

                OnDotTypeChanged(new ea_ValueChange<e_Dot>(temp, _dot));
            }

            public void SetLLimit(e_TlLim localLimit)
            {
                throw new NotImplementedException();
            }

            public e_TlLim GetLLimit()
            { return _type; }

            public e_Dot GetDependDot()
            { return _dot; }
            #endregion
            #region service
            
            #endregion
        }
    }
    
}
