using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.classes;
using alter.types;
using alter.iface;
using alter.Link.classes;
using alter.Link.iface;
using alter.Project.iface;

namespace alterTesting.Emulators
{
    public class linkClass : ILink
    {
        #region Vars
        protected Identity _ident;
        protected e_TskLim _limit;
        protected double _delay;
        protected linkMember slave;
        protected linkMember master;
        protected Dependence dependSlave;
        #endregion
        #region Props
        public linkMember lmSlave => slave;
        public linkMember lmMaster => master;
        public Dependence depend => dependSlave;
        public double delay
        {
            get { return GetDelay(); }
            set { SetDelay(value); }
        }
        public e_TskLim limit
        {
            get { return GetLimit(); }
            set { SetLimit(value); }
        }
        #endregion
        #region Events
        public event EventHandler<ea_ValueChange<double>> event_DelayChanged;
        public event EventHandler<ea_ValueChange<e_TskLim>> event_LimitChanged;
        public event EventHandler<ea_IdObject> event_ObjectDeleted;
        #endregion
        #region Constructor
        public linkClass(IId MasterID, ILine MasterLine, IId SlaveID, ILine SlaveLine, e_TskLim limit)
        {
            if(!Enum.IsDefined(typeof(e_TskLim),limit)) throw new ArgumentNullException();
            this.limit = limit;
            _ident = new Identity(e_Entity.Link);
            delay = 0;

            init_master(MasterID, MasterLine);
            Console.WriteLine("Master initialized");
            init_slave(SlaveID, SlaveLine);
            Console.WriteLine("Slave initialized");
            init_dependence();
            Console.WriteLine("Dependence initialized");
        }
        #endregion
        #region Inits
        protected void init_master(IId masterID, ILine masterLine)
        {
            master = new linkMember(this, e_DependType.Master);
            master.init_member(masterID, masterLine, Hlp.GetDepenDot(limit, e_DependType.Master));

            event_LimitChanged += master.handler_limitTypeChanged;
            master.date = master.getObjectDependDotInfo().GetDate();
            master.delay = 0;
            master.getObjectDependDotInfo().event_DateChanged += master.handler_dependDateChange;
        }
        protected void init_slave(IId slaveID, ILine slaveLine)
        {
            slave = new linkMember(this, e_DependType.Slave);
            slave.init_member(slaveID, slaveLine, Hlp.GetDepenDot(limit, e_DependType.Slave));
            
            slave.date = master.getObjectDependDotInfo().GetDate();

            event_LimitChanged += slave.handler_limitTypeChanged;
            event_DelayChanged += slave.handler_delayChanged;
            master.getObjectDependDotInfo().event_DateChanged += slave.handler_dependDateChange;
        }
        protected void init_dependence()
        {
            dependSlave = new Dependence(slave.dependDate, slave.direction, slave.dependDot);
            dependSlave.setSender(this);

            slave.event_dependDateChanged += (o, v) => dependSlave.date = v.NewValue;
            event_LimitChanged += (o, v) => dependSlave.SetDependDot(Hlp.GetDepenDot(v.NewValue, e_DependType.Slave));
        }
        #endregion
        #region Object
        public void DeleteObject()
        {
            event_ObjectDeleted?.Invoke(this, new ea_IdObject(this));
        }
        public string GetId()
        {
            return _ident.Id;
        }
        e_Entity IId.GetType()
        {
            return _ident.GetType();
        }
        #endregion
        #region Members
        public ILMember GetInfoMember(IId member)
        {
            if (slave.memberID == member) return slave;
            else if (master.memberID == member) return master;
            else throw new ArgumentException("Wrong member ID");
        }
        public ILMember GetInfoMember(e_DependType member)
        {
            if (member == e_DependType.Master && master != null) return master;
            else if (member == e_DependType.Slave && slave != null) return slave;
            else throw new ArgumentNullException();
        }
        public void Unsuscribe(string dsubscriber)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Methods
        #region Limit
        public e_TskLim GetLimit()
        {
            return _limit;
        }
        public bool SetLimit(e_TskLim limitType)
        {
            if (_limit == limitType) return false;

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
            throw new NotImplementedException();
        }

        public IDependence GetSlaveDependence()
        {
            return dependSlave;
        }
        #endregion
        #endregion
    }

}
