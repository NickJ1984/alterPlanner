using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.iface;
using alter.Project.iface;
using alter.Service.classes;
using alter.types;

namespace alter.Function.Testing
{
    #region Перечисление
    public enum e_limit
    {
        asSoonAsPossible = 1,
        asLateAsPossible = 2,
        startNotEarlier = 3,
        startNotLater = 4,
        startFixed = 5,
        finishNotEarlier = 6,
        finishNotLater = 7,
        finishFixed = 8
    }
    #endregion
    #region Абстракт
    public abstract class ALimitManager : ILine, ILimit<e_limit>
    {
        #region Переменные
        #region Значения
        protected e_limit _limit;
        protected e_Dot activeDot;

        protected double _duration;
        #endregion
        #region Ссылки
        protected IId _owner;
        protected IProject _project;
        #endregion
        #region Точки
        protected Dot _start;
        protected Dot _finish;
        #endregion
        #region Делегаты
        protected Action unsuscribe;
        #endregion
        #endregion
        #region Свойства
        public DateTime start => _start.date;
        public DateTime finish => _finish.date;

        public e_limit limit
        {
            get { return GetLimit(); }
            set { SetLimit(value); }
        }
        public double duration
        {
            get { return GetDuration(); }
            set { SetDuration(value);}
        }

        protected DateTime projectStart =>
            _project.GetDot(e_Dot.Start).GetDate();
        protected DateTime projectFinish =>
            _project.GetDot(e_Dot.Finish).GetDate();
        #endregion
        #region События
        public event EventHandler<ea_ValueChange<e_limit>> event_LimitChanged;
        public event EventHandler<ea_ValueChange<double>> event_DurationChanged;
        public event EventHandler<ea_ValueChange<DateTime>> event_startDateChange
        {
            add
            {
                _start.event_DateChanged -= value;
                _start.event_DateChanged += value;
            }
            remove { _start.event_DateChanged -= value; }
        }
        public event EventHandler<ea_ValueChange<DateTime>> event_finishDateChange
        {
            add
            {
                _finish.event_DateChanged -= value;
                _finish.event_DateChanged += value;
            }
            remove { _finish.event_DateChanged -= value; }
        }
        #endregion
        #region Конструктор
        public ALimitManager(IProject project, IId owner, e_limit limitType)
        {
            if (project == null || owner == null) throw new NullReferenceException();

            _project = project;
            _owner = owner;

            _start = new Dot(e_Dot.Start);
            _finish = new Dot(e_Dot.Finish);

            _project.GetDot(e_Dot.Start).event_DateChanged
                += handler_projectStartChanged;
            _project.GetDot(e_Dot.Finish).event_DateChanged
                += handler_projectFinishChanged;

            _limit = limitType;

            unsuscribe = () =>
            {
                _project.GetDot(e_Dot.Start).event_DateChanged
                    -= handler_projectStartChanged;
                _project.GetDot(e_Dot.Finish).event_DateChanged
                    -= handler_projectFinishChanged;
                unsuscribe = null;
            };
        }
        ~ALimitManager()
        {
            unsuscribe();
            _owner = null;
            _project = null;
            _start = null;
            _finish = null;
        }
        #endregion
        #region Обработчики
        #region Внутренние
        protected virtual void handler_projectStartChanged(object sender, ea_ValueChange<DateTime> e)
        {
            startProjectUpdated();
        }
        protected virtual void handler_projectFinishChanged(object sender, ea_ValueChange<DateTime> e)
        {
            finishProjectUpdated();
        }
        #endregion
        #region Внешние
        public virtual void handler_durationChanged(object sender, ea_ValueChange<double> e)
        {
            SetDuration(e.NewValue);
        }
        #endregion
        #endregion
        #region Методы запуска событий
        protected void durationChangedPushEvent(object sender, ea_ValueChange<double> e)
        {
            event_DurationChanged?.Invoke(sender, e);
        }
        #endregion
        #region Методы
        #region Изменения
        protected void finishProjectUpdated()
        {
            throw new NotImplementedException();
        }
        protected void startProjectUpdated()
        {
            throw new NotImplementedException();
        }
        protected void limitUpdated()
        {
            throw new NotImplementedException();
        }
        protected void durationUpdated()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
        #region ILine
        public IDot GetDot(e_Dot type)
        {
            if(!Enum.IsDefined(typeof(e_Dot), type)) throw new ArgumentNullException(nameof(type));

            return type == e_Dot.Start ? _start : _finish;
        }
        public double GetDuration()
        {
            return _duration;
        }
        #endregion
        #region ILimit<e_Limit>
        public bool SetLimit(e_limit limitType)
        {
            if(!Enum.IsDefined(typeof(e_limit), limitType)) throw new ArgumentException("Неверное значение аругмента");
            if (_limit == limitType) return false;

            e_limit temp = _limit;
            _limit = limitType;

            limitUpdated();
            event_LimitChanged?.Invoke(this, new ea_ValueChange<e_limit>(temp, _limit));

            return true;
        }
        public e_limit GetLimit()
        {
            return _limit;
        }
        #endregion
        #region Длительность
        public void SetDuration(double duration)
        {
            if(duration < 0) throw new ArgumentException("Длительность не может иметь значения меньше нуля");

            if (duration != _duration)
            {
                double temp = _duration;
                _duration = duration;
                durationUpdated();
                durationChangedPushEvent(this, new ea_ValueChange<double>(temp, _duration));
            }
        }
        #endregion
    }
    #endregion
    #region Интерфейс
    public interface ILimitManager
    {
        

    }
    #endregion
    #region Основная реализация
    public partial class limitManager
    {

    }
    #endregion
    #region Отслеживание максимальной и минимальной даты
    public partial class limitManager
    {

    }
    #endregion

}
