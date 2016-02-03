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
    public enum e_Direction
    {
        
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
        
        //RightMax = 2
    }
    /// <summary>
    /// Внутреннее ограничение объекта задачи
    /// </summary>
    public enum e_TLLim
    {
        /// <summary>
        /// Финиш не ранее
        /// </summary>
        FinishNotEarlier = 3,
        /// <summary>
        /// Старт не ранее
        /// </summary>
        StartNotEarlier = 4,
        /// <summary>
        /// Старт не позднее
        /// </summary>
        StartNotLater = 5,
        /// <summary>
        /// Финиш не позднее
        /// </summary>
        FinishNotLater = 6,
        /// <summary>
        /// Старт фиксирован
        /// </summary>
        StartFixed = 7,
        /// <summary>
        /// Финиш фиксирован
        /// </summary>
        FinishFixed = 8,
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
    public enum e_TskLim
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
    /// Элементы составляющие перечисление <see cref="e_TskLim"/>,
    /// необходимо для утилитарных функций. Данный тип имеет свойство флагов.
    /// </summary>
    [Flags]
    public enum e_TskLimChunk
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
    public enum e_GrpLim
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
    public enum e_PrjLim
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
    /*
    /// <summary>
    /// Ограничения используемые в объектах связей
    /// </summary>
    public enum e_LnkLim
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
    }*/
    #endregion

    #region General
    /// <summary>
    /// Основные типы объектов библиотеки alterPlanner
    /// </summary>
    public enum e_Entity
    {
        /// <summary>
        /// Тип отсутствует
        /// </summary>
        None = 0,
        /// <summary>
        /// Проект
        /// </summary>
        Project = 1,
        /// <summary>
        /// Фабрика
        /// </summary>
        Factory = 2,
        /// <summary>
        /// Группа
        /// </summary>
        Group = 3,
        /// <summary>
        /// Связь
        /// </summary>
        Link = 4,
        /// <summary>
        /// Задача
        /// </summary>
        Task = 5
    }
    #endregion

    #region Task

    #endregion

    #region Links
    /// <summary>
    /// Зависимость объекта учавствующего в связи
    /// </summary>
    public enum e_DependType
    {
        /// <summary>
        /// Управляющий объект
        /// </summary>
        Master = 1,
        /// <summary>
        /// Подчинённый объект
        /// </summary>
        Slave = 2
    }
    /// <summary>
    /// Состояние связи, показывает следует ли подчинённый объект ограничению связи
    /// </summary>
    public enum e_LnkState
    {
        /// <summary>
        /// Ограничение связи соблюдается
        /// </summary>
        InTime = 0,
        /// <summary>
        /// Дата подчинённого объекта позднее даты ограничения
        /// </summary>
        Later = 1,
        /// <summary>
        /// Дата подчинённого объекта раньше даты ограничения
        /// </summary>
        Early = 2
    }

    /// <summary>
    /// Тип связи
    /// </summary>
    public enum e_LnkType
    {
        /// <summary>
        /// Зависимость типа предшественник - последователь
        /// </summary>
        Depend = 1,
        /// <summary>
        /// Подчинённый объект является членом группы (управляющий объект)
        /// </summary>
        Group = 2
    }
    /// <summary>
    /// Тип точки
    /// </summary>
    public enum e_Dot
    {
        /// <summary>
        /// Начальная точка
        /// </summary>
        Start = 1,
        /// <summary>
        /// Конечная точка
        /// </summary>
        Finish = 2
    }
    #endregion
}
