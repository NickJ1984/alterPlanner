using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.Task.iface.TaskComponents;
using alter.types;
using alter.iface;


namespace alter.Task.classes
{
    public partial class task
    {
        private class cDotManager : ITDotManager
        {
            #region vars
            private eDot activeVal = eDot.start;
            private IDot _active;
            private IDot _passive;
            #endregion
            #region props

            #endregion
            #region events
            public event EventHandler<EA_valueChange<eDot>> event_activeChanged;
            #endregion
            #region constructors
            public cDotManager(IDot start, IDot finish)
            {
                _active = start;
                _passive = finish;
            }
            #endregion
            #region handlers
            private void onActiveChanged(EA_valueChange<eDot> args)
            {
                EventHandler<EA_valueChange<eDot>> handler = event_activeChanged;
                if (handler != null) handler(this, args);
            }
            #endregion
            #region methods
            public void changeActiveDot(eDot type)
            {
                if (type == activeVal) return;

                IDot temp = _active;
                _active = _passive;
                _passive = temp;

                onActiveChanged(new EA_valueChange<eDot>(temp.getDotType(), _active.getDotType()));
            }

            public IDot getActive()
            { return _active; }

            public IDot getDot(eDot type)
            { return (type == _active.getDotType()) ? _active : _passive; }

            public IDot getPassive()
            { return _passive; }
            #endregion
        }
    }
}
