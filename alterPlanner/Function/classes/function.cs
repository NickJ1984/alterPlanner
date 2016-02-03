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
    public struct VectorF
    {
        public DateTime Date;
        public e_Direction Direction;
    }
    public class function : IFunction
    {
        #region vars
        protected DateTime Date;
        protected e_Direction Direction;
        protected Func<DateTime, DateTime> Function;
        #endregion
        #region props
        public virtual DateTime date
        {
            get { return Date; }
            set
            {
                if(Date != value)
                {
                    DateTime temp = Date;
                    Date = value;
                    OnDateChange(new ea_ValueChange<DateTime>(temp, Date));
                }
            }
        }
        public virtual e_Direction direction
        {
            get { return Direction; }
            set
            {
                if (Direction != value)
                {
                    e_Direction temp = Direction;
                    Direction = value;
                    Function = GetSvcFunction(Direction);
                    OnDirectionChange(new ea_ValueChange<e_Direction>(temp, Direction));
                }
            }
        }
        #endregion
        #region events
        public event EventHandler<ea_ValueChange<DateTime>> event_DateChanged;
        public event EventHandler<ea_ValueChange<e_Direction>> event_DirectionChanged;
        #endregion
        #region constructor
        public function(DateTime date, e_Direction direction)
        {
            this.direction = direction;
            this.date = date;
            Function = GetSvcFunction(Direction);
        }
        #endregion
        #region handlers
        protected void OnDateChange(ea_ValueChange<DateTime> args)
        {
            EventHandler<ea_ValueChange<DateTime>> handler = event_DateChanged;
            if (handler != null) handler(this, args);
        }
        protected void OnDirectionChange(ea_ValueChange<e_Direction> args)
        {
            EventHandler<ea_ValueChange<e_Direction>> handler = event_DirectionChanged;
            if (handler != null) handler(this, args);
        }
        #endregion
        #region methods
        public DateTime CheckDate(DateTime date)
        {
            return Function(Date);
        }
        public Func<DateTime, DateTime, DateTime> GetFunctionDir(e_Direction direction)
        {
            switch (direction)
            {
                //case eDirection.LeftMax:
                //case eDirection.RightMax:
                case e_Direction.Fixed:
                    return new Func<DateTime, DateTime, DateTime>(
                        (limit, Date) => Date = limit);

                case e_Direction.Left:
                    return new Func<DateTime, DateTime, DateTime>(
                    (limit, Date) => (Date > limit) ? limit : Date);

                case e_Direction.Right:
                    return new Func<DateTime, DateTime, DateTime>(
                    (limit, Date) => (Date < limit) ? limit : Date);
                default:
                    throw new Exception("getSvcFunction: wrong direction value");
            }
        }


        public virtual DateTime GetDate()
        { return date; }
        public void SetDate(DateTime date)
        { this.date = date; }

        public virtual e_Direction GetDirection()
        { return direction; }
        public void SetDirection(e_Direction direction)
        { this.direction = direction; }
        #endregion
        #region service
        protected Func<DateTime, DateTime> GetSvcFunction(e_Direction direction)
        {
            switch(direction)
            {
                //case eDirection.LeftMax:
                //case eDirection.RightMax:
                case e_Direction.Fixed:
                    return new Func<DateTime, DateTime>((Date) => this.Date);

                case e_Direction.Left:
                    return new Func<DateTime, DateTime>(
                    (Date) => (Date > this.Date) ? this.Date : Date);

                case e_Direction.Right:
                    return new Func<DateTime, DateTime>(
                    (Date) => (Date < this.Date) ? this.Date : Date);
                default:
                    throw new Exception("getSvcFunction: wrong direction value");
            }   

        }
        #endregion
    }
}
