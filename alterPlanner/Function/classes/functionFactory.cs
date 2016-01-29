using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.classes;
using alter.Function.iface;
using alter.types;

namespace alter.Function.classes
{
    public class functionFactory : IFunctionFactory
    {
        #region vars
        private function _function;
        #endregion
        #region constructor
        public functionFactory()
        {
            _function = new function(__hlp.initDate, eDirection.RightMax);
        }
        #endregion
        #region methods
        public IFunction createFunction(DateTime date, eDirection direction)
        { return new function(date, direction); }
        public Func<DateTime, DateTime, DateTime> createFunctionCheck(eDirection direction)
        { return _function.getFunctionDir(direction); }
        #endregion

    }
}
