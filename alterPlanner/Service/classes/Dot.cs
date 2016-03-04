using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.iface;
using alter.types;

namespace alter.Service.classes
{
    public class Dot : IDot
    {
        #region Переменные
        public static readonly DateTime initDate = new DateTime(1, 1, 1);
        public readonly object sender;
        public readonly e_Dot type;
        protected DateTime _date; 
        #endregion
        #region Свойства
        public virtual DateTime date
        {
            get { return _date; }
            set
            {
                if (value != _date)
                {
                    DateTime temp = _date;
                    _date = value;
                    event_DateChanged?.Invoke(sender, new ea_ValueChange<DateTime>(temp, _date));
                }
            }
        } 
        #endregion
        #region События
        public event EventHandler<ea_ValueChange<DateTime>> event_DateChanged;
        #endregion
        #region Конструктор
        public Dot(e_Dot type, object eventSenderObject)
        {
            if (!Enum.IsDefined(typeof(e_Dot), type)) throw new ArgumentException("Неверное значение аргумента");

            this.type = type;
            _date = initDate;
            sender = eventSenderObject == null ? this : eventSenderObject;
        } 
        public Dot(e_Dot type)
            :this(type, null)
        { }
        #endregion
        #region Методы
        public DateTime GetDate()
        {
            return date;
        }
        public e_Dot GetDotType()
        {
            return type;
        }
        #endregion
        #region Методы запуска события
        protected void dateChangedPushEvent(ea_ValueChange<DateTime> e)
        {
            if(e == null) throw new NullReferenceException(nameof(e));

            event_DateChanged?.Invoke(sender, e);            
        }
        #endregion
    }
}
