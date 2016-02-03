using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;
using alter.types;


namespace alter.Group.iface
{
    /// <summary>
    /// Интерфейс фабрики классов групп
    /// </summary>
    public interface IGroupFactory  : IId, IChild
    {
        /// <summary>
        /// Получить количество существующих экземпляров групп.
        /// </summary>
        /// <returns>Количество существующих экземпляров групп.</returns>
        int GroupCount();
        
        /// <summary>
        /// Создать экземпляр группы с именем <paramref name="name"/>, датой ограничения <paramref name="date"/> и типом ограничения <paramref name="limit"/>.
        /// </summary>
        /// <param name="name">Имя новой группы.</param>
        /// <param name="date">Дата ограничения новой группы.</param>
        /// <param name="limit">Тип ограничения новой группы.</param>
        /// <returns>Ссылка на созданный экземпляр группы.</returns>
        IGroup CreateGroup(string name, DateTime date, e_GrpLim limit);
        /// <summary>
        /// Получить ссылку на группу с уникальным идентификатором <paramref name="groupID"/>.
        /// </summary>
        /// <param name="groupID">Уникальный идентификатор экземпляра группы.</param>
        /// <returns>Ссылка на экземпляр группы.</returns>
        IGroup GetGroup(string groupID);
    }
}
