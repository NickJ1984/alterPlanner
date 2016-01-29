using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.Function.iface;
using alter.types;

namespace alter.Function.classes
{
    public class function : IFunction
    {
        #region vars
        private DateTime _date;
        private eDirection _direction;
        private Func<DateTime, DateTime> _function;
        #endregion
        #region props
        public DateTime date
        {
            get { return _date; }
            set
            {
                if(_date != value)
                {
                    DateTime temp = _date;
                    _date = value;
                    onDateChange(new EA_valueChange<DateTime>(temp, _date));
                }
            }
        }
        public eDirection direction
        {
            get { return _direction; }
            set
            {
                if (_direction != value)
                {
                    eDirection temp = _direction;
                    _direction = value;
                    _function = getSvcFunction(_direction);
                    onDirectionChange(new EA_valueChange<eDirection>(temp, _direction));
                }
            }
        }
        #endregion
        #region events
        public event EventHandler<EA_valueChange<DateTime>> event_dateChanged;
        public event EventHandler<EA_valueChange<eDirection>> event_directionChanged;
        #endregion
        #region constructor
        public function(DateTime date, eDirection direction)
        {
            this.direction = direction;
            this.date = date;
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
        #endregion
        #region methods
        public DateTime checkDate(DateTime Date)
        {
            return _function(Date);
        }
        public Func<DateTime, DateTime, DateTime> getFunctionDir(eDirection direction)
        {
            switch (direction)
            {
                case eDirection.LeftMax:
                case eDirection.RightMax:
                case eDirection.Fixed:
                    return new Func<DateTime, DateTime, DateTime>(
                        (Limit, Date) => Date = Limit);

                case eDirection.Left:
                    return new Func<DateTime, DateTime, DateTime>(
                    (Limit, Date) => (Date > Limit) ? Limit : Date);

                case eDirection.Right:
                    return new Func<DateTime, DateTime, DateTime>(
                    (Limit, Date) => (Date < Limit) ? Limit : Date);
                default:
                    throw new Exception("getSvcFunction: wrong direction value");
            }
        }


        public DateTime getDate()
        { return date; }
        public void setDate(DateTime date)
        { this.date = date; }

        public eDirection getDirection()
        { return direction; }
        public void setDirection(eDirection direction)
        { this.direction = direction; }
        #endregion
        #region service
        private Func<DateTime, DateTime> getSvcFunction(eDirection direction)
        {
            switch(direction)
            {
                case eDirection.LeftMax:
                case eDirection.RightMax:
                case eDirection.Fixed:
                    return new Func<DateTime, DateTime>((Date) => _date);

                case eDirection.Left:
                    return new Func<DateTime, DateTime>(
                    (Date) => (Date > _date) ? _date : Date);

                case eDirection.Right:
                    return new Func<DateTime, DateTime>(
                    (Date) => (Date < _date) ? _date : Date);
                default:
                    throw new Exception("getSvcFunction: wrong direction value");
            }   

        }
        #endregion
    }
}
