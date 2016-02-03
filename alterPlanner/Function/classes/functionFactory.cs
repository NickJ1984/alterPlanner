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
    public class FunctionFactory : IFunctionFactory
    {
        #region vars
        private function _function;
        #endregion
        #region constructor
        public FunctionFactory()
        {
            _function = new function(Hlp.InitDate, e_Direction.Right);
        }
        #endregion
        #region methods
        public IFunction CreateFunction(DateTime date, e_Direction direction)
        { return new function(date, direction); }
        public Func<DateTime, DateTime, DateTime> CreateFunctionCheck(e_Direction direction)
        { return _function.GetFunctionDir(direction); }
        #endregion

    }
}
