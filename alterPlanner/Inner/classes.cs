using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections;
using alter.types;
using alter.iface;
using alter.args;

namespace alter.classes
{
    #region ID
    /// <summary>
    /// Класс реализующий механизм идентификации объектов.
    /// </summary>
    public class identity : IId
    {
        /// <summary>
        /// Уникальный идентификатор объекта, является константой для экземпляра к которому принадлежит.
        /// </summary>
        private Guid _ID;
        /// <summary>
        /// Тип объекта, является константой для экземпляра к которому принадлежит.
        /// </summary>
        private eEntity _type;

        /// <summary>
        /// Получить уникальный идентификатор объекта.
        /// </summary>
        public string ID { get { return _ID.ToString(); } }
        /// <summary>
        /// Получить тип объекта.
        /// </summary>
        public eEntity type { get { return _type; } }

        /// <summary>
        /// Конструктор класса <see cref="identity"/>.
        /// </summary>
        /// <param name="type">Тип идентифицируемого объекта.</param>
        public identity(eEntity type)
        {
            _ID = Guid.NewGuid();
            _type = type;
        }
        /// <summary>
        /// Конструктор класса <see cref="identity"/>, используется для утилитарных нужд.
        /// </summary>
        /// <param name="type">Тип идентифицируемого объекта.</param>
        /// <param name="ID">Уникальный идентификатор объекта.</param>
        public identity(eEntity type, string ID)
        {
            _ID = new Guid(ID);
            _type = type;
        }
        /// <summary>
        /// Конструктор класса <see cref="identity"/>, используемый для полного копирования экземпляра класса <see cref="identity"/>.
        /// </summary>
        /// <param name="IDobject">Экземпляр копируемого класса <see cref="identity"/>.</param>
        public identity(identity IDobject)
            : this(IDobject.type, IDobject.ID)
        { }
        /// <summary>
        /// Получить уникальный идентификатор объекта.
        /// </summary>
        /// <param name="ID">Уникальный идентификатор объекта.</param>
        public void setID(string ID)
        {
            _ID = new Guid(ID);
        }
        /// <summary>
        /// Задать тип объекта.
        /// </summary>
        /// <param name="type">Тип объекта.</param>
        public void setType(eEntity type)
        {
            _type = type;
        }
        /// <summary>
        /// Скопировать все значения экземпляра <paramref name="IDobject"/>.
        /// </summary>
        /// <param name="IDobject">Экземпляр копируемого класса <see cref="identity"/>.</param>
        public void copy(identity IDobject)
        {
            _ID = new Guid(IDobject._ID.ToString());
            _type = IDobject.type;
        }
        /// <summary>
        /// Возвращает уникальный идентификатор объекта
        /// (для генерации используется класс Guid).
        /// </summary>
        /// <returns>Уникальный идентификатор объекта.</returns>
        public string getID() { return ID; }
        /// <summary>
        /// Возвращает тип объекта.
        /// </summary>
        /// <returns>Тип объекта.</returns>
        public eEntity getType() { return type; }
    }
    #endregion
    #region Static
    /// <summary>
    /// Статический класс утилитарных функций
    /// </summary>
    public static class __hlp
    {
        /// <summary>
        /// Стандартное значение для инициализации дат сборки.
        /// </summary>
        public static readonly DateTime initDate = new DateTime(1900, 1, 1);
        /// <summary>
        /// Получить тип зависимой точки предшественника, из значения связи типа <see cref="eTskLim"/>
        /// </summary>
        /// <param name="Type">Значение ограничения типа "предшественник-последователь"</param>
        /// <returns></returns>
        public static eDot getPrecursor(eTskLim Type)
        {
            eTskLimChunk TC = (eTskLimChunk)Type;
            return ((TC & eTskLimChunk.Finish_) == eTskLimChunk.Finish_) ? eDot.finish : eDot.start;
        }
        /// <summary>
        /// Получить тип зависимой точки последователя, из значения связи типа <see cref="eTskLim"/>
        /// </summary>
        /// <param name="Type">Значение ограничения типа "предшественник-последователь"</param>
        /// <returns></returns>
        public static eDot getFollower(eTskLim Type)
        {
            eTskLimChunk TC = (eTskLimChunk)Type;
            return ((TC & eTskLimChunk._Finish) == eTskLimChunk._Finish) ? eDot.finish : eDot.start;
        }
    }
    #endregion
    #region Limits
    public class dependence : alter.Function.classes.function, IDependence
    {
        #region Vars
        private eDot _dDot;

        private Func<eDot> getDDot;
        private Func<eDirection> getDir;
        private Func<DateTime> getDateLim;

