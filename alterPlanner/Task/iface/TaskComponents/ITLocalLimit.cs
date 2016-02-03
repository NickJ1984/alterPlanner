using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;
using alter.args;
using alter.types;

namespace alter.Task.iface.TaskComponents
{
    public interface ITLocalLimit : IDependence
    {
        void SetLLimit(e_TLLim localLimit);
        e_TLLim GetLLimit();

        void SetDate(DateTime date);
    }
}
