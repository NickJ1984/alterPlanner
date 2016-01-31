using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;
using alter.types;
using alter.classes;
using alter.Function.classes;

namespace alter.Link.classes
{
    public struct linkMember : alter.Link.iface.ILMember
    {
        private IId _mbr;
        private eDependType _type;
        public dependence depend;

        public linkMember(vectorF vector, eDot dependDot)
        {
            _mbr = null;
            _type = eDependType.master;
            depend = new dependence(vector.date, vector.direction, dependDot);
        }
        
        public void setInfo(IId member)
        { _mbr = member; }
        public eDependType getDependType()
        { return _type; }
        public void setDependType(eDependType type)
        { if (_type != type) _type = type; }
        public IDependence getDependence()
        { return depend; }
        public IId getMemberID()
        { return _mbr; }
    }
}
