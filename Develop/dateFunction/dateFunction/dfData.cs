using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dateFunction
{
    public struct dfData
    {
        #region Переменные
        public e_direction direction;
        public DateTime date;
        #endregion
        #region Конструктор
        public dfData(DateTime date, e_direction direction)
        {
            this.date = date;
            this.direction = direction;
        } 
        #endregion
        #region Методы
        public bool inRange(dfData data)
        {
            return inRangeST(this, data);
        }
        public bool isIntersect(dfData data)
        {
            return isIntersectST(this, data);
        }
        public KeyValuePair<DateTime, DateTime>? getIntersection(dfData data)
        {
            return getIntersectionST(this, data);
        }
        #endregion
        #region Статические методы
        public static KeyValuePair<DateTime, DateTime>? getIntersectionST(dfData data1, dfData data2)
        {
            if (isIntersectST(data1, data2))
            {
                if (data1.direction == e_direction.Fixed)
                    return new KeyValuePair<DateTime, DateTime>(data1.date, data1.date);
                else if (data2.direction == e_direction.Fixed)
                    return new KeyValuePair<DateTime, DateTime>(data2.date, data2.date);
                else if (data1 > data2)
                    return new KeyValuePair<DateTime, DateTime>(data2.date, data1.date);
                else if (data1 < data2)
                    return new KeyValuePair<DateTime, DateTime>(data1.date, data2.date);
                else return null;
            }
            else return null;
        }
        public static bool inRangeST(dfData instance, dfData data)
        {
            switch(instance.direction)
            {
                case e_direction.Fixed:
                    return data.date == instance.date && data.direction == e_direction.Fixed
                           ? true : false;

                case e_direction.Left:
                    return data.date <= instance.date && (data.direction == e_direction.Fixed ||
                           data.direction == e_direction.Left)
                           ? true : false;

                case e_direction.Right:
                    return data.date >= instance.date && (data.direction == e_direction.Fixed ||
                           data.direction == e_direction.Right)
                           ? true : false;
                default:
                    throw new ArgumentException(nameof(inRange));
            }
        }
        public static bool isIntersectST(dfData data1, dfData data2)
        {
            if (data1.date == data2.date) return true;

            switch(data1.direction)
            {
                case e_direction.Fixed:
                    return (data2.date > data1.date && data2.direction == e_direction.Left) ||
                           (data2.date < data1.date && data2.direction == e_direction.Right)
                           ? true : false;

                case e_direction.Left:
                    return (data2.date > data1.date && data2.direction == e_direction.Right) ||
                           (data2.date < data1.date && data2.direction == e_direction.Left)
                           ? true : false;

                case e_direction.Right:
                    return (data2.date > data1.date && data2.direction == e_direction.Left) ||
                           (data2.date < data1.date && data2.direction == e_direction.Right)
                           ? true : false;

                default:
                    throw new ApplicationException(nameof(isIntersect));
            }
        }
        #endregion
        #region Перегрузка
        #region Операторы
        #region Математические
        public static dfData operator +(dfData data1, dfData data2)
        {
            throw new NotImplementedException();
        }
        public static dfData operator -(dfData data1, dfData data2)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Логические
        public static bool operator >=(dfData data1, dfData data2)
        {
            return (data1.date >= data2.date && inRangeST(data1, data2))
                   ? true : false;
        }
        public static bool operator <=(dfData data1, dfData data2)
        {
            return (data1.date <= data2.date && inRangeST(data2, data1))
                   ? true : false;
        }
        public static bool operator >(dfData data1, dfData data2)
        {
            return (data1.date > data2.date && inRangeST(data1, data2))
                   ? true : false;
        }
        public static bool operator <(dfData data1, dfData data2)
        {
            return (data1.date < data2.date && inRangeST(data2, data1))
                   ? true : false;
        }
        public static bool operator ==(dfData data1, dfData data2)
        {
            return data1.Equals(data2);
        }
        public static bool operator !=(dfData data1, dfData data2)
        {
            return !data1.Equals(data2);
        }
        #endregion
        #endregion
        #region Методы
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(dfData)) throw new ArgumentException(nameof(obj));

            dfData dfd = (dfData)obj;

            return (dfd.date == date && dfd.direction == direction) ?
                    true : false;
        }
        public override int GetHashCode()
        {
            return 13 * direction.GetHashCode() * date.GetHashCode();
        }
        public override string ToString()
        {
            string sign = direction == e_direction.Fixed ? "==" :
                                        direction == e_direction.Left ?
                                            "<" : ">";
            return string.Format("Value {0} {1}", sign, date);
        }
        #endregion
        #endregion
    }
}
