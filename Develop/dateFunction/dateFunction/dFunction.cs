using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dateFunction
{
    public class dFunction : ISender
    {
        #region Переменные
        protected static readonly DateTime initDate = new DateTime(1, 1, 1);
        protected static readonly e_direction initDirection = e_direction.Fixed;

        protected dfData data;
        protected object _sender;
        #endregion
        #region Свойства
        public virtual e_direction direction
        {
            get { return data.direction; }
            set
            {
                if (value == data.direction) return;

                e_direction temp = data.direction;
                data.direction = value;

                directionChangedPushEvent(temp, data.direction);
            }
        }
        public virtual DateTime date
        {
            get { return data.date; }
            set
            {
                if (value == data.date) return;

                DateTime temp = data.date;
                data.date = value;

                dateChangedPushEvent(temp, data.date);
            }
        }
        public virtual object sender
        {
            get { return _sender; }
            set
            {
                if (value == _sender) return;
                if (value == null) _sender = this;

                object temp = _sender;
                _sender = value;

                senderChangedPushEvent(temp, _sender);
            }
        }
        #endregion
        #region События
        public event EventHandler<valueChange<object>> event_senderChanged;
        public event EventHandler<valueChange<DateTime>> event_dateChanged;
        public event EventHandler<valueChange<e_direction>> event_directionChanged;
        #endregion
        #region Конструктор
        public dFunction()
        {
            data = new dfData()
            {
                date = initDate,
                direction = initDirection
            };

            _sender = this;
        }
        #endregion
        #region Методы запуска событий
        protected void directionChangedPushEvent(e_direction Old, e_direction New)
        {
            event_directionChanged?.Invoke(_sender, new valueChange<e_direction>(Old, New));
        }
        protected void dateChangedPushEvent(DateTime Old, DateTime New)
        {
            event_dateChanged?.Invoke(_sender, new valueChange<DateTime>(Old, New));
        }
        protected void senderChangedPushEvent(object Old, object New)
        {
            event_senderChanged?.Invoke(_sender, new valueChange<object>(Old, New));
        }
        #endregion
        #region Обработчики

        #endregion
        #region Методы
        
        #endregion
    }
}
