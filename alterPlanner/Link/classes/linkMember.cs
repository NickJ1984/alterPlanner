using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;
using alter.types;
using alter.classes;
using alter.args;
using alter.Function.classes;
using alter.Link.iface;
using alter.Service.classes;
using alter.Task.iface;

namespace alter.Link.classes
{
    /// <summary>
    /// Класс участника связи, хранит информацию об участнике, а так же зависимость связи для него
    /// </summary>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="objectAlreadyExistException"></exception>
    public class linkMember : alter.Link.iface.ILMember
    {
        //НАДО ТЕСТИРОВАТЬ!
        #region Vars

        protected dependDotAdapter ddAdapter = null;
        /// <summary>
        /// Так как в классе ILink используются только связи СтартСтарт, 
        /// СтартФиниш, ФинишСтарт, ФинишФиниш, то направление задано константой
        /// </summary>
        private const e_Direction cDirection = e_Direction.Fixed;

        /// <summary>
        /// Экземпляр класса зависимости
        /// </summary>
        protected Dependence depend = null;
        /// <summary>
        /// Переменная хранящая значение задержки связи
        /// </summary>
        protected double _delay;
        /// <summary>
        /// Переменная хранящая значение даты точки зависимости другого участника связи
        /// </summary>
        protected DateTime _date;
        #endregion
        #region Properties
        public double membersDaysDelta { get; protected set; }
        /// <summary>
        /// Свойство задающее значение даты точки зависимости другого участника связи
        /// </summary>
        public DateTime date
        {
            get { return _date; }
            set
            {
                _date = value;
                onDependDateRecalculate();
            }
        }
        /// <summary>
        /// Свойство задающее значение задержки связи
        /// </summary>
        public double delay
        {
            get { return _delay; }
            set
            {
                if(value < 0) return;
                _delay = value;
                onDependDateRecalculate();
            }
        }

        /// <summary>
        /// Свойство задающее точку зависимости подчиненного объекта
        /// </summary>
        public e_Dot dependDot
        {
            get { return depend.dependDot; }
            set
            {
                depend.dependDot = value;
                ddAdapter.setDependDot(value);
            }
        } 
        /// <summary>
        /// Свойство возвращающее идентификатор члена связи
        /// </summary>
        public IId memberID { get; protected set; }
        /// <summary>
        /// Свойство возвращающее тип зависимости члена связи
        /// </summary>
        public e_DependType dependType { get; protected set; }
        /// <summary>
        /// Свойство возвращает дату зависимости (транслирует метод IDependence.GetDate())
        /// </summary>
        public DateTime dependDate => depend.GetDate();
        #endregion
        #region Events
        public event EventHandler<ea_ValueChange<double>>  event_neighboursDeltaChanged;
        #endregion
        #region Constructor
        /// <summary>
        /// НАДО ТЕСТИРОВАТЬ!
        /// Класс участника связи, хранит информацию об участнике, а так же зависимость связи для него
        /// </summary>
        /// <param name="member">Идентификатор участника связи</param>
        /// <param name="dependType">Тип зависимости участника связи</param>
        /// <param name="delay">Значение задержки связи</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="objectAlreadyExistException"></exception>
        public linkMember(IId member, e_DependType dependType, double delay)
        {
            memberID = member;
            this.dependType = dependType;
            membersDaysDelta = 0;

            this._delay = delay;
            ddAdapter = new dependDotAdapter(memberID, memberID, (ILine)memberID);
        }
        /// <summary>
        /// Деструктор экземпляра класса
        /// </summary>
        ~linkMember()
        {
            depend = null;
            memberID = null;
            ddAdapter.clear();
            ddAdapter = null;
        }
        #endregion
        #region Initialize
        /// <summary>
        /// Инициализирует класс зависимости <seealso cref="Dependence"/>
        /// </summary>
        /// <param name="dependDate">Дата зависимости</param>
        /// <param name="limitType">Тип зависимости</param>
        /// <exception cref="objectAlreadyExistException">Выбрасывает исключение если класс <seealso cref="Dependence"/> уже был инициализирован</exception>
        public void init_Dependence(ILink parent, DateTime dependDate, e_TskLim limitType)
        {
            if(depend != null) throw new objectAlreadyExistException();

            date = dependDate;
            depend = new Dependence(date, cDirection, Hlp.GetDepenDot(limitType, dependType));
            depend.setSender(parent);
        }
        #endregion
        #region Handlers
        #region External
        /// <summary>
        /// Подсчитывает разницу в дате зависимости связи и дате зависимой точки подчиненного объект
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void handler_dependObjectDateChanged(object sender, ea_ValueChange<DateTime> e)
        {
            var diff = (dependDate - e.NewValue).Ticks;
            double newDelta = TimeSpan.FromTicks(diff).Days;
            if (newDelta != membersDaysDelta)
            {
                double old = membersDaysDelta;
                membersDaysDelta = newDelta;

                onDeltaChange(old, membersDaysDelta);
            }
        }
        /// <summary>
        /// Обработчик события изменения типа связи
        /// </summary>
        /// <param name="sender">Объект вызвавший событие</param>
        /// <param name="e">Аргумент события</param>
        public void handler_limitTypeChanged(object sender, ea_ValueChange<e_TskLim> e)
        { dependDot = Hlp.GetDepenDot(e.NewValue, dependType); }
        /// <summary>
        /// Обработчик события изменения даты зависимости (дата зависящей точки другого участника связи)
        /// </summary>
        /// <param name="sender">Объект вызвавший событие</param>
        /// <param name="e">Аргумент события</param>
        public void handler_dependDateChange(object sender, ea_ValueChange<DateTime> e)
        { date = e.NewValue; }
        /// <summary>
        /// Обработчик события изменения задержки связи
        /// </summary>
        /// <param name="sender">Объект вызвавший событие</param>
        /// <param name="e">Аргумент события</param>
        public void handler_delayChanged(object sender, ea_ValueChange<double> e)
        { delay = e.NewValue; }
        #endregion
        #region Internal

        protected void onDeltaChange(double Old, double New)
        {
            event_neighboursDeltaChanged?.Invoke(this, new ea_ValueChange<double>(Old, New));
        }
        /// <summary>
        /// Метод пересчета даты зависимости
        /// </summary>
        protected void onDependDateRecalculate()
        { depend.date = _date.AddDays(_delay); }
        #endregion
        #endregion
        #region Methods
        /// <summary>
        /// Метод передачи зависимости в виде интерфейса <seealso cref="IDependence"/>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Если класс зависимости не инициализирован</exception>
        public IDependence GetDependence()
        {
            if(depend == null) throw new NullReferenceException();
            return depend;
        }
        /// <summary>
        /// Метод получения типа зависимости члена связи
        /// </summary>
        /// <returns></returns>
        public e_DependType GetDependType()
        { return dependType; }
        /// <summary>
        /// Метод возвращает идентификатор члена связи
        /// </summary>
        /// <returns></returns>
        public IId GetMemberId()
        { return memberID; }

        public IDot getObjectDependDotInfo()
        {
            return ddAdapter;
        }
        #endregion
        #region Exceptions
        /// <summary>
        /// Исключение выбрасывается при попытке повторной инициализации объекта, для которого разрешена только одна инициализация
        /// </summary>
        public class objectAlreadyExistException : ApplicationException
        {
            /// <summary>
            /// Исключение выбрасывается при попытке повторной инициализации объекта, для которого разрешена только одна инициализация
            /// </summary>
            public objectAlreadyExistException() 
                : base("Объект уже инициализирован, повторная инициализация невозможна.")
            { }
        }
        #endregion
    }
}
