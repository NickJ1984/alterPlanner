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
        protected dFunction _binded;
        #endregion
        #region Свойства
        public virtual dFunction binded => _binded;
        public virtual bool isBinded => _binded != null && data.isDiapason(_binded.data);
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
        public event EventHandler<valueChange<dFunction>> event_bindedChanged;
        #endregion
        #region Конструктор
        protected dFunction()
            :this(initDate, initDirection)
        { }
        public dFunction(DateTime date, e_direction direction)
        {
            if (!Enum.IsDefined(typeof(e_direction), direction)) throw new ArgumentException(nameof(direction));

            data = new dfData(date, direction);            

            _sender = this;
            _binded = null;
        }

        ~dFunction()
        {
            _sender = null;
            _binded = null;
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
        protected void bindedChangedPushEvent(dFunction Old, dFunction New)
        {
            event_bindedChanged?.Invoke(_sender, new valueChange<dFunction>(Old, New));
        }
        #endregion
        #region Обработчики

        #endregion
        #region Методы
        public void setDiapason(dFunction function)
        {
            if (function == _binded) return;

            dFunction temp = _binded;
            _binded = function;

            bindedChangedPushEvent(temp, _binded);
        }
        public DateTime check(DateTime date)
        {
            DateTime result = date;

            if (isBinded) result = data.check(_binded.check(result));
            else result = data.check(result);

            return result;
        }
        #endregion
        #region Перегрузки
        #region Операторы
        #region Математические
        public static dFunction operator +(dFunction dfunc1, dFunction dfunc2)
        {
            dFunction result = new dFunction();
            result.data = dfunc1.data + dfunc2.data;

            return result;
        }
        public static dFunction operator -(dFunction dfunc1, dFunction dfunc2)
        {
            dFunction result = new dFunction();
            result.data = dfunc1.data - dfunc2.data;

            return result;
        }
        #endregion
        #region Логические

        public static bool operator ==(dFunction dfunc1, dFunction dfunc2)
        {
            object Obj = dfunc1;
            object Obj2 = dfunc2;

            if (Obj2 == null && Obj == null) return true;
            else if (Obj2 == null && Obj != null) return false;
            else if (Obj == null && Obj2 != null) return false;

            return dfunc1.data == dfunc2.data; 
        }

        public static bool operator !=(dFunction dfunc1, dFunction dfunc2)
        {
            object Obj = dfunc1;
            object Obj2 = dfunc2;

            if (Obj2 == null && Obj != null) return true;
            else if (Obj2 == null && Obj == null) return false;
            else if (Obj == null && Obj2 != null) return true;

            return dfunc1.data != dfunc2.data;
        }
        public static bool operator >(dFunction dfunc1, dFunction dfunc2)
        { return dfunc1.data > dfunc2.data; }
        public static bool operator <(dFunction dfunc1, dFunction dfunc2)
        { return dfunc1.data < dfunc2.data; }
        public static bool operator >=(dFunction dfunc1, dFunction dfunc2)
        { return dfunc1.data >= dfunc2.data; }
        public static bool operator <=(dFunction dfunc1, dFunction dfunc2)
        { return dfunc1.data <= dfunc2.data; }
        #endregion
        #endregion
        #region Методы
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(dFunction)) throw new ArgumentException("Неверный тип аргумента");

            return data.Equals(((dFunction)obj).data);
        }
        public override int GetHashCode()
        {
            return 17 * data.GetHashCode() * sender.GetHashCode();
        }
        public override string ToString()
        {
            return data.ToString();
        }
        #endregion
        #endregion
    }
}
