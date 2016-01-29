using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.Group.iface;
using alter.iface;
using alter.Link.iface;
using alter.types;

namespace alter.Group.classes
{
    public class group : IGroup
    {
        #region vars

        #endregion
        #region events
        public event EventHandler<EA_valueChange<double>> event_durationChanged;
        public event EventHandler<EA_IDObject> event_objectDeleted;
        #endregion
        #region constructors

        #endregion
        #region handlers

        #endregion
        #region methods
        #region object info
        public string getID()
        {
            throw new NotImplementedException();
        }
        public eEntity getType()
        {
            throw new NotImplementedException();
        }
        public string getName()
        {
            throw new NotImplementedException();
        }
        public IId getGroupOwner()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region group info

        public IDot getDot(eDot type)
        {
            throw new NotImplementedException();
        }
        public double getDuration()
        {
            throw new NotImplementedException();
        }
        public int enclosureCount()
        {
            throw new NotImplementedException();
        }

        #endregion
        #region manage elements
        public bool addInGroup(IDock newObject)
        {
            throw new NotImplementedException();
        }
        public bool delFromGroup(string objectID)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region limit
        public eGrpLim getLimit()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region object
        public void deleteObject()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
        

        

        

        public int groupCount()
        {
            throw new NotImplementedException();
        }

        public bool inGroup(string ID)
        {
            throw new NotImplementedException();
        }

        public bool setLimit(eGrpLim limitType)
        {
            throw new NotImplementedException();
        }

        public void setName(string name)
        {
            throw new NotImplementedException();
        }

        public IDot subscribe(eDependType dType, ILink link)
        {
            throw new NotImplementedException();
        }

        public int taskCount()
        {
            throw new NotImplementedException();
        }
    }
}
