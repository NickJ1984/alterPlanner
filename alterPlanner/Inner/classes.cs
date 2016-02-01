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

        private Action autoupdateUnsuscribe;
        /// <summary>
        /// Свойство возвращает истину если экземпляр класса подписан на события управляющих параметров.
        /// </summary>
        public bool subscribed { get; protected set; }
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
        /// <summary>
        /// Событие изменения зависимой точки подчиненного объекта.
        /// </summary>
        public event EventHandler<EA_valueChange<eDot>> event_dependDotChanged;
        #endregion
        #region Constructors
        /// <summary>
        /// Конструктор экземпляра класса.
        /// </summary>
        /// <param name="dateLimit">Значение управляющей даты.</param>
        /// <param name="direction">Значение направления.</param>
        /// <param name="dependDot">Зависимая точка подчиненного объекта.</param>
        public dependence(DateTime dateLimit, eDirection direction, eDot dependDot)
            : base(dateLimit, direction)
        {
            _dDot = dependDot;
            setDependDot(null);
            setDirection(null);
            setDate(null);

            init_default();
        }
        /// <summary>
        /// Конструктор экземпляра класса.
        /// </summary>
        /// <param name="fDateLimit">Функция возвращающая значение управляющей даты.</param>
        /// <param name="fDirection">Функция возвращающая значение направления.</param>
        /// <param name="fDependDot">Функция возвращающая зависимую точки подчиненного объекта.</param>
        public dependence(Func<DateTime> fDateLimit, Func<eDirection> fDirection, Func<eDot> fDependDot)
            : base(fDateLimit(), fDirection())
        {
            setDependDot(fDependDot);
            setDirection(fDirection);
            setDate(fDateLimit);

            init_default();
        }
        /// <summary>
        /// Метод инициализирующий стандартный набор переменных
        /// </summary>
        protected void init_default()
        {
            subscribed = false;
            autoupdateUnsuscribe = () => { };
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
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
        /// <summary>
        /// Обработчик события изменения управляющей даты
        /// </summary>
        /// <param name="sender">Объект источник события</param>
        /// <param name="e">Аргументы события</param>
        protected void onDateChange(object sender, EA_valueChange<DateTime> e)
        { onDateChange(e); }
        /// <summary>
        /// Обработчик события изменения направления управляющей функции
        /// </summary>
        /// <param name="sender">Объект источник события</param>
        /// <param name="e">Аргументы события</param>
        protected void onDirectionChange(object sender, EA_valueChange<eDirection> e)
        { onDirectionChange(e); }
        /// <summary>
        /// Обработчик события изменения зависимой точки подчиненного объекта
        /// </summary>
        /// <param name="sender">Объект источник события</param>
        /// <param name="e">Аргументы события</param>
        protected void onDependDotChange(object sender, EA_valueChange<eDot> e)
        {
            _dDot = e.newValue;
            onDDotChange(e);
        }
        #endregion
        #region service
        /// <summary>
        /// Метод подписки экземпляра класса на изменение управляющих параметров.
        /// </summary>
        /// <param name="event_DateChanged">Анонимный метод подписки на изменение управляющей даты, принимающий на вход делегат метода обработчика события изменения управляющей даты, возвращает метод отписки обработчика.</param>
        /// <param name="event_directionChanged">Анонимный метод подписки на изменение направления управляющей функции, принимающий на вход делегат метода обработчика события изменения направления, возвращает метод отписки обработчика.</param>
        /// <param name="event_dependDotChanged">Анонимный метод подписки на изменение зависимой точки подчиненного объекта, принимающий на вход делегат метода обработчика события изменения подчиненной точки, возвращает метод отписки обработчика.</param>
        /// <returns></returns>
        public bool setAutoupdate
        (
            Func<EventHandler<EA_valueChange<DateTime>>, Action> event_DateChanged,
            Func<EventHandler<EA_valueChange<eDirection>>, Action> event_directionChanged,
            Func<EventHandler<EA_valueChange<eDot>>, Action> event_dependDotChanged
        )
        {
            if (event_DateChanged == null
               || event_directionChanged == null
               || event_dependDotChanged == null) return false;

            Action auDate = event_DateChanged(onDateChange);
            Action auDot = event_dependDotChanged(onDependDotChange);
            Action auDir = event_directionChanged(onDirectionChange);

            autoupdateUnsuscribe = () =>
                { auDate(); auDot(); auDir(); subscribed = false; };
            subscribed = true;
            return true;
        }
    }
    #endregion
    #endregion
    #region iEvents
    /// <summary>
    /// Класс событий с предоставляемым функционалом по удаленной отписке делегатов.
    /// </summary>
    /// <typeparam name="T">Тип значений eventyArgs данного события.</typeparam>
    public class iEvent<T>
        where T : EventArgs
    {
        #region Indexer
        /// <summary>
        /// Массив делегатов подписчиков события
        /// </summary>
        protected EventHandler<T>[] invokationList;
        /// Предоставляет ссылки на методы подписанные на событие.
        /// </summary>
        /// <param name="index">Индекс элемента</param>
        /// <returns></returns>
        public EventHandler<T> this[int index]
        {
            get { return invokationList[index]; }
        }
        /// <summary>
        /// Количество элементов массива <seealso cref="invokationList"/>
        /// </summary>
        public int count { get { return invokationList.Length; } }
        #endregion
        #region delegate
        /// <summary>
        /// Основной делегат класса
        /// </summary>
        protected EventHandler<T> eventDelegate;
        #endregion
        #region property
        /// <summary>
        /// Количество делегатов методов подписанных на <seealso cref="eventDelegate"/>
        /// </summary>
        protected int ilLength { get { return eventDelegate.GetInvocationList().Length; } }
        /// <summary>
        /// Свойство переменной события 
        /// </summary>
        public virtual event EventHandler<T> iEventHandler
        {
            add
            {
                lock (this)
                {
                    eventDelegate -= value;
                    eventDelegate += value;
                }
                updateInvokationList();
            }
            remove
            {
                lock (this) { eventDelegate -= value; }
                updateInvokationList();
            }
        }
        #endregion
        #region Constructor
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public iEvent()
        {
            invokationList = new EventHandler<T>[0];
        }
        #endregion
        #region destructor
        /// <summary>
        /// Деструктор класса
        /// </summary>
        ~iEvent()
        {
            eventClean();

            eventDelegate = null;

            invokationList = null;
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
        /// Подписать метод на событие.
        /// </summary>
        /// <param name="delegateMethod">Делегат подписываемого метода.</param>
        public void add(EventHandler<T> delegateMethod)
        { iEventHandler += delegateMethod; }
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
        /// <param name="rDelegate">Донор подписчиков</param>
        /// <returns>Анонимный метод отписки подписанных на событие методов.</returns>
        public Action addDelegateSubscribers(EventHandler<T> rDelegate)
        {
            if (rDelegate == null) throw new NullReferenceException();
            EventHandler<T>[] dlgs = rDelegate.GetInvocationList().Select(v => (EventHandler<T>)v).ToArray();
            if (dlgs == null || dlgs.Length == 0) throw new NullReferenceException();
            for (int i = 0; i < dlgs.Length; i++) iEventHandler += dlgs[i];
            return new Action(() =>
            {
                for (int i = 0; i < dlgs.Length; i++) iEventHandler -= dlgs[i];
            });
        }
        /// <summary>
        /// Отписать метод delegateMethod от данного события.
        /// </summary>
        /// <param name="delegateMethod">Делегат отписываемого метода.</param>
        public void remove(EventHandler<T> delegateMethod)
        { iEventHandler -= delegateMethod; }
        /// <summary>
        /// Отписывает подписанный метод c указанным индексом перечисления getInvocationList()
        /// </summary>
        /// <param name="index"></param>
        public void removeAt(int index)
        { iEventHandler -= this[index]; }
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
        #region invokation members methods
        /// <summary>
        /// Предоставить анонимный метод отписки от события, для указанного в параметрах метода.
        /// </summary>
        /// <param name="method">Делегат метода подписчика отписываемого от события.</param>
        /// <returns></returns>
        public Action getUnsubscribeMethod(EventHandler<T> method)
        { return () => iEventHandler -= method; }
        /// <summary>
        /// Возвращает индекс элемента массива подписчиков на событие. (Не тестировал)
        /// </summary>
        /// <param name="invokeMember"></param>
        /// <returns></returns>
        public int indexOf(EventHandler<T> invokeMember)
        {
            int result = -1;
            result = invokationList.Where((v, i) => v == invokeMember).Select((v, i) => i).First();
            return result;
        }
        #endregion
        #region service
        /// <summary>
        /// Метод обновления массива делегатов подписчиков <seealso cref="invokationList"/>
        /// </summary>
        protected virtual void updateInvokationList()
        {
            if(ilLength != count)
                invokationList = 
                    eventDelegate.GetInvocationList()
                    .Select(v => (EventHandler<T>)v).ToArray();
        }
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
    public class iEventObservable<T> : iEvent<T> where T : EventArgs
    {
        #region property
        /// <summary>
        /// Свойство переменной события 
        /// </summary>
        public override event EventHandler<T> iEventHandler
        {
            add
            {
                if (value == null) throw new NullReferenceException();

                lock (this)
                {
                    eventDelegate -= value;
                    eventDelegate += value;
                }
                if (count != ilLength)
                {
                    updateInvokationList();
                    onDelegateAdd(value);
                }
            }
            remove
            {
                if (value == null) throw new NullReferenceException();

                lock (this) { eventDelegate -= value; }

                int current = ilLength;
                if (count != current)
                {
                    updateInvokationList();

                    onDelegateRemove(value);
                    if(current == 0) onNoSubscribers();
                }
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
        #region Constructor
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public iEventObservable() : base()
        { }
        #endregion
        #region destructor
        /// <summary>
        /// Деструктор класса
        /// </summary>
        ~iEventObservable()
        {
            eventClean();
            eventDelegate = null;
            event_delegateAdded = null;
            event_delegateRemoved = null;
            event_noSubscribers = null;

            invokationList = null;
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
