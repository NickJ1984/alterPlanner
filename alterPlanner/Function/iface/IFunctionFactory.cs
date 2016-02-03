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
    /// Интерфейс фабрики функций зависимости
    /// </summary>
    public interface IFunctionFactory
    {
        /// <summary>
        /// Метод генерирующий анонимный метод проверки зависимости со значением направления <paramref name="direction"/> и динамической датой ограничения (датой ограничения является первый параметр делегата).
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>Анонимный метод проверки зависимости с направлением <paramref name="direction"/>, первый параметр - дата ограничения функции, второй параметр - проверяемая дата.</returns>
        Func<DateTime, DateTime, DateTime> CreateFunctionCheck(e_Direction direction);
        /// <summary>
        /// Метод генерирующий функцию зависимости со значением направления <paramref name="direction"/> и динамической датой ограничения <typeparamref name="date"/>.
        /// </summary>
        /// <param name="direction">Направление зависимости для генерируемой функции.</param>
        /// /// <param name="date">Дата ограничения зависимости для генерируемой функции.</param>
        /// <returns>Функция зависимости .</returns>
        IFunction CreateFunction(DateTime date, e_Direction direction);
    }
}