        private Action unsuscribe;
        #endregion
        #region Props
        public override DateTime date
        {
            get { return getDateLim(); }
            set { base.date = value; }
        }
        public override eDirection direction
        {
            get { return getDir(); }
            set { base.direction = value; }
        }
        #endregion
        #region Events
        public event EventHandler<EA_valueChange<eDot>> event_dependDotChanged;
        #endregion
        #region Constructors
        public dependence(DateTime dateLimit, eDirection direction, eDot dependDot)
            : base(dateLimit, direction)
        {
            _dDot = dependDot;
            setDependDot(null);
            setDirection(null);
            setDate(null);
        }
        public dependence(Func<DateTime> fDateLimit, Func<eDirection> fDirection, Func<eDot> fDependDot)
            : base(fDateLimit(), fDirection())
        {
            setDependDot(fDependDot);
            setDirection(fDirection);
            setDate(fDateLimit);
        }
        ~dependence()
        {
            getDateLim = null;
            getDDot = null;
            getDir = null;
        }
        #endregion
        #region Params
        public eDot getDependDot()
        { return getDDot(); }
        public void setDependDot(eDot dot)
        {
            if (dot != _dDot)
            {
                eDot tmp = _dDot;
                _dDot = dot;

                EventHandler<EA_valueChange<eDot>> handler = event_dependDotChanged;
                if (handler != null) handler(this, new EA_valueChange<eDot>(tmp, _dDot));
            }
        }
        public void setDependDot(Func<eDot> fDot)
        {
            if (fDot == null) getDDot = () => _dDot;
            else getDDot = fDot;
        }
        public void setDirection(Func<eDirection> fDirection)
        {
            if (fDirection == null) getDir = () => _direction;
            else getDir = fDirection;
        }
        public override eDirection getDirection()
        { return getDir(); }
        public void setDate(Func<DateTime> fDate)
        {
            if (fDate == null) getDateLim = () => _date;
            else getDateLim = fDate;
        }
        public override DateTime getDate()
        { return getDateLim(); }
        #endregion
        #region handlers
        private void onDDotChange(EA_valueChange<eDot> args)
        {
            EventHandler<EA_valueChange<eDot>> handler = event_dependDotChanged;
            if (handler != null) handler(this, args);
        }
        #endregion
        #region outer handlers
        public void onMasterDateChange(object sender, EA_valueChange<DateTime> e)
        { onDateChange(e); }
        public void onDirectionChange(object sender, EA_valueChange<eDirection> e)
        { onDirectionChange(e); }
        public void onDependDotChange(object sender, EA_valueChange<eDot> e)
        {
            _dDot = e.newValue;
            onDDotChange(e);
        }
        #endregion
        public bool setMasterAutoupdate
        (
            Action<EventHandler<EA_valueChange<DateTime>>> event_MasterDateChanged,
            Action<EventHandler<EA_valueChange<eDirection>>> event_directionChanged,
            Action<EventHandler<EA_valueChange<eDot>>> event_dependDotChanged
        )
        {
            if (event_MasterDateChanged == null
               || event_directionChanged == null
               || event_dependDotChanged == null) return false;

            event_MasterDateChanged(onMasterDateChange);
            event_dependDotChanged(onDependDotChange);
            event_directionChanged(onDirectionChange);
            return true;

        }

        
    }

