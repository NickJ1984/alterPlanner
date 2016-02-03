using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.types;
using alter.iface;
using alter.Link.iface;
using alter.Project.iface;

namespace alterTesting.Emulators
{

    public class tstLink : alter.Link.iface.ILink
    {
        #region vars
        private IProject _project;
        private alter.iface.IId _ident;
        private e_LnkState _state;
        private e_TskLim _limit;
        private double _delay;

        private alter.Link.classes.LinkMember masterMbr;
        private alter.Link.classes.LinkMember slaveMbr;
        private IDot dotSlave;
        private IDot dotMaster;

        private Action unsuscribeMbrs;

        private Func<e_Dot> dependDotMst;
        private Func<e_Dot> dependDotSlv;
        #endregion
        #region events
        public event EventHandler<ea_IdObject> event_ObjectDeleted;
        #endregion
        #region constructors
        public tstLink(IProject project, IDock master, IDock slave)
        {
            _project = project;
            _delay = 0;
            
            dotMaster = master.Subscribe(e_DependType.Master, this);
            dotSlave = slave.Subscribe(e_DependType.Slave, this);

            
        }
        public void subscribe(IDock master, IDock slave)
        {
            masterMbr = new alter.Link.classes.LinkMember(
                        new alter.Function.classes.VectorF()
                        {
                            Date = alter.classes.Hlp.InitDate,
                            Direction = e_Direction.Fixed
                        },
                        dependDotMst()
                        );
            masterMbr.SetInfo(master);
            masterMbr.SetDependType(e_DependType.Master);

            dotMaster = master.Subscribe(e_DependType.Master, this);

            masterMbr.Depend.SetDate(() => dotMaster.GetDate());
            masterMbr.Depend.SetDependDot(() => dependDotMst());

            master.event_ObjectDeleted += onDeleteTask;
            dotMaster.event_DateChanged += onMasterDateChange;


            slaveMbr = new alter.Link.classes.LinkMember(
                new alter.Function.classes.VectorF()
                {
                    Date = getDateLimit(),
                    Direction = e_Direction.Fixed
                },
                dependDotSlv()
                );
            slaveMbr.SetInfo(slave);
            slaveMbr.SetDependType(e_DependType.Slave);

            dotSlave = slave.Subscribe(e_DependType.Slave, this);
            



        }
        public void initUnsuscribe(IDock dock, e_DependType type)
        {

        }
        public void initFunctions(IId mstID, IId slvID)
        {
            dependDotMst = () => alter.classes.Hlp.GetPrecursor(_limit);
            dependDotSlv = () => alter.classes.Hlp.GetFollower(_limit);
        }
        #endregion
        #region handlers
        private void onMasterDateChange(object sender, ea_ValueChange<DateTime> e)
        {

        }
        private void onSlaveDateChange(object sender, ea_ValueChange<DateTime> e)
        {

        }
        private void onDeleteTask(object sender, ea_IdObject e)
        { onDeleteLink(); }
        private void onDeleteLink()
        {
            //unsuscribe();
            EventHandler<ea_IdObject> handler = event_ObjectDeleted;
            if (handler != null) handler(this, new ea_IdObject(this));
        }
        #endregion
        #region Object
        #region ID
        public e_Entity GetType()
        { return _ident.GetType(); }
        public string GetId()
        { return _ident.GetId(); }
        #endregion
        #region params
        public e_LnkState GetLinkState()
        {
            throw new NotImplementedException();
        }
        public double GetDelay()
        { return _delay; }
        public void SetDelay(double days)
        {
            throw new NotImplementedException();
        }
        public e_TskLim GetLimit()
        { return _limit; }
        public bool SetLimit(e_TskLim limitType)
        {
            throw new NotImplementedException();
        } 
        #endregion
        public void Unsuscribe(string IDsubscriber)
        {
            //if (IDsubscriber == masterID || IDsubscriber == slaveID) onDeleteLink();
        }
        public void DeleteObject()
        {
            onDeleteLink();
        } 
        #endregion
        public ILMember GetInfoMember(IId member)
        {
            throw new NotImplementedException();
        }
        public ILMember GetInfoMember(e_DependType member)
        {
            throw new NotImplementedException();
        }
        #region Service
        private DateTime getDateLimit()
        { return dotMaster.GetDate().AddDays(_delay); }
        
        private e_DependType getDepend(string memberID)
        {
           return (memberID == masterMbr.GetMemberId().GetId()) ? e_DependType.Master : e_DependType.Slave;
        } 
        #endregion
    }
}
