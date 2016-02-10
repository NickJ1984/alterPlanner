using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections;
using System.Runtime.CompilerServices;
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
            return ((tc & e_TskLimChunk.Finish_) == e_TskLimChunk.Finish_) ? e_Dot.Finish : e_Dot.Start;
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
        /// <summary>
        /// Получить тип зависимой точки, из значения связи типа <see cref="e_TskLim"/>
        /// </summary>
        /// <param name="type">Значение ограничения типа "предшественник-последователь"</param>
        /// <param name="dependType">Тип зависимости</param>
        /// <returns></returns>
        public static e_Dot GetDepenDot(e_TskLim type, e_DependType dependType)
        {
            e_TskLimChunk tc = (e_TskLimChunk)type;

            return (dependType == e_DependType.Master)
                ? ((tc.HasFlag(e_TskLimChunk.Finish_))
                    ? e_Dot.Finish
                    : e_Dot.Start)
                : ((tc.HasFlag(e_TskLimChunk._Finish))
                    ? e_Dot.Finish
                    : e_Dot.Start);
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
        #endregion
        #region Props
        /// <summary>
        /// Свойство установки точки зависимости подчиненного объекта
        /// </summary>
        public e_Dot dependDot
        {
            get { return DDot; }
            set
            {
                if (DDot != value && Enum.IsDefined(typeof(e_Dot), value))
                {
                    e_Dot tmp = DDot;
                    DDot = value;

                    OnDDotChange(new ea_ValueChange<e_Dot>(tmp, DDot));
                }
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

            init_default();
        }
        /// <summary>
        /// Метод инициализирующий стандартный набор переменных
        /// </summary>
        protected void init_default()
        {
            sender = this;
        }
        #endregion
        #region Params
        /// <summary>
        /// Метод получения точки зависимости подчиненного объекта
        /// </summary>
        /// <returns>Точка зависимости подчиненного объекта</returns>
        public e_Dot GetDependDot()
        { return dependDot; }
        /// <summary>
        /// Метод установки точки зависимости подчиненного объекта
        /// </summary>
        /// <param name="dot">Точка зависимости подчиненного объекта</param>
        public void SetDependDot(e_Dot dot)
        {
            dependDot = dot;
        }
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
            handler?.Invoke(sender, args);
        }
        #endregion
        #region outer handlers
        /// <summary>
        /// Обработчик события изменения управляющей даты
        /// </summary>
        /// <param name="sender">Объект источник события</param>
        /// <param name="e">Аргументы события</param>
        public void handler_DateChange(object sender, ea_ValueChange<DateTime> e)
        { date = e.NewValue; }
        /// <summary>
        /// Обработчик события изменения направления управляющей функции
        /// </summary>
        /// <param name="sender">Объект источник события</param>
        /// <param name="e">Аргументы события</param>
        public void handler_DirectionChange(object sender, ea_ValueChange<e_Direction> e)
        { direction = e.NewValue; }
        /// <summary>
        /// Обработчик события изменения зависимой точки подчиненного объекта
        /// </summary>
        /// <param name="sender">Объект источник события</param>
        /// <param name="e">Аргументы события</param>
        public void handler_DependDotChange(object sender, ea_ValueChange<e_Dot> e)
        {
            DDot = e.NewValue;
            OnDDotChange(e);
        }
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
        #region Vars
        /// <summary>
        /// Ссылк на объект в аргументе события
        /// </summary>
        protected object sender;
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
            sender = this;
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
        /// <param name="args">Аргументы события наследуемые от <seealso cref="EventArgs"/></param>
        /// <returns></returns>
        public bool InvokeEvent(T args)
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
        #region identification
        /// <summary>
        /// Определяет аргумент события sender
        /// </summary>
        /// <param name="sender">Ссылка на объект запустивший событиек</param>
        /// <exception cref="NullReferenceException"></exception>
        public void setSender(object sender)
        {
            if (sender == null) throw new NullReferenceException();
            this.sender = sender;
        }
        #endregion
    }
    /// <summary>
    /// Класс событий с предоставляемым функционалом по удаленной отписке делегатов, и возможностью отслеживания манипуляций с подписчиками.
    /// </summary>
    /// <typeparam name="T">Тип значений eventyArgs данного события.</typeparam>
    public class IEventObservable<T> : IEvent<T> where T : EventArgs
    {
        
        #endregion
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
        {
            sender = this;
        }
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
