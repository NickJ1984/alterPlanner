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
    public partial class link : ILink
    {
        //НАДО ТЕСТИРОВАТЬ!
        #region Vars
        protected Identity _identity;
        protected double _delay;
        protected e_TskLim _limit;
        protected e_LnkState _state;
        #region Classes
        protected linkMember _slave;
        protected linkMember _master;
        protected Dependence _dependSlave; 
        #endregion
        #region Delegates
        protected Action aUnsuscribeMaster = () => { };
        protected Action aUnsuscribeSlave = () => { };
        protected Action aUnsuscribeDependSlave = () => { };
        #endregion
        #endregion
        #region Props
        #region Limit
        public e_TskLim limit
        {
            get { return GetLimit(); }
            set { SetLimit(value); }
        }
        #endregion
        #region Delay
        public double delay
        {
            get { return GetDelay(); }
            set { SetDelay(value); }
        }
        #endregion
        #region State
        public e_LnkState state => GetLinkState();
        #endregion
        #region protected
        protected e_Dot dotDependMaster => Hlp.GetPrecursor(_limit);
        protected e_Dot dotDependSlave => Hlp.GetFollower(_limit);
        #endregion
        #endregion
        #region Events
        public event EventHandler<ea_ValueChange<double>> event_DelayChanged;
        public event EventHandler<ea_ValueChange<e_TskLim>> event_LimitChanged;
        public event EventHandler<ea_IdObject> event_ObjectDeleted;
        #endregion
        #region Constructor
        public link(IDock master, IDock slave, e_TskLim limitType, double delay)
        {
            init_default(limitType, delay);
            init_master(master, master);
            init_slave(slave, slave);
            init_slaveDependence();

            master.connect(this);
            slave.connect(this);
        }
        public link(IDock master, IDock slave, e_TskLim limitType)
            :this(master, slave, limitType, 0)
        { }
        #endregion
        #region Initializers
        #region Default
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
        protected void init_slave(IId slaveID, ILine slaveLine)
        {
            //Запускать третьим из инитов
            _slave.date = _master.getObjectDependDotInfo().GetDate();

            _slave.init_member(slaveID, slaveLine, dotDependSlave);

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
        protected void handler_deltaSlaveChange(object sender, ea_ValueChange<double> e)
        {
            setLinkState(e.NewValue);
        }
        #endregion
        #region Methods
        #region Object
        public string GetId()
        {
            throw new NotImplementedException();
        }
        e_Entity IId.GetType()
        {
            throw new NotImplementedException();
        }
        public void DeleteObject()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Limit
        public e_TskLim GetLimit()
        {
            return _limit;
        }
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
        public double GetDelay()
        {
            return _delay;
        }
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
        public e_LnkState GetLinkState()
        {
            return _state;
        }
        protected void setLinkState(double slaveDelta)
        {
            if(_slave.dependDate == _slave.getObjectDependDotInfo().GetDate()) _state = e_LnkState.InTime;
            else if (slaveDelta > 0) _state = e_LnkState.Later;
            else _state = e_LnkState.Early;
        }
        #endregion
        #region Dependence
        public IDependence GetSlaveDependence()
        {
            return _dependSlave;
        }
        #endregion
        #region Members
        public ILMember GetInfoMember(IId member)
        {
            if (member == _slave.memberID) return _slave;
            else if (member == _master.memberID) return _master;
            else return null;
        }
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
