using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.iface;
using alter.types;

namespace alter.Service.classes
{
    public class line : ISender, ILine
    {
        #region Переменные
        public static readonly DateTime initDate = new DateTime(1,1,1);

        protected eSender _sender;

        protected Dot _start;
        protected Dot _finish;

        protected double _duration;
        #endregion
        #region Свойства
        public object sender
        {
            get { return _sender.sender; }
            set { _sender.sender = value; }
        }

        public Dot dotStart => _start;
        public Dot dotFinish => _finish;

        public double duration
        {
            get { return _duration; }
            protected set
            {
                if (value == _duration) return;
                if (value < 0) throw new ApplicationException("Длительность не может быть менее нуля");

                double temp = _duration;
                _duration = value;

                event_DurationChanged?.Invoke(sender, new ea_ValueChange<double>(temp, _duration));
            }
        } 
        public DateTime start
        {
            get { return _start.date; }
            set
            {
                if(value > _finish.date) throw new ArgumentException("Дата старта должна быть меньше или равна дате финиша");

                _start.date = value;
                updateDuration();
            }
        }
        public DateTime finish
        {
            get { return _finish.date; }
            set
            {
                if (value < _start.date) throw new ArgumentException("Дата финиша должна быть больше или равна дате старта");

                _finish.date = value;
                updateDuration();
            }
        }
        #endregion
        #region События
        public event EventHandler<ea_ValueChange<double>> event_DurationChanged;
        public event EventHandler<ea_ValueChange<object>> event_senderChanged
        {
            add { _sender.event_senderChanged += value; }
            remove { _sender.event_senderChanged -= value; }
        }
        #endregion
        #region Конструктор
        public line(object owner, DateTime start, DateTime finish)
        {
            if(owner == null) throw new ArgumentNullException(nameof(owner));
            if(start >= finish) throw new ArgumentException("Дата старта должна быть меньше или равна дате финиша");

            _start = new Dot(e_Dot.Start);
            _finish = new Dot(e_Dot.Finish);

            _start.date = start;
            _finish.date = finish;

            updateDuration();
        }
        public line(object owner, DateTime start, double duration)
            :this(owner, start, start.AddDays(duration))
        { }
        public line(object owner, DateTime StartFinishDate)
            :this(owner, StartFinishDate, StartFinishDate)
        { }
        public line(object owner)
            :this(owner, initDate, initDate)
        { }
        ~line()
        {
            _sender = null;
            _start = null;
            _finish = null;
        }
        #endregion
        #region Методы
        public bool durationChange(e_Dot moveableDot, double newDuration)
        {
            if (newDuration < 0 || newDuration == duration) return false;

            if (moveableDot == e_Dot.Start)
                _start.date = _finish.date.AddDays(-newDuration);
            else if (moveableDot == e_Dot.Finish)
                _finish.date = _start.date.AddDays(newDuration);
            else return false;

            return true;
        }
        public bool move(e_Dot dot, DateTime date)
        {
            if (dot == e_Dot.Finish)
            {
                if (_finish.date == date) return false;

                _finish.date = date;
                _start.date = _finish.date.AddDays(-duration);

                return true;
            }
            else if (dot == e_Dot.Start)
            {
                if (_start.date == date) return false;

                _start.date = date;
                _finish.date = _start.date.AddDays(duration);

                return true;
            }
            else return false;
        }
        #endregion
        #region Реализация интерфейса
        #region ILine
        public IDot GetDot(e_Dot type)
        {
            if (type == e_Dot.Start) return _start;
            else if (type == e_Dot.Finish) return _finish;
            else throw new ApplicationException(nameof(GetDot));
        }
        public double GetDuration()
        {
            return duration;
        }
        #endregion
        #endregion
        #region Служебные
        protected void updateDuration()
        {
            duration = _finish.date.Subtract(_start.date).Days;
        }
        #endregion
    }
}
