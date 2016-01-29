using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.iface;
using alter.Link.iface;
using alter.types;

namespace alterTesting.Emulators
{
    public class tstLink : alter.Link.iface.ILink
    {
        #region vars
        private alter.classes.identity _ID;
        private double _delay;
        #endregion
        #region props
        public double delay
        {
            get { return _delay; }
            set
            {
                if(value != _delay)
                {
                    if (value < 0) value = 0;
                    _delay = value;
                }
            }
        }
        #endregion
        #region events
        public event EventHandler<EA_IDObject> event_objectDeleted;
        #endregion
        #region constructors
        public tstLink()
        {
            _ID = new alter.classes.identity(eEntity.link);
        }
        #endregion
        #region handlers
        private void onDeleteLink()
        {
            EventHandler<EA_IDObject> handler = event_objectDeleted;
            if (handler != null) handler(this, new EA_IDObject(this));
        }
        #endregion
        #region methods
        #region object
        public string getID()
        { return _ID.ID; }
        public eEntity getType()
        { return _ID.type; }
        public void deleteObject()
        { onDeleteLink(); }
        #endregion
        #region members
        public ILMember getInfoMember(IId member)
        {
            throw new NotImplementedException();
        }
        public ILMember getInfoMember(eDependType member)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region limts
        public eLnkLim getLimit()
        {
            throw new NotImplementedException();
        }
        public bool setLimit(eLnkLim limitType)
        {
            throw new NotImplementedException();
        }
        public Type getLimitType()
        {
            throw new NotImplementedException();
        }
        #endregion
        public double getDelay()
        { return delay; }
        public void setDelay(double days)
        { delay = days; }
        public eLnkState getLinkState()
        {
            throw new NotImplementedException();
        }

        
        public void unsuscribe(string IDsubscriber)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public struct linkMember : alter.Link.iface.ILMember
    {
        public eDependType dependType;
        
        public IDependence getDependence()
        {
            throw new NotImplementedException();
        }

        public eDependType getDependType()
        {
            throw new NotImplementedException();
        }

        public IId getMemberID()
        {
            throw new NotImplementedException();
        }
    }
}
