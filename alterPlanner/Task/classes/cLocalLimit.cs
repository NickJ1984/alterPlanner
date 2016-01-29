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
        private class cLocalLimit : ITLocalLimit
        {
            #region vars
            private eDirection _dir;
            private DateTime _date;
            private eDot _dot;
            private eTLLim _type;
            private IFunction func;
            #endregion
            #region props

            #endregion
            #region events
            public event EventHandler<EA_valueChange<DateTime>> event_dateChanged;
            public event EventHandler<EA_valueChange<eDirection>> event_directionChanged;
            public event EventHandler<EA_valueChange<eDot>> event_dependDotChanged;
            #endregion
            #region constructors
            public cLocalLimit(DateTime date, eDirection direction)
            {
                _date = date;
                _dir = direction;

                func = new function(_date, _dir);
            }
            #endregion
            #region handlers
            private void onDateChange(EA_valueChange<DateTime> args)
            {
                EventHandler<EA_valueChange<DateTime>> handler = event_dateChanged;
                if (handler != null) handler(this, args);
            }
            private void onDirectionChange(EA_valueChange<eDirection> args)
            {
                EventHandler<EA_valueChange<eDirection>> handler = event_directionChanged;
                if (handler != null) handler(this, args);
            }
            private void onDotTypeChanged(EA_valueChange<eDot> args)
            {
                EventHandler<EA_valueChange<eDot>> handler = event_dependDotChanged;
                if (handler != null) handler(this, args);
            }
            #endregion
            #region methods
            public DateTime checkDate(DateTime Date)
            { return func.checkDate(Date); }

            public DateTime getDate()
            { return _date; }
            public void setDate(DateTime date)
            {
                if (date == _date) return;

                DateTime temp = _date;
                _date = date;

                func.setDate(_date);

                onDateChange(new EA_valueChange<DateTime>(temp, _date));
            }

            public eDirection getDirection()
            { return _dir; }
            public void setDirection(eDirection direction)
            {
                if (_dir == direction) return;

                eDirection temp = _dir;
                _dir = direction;

                func.setDirection(_dir);
                
                onDirectionChange(new EA_valueChange<eDirection>(temp, _dir));
            }

            public eDot getDotType()
            { return _dot; }
            public void setDotType(eDot type)
            {
                if (_dot == type) return;

                eDot temp = _dot;
                _dot = type;

                onDotTypeChanged(new EA_valueChange<eDot>(temp, _dot));
            }

            public void setLLimit(eTLLim localLimit)
            {
                throw new NotImplementedException();
            }

            public eTLLim getLLimit()
            { return _type; }

            public eDot getDependDot()
            { return _dot; }
            #endregion
            #region service
            
            #endregion
        }
    }
    
}
