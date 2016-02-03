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
        private class CDotManager : ITDotManager
        {
            #region vars
            private e_Dot _activeVal = e_Dot.Start;
            private IDot _active;
            private IDot _passive;
            #endregion
            #region props

            #endregion
            #region events
            public event EventHandler<ea_ValueChange<e_Dot>> event_ActiveChanged;
            #endregion
            #region constructors
            public CDotManager(IDot start, IDot finish)
            {
                _active = start;
                _passive = finish;
            }
            #endregion
            #region handlers
            private void OnActiveChanged(ea_ValueChange<e_Dot> args)
            {
                EventHandler<ea_ValueChange<e_Dot>> handler = event_ActiveChanged;
                if (handler != null) handler(this, args);
            }
            #endregion
            #region methods
            public void ChangeActiveDot(e_Dot type)
            {
                if (type == _activeVal) return;

                IDot temp = _active;
                _active = _passive;
                _passive = temp;

                OnActiveChanged(new ea_ValueChange<e_Dot>(temp.GetDotType(), _active.GetDotType()));
            }

            public IDot GetActive()
            { return _active; }

            public IDot GetDot(e_Dot type)
            { return (type == _active.GetDotType()) ? _active : _passive; }

            public IDot GetPassive()
            { return _passive; }
            #endregion
        }
    }
}
