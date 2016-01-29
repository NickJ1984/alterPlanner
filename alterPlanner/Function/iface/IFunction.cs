﻿using System;
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
    public interface IFNCInfo
    {
        /// <summary>
        /// Получить направление функции.
        /// </summary>
        /// <returns>Направление функции.</returns>
        eDirection getDirection();
        /// <summary>
        /// Получить дату ограничения функции.
        /// </summary>
        /// <returns>Дата ограничения функции.</returns>
        DateTime getDate();

        /// <summary>
        /// Метод проверки даты <paramref name="Date"/> на ограничение функции.
        /// </summary>
        /// <param name="Date">Результат проверки на ограничение функции.</param>
        /// <returns></returns>
        DateTime checkDate(DateTime Date);

        /// <summary>
        /// Событие срабатывающее при изменении направления функции.
        /// </summary>
        event EventHandler<EA_valueChange<eDirection>> event_directionChanged;
        /// <summary>
        /// Событие срабатывающее при изменении даты ограничения функции.
        /// </summary>
        event EventHandler<EA_valueChange<DateTime>> event_dateChanged;
    }
    /// <summary>
    /// Функция зависимости.
    /// </summary>
    public interface IFunction : IFNCInfo
    {
        /// <summary>
        /// Задать направление функции.
        /// </summary>
        /// <param name="direction">Значение направления <see cref="eDirection"/>.</param>
        void setDirection(eDirection direction);
        /// <summary>
        /// Задать дату ограничения функции.
        /// </summary>
        /// <param name="Date">Дата ограничения.</param>
        void setDate(DateTime Date);

        /// <summary>
        /// Метод генерирующий анонимный метод проверки зависимости со значением направления <paramref name="direction"/> и динамической датой ограничения (датой ограничения является первый параметр делегата).
        /// </summary>
        /// <param name="direction">Направление функции зависимости для генерируемого метода.</param>
        /// <returns>Анонимный метод проверки зависимости с направлением <paramref name="direction"/>, первый параметр - дата ограничения функции, второй параметр - проверяемая дата.</returns>
        Func<DateTime, DateTime, DateTime> getFunctionDir(eDirection direction);
    }
}
