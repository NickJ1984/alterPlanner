using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.OleDb;
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
        //Работает тестировал
        #region Vars
        /// <summary>
        /// Ссылка на объект родитель
        /// </summary>
        private object sender;
        /// <summary>
        /// Класс выбора зависимой точки объекта
        /// </summary>
        protected dependDotAdapter ddAdapter;
        /// <summary>
        /// Так как в классе ILink используются только связи СтартСтарт, 
        /// СтартФиниш, ФинишСтарт, ФинишФиниш, то направление задано константой
        /// </summary>
        protected const e_Direction cDirection = e_Direction.Fixed;
        /// <summary>
        /// Переменная хранящая значение задержки связи
        /// </summary>
        protected double _delay;
        /// <summary>
        /// Переменная хранящая значение даты точки зависимости другого участника связи
        /// </summary>
        protected DateTime _date;
        /// <summary>
        /// Дата зависимости подчиненного объекта
        /// </summary>
        protected DateTime _dependDate;
        #endregion
        #region Properties
        /// <summary>
        /// Направление зависимости
        /// </summary>
        public e_Direction direction => cDirection;
        /// <summary>
        /// Разность дат зависимости и зависимой точки подчиненного объекта
        /// </summary>
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
                dependDateRecalculate();
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
                _delay = value;
                dependDateRecalculate();
            }
        }
        /// <summary>
        /// Свойство задающее точку зависимости подчиненного объекта
        /// </summary>
        public e_Dot dependDot
        {
            get { return ddAdapter.dotType; }
            set
            {
                e_Dot Old = ddAdapter.dotType;
                if (ddAdapter.setDependDot(value))
                    event_dependDotChanged?.Invoke(sender, new ea_ValueChange<e_Dot>(Old, ddAdapter.dotType));
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
        /// Свойство возвращает дату зависимости подчиненного объекта
        /// </summary>
        public DateTime dependDate
        {
            get { return _dependDate; }
            protected set
            {
                if (value != _dependDate)
                {
                    _dependDate = value;
                    deltaRecalculate();
                }
            }
        }
        #endregion
        #region Events
        /// <summary>
        /// Событие при изменении разности дат зависимости связи и зависимой точки подчиненного объекта
        /// </summary>
        public event EventHandler<ea_ValueChange<double>>  event_neighboursDeltaChanged;
        /// <summary>
        /// Событие при изменении даты зависисмости подчиненного объекта
        /// </summary>
        public event EventHandler<ea_ValueChange<DateTime>> event_dependDateChanged;
        /// <summary>
        /// Событие при изменении зависимой точки подчиненного объекта
        /// </summary>
        public event EventHandler<ea_ValueChange<e_Dot>> event_dependDotChanged;
        #endregion
        #region Constructor
        /// <summary>
        /// Класс участника связи, хранит информацию об участнике, а так же зависимость связи для него
        /// </summary>
        /// <param name="parent">Ссылка на объект владелец</param>
        /// <param name="dependType">Тип зависимости участника связи</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="objectAlreadyExistException"></exception>
        public linkMember(ILink parent, e_DependType dependType)
        {
            sender = parent;
            this.dependType = dependType;
            this._delay = delay;
            membersDaysDelta = 0;

            memberID = null;
            ddAdapter = null;
        }
        /// <summary>
        /// Деструктор экземпляра класса
        /// </summary>
        ~linkMember()
        {
            memberID = null;
            ddAdapter.event_DateChanged -= handler_dependObjectDateChanged;
            ddAdapter?.clear();
            ddAdapter = null;
            sender = null;
        }
        #endregion
        #region Initialize
        /// <summary>
        /// Инициализирует класс зависимой точки <seealso cref="dependDotAdapter"/>
        /// </summary>
        /// <param name="memberID">Идентификатор участника связи</param>
        /// <param name="memberLine">Интерфейс <seealso cref="ILine"/> участника связи</param>
        /// <param name="dependDot">Зависимая точка подчиненного объекта</param>
        /// <exception cref="objectAlreadyExistException">Выбрасывает исключение если класс <seealso cref="dependDotAdapter"/> уже был инициализирован</exception>
        public void init_member(IId memberID, ILine memberLine, e_Dot dependDot)
        {
            if(ddAdapter != null) throw new objectAlreadyExistException();

            this.memberID = memberID;
            ddAdapter = new dependDotAdapter(sender, memberID, memberLine);
            ddAdapter.event_DateChanged += handler_dependObjectDateChanged;
            ddAdapter.setDependDot(dependDot);
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
            deltaRecalculate();
        }
        /// <summary>
        /// Обработчик события изменения типа связи
        /// </summary>
        /// <param name="sender">Объект вызвавший событие</param>
        /// <param name="e">Аргумент события</param>
        public void handler_limitTypeChanged(object sender, ea_ValueChange<e_TskLim> e)
        {
            dependDot = Hlp.GetDepenDot(e.NewValue, dependType); 
        }

        /// <summary>
        /// Обработчик события изменения даты зависимости (дата зависящей точки другого участника связи)
        /// </summary>
        /// <param name="sender">Объект вызвавший событие</param>
        /// <param name="e">Аргумент события</param>
        public void handler_dependDateChange(object sender, ea_ValueChange<DateTime> e)
        {
            date = e.NewValue; 
        }

        /// <summary>
        /// Обработчик события изменения задержки связи
        /// </summary>
        /// <param name="sender">Объект вызвавший событие</param>
        /// <param name="e">Аргумент события</param>
        public void handler_delayChanged(object sender, ea_ValueChange<double> e)
        {
            delay = e.NewValue; 
        }
        #endregion
        #region Internal
        protected void onDeltaChange(double Old, double New)
        {
            event_neighboursDeltaChanged?.Invoke(this, new ea_ValueChange<double>(Old, New));
        }
        /// <summary>
        /// Метод пересчета даты зависимости
        /// </summary>
        protected void dependDateRecalculate()
        {
            DateTime old = dependDate;
            dependDate = _date.AddDays(_delay);
            if(dependDate != old)
                event_dependDateChanged?.Invoke
                    (sender, new ea_ValueChange<DateTime>(old, dependDate));
        }
        #endregion
        #endregion
        #region Methods
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
        /// <summary>
        /// Возвращает ссылку на зависимую точку подчиненного объекта (используется <seealso cref="dependDotAdapter"/>)
        /// </summary>
        /// <returns>Ссылка на зависимую точку подчиненного объекта </returns>
        public IDot getObjectDependDotInfo()
        {
            return ddAdapter;
        }
        #region Service
        /// <summary>
        /// Пересчет разности дат зависимости и зависимой точки подчиненного объекта
        /// </summary>
        protected void deltaRecalculate()
        {
            var diff = (ddAdapter.date - dependDate).Ticks;
            double newDelta = TimeSpan.FromTicks(diff).Days;
            if (!newDelta.Equals(membersDaysDelta))
            {
                double old = membersDaysDelta;
                membersDaysDelta = newDelta;
                onDeltaChange(old, membersDaysDelta);
            }
        }
        #endregion
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
