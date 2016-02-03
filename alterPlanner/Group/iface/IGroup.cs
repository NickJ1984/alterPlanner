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
    public interface IGroup : IId, IRemovable, ILimit<e_GrpLim>, ILine, IName, IDock
    {
        /// <summary>
        /// Получить количество задач принадлежащих группе.
        /// </summary>
        /// <returns>Количество задач принадлежащих группе.</returns>
        int TaskCount();

        /// <summary>
        /// Получить количество групп принадлежащих группе.
        /// </summary>
        /// <returns>Количество групп принадлежащих группе.</returns>
        int GroupCount();

        /// <summary>
        /// Получить глубину вложенности группы (в другие группы), минимальная величина = 0. 
        /// </summary>
        /// <returns>Значение глубины вложенности группы.</returns>
        int EnclosureCount();
        /// <summary>
        /// При значении вложенности больше 0, выдает идентификатор группы владельца.
        /// </summary>
        /// <returns>Ссылка на идентификатор владельца.</returns>
        IId GetGroupOwner(); 

        /// <summary>
        /// Проверка принадлежности объекта с уникальным идентификатором <paramref name="id"/> данной группе.
        /// </summary>
        /// <param name="id">Значение уникального идентификатора объекта.</param>
        /// <returns>Истина если объект принадлежит группе.</returns>
        bool InGroup(string id);
        /// <summary>
        /// Добавить объект <paramref name="newObject"/> в группу (корректные типы объектов: Группа, Задача).
        /// </summary>
        /// <param name="newObject"></param>
        /// <returns>Истина если объект был добавлен в группу.</returns>
        bool AddInGroup(IDock newObject);
        /// <summary>
        /// Исключить объект с уникалиьным идентификатором <paramref name="objectId"/> из группы.
        /// </summary>
        /// <param name="objectId">Уникалиьный идентификатор объекта являющегося членом данной группы.</param>
        /// <returns>Истина если объект был исключен из группы.</returns>
        bool DelFromGroup(string objectId);
    }
}
