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
        int groupCount();
        
        /// <summary>
        /// Создать экземпляр группы с именем <paramref name="name"/>, датой ограничения <paramref name="date"/> и типом ограничения <paramref name="limit"/>.
        /// </summary>
        /// <param name="name">Имя новой группы.</param>
        /// <param name="date">Дата ограничения новой группы.</param>
        /// <param name="limit">Тип ограничения новой группы.</param>
        /// <returns>Ссылка на созданный экземпляр группы.</returns>
        IGroup createGroup(string name, DateTime date, eGrpLim limit);
        /// <summary>
        /// Получить ссылку на группу с уникальным идентификатором <paramref name="IDgroup"/>.
        /// </summary>
        /// <param name="IDgroup">Уникальный идентификатор экземпляра группы.</param>
        /// <returns>Ссылка на экземпляр группы.</returns>
        IGroup getGroup(string IDgroup);
    }
}
