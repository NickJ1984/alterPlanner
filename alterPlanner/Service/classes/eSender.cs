using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.iface;

namespace alter.Service.classes
{
    public class eSender : ISender
    {
        #region Переменные
        protected readonly object _owner;
        protected object _sender;
        #endregion
        #region Свойства
        public object owner => _owner;
        public virtual object sender
        {
            get { return _sender; }
            set
            {
                if (_sender == value) return;

                object temp = _sender;
                if (value == null) _sender = _owner;
                else _sender = value;

                senderChangedPushEvent(temp, _sender);
            }
        }
        #endregion
        #region События
        public event EventHandler<ea_ValueChange<object>> event_senderChanged;
        #endregion
        #region Конструктор
        public eSender(object owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));

            _owner = owner;
            _sender = owner;
        }
        ~eSender()
        {
            _sender = null;
        }
        #endregion
        #region Методы запуска событий
        protected virtual void senderChangedPushEvent(object Old, object New)
        {
            event_senderChanged?.Invoke(Old, new ea_ValueChange<object>(Old, New));
        }
        #endregion
    }
}
