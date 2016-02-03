using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.Function.iface;
using alter.types;

namespace alter.Function.classes
{
    /// <summary>
    /// Структура передающая основные характеристики функции зависимости от даты
    /// </summary>
    public struct VectorF
    {
        /// <summary>
        /// Дата зависимости
        /// </summary>
        public DateTime Date;
        /// <summary>
        /// Направление зависимости
        /// </summary>
        public e_Direction Direction;
    }
    /// <summary>
    /// Класс реализующий временную зависимость (зависимость от даты)
    /// </summary>
    public class function : IFunction
    {
        #region vars
        /// <summary>
        /// Дата зависимости
        /// </summary>
        protected DateTime Date;
        /// <summary>
        /// Направление зависимости
        /// </summary>
        protected e_Direction Direction;
        /// <summary>
        /// Делегат реализующий механику зависимости
        /// </summary>
        protected Func<DateTime, DateTime> Function;
        #endregion
        #region props
        /// <summary>
        /// Свойство даты зависимости
        /// </summary>
        public virtual DateTime date
        {
            get { return Date; }
            set
            {
                if(Date != value)
                {
                    DateTime temp = Date;
                    Date = value;
                    OnDateChange(new ea_ValueChange<DateTime>(temp, Date));
                }
            }
        }
        /// <summary>
        /// Свойство направление зависимости
        /// </summary>
        public virtual e_Direction direction
        {
            get { return Direction; }
            set
            {
                if (Direction != value)
                {
                    e_Direction temp = Direction;
                    Direction = value;
                    Function = GetSvcFunction(Direction);
                    OnDirectionChange(new ea_ValueChange<e_Direction>(temp, Direction));
                }
            }
        }
        #endregion
        #region events
        /// <summary>
        /// Событие изменения даты зависимости
        /// </summary>
        public event EventHandler<ea_ValueChange<DateTime>> event_DateChanged;
        /// <summary>
        /// Событие изменения направления зависимости
        /// </summary>
        public event EventHandler<ea_ValueChange<e_Direction>> event_DirectionChanged;
        #endregion
        #region constructor
        /// <summary>
        /// Конструктор экземпляра класса зависимости от даты
        /// </summary>
        /// <param name="date">Дата зависимости</param>
        /// <param name="direction">Направление зависимости</param>
        public function(DateTime date, e_Direction direction)
        {
            this.direction = direction;
            this.date = date;
            Function = GetSvcFunction(Direction);
        }
        #endregion
        #region handlers
        /// <summary>
        /// Метод запуска события <seealso cref="event_DateChanged"/>
        /// </summary>
        /// <param name="args">Аргументы события</param>
        protected void OnDateChange(ea_ValueChange<DateTime> args)
        {
            EventHandler<ea_ValueChange<DateTime>> handler = event_DateChanged;
            if (handler != null) handler(this, args);
        }
        /// <summary>
        /// Метод запуска события <seealso cref="event_DirectionChanged"/>
        /// </summary>
        /// <param name="args">Аргументы события</param>
        protected void OnDirectionChange(ea_ValueChange<e_Direction> args)
        {
            EventHandler<ea_ValueChange<e_Direction>> handler = event_DirectionChanged;
            if (handler != null) handler(this, args);
        }
        #endregion
        #region methods
        /// <summary>
        /// Метод реализующий механику зависимости
        /// </summary>
        /// <param name="date">Проверяемая дата</param>
        /// <returns></returns>
        public DateTime CheckDate(DateTime date)
        {
            return Function(Date);
        }
        /// <summary>
        /// Метод получения нового делегата реализующего механику зависимости с направлением заданным параметром метода.
        /// </summary>
        /// <param name="direction">Направление зависимости получаемого делегата</param>
        /// <returns>Делегат реализующий механику зависимости </returns>
        public Func<DateTime, DateTime, DateTime> GetFunctionDir(e_Direction direction)
        {
            switch (direction)
            {
                //case eDirection.LeftMax:
                //case eDirection.RightMax:
                case e_Direction.Fixed:
                    return new Func<DateTime, DateTime, DateTime>(
                        (limit, date) => date = limit);

                case e_Direction.Left:
                    return new Func<DateTime, DateTime, DateTime>(
                    (limit, date) => (date > limit) ? limit : date);

                case e_Direction.Right:
                    return new Func<DateTime, DateTime, DateTime>(
                    (limit, date) => (date < limit) ? limit : date);
                default:
                    throw new Exception("getSvcFunction: wrong direction value");
            }
        }

        /// <summary>
        /// Метод получения даты зависимости
        /// </summary>
        /// <returns>Дата зависимости</returns>
        public virtual DateTime GetDate()
        { return date; }
        /// <summary>
        /// Метод установки даты зависимости
        /// </summary>
        /// <param name="date">Дата зависимости</param>
        public void SetDate(DateTime date)
        { this.date = date; }
        /// <summary>
        /// Метод получения направления зависимости
        /// </summary>
        /// <returns>Направление зависимости</returns>
        public virtual e_Direction GetDirection()
        { return direction; }
        /// <summary>
        /// Метод получения направления зависимости
        /// </summary>
        /// <param name="direction">Направление зависимости</param>
        public void SetDirection(e_Direction direction)
        { this.direction = direction; }
        #endregion
        #region service
        /// <summary>
        /// Получение делегата реализации механизма зависимости основывающееся на внутренней дате зависимости
        /// </summary>
        /// <param name="direction">Направление зависимости</param>
        /// <returns>Делегат реализации механизма зависимости основывающееся на внутренней дате зависимости</returns>
        protected Func<DateTime, DateTime> GetSvcFunction(e_Direction direction)
        {
            switch(direction)
            {
                //case eDirection.LeftMax:
                //case eDirection.RightMax:
                case e_Direction.Fixed:
                    return new Func<DateTime, DateTime>((Date) => this.Date);

                case e_Direction.Left:
                    return new Func<DateTime, DateTime>(
                    (Date) => (Date > this.Date) ? this.Date : Date);

                case e_Direction.Right:
                    return new Func<DateTime, DateTime>(
                    (Date) => (Date < this.Date) ? this.Date : Date);
                default:
                    throw new Exception("getSvcFunction: wrong direction value");
            }   

        }
        #endregion
    }
}
