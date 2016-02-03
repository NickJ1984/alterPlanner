using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.types;
using alter.iface;

namespace alter.Task.iface
{
    public interface ITaskFactory : IId, ILine, IChild
    {
        ITask GetTask(string dtask);

        ITask CreateTask(DateTime startDate, double duration, e_TlLim localLimit);
        
        bool deleteTask(string dtask);
        bool deleteTask();
    }
}
