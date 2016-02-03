using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;
using alter.args;
using alter.Function.iface;
using alter.types;
using alter.Task.iface.TaskComponents;

namespace alter.Task.iface
{
    public interface ITask : IId, IRemovable, ILine, IName, ILimit<e_TLLim>, IDock
    {
        
    }
}
