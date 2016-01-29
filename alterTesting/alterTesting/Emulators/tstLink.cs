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
        #endregion
        #region props

        #endregion
        #region events
        public event EventHandler<EA_IDObject> event_objectDeleted;
        #endregion
        #region constructors

        #endregion
        #region handlers

        #endregion
        #region methods
        #region object
        public string getID()
        {
            throw new NotImplementedException();
        }
        public void deleteObject()
        {
            throw new NotImplementedException();
        }
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
        {
            throw new NotImplementedException();
        }
        public void setDelay(double days)
        {
            throw new NotImplementedException();
        }
        public eLnkState getLinkState()
        {
            throw new NotImplementedException();
        }

        public eEntity getType()
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
