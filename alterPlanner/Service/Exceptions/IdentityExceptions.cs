using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alter.Service.Exceptions
{
    public class wrongObjectEntityException : ApplicationException
    {
        public wrongObjectEntityException(object wrongObject)
            :base(string.Format("Некорректная сущность объекта {0}", nameof(wrongObject)))
        { }
    }
}
