using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dateFunction
{
    public interface ISender
    {
        object sender { get; set; }

        event EventHandler<valueChange<object>> event_senderChanged;
    }
}
