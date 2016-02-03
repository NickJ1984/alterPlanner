using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;
using alter.types;
using alter.args;

namespace alter.Service.iface
{
    /// <summary>
    /// Интерфейс адаптера точек, позволяющий иметь постоянную датируемую точку, ссылающуюся на любую внутреннюю точку объекта.
    /// </summary>
    public interface IDotAdapter : IDot
    {
        /// <summary>
        /// Событие срабатывающее при изменении ссылки на внутреннюю точку.
        /// </summary>
        event EventHandler<ea_ValueChange<IDot>> event_DotChanged;
        /// <summary>
        /// Установить ссылку на внутреннюю точку.
        /// </summary>
        /// <param name="innerDot">Внутренняя точка.</param>
        void SetInnerDot(IDot innerDot);
    }
}
