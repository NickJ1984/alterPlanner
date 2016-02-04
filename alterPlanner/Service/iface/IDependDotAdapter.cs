using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.iface;
using alter.types;

namespace alter.Service.iface
{
    public interface IDependDotAdapter : IDot
    {
        bool setDependDot(e_Dot dependDot);
        event EventHandler<ea_ValueChange<e_Dot>> event_dotTypeChanged;
        IId getParentId();
    }
}
