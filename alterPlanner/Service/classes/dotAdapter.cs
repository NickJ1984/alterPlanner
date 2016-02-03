using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.Service.iface;
using alter.iface;
using alter.args;
using alter.types;

namespace alter.Service.classes
{
    public class DotAdapter : IDotAdapter
    {
        #region vars
        private IDot _innerDot;
        #endregion
        #region props

        #endregion
        #region events
        public event EventHandler<ea_ValueChange<IDot>> event_DotChanged;
        public event EventHandler<ea_ValueChange<DateTime>> event_DateChanged;
        #endregion
        #region constructor
        public DotAdapter(IDot innerDot)
        {
            SetInnerDot(innerDot);
        }
        #endregion
        #region handlers
        private void OnDateChanged(ea_ValueChange<DateTime> args)
        {
            EventHandler<ea_ValueChange<DateTime>> handler = event_DateChanged;
            if (handler != null) handler(this, args);
        }
        private void OnDotChanged(ea_ValueChange<IDot> args)
        {
            EventHandler<ea_ValueChange<IDot>> handler = event_DotChanged;
            if (handler != null) handler(this, args);
        }
        private void handler_dateChanged(object sender, ea_ValueChange<DateTime> e)
        {
            OnDateChanged(new ea_ValueChange<DateTime>(e.OldValue, e.NewValue));
        }
        #endregion
        #region methods
        public DateTime GetDate()
        {
            return _innerDot.GetDate();
        }
        public e_Dot GetDotType()
        {
            return _innerDot.GetDotType();
        }
        public void SetInnerDot(IDot dot)
        {
            if (dot == null || dot == _innerDot) return;

            _innerDot.event_DateChanged -= handler_dateChanged;

            DateTime oldDate = _innerDot.GetDate();
            IDot oldDot = _innerDot;
            _innerDot = dot;

            _innerDot.event_DateChanged += handler_dateChanged;

            OnDotChanged(new ea_ValueChange<IDot>(oldDot, _innerDot));

            if (oldDate != _innerDot.GetDate())
                OnDateChanged(new ea_ValueChange<DateTime>(oldDate, _innerDot.GetDate()));
        }
        #endregion
    }
}
