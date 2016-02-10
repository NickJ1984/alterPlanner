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
    public interface ILink : IId, IRemovable, ILimit<e_TskLim>
    {
        e_LnkState GetLinkState();

        bool SetDelay(double days);
        double GetDelay();

        event EventHandler<ea_ValueChange<double>> event_DelayChanged;

        IDependence GetSlaveDependence();
        ILMember GetInfoMember(e_DependType member);
        ILMember GetInfoMember(IId member);
        //void Unsuscribe(string dsubscriber); реализовать через фабрику
    }
}
