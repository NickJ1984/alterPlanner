using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.classes;
using alter.iface;
using alter.types;

namespace alterTesting.Emulators
{
    public class dotCLass : IDot
    {
        protected DateTime _date;
        protected e_Dot _type;
        public virtual DateTime date
        {
            get { return _date; }
            set
            {
                if (_date != value)
                {
                    DateTime old = _date;
                    _date = value;
                    event_DateChanged?.Invoke(this, new ea_ValueChange<DateTime>(old, _date));
                }
            } 
        }
        public virtual e_Dot type => _type;

        public event EventHandler<ea_ValueChange<DateTime>> event_DateChanged;

        public dotCLass(e_Dot type)
        {
            this._type = type;
            _date = Hlp.InitDate;
        }

        public DateTime GetDate()
        {
            return date;
        }
        public e_Dot GetDotType()
        {
            return type;
        }
    }
}
