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
        ITask getTask(string IDtask);

        ITask createTask(DateTime startDate, double duration, eTLLim localLimit);
        
        bool deleteTask(string IDtask);
        bool deleteTask();
    }
}
