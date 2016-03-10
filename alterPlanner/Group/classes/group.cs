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
    public class Group : IGroup
    {
        public event EventHandler<ea_ValueChange<double>> event_DurationChanged;
        public event EventHandler<ea_ValueChange<e_GrpLim>> event_LimitChanged;
        public event EventHandler<ea_IdObject> event_ObjectDeleted;

        public bool AddInGroup(IDock newObject)
        {
            throw new NotImplementedException();
        }

        public bool connect(ILink link)
        {
            throw new NotImplementedException();
        }

        public void DeleteObject()
        {
            throw new NotImplementedException();
        }

        public bool DelFromGroup(string objectId)
        {
            throw new NotImplementedException();
        }

        public int EnclosureCount()
        {
            throw new NotImplementedException();
        }

        public IDot GetDot(e_Dot type)
        {
            throw new NotImplementedException();
        }

        public double GetDuration()
        {
            throw new NotImplementedException();
        }

        public IDependence getGroupDepend()
        {
            throw new NotImplementedException();
        }

        public IId GetGroupOwner()
        {
            throw new NotImplementedException();
        }

        public string GetId()
        {
            throw new NotImplementedException();
        }

        public e_GrpLim GetLimit()
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

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

        public int TaskCount()
        {
            throw new NotImplementedException();
        }

        e_Entity IId.GetType()
        {
            throw new NotImplementedException();
        }
    }

}
