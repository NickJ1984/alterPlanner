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
    public struct LinkMember : alter.Link.iface.ILMember
    {
        private IId _mbr;
        private e_DependType _type;
        public Dependence Depend;

        public LinkMember(VectorF vector, e_Dot dependDot)
        {
            _mbr = null;
            _type = e_DependType.Master;
            Depend = new Dependence(vector.Date, vector.Direction, dependDot);
        }
        
        public void SetInfo(IId member)
        { _mbr = member; }
        public e_DependType GetDependType()
        { return _type; }
        public void SetDependType(e_DependType type)
        { if (_type != type) _type = type; }
        public IDependence GetDependence()
        { return Depend; }
        public IId GetMemberId()
        { return _mbr; }
    }
}