    #endregion
    #region iEvents
    /// <summary>
    /// Класс событий с предоставляемым функционалом по удаленной отписке делегатов.
    /// </summary>
    /// <typeparam name="T">Тип значений eventyArgs данного события.</typeparam>
    public class iEventclass<T>
        where T : EventArgs
    {
        #region vars
        private EventHandlerList hndList;
        #endregion
        #region Indexer
        /// <summary>
        /// Предоставляет ссылки на методы подписанные на событие.
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <returns></returns>
        public EventHandler<T> this[int index]
        {
            get { return (EventHandler<T>)(eventDelegate.GetInvocationList())[index]; }
        }
        /// <summary>
        /// Количество методов  подписанных на событие.
        /// </summary>
        public int count { get { return (eventDelegate == null) ? 0 : eventDelegate.GetInvocationList().Length; } }

        #endregion
        #region entities
        private readonly object eEmp = new object();
        private readonly object eAdd = new object();
        private readonly object eRem = new object();
        #endregion
        #region events
        public event EventHandler<EA_delegateInfo> event_delegateAdded
        {
            add
            { hndList.AddHandler(eAdd, value); }
            remove 
            { hndList.RemoveHandler(eAdd, value); }
        }
        public event EventHandler<EA_delegateInfo> event_delegateRemoved
        {
            add
            { hndList.AddHandler(eRem, value); }
            remove
            { hndList.RemoveHandler(eRem, value); }
        }
        public event EventHandler<EventArgs> event_noSubscribers
        {
            add
            { hndList.AddHandler(eEmp, value); }
            remove
            { hndList.RemoveHandler(eEmp, value); }
        }
        #endregion
        #region Handlers
        private void onDelegateAdd(EventHandler<T> method)
        {
            var handler = hndList[eAdd] as EventHandler<EA_delegateInfo>;
            if (handler != null) handler(this, new EA_delegateInfo(method));
        }
        private void onDelegateRemove(EventHandler<T> method)
        {
            var handler = hndList[eRem] as EventHandler<EA_delegateInfo>;
            if (handler != null) handler(this, new EA_delegateInfo(method));
        }

        private void onNoSubscribers()
        {
            EventHandler<EventArgs> handler = hndList[eEmp] as EventHandler<EventArgs>;
            if (handler != null) handler(this, new EventArgs());
        } 
        #endregion
        #region delegate
        private EventHandler<T> eventDelegate;
        #endregion
        #region property
        /// <summary>
        /// Свойство переменной события 
        /// </summary>
        public event EventHandler<T> iEvent
        {
            add
            {
                lock (this)
                {
                    eventDelegate -= value;
                    eventDelegate += value;
                    onDelegateAdd(value);
                }
            }
            remove
            {
                int oldLng = this.count;

                lock (this) { eventDelegate -= value; }

                if (this.count == 0) onNoSubscribers();
                if (oldLng > this.count) onDelegateRemove(value);
            }
        }
        #endregion
        #region constructor
        public iEventclass()
        {
            hndList = new EventHandlerList();
        }
        #endregion
        #region destructor
        /// <summary>
        /// Деструктор класса
        /// </summary>
        ~iEventclass()
        {
            clear();
            eventDelegate = null;
            
            hndList.Dispose();
            hndList = null;
        }
        public void eventClean()
        {
            Action<object> clr = (type) =>
            {
                if (hndList[type] == null) return;
                Delegate[] temp = hndList[type].GetInvocationList();
                if (temp == null || temp.Length == 0) return;

                if (type == eAdd)
                {
                    for (int i = 0; i < temp.Length; i++)
                        event_delegateAdded -= (EventHandler<EA_delegateInfo>)temp[i];
                }
                else if (type == eRem)
                {
                    for (int i = 0; i < temp.Length; i++)
                        event_delegateRemoved -= (EventHandler<EA_delegateInfo>)temp[i];
                }
                else if (type == eEmp)
                {
                    for (int i = 0; i < temp.Length; i++)
                        event_noSubscribers -= (EventHandler<EventArgs>)temp[i];
                }
            };

            clr(eEmp);
            clr(eRem);
            clr(eAdd);
        }
        

        #endregion
        #region subscribe methods
            /// <summary>
            /// Предоставить анонимный метод отписки от события, для указанного в параметрах метода.
            /// </summary>
            /// <param name="method">Делегат метода подписчика отписываемого от события.</param>
            /// <returns></returns>
        public Action unsubscribe(EventHandler<T> method)
        {
            return () => iEvent -= method;
        }
        /// <summary>
        /// Подписать метод delegateMethod на срабатывание события.
        /// </summary>
        /// <param name="delegateMethod">Делегат подписываемого метода.</param>
        public void add(EventHandler<T> delegateMethod)
        { iEvent += delegateMethod; }

        /// <summary>
        /// Отписать метод delegateMethod от данного события.
        /// </summary>
        /// <param name="delegateMethod">Делегат отписываемого метода.</param>
        public void remove(EventHandler<T> delegateMethod)
        { iEvent -= delegateMethod; }
        /// <summary>
        /// Отписывает подписанный метод c указанным индексом перечисления getInvocationList()
        /// </summary>
        /// <param name="index"></param>
        public void removeAt(int index)
        { iEvent -= this[index]; }
        #endregion
        #region parent methods
        /// <summary>
        /// Запустить событие.
        /// </summary>
        /// <param name="sender">Объект запустивший событие.</param>
        /// <param name="args">Аргументы события наследуемые от <seealso cref="EventArgs"/></param>
        /// <returns></returns>
        public bool invokeEvent(object sender, T args)
        {
            EventHandler<T> handler = eventDelegate;
            if (handler != null)
            {
                handler(sender, args);
                return true;
            }
            else return false;
        }
        /// <summary>
        /// Отписывает событие от всех его подписчиков.
        /// </summary>
        public void clear()
        {
            if (eventDelegate != null)
            {
                Delegate[] delegates = eventDelegate.GetInvocationList();
                for (int i = 0; i < delegates.Length; i++) eventDelegate -= (EventHandler<T>)delegates[i];
            }
        } 
        #endregion
    }

    #endregion
}
