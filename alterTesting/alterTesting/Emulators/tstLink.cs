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
        private eLnkState _state;
        private eTskLim _limit;
        private double _delay;

        private alter.Link.classes.linkMember masterMbr;
        private alter.Link.classes.linkMember slaveMbr;
        private IDot dotSlave;
        private IDot dotMaster;

        private Action unsuscribeMbrs;

        private Func<eDot> dependDotMst;
        private Func<eDot> dependDotSlv;
        #endregion
        #region events
        public event EventHandler<EA_IDObject> event_objectDeleted;
        #endregion
        #region constructors
        public tstLink(IProject project, IDock master, IDock slave)
        {
            _project = project;
            _delay = 0;
            
            dotMaster = master.subscribe(eDependType.master, this);
            dotSlave = slave.subscribe(eDependType.slave, this);

            
        }
        public void subscribe(IDock master, IDock slave)
        {
            masterMbr = new alter.Link.classes.linkMember(
                        new alter.Function.classes.vectorF()
                        {
                            date = alter.classes.__hlp.initDate,
                            direction = eDirection.Fixed
                        },
                        dependDotMst()
                        );
            masterMbr.setInfo(master);
            masterMbr.setDependType(eDependType.master);

            dotMaster = master.subscribe(eDependType.master, this);

            masterMbr.depend.setDate(() => dotMaster.getDate());
            masterMbr.depend.setDependDot(() => dependDotMst());

            master.event_objectDeleted += onDeleteTask;
            dotMaster.event_dateChanged += onMasterDateChange;


            slaveMbr = new alter.Link.classes.linkMember(
                new alter.Function.classes.vectorF()
                {
                    date = getDateLimit(),
                    direction = eDirection.Fixed
                },
                dependDotSlv()
                );
            slaveMbr.setInfo(slave);
            slaveMbr.setDependType(eDependType.slave);

            dotSlave = slave.subscribe(eDependType.slave, this);
            



        }
        public void initUnsuscribe(IDock dock, eDependType type)
        {

        }
        public void initFunctions(IId mstID, IId slvID)
        {
            dependDotMst = () => alter.classes.__hlp.getPrecursor(_limit);
            dependDotSlv = () => alter.classes.__hlp.getFollower(_limit);
        }
        #endregion
        #region handlers
        private void onMasterDateChange(object sender, EA_valueChange<DateTime> e)
        {

        }
        private void onSlaveDateChange(object sender, EA_valueChange<DateTime> e)
        {

        }
        private void onDeleteTask(object sender, EA_IDObject e)
        { onDeleteLink(); }
        private void onDeleteLink()
        {
            //unsuscribe();
            EventHandler<EA_IDObject> handler = event_objectDeleted;
            if (handler != null) handler(this, new EA_IDObject(this));
        }
        #endregion
        #region Object
        #region ID
        public eEntity getType()
        { return _ident.getType(); }
        public string getID()
        { return _ident.getID(); }
        #endregion
        #region params
        public eLnkState getLinkState()
        {
            throw new NotImplementedException();
        }
        public double getDelay()
        { return _delay; }
        public void setDelay(double days)
        {
            throw new NotImplementedException();
        }
        public eTskLim getLimit()
        { return _limit; }
        public bool setLimit(eTskLim limitType)
        {
            throw new NotImplementedException();
        } 
        #endregion
        public void unsuscribe(string IDsubscriber)
        {
            //if (IDsubscriber == masterID || IDsubscriber == slaveID) onDeleteLink();
        }
        public void deleteObject()
        {
            onDeleteLink();
        } 
        #endregion
        public ILMember getInfoMember(IId member)
        {
            throw new NotImplementedException();
        }
        public ILMember getInfoMember(eDependType member)
        {
            throw new NotImplementedException();
        }
        #region Service
        private DateTime getDateLimit()
        { return dotMaster.getDate().AddDays(_delay); }
        
        private eDependType getDepend(string memberID)
        {
           return (memberID == masterMbr.getMemberID().getID()) ? eDependType.master : eDependType.slave;
        } 
        #endregion
    }
}
