using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;
using alter.types;
using alter.args;

namespace alter.Link.iface
{
    /// <summary>
    /// Интерфейс класса информации о члене связи
    /// </summary>
    public interface ILMember
    {
        /// <summary>
        /// Метод получения идентификатора члена связи
        /// </summary>
        /// <returns>Ссылка на идентификатор члена связи</returns>
        IId GetMemberId();
        /// <summary>
        /// Метод получения вида зависимости связи для данного члена
        /// </summary>
        /// <returns>Вид зависимости связи для данного члена</returns>
        e_DependType GetDependType();
        /// <summary>
        /// Метод получения зависимой точки данного члена
        /// </summary>
        /// <returns>Ссылка на зависимую точку данного члена</returns>
        IDot getObjectDependDotInfo();
    }
}
