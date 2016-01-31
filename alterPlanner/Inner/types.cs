using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace alter.types
{
    #region Limits
    /// <summary>
    /// Характеризует направление в котором распространяется функция
    /// </summary>
    public enum eDirection
    {
        /// <summary>
        /// Тип "Как можно раньше"
        /// </summary>
        //LeftMax = -2,
        /// <summary>
        /// Тип "Не позднее"
        /// </summary>
        Left = -1,
        /// <summary>
        /// Фиксировано на определенной дате
        /// </summary>
        Fixed = 0,
        /// <summary>
        /// Тип "Не ранее"
        /// </summary>
        Right = 1,
        /// <summary>
        /// Тип "Как можно позже"
        /// </summary>
        //RightMax = 2
    }
    /// <summary>
    /// Внутреннее ограничение объекта задачи
    /// </summary>
    public enum eTLLim
    {
        /// <summary>
        /// Финиш не ранее
        /// </summary>
        finishNotEarlier = 3,
        /// <summary>
        /// Старт не ранее
        /// </summary>
        startNotEarlier = 4,
        /// <summary>
        /// Старт не позднее
        /// </summary>
        startNotLater = 5,
        /// <summary>
        /// Финиш не позднее
        /// </summary>
        finishNotLater = 6,
        /// <summary>
        /// Старт фиксирован
        /// </summary>
        startFixed = 7,
        /// <summary>
        /// Финиш фиксирован
        /// </summary>
        finishFixed = 8,
        /// <summary>
        /// Как можно раньше
        /// </summary>
        Earlier = 30,
        /// <summary>
        /// Как можно позже
        /// </summary>
        Later = 31
    }
    /// <summary>
    /// Типы ограничения, при типе связи "предшественник-последователь"
    /// </summary>
    public enum eTskLim
    {
        /// <summary>
        /// старт-старт
        /// </summary>
        StartStart = 10,
        /// <summary>
        /// финиш-финиш
        /// </summary>
        FinishStart = 12,
        /// <summary>
        /// финиш-старт
        /// </summary>
        StartFinish = 18,
        /// <summary>
        /// финиш-финиш
        /// </summary>
        FinishFinish = 20
    }
    /// <summary>
    /// Элементы составляющие перечисление <see cref="eTskLim"/>,
    /// необходимо для утилитарных функций. Данный тип имеет свойство флагов.
    /// </summary>
    [Flags]
    public enum eTskLimChunk
    {
        /// <summary>
        /// Старт - значение предшественника
        /// </summary>
        Start_ = 2,
        /// <summary>
        /// Финиш - значение предшественника
        /// </summary>
        Finish_ = 4,
        /// <summary>
        /// Старт - значение последователя
        /// </summary>
        _Start = 8,
        /// <summary>
        /// Финиш - значение последователя
        /// </summary>
        _Finish = 16
    }
    /// <summary>
    /// Ограничения используемые в объектах групп
    /// </summary>
    public enum eGrpLim
    {
        /// <summary>
        /// Как можно раньше
        /// </summary>
        Earlier = 30,
        /// <summary>
        /// Как можно позже
        /// </summary>
        Later = 31,
        /// <summary>
        /// Не ранее
        /// </summary>
        NotEarlier = 32,
        /// <summary>
        /// Не позднее
        /// </summary>
        NotLater = 33
    }
    /// <summary>
    /// Ограничения используемые в объекте проекта
    /// </summary>
    public enum ePrjLim
    {
        /// <summary>
        /// Как можно раньше
        /// </summary>
        Earlier = 30,
        /// <summary>
        /// Как можно позже
        /// </summary>
        Later = 31
    }
    /// <summary>
    /// Ограничения используемые в объектах связей
    /// </summary>
    public enum eLnkLim
    {
        /// <summary>
        /// старт-старт
        /// </summary>
        StartStart = 10,
        /// <summary>
        /// финиш-финиш
        /// </summary>
        FinishStart = 12,
        /// <summary>
        /// финиш-старт
        /// </summary>
        StartFinish = 18,
        /// <summary>
        /// финиш-финиш
        /// </summary>
        FinishFinish = 20,
        /// <summary>
        /// Как можно раньше
        /// </summary>
        Earlier = 30,
        /// <summary>
        /// Как можно позже
        /// </summary>
        Later = 31,
        /// <summary>
        /// Не ранее
        /// </summary>
        NotEarlier = 32,
        /// <summary>
        /// Не позднее
        /// </summary>
        NotLater = 33
    }
    #endregion

    #region General
    /// <summary>
    /// Основные типы объектов библиотеки alterPlanner
    /// </summary>
    public enum eEntity
    {
        /// <summary>
        /// Тип отсутствует
        /// </summary>
        none = 0,
        /// <summary>
        /// Проект
        /// </summary>
        project = 1,
        /// <summary>
        /// Фабрика
        /// </summary>
        factory = 2,
        /// <summary>
        /// Группа
        /// </summary>
        group = 3,
        /// <summary>
        /// Связь
        /// </summary>
        link = 4,
        /// <summary>
        /// Задача
        /// </summary>
        task = 5
    }
    #endregion

    #region Task

    #endregion

    #region Links
    /// <summary>
    /// Зависимость объекта учавствующего в связи
    /// </summary>
    public enum eDependType
    {
        /// <summary>
        /// Управляющий объект
        /// </summary>
        master = 1,
        /// <summary>
        /// Подчинённый объект
        /// </summary>
        slave = 2
    }
    /// <summary>
    /// Состояние связи, показывает следует ли подчинённый объект ограничению связи
    /// </summary>
    public enum eLnkState
    {
        /// <summary>
        /// Ограничение связи соблюдается
        /// </summary>
        inTime = 0,
        /// <summary>
        /// Дата подчинённого объекта позднее даты ограничения
        /// </summary>
        later = 1,
        /// <summary>
        /// Дата подчинённого объекта раньше даты ограничения
        /// </summary>
        early = 2
    }

    /// <summary>
    /// Тип связи
    /// </summary>
    public enum eLnkType
    {
        /// <summary>
        /// Зависимость типа предшественник - последователь
        /// </summary>
        depend = 1,
        /// <summary>
        /// Подчинённый объект является членом группы (управляющий объект)
        /// </summary>
        group = 2
    }
    /// <summary>
    /// Тип точки
    /// </summary>
    public enum eDot
    {
        /// <summary>
        /// Начальная точка
        /// </summary>
        start = 1,
        /// <summary>
        /// Конечная точка
        /// </summary>
        finish = 2
    }
    #endregion
}
