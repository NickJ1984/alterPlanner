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
        //Допилить
        #region vars

        #endregion
        #region events
        public event EventHandler<ea_ValueChange<double>> event_DurationChanged;
        public event EventHandler<ea_IdObject> event_ObjectDeleted;
        public event EventHandler<ea_ValueChange<e_GrpLim>> event_LimitChanged;
        #endregion
        #region constructors

        #endregion
        #region handlers

        #endregion
        #region methods
        #region object info
        public string GetId()
        {
            throw new NotImplementedException();
        }
        public e_Entity GetType()
        {
            throw new NotImplementedException();
        }
        public string GetName()
        {
            throw new NotImplementedException();
        }
        public IId GetGroupOwner()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region group info

        public IDot GetDot(e_Dot type)
        {
            throw new NotImplementedException();
        }
        public double GetDuration()
        {
            throw new NotImplementedException();
        }
        public int EnclosureCount()
        {
            throw new NotImplementedException();
        }

        #endregion
        #region manage elements
        public bool AddInGroup(IDock newObject)
        {
            throw new NotImplementedException();
        }
        public bool DelFromGroup(string objectId)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region limit
        public e_GrpLim GetLimit()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region object
        public void DeleteObject()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
        

        

        

        public int GroupCount()
        {
            throw new NotImplementedException();
        }

        public bool InGroup(string id)
        {
            throw new NotImplementedException();
        }

        public bool SetLimit(e_GrpLim limitType)
        {
            throw new NotImplementedException();
        }

        public void SetName(string name)
        {
            throw new NotImplementedException();
        }

        public IDot Subscribe(e_DependType dType, ILink link)
        {
            throw new NotImplementedException();
        }

        public int TaskCount()
        {
            throw new NotImplementedException();
        }

        public ILinkConnector getConnector()
        {
            throw new NotImplementedException();
        }
    }
}
