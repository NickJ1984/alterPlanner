using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
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
    public class simpleLink : ILink
    {
        public Identity id = new Identity(e_Entity.Link);
        protected e_TskLim limit;
        protected double delay;
        public simpleDepend depend;
        public simpleLMember master;
        public simpleLMember slave;

        public event EventHandler<ea_ValueChange<double>> event_DelayChanged;
        public event EventHandler<ea_ValueChange<e_TskLim>> event_LimitChanged;
        public event EventHandler<ea_IdObject> event_ObjectDeleted;

        public simpleLink(IId idMaster, ILine lMaster, IId idSlave, ILine lSlave, e_TskLim limit)
        {
            id = new Identity(e_Entity.Link);
            this.limit = limit;
            master = new simpleLMember(this, idMaster, lMaster, e_DependType.Master);
            depend = new simpleDepend(lMaster, Hlp.GetPrecursor(limit));
            slave = new simpleLMember(this, idSlave, lSlave, e_DependType.Slave);
        }

        public void DeleteObject()
        {
            event_ObjectDeleted?.Invoke(this, new ea_IdObject(this));
        }

        public double GetDelay()
        {
            return delay;
        }

        public string GetId()
        {
            return id.Id;
        }

        public ILMember GetInfoMember(IId member)
        {
            return (member.GetId() == master.GetMemberId().GetId()) ? master : slave;
        }

        public ILMember GetInfoMember(e_DependType member)
        {
            return member == e_DependType.Master ? master : slave;
        }

        public e_TskLim GetLimit()
        {
            return limit;
        }

        public e_LnkState GetLinkState()
        {
            return e_LnkState.InTime;
        }

        public IDependence GetSlaveDependence()
        {
            return depend;
        }

        public bool SetDelay(double days)
        {
            double Old = delay;
            delay = days;
            event_DelayChanged?.Invoke(this, new ea_ValueChange<double>(Old, delay));
            return true;
        }

        public bool SetLimit(e_TskLim limitType)
        {
            e_TskLim Old = limit;
            limit = limitType;
            event_LimitChanged?.Invoke(this, new ea_ValueChange<e_TskLim>(Old, limit));
            return true;
        }

        e_Entity IId.GetType()
        {
            return id.Type;
        }
        #region SimpleDependence
        public class simpleDepend : IDependence
        {
            protected ILine parent;
            protected e_Dot dependDot;
            public object sender;
            public DateTime lastDate { get; protected set; }

            public event EventHandler<ea_ValueChange<DateTime>> event_DateChanged;
            public event EventHandler<ea_ValueChange<e_Dot>> event_DependDotChanged;
            public event EventHandler<ea_ValueChange<e_Direction>> event_DirectionChanged;

            public simpleDepend(ILine parent, e_Dot dependDot)
            {
                this.parent = parent;
                this.dependDot = dependDot;
                lastDate = parent.GetDot(dependDot).GetDate();
                parent.GetDot(e_Dot.Finish).event_DateChanged += onDateChange;
                parent.GetDot(e_Dot.Start).event_DateChanged += onDateChange;
            }

            public DateTime CheckDate(DateTime date)
            {
                return parent.GetDot(dependDot).GetDate();
            }

            public DateTime GetDate()
            {
                return parent.GetDot(dependDot).GetDate();
            }

            public e_Dot GetDependDot()
            {
                return dependDot;
            }

            public e_Direction GetDirection()
            {
                return e_Direction.Fixed;
            }

            public void setDependDot(e_Dot dependDot)
            {
                changeDot(parent.GetDot(this.dependDot), parent.GetDot(dependDot));
            }
            
            private void onDateChange(object sender, ea_ValueChange<DateTime> e)
            {
                if (parent.GetDot(this.dependDot).GetDate() != lastDate)
                {
                    lastDate = e.NewValue;
                    event_DateChanged?.Invoke(sender, e);
                }
            }
            private void changeDot(IDot Old, IDot New)
            {
                e_Dot dOld = dependDot;
                if (Old != null)
                {
                    Old.event_DateChanged -= onDateChange;
                    dOld = Old.GetDotType();
                }
                New.event_DateChanged += onDateChange;
                e_Dot dNew = New.GetDotType();
                dependDot = dNew;
                event_DependDotChanged?.Invoke(sender, new ea_ValueChange<e_Dot>(dOld, dNew));

                if (lastDate != parent.GetDot(dependDot).GetDate())
                {
                    DateTime OldDate = lastDate;
                    lastDate = parent.GetDot(dependDot).GetDate();
                    event_DateChanged?.Invoke(sender, new ea_ValueChange<DateTime>(OldDate, lastDate));
                }
            }
        }

        #endregion
        #region SimpleLM
        public class simpleLMember : ILMember
        {
            protected simpleLink parent;
            protected IId memberID;
            protected ILine lMember;
            protected e_DependType depend;

            public simpleLMember(simpleLink parent, IId memberID, ILine lMember, e_DependType dependType)
            {
                depend = dependType;
                this.memberID = memberID;
                this.lMember = lMember;
            }

            public e_DependType GetDependType()
            {
                return depend;
            }

            public IId GetMemberId()
            {
                return memberID;
            }

            public IDot getObjectDependDotInfo()
            {
                return lMember.GetDot(Hlp.GetDepenDot(parent.limit, depend));
            }
        }
        #endregion
    }
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
