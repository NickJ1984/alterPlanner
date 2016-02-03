using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.iface;
using alter.Task.iface;
using alter.Link.iface;
using alter.Function.iface;
using alter.Group.iface;
using alter.types;

namespace alter.Project.iface
{
    /// <summary>
    /// Интерфейс класса проекта.
    /// </summary>
    public interface IProject : IId, IRemovable, ILine, IName, ILimit<e_PrjLim>
    {
        /// <summary>
        /// Получить ссылку на экземпляр информации о функции зависимости проекта.
        /// </summary>
        /// <returns>Ссылка на экземпляр информации о функции зависимости проекта.</returns>
        IFncInfo GetCheckFunction();
        /// <summary>
        /// Установить дату ограничения зависимости проекта.
        /// </summary>
        /// <param name="date">Дата ограничения зависимости проекта.</param>
        void SetLimitDate(DateTime date);

        /// <summary>
        /// Получить ссылку на экземпляр фабрики связей.
        /// </summary>
        /// <returns>Ссылка на фабрику связей.</returns>
        ILinkFactory GetLinkFactory();
        /// <summary>
        /// Получить ссылку на экземпляр фабрики задач.
        /// </summary>
        /// <returns>Ссылка на фабрику задач.</returns>
        ITaskFactory GetTaskFactory();
        /// <summary>
        /// Получить ссылку на экземпляр фабрики групп.
        /// </summary>
        /// <returns>Ссылка на фабрику групп.</returns>
        IGroupFactory GetGroupFactory();
        /// <summary>
        /// Получить ссылку на экземпляр фабрики функций.
        /// </summary>
        /// <returns>Ссылка на фабрику функций.</returns>
        IFunctionFactory GetFunctionFactory();
    }
}
