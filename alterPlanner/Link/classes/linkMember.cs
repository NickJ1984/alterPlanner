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
        /// <summary>
        /// Так как в классе ILink используются только связи СтартСтарт, 
        /// СтартФиниш, ФинишСтарт, ФинишФиниш, то направление задано константой
        /// </summary>
        private const e_Direction direction = e_Direction.Fixed;

        /// <summary>
        /// Экземпляр класса зависимости
        /// </summary>
        protected Dependence depend = null;
        /// <summary>
        /// Переменная хранящая последнее значение задержки связи
        /// </summary>
        protected double delay;
        /// <summary>
        /// Переменная хранящая последнее значение даты зависимой точки другого участника связи
        /// </summary>
        protected DateTime date;
        #endregion
        #region Properties
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

            this.delay = delay;
        }
        /// <summary>
        /// Деструктор экземпляра класса
        /// </summary>
        ~linkMember()
        {
            depend = null;
            memberID = null;
        }
        #endregion
        #region Initialize
        /// <summary>
        /// Инициализирует класс зависимости <seealso cref="Dependence"/>
        /// </summary>
        /// <param name="dependDate">Дата зависимости</param>
        /// <param name="limitType">Тип зависимости</param>
        /// <exception cref="objectAlreadyExistException">Выбрасывает исключение если класс <seealso cref="Dependence"/> уже был инициализирован</exception>
        public void init_Dependence(DateTime dependDate, e_TskLim limitType)
        {
            if(depend != null) throw new objectAlreadyExistException();

            date = dependDate;
            depend = new Dependence(date, direction, Hlp.GetDepenDot(limitType, dependType));
        }
        #endregion
        #region Handlers
        #region External
        /// <summary>
        /// Обработчик события изменения типа связи
        /// </summary>
        /// <param name="sender">Объект вызвавший событие</param>
        /// <param name="e">Аргумент события</param>
        public void handler_limitTypeChanged(object sender, ea_ValueChange<e_TskLim> e)
        { depend.SetDependDot(Hlp.GetDepenDot(e.NewValue, dependType)); }
        /// <summary>
        /// Обработчик события изменения даты зависимости (дата зависящей точки другого участника связи)
        /// </summary>
        /// <param name="sender">Объект вызвавший событие</param>
        /// <param name="e">Аргумент события</param>
        public void handler_dependDateChange(object sender, ea_ValueChange<DateTime> e)
        {
            date = e.NewValue;
            onDependDateRecalculate();
        }
        /// <summary>
        /// Обработчик события изменения задержки связи
        /// </summary>
        /// <param name="sender">Объект вызвавший событие</param>
        /// <param name="e">Аргумент события</param>
        public void handler_delayChanged(object sender, ea_ValueChange<double> e)
        {
            delay = e.NewValue;
            onDependDateRecalculate();
        }
        #endregion
        #region Internal
        /// <summary>
        /// Метод пересчета даты зависимости
        /// </summary>
        protected void onDependDateRecalculate()
        { depend.date = date.AddDays(delay); }
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
    /*
    /// <summary>
    /// Структура отражающая информацию по участнику связи
    /// </summary>
    public struct LinkMember : alter.Link.iface.ILMember
    {
        private IId _mbr;
        private e_DependType _type;
        public Dependence Depend;

        public LinkMember(IId member, e_DependType dependType, e_TskLim limitType, DateTime limitDate)
        {
            _mbr = member;
            _type = dependType;
            
            Depend = new Dependence(limitDate, e_Direction.Fixed, Hlp.GetDepenDot(limitType, dependType));
        }
        public LinkMember(IId member, e_DependType dependType, e_TskLim limitType)
            :this(member, dependType, limitType, Hlp.InitDate)
        { }


        public void SetInfo(IId member)
        { _mbr = member; }
        public e_DependType GetDependType()
        { return _type; }
        public void SetDependType(e_DependType type)
        { if (_type != type) _type = type; }
        public IDependence GetDependence()
        { return Depend; }
        public IId GetMemberId()
        { return _mbr; }
    }*/
}
