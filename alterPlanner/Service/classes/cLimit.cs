using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.iface;

namespace alter.Service.classes
{
    public class cLimit<T> : ILimit<T> where T : struct, IConvertible
    {
        #region Переменные
        protected T _value;
        #endregion
        #region Свойства
        public object sender { get; set; }
        public T limit
        {
            get { return _value; }
            set
            {
                if (_value.Equals(value)) return;

                T temp = _value;
                _value = value;

                event_LimitChanged?.Invoke(sender == null ? this : sender, new ea_ValueChange<T>(temp, _value));
            }
        }
        #endregion
        #region События
        public event EventHandler<ea_ValueChange<T>> event_LimitChanged;
        #endregion
        #region Конструктор
        public cLimit(object sender, T value)
        {
            _value = value;
            this.sender = sender;
        }
        public cLimit(object sender)
            : this(sender, default(T))
        { }
        #endregion
        #region Методы
        public T GetLimit()
        {
            return _value;
        }
        public bool SetLimit(T limitType)
        {
            if (_value.Equals(limitType)) return false;

            limit = limitType;

            return true;
        }
        #endregion
        #region Перегрузка
        public static implicit operator T(cLimit<T> instance)
        {
            return instance.limit;
        }
        #endregion
    }
}
