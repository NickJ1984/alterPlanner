using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.classes;
using alter.iface;
using alter.Link.iface;
using alter.types;

namespace alter.Link.classes
{
    /// <summary>
    /// Класс связи
    /// </summary>
    public class link : ILink
    {
        #region Vars
        /// <summary>
        /// Идентификатор данного экземпляра связи
        /// </summary>
        protected Identity _identity;
        #region Classes
        /// <summary>
        /// Информация о подчиненном члене связи
        /// </summary>
        protected linkMember _slave;
        /// <summary>
        /// Информация об управляющем члене связи
        /// </summary>
        protected linkMember _master;
        /// <summary>
        /// Зависимость для подчиненного члена связи
        /// </summary>
        protected Dependence _dependSlave;
        #endregion
        #region Variables
        /// <summary>
        /// Переменная значения задержки связи
        /// </summary>
        protected double _delay;
        /// <summary>
        /// Переменная вида связи
        /// </summary>
        protected e_TskLim _limit;
        /// <summary>
        /// Переменная состояния связи
        /// </summary>
        protected e_LnkState _state; 
        #endregion
        #region Delegates
        /// <summary>
        /// Делегат отписки от основного члена связи
        /// </summary>
        protected Action aUnsuscribeMaster = () => { };
        /// <summary>
        /// Делегат отписки от подчиненного члена связи
        /// </summary>
        protected Action aUnsuscribeSlave = () => { };
        /// <summary>
        /// Делегат отписки от зависимости для подчиненного члена связи
        /// </summary>
        protected Action aUnsuscribeDependSlave = () => { };
        #endregion
        #endregion
        #region Props
        #region Limit
        /// <summary>
        /// Свойство вида связи
        /// </summary>
        public e_TskLim limit
        {
            get { return GetLimit(); }
            set { SetLimit(value); }
        }
        #endregion
        #region Delay
        /// <summary>
        /// Свойство задержки связи
        /// </summary>
        public double delay
        {
            get { return GetDelay(); }
            set { SetDelay(value); }
        }
        #endregion
        #region State
        /// <summary>
        /// Свойство состояния связи
        /// </summary>
        public e_LnkState state => GetLinkState();
        #endregion
        #region protected
        /// <summary>
        /// Свойство получения зависимой точки управляющего члена связи
        /// </summary>
        protected e_Dot dotDependMaster => Hlp.GetPrecursor(_limit);
        /// <summary>
        /// Свойство получения зависимой точки подчиненного члена связи
        /// </summary>
        protected e_Dot dotDependSlave => Hlp.GetFollower(_limit);
        #endregion
        #endregion
        #region Events
        /// <summary>
        /// Событие изменения значения задержки связи
        /// </summary>
        public event EventHandler<ea_ValueChange<double>> event_DelayChanged;
        /// <summary>
        /// Событие изменения значения вида связи
        /// </summary>
        public event EventHandler<ea_ValueChange<e_TskLim>> event_LimitChanged;
        /// <summary>
        /// Событие удаления экземпляра связи
        /// </summary>
        public event EventHandler<ea_IdObject> event_ObjectDeleted;
        #endregion
        #region Constructor
        /// <summary>
        /// Конструктор экземпляра связи
        /// </summary>
        /// <param name="master">Интерфейс управляющего члена связи</param>
        /// <param name="slave">Интерфейс подчиеннного члена связи</param>
        /// <param name="limitType">Вид связи</param>
        /// <param name="delay">Задержка связи</param>
        public link(IDock master, IDock slave, e_TskLim limitType, double delay)
        {
            init_default(limitType, delay);
            init_master(master, master);
            init_slave(slave, slave);
            init_slaveDependence();

            master.connect(this);
            slave.connect(this);
        }
        /// <summary>
        /// Конструктор экземпляра связи
        /// </summary>
        /// <param name="master">Интерфейс управляющего члена связи</param>
        /// <param name="slave">Интерфейс подчиеннного члена связи</param>
        /// <param name="limitType">Вид связи</param>
        public link(IDock master, IDock slave, e_TskLim limitType)
            :this(master, slave, limitType, 0)
        { }
        #endregion
        #region Initializers
        #region Default
        /// <summary>
        /// Метод основной инициализации (запускается первым)
        /// </summary>
        /// <param name="limit">Вид связи</param>
        /// <param name="delay">Задержка связи</param>
        protected void init_default(e_TskLim limit, double delay = 0)
        {
            //Запускать первым из инитов
            if (!Enum.IsDefined(typeof(e_TskLim), limit)) throw new ArgumentException("Wrong e_TskLim value");

            _identity = new Identity(e_Entity.Link);
            _limit = limit;
            _delay = delay;
            _state = e_LnkState.InTime;

            _master = new linkMember(this, e_DependType.Master);
            _slave = new linkMember(this, e_DependType.Slave);
        }
        #endregion
        #region Master
        /// <summary>
        /// Метод инициализации управляющего члена связи (запускается вторым)
        /// </summary>
        /// <param name="masterID">Интерфейс идентификатора</param>
        /// <param name="masterLine">Интерфейс отрезка</param>
        protected void init_master(IId masterID, ILine masterLine)
        {
            //Запускать вторым из инитов
            _master.init_member(masterID, masterLine, dotDependMaster);

            event_LimitChanged += _master.handler_limitTypeChanged;

            aUnsuscribeMaster = () =>
            {
                event_LimitChanged -= _master.handler_limitTypeChanged;

                aUnsuscribeMaster = () => { };
            };
        }
        #endregion
        #region Slave
        /// <summary>
        /// Метод инициализации подчиненного члена связи (запускается третьим)
        /// </summary>
        /// <param name="slaveID">Интерфейс идентификатора</param>
        /// <param name="slaveLine">Интерфейс отрезка</param>
        protected void init_slave(IId slaveID, ILine slaveLine)
        {
            //Запускать третьим из инитов
            _slave.init_member(slaveID, slaveLine, dotDependSlave);
            _slave.date = _master.getObjectDependDotInfo().GetDate();

            event_LimitChanged += _slave.handler_limitTypeChanged;
            event_DelayChanged += _slave.handler_delayChanged;
            _master.getObjectDependDotInfo().event_DateChanged += _slave.handler_dependDateChange;
            _slave.event_neighboursDeltaChanged += handler_deltaSlaveChange;

            aUnsuscribeSlave = () =>
            {
                event_LimitChanged -= _slave.handler_limitTypeChanged;
                event_DelayChanged -= _slave.handler_delayChanged;
                _master.getObjectDependDotInfo().event_DateChanged -= _slave.handler_dependDateChange;
                _slave.event_neighboursDeltaChanged -= handler_deltaSlaveChange;

                aUnsuscribeSlave = () => { };
            };
        }
        #endregion
        #region Dependence
        /// <summary>
        /// Метод инициализации зависимости для подчиненного члена связи (запускается четвертым)
        /// </summary>
        protected void init_slaveDependence()
        {
            //Запускать четвертым из инитов
            _dependSlave = 
                new Dependence(
                    _master.getObjectDependDotInfo().GetDate(),
                    _slave.direction,
                    dotDependSlave);
            _dependSlave.setSender(this);

            _slave.event_dependDateChanged += _dependSlave.handler_DateChange;
            _slave.event_dependDotChanged += _dependSlave.handler_DependDotChange;

            aUnsuscribeDependSlave = () =>
            {
                _slave.event_dependDateChanged -= _dependSlave.handler_DateChange;
                _slave.event_dependDotChanged -= _dependSlave.handler_DependDotChange;

                aUnsuscribeDependSlave = () => { };
            };
        }
        #endregion
        #endregion
        #region Handlers
        /// <summary>
        /// Обработчик изменения разности дат зависимости связи и зависимой точки подчиненного объекта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void handler_deltaSlaveChange(object sender, ea_ValueChange<double> e)
        {
            setLinkState(e.NewValue);
        }
        #endregion
        #region Methods
        #region Object
        /// <summary>
        /// Метод получения уникального номера экземпляра связи
        /// </summary>
        /// <returns>Значение уникального номера экземпляра связи</returns>
        public string GetId()
        {
            return _identity.Id;
        }
        /// <summary>
        /// Метод получения типа сущности экземпляра связи
        /// </summary>
        /// <returns>Значение типа сущности экземпляра связи</returns>
        e_Entity IId.GetType()
        {
            return _identity.Type;
        }
        /// <summary>
        /// Метод удаления объекта
        /// </summary>
        public void DeleteObject()
        {
            aUnsuscribeDependSlave();
            aUnsuscribeSlave();
            aUnsuscribeMaster();
            _dependSlave = null;
            _slave = null;
            _master = null;
            _identity = null;
            aUnsuscribeDependSlave = aUnsuscribeMaster = aUnsuscribeSlave = null;
            event_ObjectDeleted?.Invoke(this, new ea_IdObject(this));
        }
        #endregion
        #region Limit
        /// <summary>
        /// Метод получения вида связи
        /// </summary>
        /// <returns>Значение вида связи</returns>
        public e_TskLim GetLimit()
        {
            return _limit;
        }
        /// <summary>
        /// Метод установки значения вида связи
        /// </summary>
        /// <param name="limitType">вид связи</param>
        /// <returns>Истина если новый вид связи установлен</returns>
        /// <exception cref="ArgumentException">Если значение не подходит под <seealso cref="e_TskLim"/></exception>
        public bool SetLimit(e_TskLim limitType)
        {
            if(!Enum.IsDefined(typeof(e_TskLim), limitType )) throw new ArgumentException("Wrong argument value of e_TskLim type");
            if (limitType == _limit) return false;

            e_TskLim Old = _limit;
            _limit = limitType;

            event_LimitChanged?.Invoke(this, new ea_ValueChange<e_TskLim>(Old, _limit));

            return true;
        }
        #endregion
        #region Delay
        /// <summary>
        /// Метод получения задержки связи в днях
        /// </summary>
        /// <returns>Значение задержки связи в днях</returns>
        public double GetDelay()
        {
            return _delay;
        }
        /// <summary>
        /// Метод установки задержки связи в днях
        /// </summary>
        /// <param name="days">Задержка связи в днях</param>
        /// <returns>Истина если новое значение установлено</returns>
        public bool SetDelay(double days)
        {
            if (_delay == days) return false;

            double Old = _delay;
            _delay = days;

            event_DelayChanged?.Invoke(this, new ea_ValueChange<double>(Old, _delay));

            return true;
        }
        #endregion
        #region State
        /// <summary>
        /// Метод получения состояния связи
        /// </summary>
        /// <returns>Значение состояния связи</returns>
        public e_LnkState GetLinkState()
        {
            return _state;
        }
        /// <summary>
        /// Метод установки значения состояния связи
        /// </summary>
        /// <param name="slaveDelta">Значение разности дат зависимости связи и зависимой точки подчиненного члена</param>
        protected void setLinkState(double slaveDelta)
        {
            if(_slave.dependDate == _slave.getObjectDependDotInfo().GetDate()) _state = e_LnkState.InTime;
            else if (slaveDelta > 0) _state = e_LnkState.Later;
            else _state = e_LnkState.Early;
        }
        #endregion
        #region Dependence
        /// <summary>
        /// Метод получения ссылки на интерфейс зависимости для зависимого члена связи
        /// </summary>
        /// <returns>Ссылка на интерфейс зависимости для зависимого члена связи</returns>
        public IDependence GetSlaveDependence()
        {
            return _dependSlave;
        }
        #endregion
        #region Members
        /// <summary>
        /// Метод получения ссылки на интерфейс класса информации о члене связи
        /// </summary>
        /// <param name="member">Идентификатор члена связи</param>
        /// <returns>Ссылка на интерфейс класса информации о члене связи</returns>
        public ILMember GetInfoMember(IId member)
        {
            if (member == _slave.memberID) return _slave;
            else if (member == _master.memberID) return _master;
            else return null;
        }
        /// <summary>
        /// Метод получения ссылки на интерфейс класса информации о члене связи
        /// </summary>
        /// <param name="member">Зависимость члена связи</param>
        /// <returns>Ссылка на интерфейс класса информации о члене связи</returns>
        public ILMember GetInfoMember(e_DependType member)
        {
            if (Enum.IsDefined(typeof(e_DependType), member))
                return (member == e_DependType.Master) ? _master : _slave;
            else
                return null;
        }
        #endregion
        #endregion
    }
}
