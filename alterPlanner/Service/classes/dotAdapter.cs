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
    public class dotAdapter : IDotAdapter
    {
        #region vars
        private IDot _innerDot;
        #endregion
        #region props

        #endregion
        #region events
        public event EventHandler<EA_valueChange<IDot>> event_dotChanged;
        public event EventHandler<EA_valueChange<DateTime>> event_dateChanged;
        #endregion
        #region constructor
        public dotAdapter(IDot innerDot)
        {
            setInnerDot(innerDot);
        }
        #endregion
        #region handlers
        private void onDateChanged(EA_valueChange<DateTime> args)
        {
            EventHandler<EA_valueChange<DateTime>> handler = event_dateChanged;
            if (handler != null) handler(this, args);
        }
        private void onDotChanged(EA_valueChange<IDot> args)
        {
            EventHandler<EA_valueChange<IDot>> handler = event_dotChanged;
            if (handler != null) handler(this, args);
        }
        private void handler_dateChanged(object sender, EA_valueChange<DateTime> e)
        {
            onDateChanged(new EA_valueChange<DateTime>(e.oldValue, e.newValue));
        }
        #endregion
        #region methods
        public DateTime getDate()
        {
            return _innerDot.getDate();
        }
        public eDot getDotType()
        {
            return _innerDot.getDotType();
        }
        public void setInnerDot(IDot dot)
        {
            if (dot == null || dot == _innerDot) return;

            _innerDot.event_dateChanged -= handler_dateChanged;

            DateTime oldDate = _innerDot.getDate();
            IDot oldDot = _innerDot;
            _innerDot = dot;

            _innerDot.event_dateChanged += handler_dateChanged;

            onDotChanged(new EA_valueChange<IDot>(oldDot, _innerDot));

            if (oldDate != _innerDot.getDate())
                onDateChanged(new EA_valueChange<DateTime>(oldDate, _innerDot.getDate()));
        }
        #endregion
    }
}
