using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;
using alter.types;
using alter.args;

namespace alter.Link.iface
{
    public interface ILMember
    {
        IId getMemberID();
        eDependType getDependType();
        IDependence getDependence();
    }
}
