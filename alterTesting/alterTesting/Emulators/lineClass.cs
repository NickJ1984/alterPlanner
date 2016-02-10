
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.classes;
using alter.iface;
using alter.types;

namespace alterTesting.Emulators
{
    public class lineClass : IId, ILine
    {
        protected alter.classes.Identity ident;
        protected dotCLass _start;
        protected dotCLass _finish;
        public event EventHandler<ea_ValueChange<double>> event_DurationChanged;

        public double duration
        {
            get { return GetDuration(); }
            set
            {
                if(value >= 0) _finish.date = start.AddDays(value);
            }
        }
        public DateTime start
        {
            get { return _start.date; }
            set { _start.date = value; }
        }
        public DateTime finish
        {
            get { return _finish.date; }
            set { _finish.date = value; }
        }

        public lineClass()
        {
            _start = new dotCLass(e_Dot.Start);
            _finish = new dotCLass(e_Dot.Finish);
            ident = new Identity(e_Entity.Task);
        }


        public IDot GetDot(e_Dot type)
        {
            if(!Enum.IsDefined(typeof(e_Dot), type)) throw new ArgumentException();
            return (type == e_Dot.Start) ? _start : _finish;
        }

        public double GetDuration()
        {
            return _finish.date.Subtract(_start.date).Days;
        }

        public string GetId()
        {
            return ident.Id;
        }

        e_Entity IId.GetType()
        {
            return ident.Type;
        }
    }
}
