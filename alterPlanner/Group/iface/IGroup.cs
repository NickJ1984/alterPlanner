using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;
using alter.types;
using alter.args;

namespace alter.Group.iface
{
    /// <summary>
    /// Интерфейс группы.
    /// </summary>
    public interface IGroup : IId, IRemovable, ILimit<e_GrpLim>, ILine, IName, IDock, IGroupable
    {
        /// <summary>
        /// Свойство возвращающее количество объектов принадлежащих группе.
        /// </summary>
        /// <returns>Количество объектов принадлежащих группе.</returns>
        int count { get; }
        /// <summary>
        /// Свойство возвращающее глубину вложенности группы (в другие группы), минимальная величина = 0. 
        /// </summary>
        /// <returns>Значение глубины вложенности группы.</returns>
        int enclosureCount { get; }
        /// <summary>
        /// Метод возвращающий группу владельца, если таковой не существует возвращает null
        /// </summary>
        /// <returns>Ссылка на группу владельца или null если группы владельца не существует</returns>
        IGroup GetGroupOwner();
        /// <summary>
        /// Метод получения зависимости группы
        /// </summary>
        /// <returns>Зависимость группы</returns>
        IDependence getGroupDepend();
        /// <summary>
        /// Проверка принадлежности объекта с уникальным идентификатором <paramref name="id"/> данной группе.
        /// </summary>
        /// <param name="id">Значение уникального идентификатора объекта.</param>
        /// <returns>Истина если объект принадлежит группе.</returns>
        bool InGroup(string id);
        /// <summary>
        /// Метод добавляющий объект <paramref name="newObject"/> в группу
        /// </summary>
        /// <param name="newObject"></param>
        /// <returns>Истина если объект был добавлен в группу.</returns>
        bool addInGroup(IGroupable newObject);
        /// <summary>
        /// Метод исключающий объект с уникалиьным идентификатором <paramref name="objectId"/> из группы.
        /// </summary>
        /// <param name="objectId">Уникалиьный идентификатор объекта являющегося членом данной группы.</param>
        /// <returns>Истина если объект был исключен из группы</returns>
        bool removeFromGroup(string objectId);
    }
}
