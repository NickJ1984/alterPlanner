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
    public class Identity : IId
    {
        /// <summary>
        /// Уникальный идентификатор объекта, является константой для экземпляра к которому принадлежит.
        /// </summary>
        private Guid _id;
        /// <summary>
        /// Тип объекта, является константой для экземпляра к которому принадлежит.
        /// </summary>
        private e_Entity _type;

        /// <summary>
        /// Получить уникальный идентификатор объекта.
        /// </summary>
        public string Id { get { return _id.ToString(); } }
        /// <summary>
        /// Получить тип объекта.
        /// </summary>
        public e_Entity Type { get { return _type; } }

        /// <summary>
        /// Конструктор класса <see cref="Identity"/>.
        /// </summary>
        /// <param name="type">Тип идентифицируемого объекта.</param>
        public Identity(e_Entity type)
        {
            _id = Guid.NewGuid();
            _type = type;
        }
        /// <summary>
        /// Конструктор класса <see cref="Identity"/>, используется для утилитарных нужд.
        /// </summary>
        /// <param name="type">Тип идентифицируемого объекта.</param>
        /// <param name="id">Уникальный идентификатор объекта.</param>
        public Identity(e_Entity type, string id)
        {
            _id = new Guid(id);
            _type = type;
        }
        /// <summary>
        /// Конструктор класса <see cref="Identity"/>, используемый для полного копирования экземпляра класса <see cref="Identity"/>.
        /// </summary>
        /// <param name="objectID">Экземпляр копируемого класса <see cref="Identity"/>.</param>
        public Identity(Identity objectID)
            : this(objectID.Type, objectID.Id)
        { }
        /// <summary>
        /// Получить уникальный идентификатор объекта.
        /// </summary>
        /// <param name="id">Уникальный идентификатор объекта.</param>
        public void SetId(string id)
        {
            _id = new Guid(id);
        }
        /// <summary>
        /// Задать тип объекта.
        /// </summary>
        /// <param name="type">Тип объекта.</param>
        public void SetType(e_Entity type)
        {
            _type = type;
        }
        /// <summary>
        /// Скопировать все значения экземпляра <paramref name="objectID"/>.
        /// </summary>
        /// <param name="objectID">Экземпляр копируемого класса <see cref="Identity"/>.</param>
        public void Copy(Identity objectID)
        {
            _id = new Guid(objectID._id.ToString());
            _type = objectID.Type;
        }
        /// <summary>
        /// Возвращает уникальный идентификатор объекта
        /// (для генерации используется класс Guid).
        /// </summary>
        /// <returns>Уникальный идентификатор объекта.</returns>
        public string GetId() { return Id; }
        /// <summary>
        /// Возвращает тип объекта.
        /// </summary>
        /// <returns>Тип объекта.</returns>
        public e_Entity GetType() { return Type; }
    }
    #endregion
    #region Static
    /// <summary>
    /// Статический класс утилитарных функций
    /// </summary>
    public static class Hlp
    {
        /// <summary>
        /// Стандартное значение для инициализации дат сборки.
        /// </summary>
        public static readonly DateTime InitDate = new DateTime(1900, 1, 1);
        /// <summary>
        /// Получить тип зависимой точки предшественника, из значения связи типа <see cref="e_TskLim"/>
        /// </summary>
        /// <param name="type">Значение ограничения типа "предшественник-последователь"</param>
        /// <returns></returns>
        public static e_Dot GetPrecursor(e_TskLim type)
        {
            e_TskLimChunk tc = (e_TskLimChunk)type;
            return ((tc & e_TskLimChunk.Finish) == e_TskLimChunk.Finish) ? e_Dot.Finish : e_Dot.Start;
        }
        /// <summary>
        /// Получить тип зависимой точки последователя, из значения связи типа <see cref="e_TskLim"/>
        /// </summary>
        /// <param name="type">Значение ограничения типа "предшественник-последователь"</param>
        /// <returns></returns>
        public static e_Dot GetFollower(e_TskLim type)
        {
            e_TskLimChunk tc = (e_TskLimChunk)type;
            return ((tc & e_TskLimChunk._Finish) == e_TskLimChunk._Finish) ? e_Dot.Finish : e_Dot.Start;
        }
    }
    #endregion
    #region Limits
    /// <summary>
    /// Класс реализующий зависимость от календарной даты для некоей подчиненной точки.
    /// </summary>
    public class Dependence : alter.Function.classes.function, IDependence
    {
        //Тестировал, все ОК
        #region Vars
        /// <summary>
        /// Зависимая точка подчиненного объекта
        /// </summary>
        protected e_Dot DDot;

        /// <summary>
        /// Функция получения зависимой точки
        /// </summary>
        protected Func<e_Dot> GetDDot;
        /// <summary>
        /// Функция получения направления
        /// </summary>
        protected Func<e_Direction> GetDir;
        /// <summary>
        /// Функция получения даты зависимости
        /// </summary>
        protected Func<DateTime> GetDateLim;

        /// <summary>
        /// Делегат метода отписки от внешнего источника
        /// </summary>
        protected Action AutoupdateUnsuscribe;
        /// <summary>
        /// Свойство возвращает истину если экземпляр класса подписан на события управляющих параметров.
        /// </summary>
        public bool Subscribed { get; protected set; }
        #endregion
        #region Props
        /// <summary>
        /// Дата зависимости
        /// </summary>
        public override DateTime date
        {
            get { return GetDateLim(); }
            set
            {
                if (Subscribed) base.date = GetDateLim();
                else base.date = value;
            }
        }
        /// <summary>
        /// Направление зависимости
        /// </summary>
        public override e_Direction direction
        {
            get { return GetDir(); }
            set
            {
                if (Subscribed) base.direction = GetDir();
                else base.direction = value;
            }
        }
        #endregion
        #region Events
        /// <summary>
        /// Событие изменения зависимой точки подчиненного объекта.
        /// </summary>
        public event EventHandler<ea_ValueChange<e_Dot>> event_DependDotChanged;
        #endregion
        #region Constructors
        /// <summary>
        /// Конструктор экземпляра класса.
        /// </summary>
        /// <param name="dateLimit">Значение управляющей даты.</param>
        /// <param name="direction">Значение направления.</param>
        /// <param name="dependDot">Зависимая точка подчиненного объекта.</param>
        public Dependence(DateTime dateLimit, e_Direction direction, e_Dot dependDot)
            : base(dateLimit, direction)
        {
            DDot = dependDot;
            SetDependDot(null);
            SetDirection(null);
            SetDate(null);

            init_default();
        }
        /// <summary>
        /// Конструктор экземпляра класса.
        /// </summary>
        /// <param name="fDateLimit">Функция возвращающая значение управляющей даты.</param>
        /// <param name="fDirection">Функция возвращающая значение направления.</param>
        /// <param name="fDependDot">Функция возвращающая зависимую точки подчиненного объекта.</param>
        public Dependence(Func<DateTime> fDateLimit, Func<e_Direction> fDirection, Func<e_Dot> fDependDot)
            : this(fDateLimit(), fDirection(), fDependDot())
        {
            SetDependDot(fDependDot);
            SetDirection(fDirection);
            SetDate(fDateLimit);
        }
        /// <summary>
        /// Конструктор экземпляра класса.
        /// </summary>
        /// <param name="fDateLimit">Функция возвращающая значение управляющей даты.</param>
        /// <param name="fDirection">Функция возвращающая значение направления.</param>
        /// <param name="dependDot">Зависимая точка подчиненного объекта.</param>
        public Dependence(Func<DateTime> fDateLimit, Func<e_Direction> fDirection, e_Dot dependDot)
            : this(fDateLimit(), fDirection(), dependDot)
        {
            SetDirection(fDirection);
            SetDate(fDateLimit);
        }
        /// <summary>
        /// Метод инициализирующий стандартный набор переменных
        /// </summary>
        protected void init_default()
        {
            Subscribed = false;
            AutoupdateUnsuscribe = () => { };
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~Dependence()
        {
            AutoupdateUnsuscribe();
            GetDateLim = null;
            GetDDot = null;
            GetDir = null;
        }
        #endregion
        #region Params
        #region dependDot
        /// <summary>
        /// Метод получения точки зависимости подчиненного объекта
        /// </summary>
        /// <returns>Точка зависимости подчиненного объекта</returns>
        public e_Dot GetDependDot()
        { return GetDDot(); }
        /// <summary>
        /// Метод установки точки зависимости подчиненного объекта
        /// </summary>
        /// <param name="dot">Точка зависимости подчиненного объекта</param>
        public void SetDependDot(e_Dot dot)
        {
            if (dot != DDot)
            {
                e_Dot tmp = DDot;
                DDot = dot;

                EventHandler<ea_ValueChange<e_Dot>> handler = event_DependDotChanged;
                if (handler != null) handler(this, new ea_ValueChange<e_Dot>(tmp, DDot));
            }
        }
        /// <summary>
        /// Метод установки точки зависимости подчиненного объекта
        /// </summary>
        /// <param name="fDot">Функция получения точки зависимости подчиненного объекта</param>
        public void SetDependDot(Func<e_Dot> fDot)
        {
            if (fDot == null) GetDDot = () => DDot;
            else GetDDot = fDot;
        }
        #endregion
        #region direction
        /// <summary>
        /// Метод установки направления зависимости
        /// </summary>
        /// <param name="fDirection">Направление зависимости</param>
        public void SetDirection(Func<e_Direction> fDirection)
        {
            if (fDirection == null) GetDir = () => Direction;
            else GetDir = fDirection;
        }
        /// <summary>
        /// Метод получения направления зависимости
        /// </summary>
        /// /// <returns>Направление зависимости</returns>
        public override e_Direction GetDirection()
        { return GetDir(); }
        #endregion
        #region date
        /// <summary>
        /// Метод установки даты зависимости
        /// </summary>
        /// <param name="fDate">Функция получения даты зависимости</param>
        public void SetDate(Func<DateTime> fDate)
        {
            if (fDate == null) GetDateLim = () => Date;
            else GetDateLim = fDate;
        }
        /// <summary>
        /// Метод получения даты зависимости
        /// </summary>
        /// <returns></returns>
        public override DateTime GetDate()
        { return GetDateLim(); }
        #endregion
        #endregion
        #region handlers
        #region inner handlers
        /// <summary>
        /// Метод вызова события <see cref="event_DependDotChanged"/>
        /// </summary>
        /// <param name="args"></param>
        protected void OnDDotChange(ea_ValueChange<e_Dot> args)
        {
            var handler = event_DependDotChanged;
            handler?.Invoke(this, args);
        }
        #endregion
        #region outer handlers
        /// <summary>
        /// Обработчик события изменения управляющей даты
        /// </summary>
        /// <param name="sender">Объект источник события</param>
        /// <param name="e">Аргументы события</param>
        protected void OnDateChange(object sender, ea_ValueChange<DateTime> e)
        { date = e.NewValue; }
        /// <summary>
        /// Обработчик события изменения направления управляющей функции
        /// </summary>
        /// <param name="sender">Объект источник события</param>
        /// <param name="e">Аргументы события</param>
        protected void OnDirectionChange(object sender, ea_ValueChange<e_Direction> e)
        { direction = e.NewValue; }
        /// <summary>
        /// Обработчик события изменения зависимой точки подчиненного объекта
        /// </summary>
        /// <param name="sender">Объект источник события</param>
        /// <param name="e">Аргументы события</param>
        protected void OnDependDotChange(object sender, ea_ValueChange<e_Dot> e)
        {
            DDot = e.NewValue;
            OnDDotChange(e);
        }
        #endregion 
        #endregion
        #region Methods
        #region autoUpdate
        /// <summary>
        /// Метод подписки экземпляра класса на изменение управляющих параметров.
        /// </summary>
        /// <param name="event_DateChanged">Анонимный метод подписки на изменение управляющей даты, принимающий на вход делегат метода обработчика события изменения управляющей даты, возвращает метод отписки обработчика.</param>
        /// <param name="event_DirectionChanged">Анонимный метод подписки на изменение направления управляющей функции, принимающий на вход делегат метода обработчика события изменения направления, возвращает метод отписки обработчика.</param>
        /// <param name="event_DependDotChanged">Анонимный метод подписки на изменение зависимой точки подчиненного объекта, принимающий на вход делегат метода обработчика события изменения подчиненной точки, возвращает метод отписки обработчика.</param>
        /// <returns></returns>
        public bool setAutoupdate
            (
            Func<EventHandler<ea_ValueChange<DateTime>>, Action> event_DateChanged,
            Func<EventHandler<ea_ValueChange<e_Direction>>, Action> event_DirectionChanged,
            Func<EventHandler<ea_ValueChange<e_Dot>>, Action> event_DependDotChanged
            )
        {
            if (Subscribed) AutoupdateUnsuscribe();

            if (event_DateChanged == null
                || event_DirectionChanged == null
                || event_DependDotChanged == null) return false;

            Action auDate = event_DateChanged(OnDateChange);
            Action auDot = event_DependDotChanged(OnDependDotChange);
            Action auDir = event_DirectionChanged(OnDirectionChange);

            AutoupdateUnsuscribe = () =>
            {
                auDate();
                auDot();
                auDir();
                Subscribed = false;
                AutoupdateUnsuscribe = () => { };
            };

            Subscribed = true;
            return true;
        }

        /// <summary>
        /// Метод подписки экземпляра класса на изменение управляющих параметров.
        /// </summary>
        /// <param name="event_DateChanged">Анонимный метод подписки на изменение управляющей даты, принимающий на вход делегат метода обработчика события изменения управляющей даты, возвращает метод отписки обработчика.</param>
        /// <param name="event_DirectionChanged">Анонимный метод подписки на изменение направления управляющей функции, принимающий на вход делегат метода обработчика события изменения направления, возвращает метод отписки обработчика.</param>
        /// <returns></returns>
        public bool setAutoupdate
            (
            Func<EventHandler<ea_ValueChange<DateTime>>, Action> event_DateChanged,
            Func<EventHandler<ea_ValueChange<e_Direction>>, Action> event_DirectionChanged
            )
        {
            if (Subscribed) AutoupdateUnsuscribe();

            if (event_DateChanged == null
                || event_DirectionChanged == null) return false;

            Action auDate = event_DateChanged(OnDateChange);
            Action auDir = event_DirectionChanged(OnDirectionChange);

            AutoupdateUnsuscribe = () =>
            {
                auDate();
                auDir();
                Subscribed = false;
                AutoupdateUnsuscribe = () => { };
            };

            Subscribed = true;
            return true;
        }

        public void unsetAutoupdate()
        { AutoupdateUnsuscribe(); }
        #endregion
        #endregion
    }
    #endregion
    #region iEvents
    /// <summary>
    /// Класс событий с предоставляемым функционалом по удаленной отписке делегатов.
    /// </summary>
    /// <typeparam name="T">Тип значений eventyArgs данного события.</typeparam>
    public class IEvent<T>
        where T : EventArgs
    {
        #region Indexer
        /// <summary>
        /// Массив делегатов подписчиков события
        /// </summary>
        protected EventHandler<T>[] InvokationList;
        /// <summary>
        /// Предоставляет ссылки на методы подписанные на событие.
        /// </summary>
        /// <param name="index">Индекс элемента</param>
        /// <returns></returns>
        public EventHandler<T> this[int index] => InvokationList[index];
        /// <summary>
        /// Количество элементов массива <seealso cref="InvokationList"/>
        /// </summary>
        public int Count => InvokationList.Length;
        #endregion
        #region delegate
        /// <summary>
        /// Основной делегат класса
        /// </summary>
        protected EventHandler<T> EventDelegate;
        #endregion
        #region property
        /// <summary>
        /// Количество делегатов методов подписанных на <seealso cref="EventDelegate"/>
        /// </summary>
        protected int IlLength { get { return EventDelegate.GetInvocationList().Length; } }
        /// <summary>
        /// Свойство переменной события 
        /// </summary>
        public virtual event EventHandler<T> iEventHandler
        {
            add
            {
                lock (this)
                {
                    EventDelegate -= value;
                    EventDelegate += value;
                }
                UpdateInvokationList();
            }
            remove
            {
                lock (this) { EventDelegate -= value; }
                UpdateInvokationList();
            }
        }
        #endregion
        #region Constructor
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public IEvent()
        {
            InvokationList = new EventHandler<T>[0];
        }
        #endregion
        #region destructor
        /// <summary>
        /// Деструктор класса
        /// </summary>
        ~IEvent()
        {
            EventClean();

            EventDelegate = null;

            InvokationList = null;
        }
        /// <summary>
        /// Отписка всех событий данного экземпляра классов от их подписчиков.
        /// </summary>
        protected virtual void EventClean()
        {
            ClearEvent(ref EventDelegate);
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
        public Action AddDelegateSubscribers(EventHandler<T> rDelegate)
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
        public void Remove(EventHandler<T> delegateMethod)
        { iEventHandler -= delegateMethod; }
        /// <summary>
        /// Отписывает подписанный метод c указанным индексом перечисления getInvocationList()
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        { iEventHandler -= this[index]; }
        #endregion
        #region parent methods
        /// <summary>
        /// Запустить событие.
        /// </summary>
        /// <param name="sender">Объект запустивший событие.</param>
        /// <param name="args">Аргументы события наследуемые от <seealso cref="EventArgs"/></param>
        /// <returns></returns>
        public bool InvokeEvent(object sender, T args)
        {
            EventHandler<T> handler = EventDelegate;
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
        public void Clear()
        { EventClean(); }
        #endregion
        #region invokation members methods
        /// <summary>
        /// Предоставить анонимный метод отписки от события, для указанного в параметрах метода.
        /// </summary>
        /// <param name="method">Делегат метода подписчика отписываемого от события.</param>
        /// <returns></returns>
        public Action GetUnsubscribeMethod(EventHandler<T> method)
        { return () => iEventHandler -= method; }
        /// <summary>
        /// Возвращает индекс элемента массива подписчиков на событие. (Не тестировал)
        /// </summary>
        /// <param name="invokeMember"></param>
        /// <returns></returns>
        public int IndexOf(EventHandler<T> invokeMember)
        {
            return InvokationList.Where((v, i) => v == invokeMember).Select((v, i) => i).First();
        }
        #endregion
        #region service
        /// <summary>
        /// Метод обновления массива делегатов подписчиков <seealso cref="InvokationList"/>
        /// </summary>
        protected virtual void UpdateInvokationList()
        {
            if(IlLength != Count)
                InvokationList = 
                    EventDelegate.GetInvocationList()
                    .Select(v => (EventHandler<T>)v).ToArray();
        }
        /// <summary>
        /// Отписка события ото всех подписчиков.
        /// </summary>
        /// <typeparam name="TArg">Тип EventArgs</typeparam>
        /// <param name="handler">Делегат</param>
        protected void ClearEvent<TArg>(ref EventHandler<TArg> handler) where TArg : EventArgs
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
    public class IEventObservable<T> : IEvent<T> where T : EventArgs
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
                    EventDelegate -= value;
                    EventDelegate += value;
                }
                if (Count != IlLength)
                {
                    UpdateInvokationList();
                    OnDelegateAdd(value);
                }
            }
            remove
            {
                if (value == null) throw new NullReferenceException();

                lock (this) { EventDelegate -= value; }

                int current = IlLength;
                if (Count != current)
                {
                    UpdateInvokationList();

                    OnDelegateRemove(value);
                    if(current == 0) OnNoSubscribers();
                }
            }
        }
        #endregion
        #region events
        /// <summary>
        /// Событие при добавлении нового делегата в подписчики.
        /// </summary>
        public event EventHandler<ea_DelegateInfo> EventDelegateAdded;
        /// <summary>
        /// Событие при отписке делегата из подписчиков.
        /// </summary>
        public event EventHandler<ea_DelegateInfo> EventDelegateRemoved;
        /// <summary>
        /// Событие при сокращении количества подписчиков до 0.
        /// </summary>
        public event EventHandler<EventArgs> EventNoSubscribers;
        #endregion
        #region Constructor
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public IEventObservable() : base()
        { }
        #endregion
        #region destructor
        /// <summary>
        /// Деструктор класса
        /// </summary>
        ~IEventObservable()
        {
            EventClean();
            EventDelegate = null;
            EventDelegateAdded = null;
            EventDelegateRemoved = null;
            EventNoSubscribers = null;

            InvokationList = null;
        }
        /// <summary>
        /// Отписка всех событий данного экземпляра классов от их подписчиков.
        /// </summary>
        protected override void EventClean()
        {
            base.EventClean();
            ClearEvent(ref EventDelegateAdded);
            ClearEvent(ref EventDelegateRemoved);
            ClearEvent(ref EventNoSubscribers);
        }
        #endregion
        #region Handlers
        /// <summary>
        /// Метод запуска события event_delegateAdded
        /// </summary>
        /// <param name="method">Ссылка на метод обрабатываемый основным событием</param>
        protected void OnDelegateAdd(EventHandler<T> method)
        {
            EventHandler<ea_DelegateInfo> handler = EventDelegateAdded;
            if (handler != null) handler(this, new ea_DelegateInfo(method));
        }
        /// <summary>
        /// Метод запуска события event_delegateRemoved
        /// </summary>
        /// <param name="method">Ссылка на метод обрабатываемый основным событием</param>
        protected void OnDelegateRemove(EventHandler<T> method)
        {
            EventHandler<ea_DelegateInfo> handler = EventDelegateRemoved;
            if (handler != null) handler(this, new ea_DelegateInfo(method));
        }
        /// <summary>
        /// Метод запуска события event_noSubscribers
        /// </summary>
        protected void OnNoSubscribers()
        {
            EventHandler<EventArgs> handler = EventNoSubscribers;
            if (handler != null) handler(this, new EventArgs());
        }
        #endregion
    }
    #endregion
}
