
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.classes;
using alter.iface;
using alter.Link.iface;
using alter.types;

namespace alterTesting.Emulators
{
    public class lineClass : IDock
    {
        protected alter.classes.Identity ident;
        protected dotCLass _start;
        protected dotCLass _finish;
        protected Dependence dcDepend = null;
        public event EventHandler<ea_ValueChange<double>> event_DurationChanged;
        public event EventHandler<ea_IdObject> event_ObjectDeleted;

        public double duration
        {
            get { return GetDuration(); }
            set
            {
                if (value >= 0)
                {
                    _finish.date = start.AddDays(value);
                }
            }
        }
        public DateTime start
        {
            get
            {
                if (dcDepend != null)
                {
                    if (dcDepend.dependDot == e_Dot.Start) return dcDepend.date;
                    else return finish.AddDays(-duration);
                }
                return _start.date;
            }
            set { _start.date = value; }
        }
        public DateTime finish
        {
            get
            {
                if (dcDepend != null)
                {
                    if (dcDepend.dependDot == e_Dot.Finish) return dcDepend.date;
                    else return start.AddDays(duration);
                }
                return _finish.date;
            }
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

        public bool connect(ILink link)
        {
            return true;
        }

        public void setDependence(Dependence depend)
        {
            dcDepend = depend;
        }
        public void DeleteObject()
        {
            throw new NotImplementedException();
        }    
    }
}
