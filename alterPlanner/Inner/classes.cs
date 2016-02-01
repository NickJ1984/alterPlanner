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
        #region delegate
        /// <summary>
        /// Основной делегат класса
        /// </summary>
        protected EventHandler<T> eventDelegate;
        #endregion
        #region property
        /// <summary>
        /// Свойство переменной события 
        /// </summary>
        public virtual event EventHandler<T> iEvent
        {
            add
            {
                lock (this)
                {
                    eventDelegate -= value;
                    eventDelegate += value;
                }
            }
            remove
            { lock (this) { eventDelegate -= value; } }
        }
        #endregion
        #region destructor
        /// <summary>
        /// Деструктор класса
        /// </summary>
        ~iEventclass()
        {
            eventClean();
            eventDelegate = null;
        }
        /// <summary>
        /// Отписка всех событий данного экземпляра классов от их подписчиков.
        /// </summary>
        protected virtual void eventClean()
        {
            clearEvent(ref eventDelegate);
        }
        #endregion
        #region subscribe methods
            /// <summary>
            /// Предоставить анонимный метод отписки от события, для указанного в параметрах метода.
            /// </summary>
            /// <param name="method">Делегат метода подписчика отписываемого от события.</param>
            /// <returns></returns>
        public Action unsubscribe(EventHandler<T> method)
        { return () => iEvent -= method; }
        /// <summary>
        /// Подписать метод на событие.
        /// </summary>
        /// <param name="delegateMethod">Делегат подписываемого метода.</param>
        public void add(EventHandler<T> delegateMethod)
        { iEvent += delegateMethod; }
        /// <summary>
        /// Подписать несколько методов на событие.
        /// </summary>
        /// <param name="delegateMethod">Делегаты подписываемых методов.</param>
        public void add(params EventHandler<T>[] delegateMethod)
        {
            if (delegateMethod == null || delegateMethod.Length == 0) return;
            for (int i = 0; i < delegateMethod.Length; i++)
                add(delegateMethod[i]);
        }
        /// <summary>
        /// Подписывает на событие всех подписчиков передаваемого параметром делегата, возвращает анонимный метод отписки передаваемых делегатом подписчиков.
        /// </summary>
        /// <param name="rDelegate">Донор подписчиков, он же  обычный делегат</param>
        /// <returns>Анонимный метод отписки подписанных на событие методов.</returns>
        public Action addDelegateSubscribers(Delegate rDelegate)
        {
            if (rDelegate == null) return new Action(() => Console.WriteLine("Netu nihera..."));
            EventHandler<T>[] dlgs = rDelegate.GetInvocationList().Select(v => (EventHandler<T>)v).ToArray();
            for (int i = 0; i < dlgs.Length; i++) iEvent += dlgs[i];
            return new Action(() =>
            {
                for (int i = 0; i < dlgs.Length; i++) iEvent -= dlgs[i];
            });
        }
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
        { eventClean(); }
        #endregion
        #region service
        /// <summary>
        /// Отписка события ото всех подписчиков.
        /// </summary>
        /// <typeparam name="TArg">Тип EventArgs</typeparam>
        /// <param name="handler">Делегат</param>
        protected void clearEvent<TArg>(ref EventHandler<TArg> handler) where TArg : EventArgs
        {
            if (handler == null) return;
            EventHandler<TArg>[] dlgs = handler.GetInvocationList().Select(v => (EventHandler<TArg>)v).ToArray();
            if (dlgs == null || dlgs.Length == 0) return;
            for (int i = 0; i < dlgs.Length; i++) handler -= dlgs[i];
        }
        #endregion
    }
    /// <summary>
    /// Класс событий с предоставляемым функционалом по удаленной отписке делегатов, и возможностью отслеживания манипуляций с подписчиками.
    /// </summary>
    /// <typeparam name="T">Тип значений eventyArgs данного события.</typeparam>
    public class iEventclassObservable<T> : iEventclass<T> where T : EventArgs
    {
        #region property
        /// <summary>
        /// Свойство переменной события 
        /// </summary>
        public override event EventHandler<T> iEvent
        {
            add
            {
                lock (this)
                {
                    eventDelegate -= value;
                    eventDelegate += value;
                }
                onDelegateAdd(value);
            }
            remove
            {
                int old = this.count;

                lock (this) { eventDelegate -= value; }

                if (this.count == 0 && old == 1) onNoSubscribers();
                if (old > this.count) onDelegateRemove(value);
            }
        }
        #endregion
        #region events
        /// <summary>
        /// Событие при добавлении нового делегата в подписчики.
        /// </summary>
        public event EventHandler<EA_delegateInfo> event_delegateAdded;
        /// <summary>
        /// Событие при отписке делегата из подписчиков.
        /// </summary>
        public event EventHandler<EA_delegateInfo> event_delegateRemoved;
        /// <summary>
        /// Событие при сокращении количества подписчиков до 0.
        /// </summary>
        public event EventHandler<EventArgs> event_noSubscribers;
        #endregion
        #region destructor
        /// <summary>
        /// Деструктор класса
        /// </summary>
        ~iEventclassObservable()
        {
            eventClean();
            eventDelegate = null;
            event_delegateAdded = null;
            event_delegateRemoved = null;
            event_noSubscribers = null;
        }
        /// <summary>
        /// Отписка всех событий данного экземпляра классов от их подписчиков.
        /// </summary>
        protected override void eventClean()
        {
            base.eventClean();
            clearEvent(ref event_delegateAdded);
            clearEvent(ref event_delegateRemoved);
            clearEvent(ref event_noSubscribers);
        }
        #endregion
        #region Handlers
        /// <summary>
        /// Метод запуска события event_delegateAdded
        /// </summary>
        /// <param name="method">Ссылка на метод обрабатываемый основным событием</param>
        protected void onDelegateAdd(EventHandler<T> method)
        {
            EventHandler<EA_delegateInfo> handler = event_delegateAdded;
            if (handler != null) handler(this, new EA_delegateInfo(method));
        }
        /// <summary>
        /// Метод запуска события event_delegateRemoved
        /// </summary>
        /// <param name="method">Ссылка на метод обрабатываемый основным событием</param>
        protected void onDelegateRemove(EventHandler<T> method)
        {
            EventHandler<EA_delegateInfo> handler = event_delegateRemoved;
            if (handler != null) handler(this, new EA_delegateInfo(method));
        }
        /// <summary>
        /// Метод запуска события event_noSubscribers
        /// </summary>
        protected void onNoSubscribers()
        {
            EventHandler<EventArgs> handler = event_noSubscribers;
            if (handler != null) handler(this, new EventArgs());
        }
        #endregion
    }
    #endregion
}
