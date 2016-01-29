using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;
using alter.args;
using alter.types;
using alter.Function.iface;

namespace alter.Link.iface
{
    public interface ILink : IId, IRemovable, ILimit<eLnkLim>
    {
        eLnkState getLinkState();
        Type getLimitType();

        void setDelay(double days);
        double getDelay();

        ILMember getInfoMember(eDependType member);
        ILMember getInfoMember(IId member);
        void unsuscribe(string IDsubscriber);
    }
}
