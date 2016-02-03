using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.types;
using alter.args;
using alter.iface;

namespace alter.Function.iface
{
    /// <summary>
    /// Информация о функции зависимости.
    /// </summary>
    public interface IFncInfo
    {
        /// <summary>
        /// Получить направление функции.
        /// </summary>
        /// <returns>Направление функции.</returns>
        e_Direction GetDirection();
        /// <summary>
        /// Получить дату ограничения функции.
        /// </summary>
        /// <returns>Дата ограничения функции.</returns>
        DateTime GetDate();

        /// <summary>
        /// Метод проверки даты <paramref name="date"/> на ограничение функции.
        /// </summary>
        /// <param name="date">Результат проверки на ограничение функции.</param>
        /// <returns></returns>
        DateTime CheckDate(DateTime date);

        /// <summary>
        /// Событие срабатывающее при изменении направления функции.
        /// </summary>
        event EventHandler<ea_ValueChange<e_Direction>> event_DirectionChanged;
        /// <summary>
        /// Событие срабатывающее при изменении даты ограничения функции.
        /// </summary>
        event EventHandler<ea_ValueChange<DateTime>> event_DateChanged;
    }
    /// <summary>
    /// Функция зависимости.
    /// </summary>
    public interface IFunction : IFncInfo
    {
        /// <summary>
        /// Задать направление функции.
        /// </summary>
        /// <param name="direction">Значение направления <see cref="e_Direction"/>.</param>
        void SetDirection(e_Direction direction);
        /// <summary>
        /// Задать дату ограничения функции.
        /// </summary>
        /// <param name="date">Дата ограничения.</param>
        void SetDate(DateTime date);

        /// <summary>
        /// Метод генерирующий анонимный метод проверки зависимости со значением направления <paramref name="direction"/> и динамической датой ограничения (датой ограничения является первый параметр делегата).
        /// </summary>
        /// <param name="direction">Направление функции зависимости для генерируемого метода.</param>
        /// <returns>Анонимный метод проверки зависимости с направлением <paramref name="direction"/>, первый параметр - дата ограничения функции, второй параметр - проверяемая дата.</returns>
        Func<DateTime, DateTime, DateTime> GetFunctionDir(e_Direction direction);
    }
}
