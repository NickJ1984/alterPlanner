using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;
using alter.types;

namespace alter.Task.classes
{
    public interface ITask2 : IId, IRemovable, ILine, IName, ILimit<e_TLLim>
    {
        
    }

    public partial class task2
    {

    }

}
