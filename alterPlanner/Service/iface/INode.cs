using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.Service.classes;

namespace alter.Service.iface
{
    public interface INode<T>
        where T : IComparable
    {
        bool isTail { get; }
        bool isHead { get; }
        string ID { get; }
        bool isConnected { get; }
        T data { get; }

        void handler_dataChange(object sender, EventArgs e);

        int comparePrevious();
        int compareNext();
        bool processData(Action<T> aDataProcess);
        bool setData(T data);
    }

}
