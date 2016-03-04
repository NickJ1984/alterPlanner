using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.iface;
using alter.args;
using alter.types;
using alter.Function.iface;

namespace alter.Link.iface
{
    /// <summary>
    /// Интерфейс класса связи
    /// </summary>
    public interface ILink : IId, IRemovable, ILimit<e_TskLim>
    {
        /// <summary>
        /// Метод получения значения состояния связи
        /// </summary>
        /// <returns>Значение состояния связи</returns>
        e_LnkState GetLinkState();
        /// <summary>
        /// Метод установки значения задержки связи
        /// </summary>
        /// <param name="days">Задержка указывается в днях (может иметь отрицательное значение)</param>
        /// <returns>Истина если новое значение установлено</returns>
        bool SetDelay(double days);
        /// <summary>
        /// Метод получения значения задержки связи в днях
        /// </summary>
        /// <returns>Значение задержки связи в днях</returns>
        double GetDelay();
        /// <summary>
        /// Событие изменения значения задержки связи
        /// </summary>
        event EventHandler<ea_ValueChange<double>> event_DelayChanged;
        /// <summary>
        /// Метод получения зависимости для подчиненного члена связи
        /// </summary>
        /// <returns>Ссылка на зависимость для подчиненного члена связи</returns>
        IDependence GetSlaveDependence();
        /// <summary>
        /// Метод получения ссылки на интерфейс класса информации о члене связи
        /// </summary>
        /// <param name="member">Зависимость члена связи</param>
        /// <returns>Ссылка на интерфейс класса информации о члене связи</returns>
        ILMember GetInfoMember(e_DependType member);
        /// <summary>
        /// Метод получения ссылки на интерфейс класса информации о члене связи
        /// </summary>
        /// <param name="member">Идентификатор члена связи</param>
        /// <returns>Ссылка на интерфейс класса информации о члене связи</returns>
        ILMember GetInfoMember(IId member);
        /// <summary>
        /// Метод определяет принадлежит ли идентификатор <paramref name="objectID"/> одному из членов связи
        /// </summary>
        /// <param name="objectID">Идентификатор объекта</param>
        /// <returns>Истина если идентификатор принадлежит члену связит</returns>
        bool isItMember(string objectID);
    }
}
