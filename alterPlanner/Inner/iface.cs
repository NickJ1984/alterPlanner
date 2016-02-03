using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.classes;
using alter.types;
using alter.args;
using alter.Task.iface;
using alter.Link.iface;
using alter.Function.iface;



namespace alter.iface
{
    #region object
    #region Identification
    /// <summary>
    /// Служит для идентификации основных объектов библиотеки alterPlanner
    /// (основные объекты указаны в перечислении eEntity).
    /// </summary>
    public interface IId
    {
        /// <summary>
        /// Возвращает уникальный идентификатор объекта
        /// (для генерации используется класс Guid).
        /// </summary>
        /// <returns>Уникальный идентификатор объекта.</returns>
        string GetId();
        /// <summary>
        /// Возвращает тип объекта.
        /// </summary>
        /// <returns>Тип объекта.</returns>
        e_Entity GetType();
    }
    #endregion
    #region Inheritance
    /// <summary>
    /// Используется если объект зависит от некоего родительского объекта, и при уничтожении родителя должен уничтожиться сам.
    /// </summary>
    public interface IChild
    {
        /// <summary>
        /// Обработчик на событие уничтожения родителя.
        /// </summary>
        /// <param name="sender">Объект родитель события.</param>
        /// <param name="objectId">Идентификатор уничтожаемого объекта.</param>
        void handler_ownerDelete(object sender, ea_IdObject objectId);
    }
    #endregion
    #region properties
    /// <summary>
    /// Возможность удаления объекта.
    /// </summary>
    public interface IRemovable : IId
    {
        /// <summary>
        /// Событие срабатывающее при уничтожении объекта, значение параметра является идентификатором уничтожаемого объекта.
        /// </summary>
        event EventHandler<ea_IdObject> event_ObjectDeleted;
        /// <summary>
        /// Уничтожение объекта.
        /// </summary>
        void DeleteObject();
    }
    /// <summary>
    /// Возможность объекта принимать ограничения типа <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Корректные перечисления: 
    /// <see cref="e_TLLim"/>,
    /// <see cref="e_TskLim"/>,
    /// <see cref="e_GrpLim"/>,
    /// <see cref="e_PrjLim"/>.
    /// </typeparam>
    public interface ILimit<T> where T : struct, IConvertible
    {
        /// <summary>
        /// Задать значение ограничения.
        /// </summary>
        /// <param name="limitType">Значение ограничения типа <typeparamref name="T"/>.</param>
        /// <returns>True - если значение принято.</returns>
        bool SetLimit(T limitType);
        /// <summary>
        /// Получить значение ограничения типа <typeparamref name="T"/>.
        /// </summary>
        /// <returns>Значение ограничения типа <typeparamref name="T"/>.</returns>
        T GetLimit();
        /// <summary>
        /// Событие изменения ограничения
        /// </summary>
        event EventHandler<ea_ValueChange<T>> event_LimitChanged;
    }
    /// <summary>
    /// Возможность именовать объект.
    /// </summary>
    public interface IName : IId
    {
        /// <summary>
        /// Присвоить имя объекту.
        /// </summary>
        /// <param name="name">Имя объекта.</param>
        void SetName(string name);
        /// <summary>
        /// Получить имя объекта.
        /// </summary>
        /// <returns>Имя объекта.</returns>
        string GetName();
    }
    #endregion
    #region planner
    /// <summary>
    /// Интерфейс датируемой точкой.
    /// </summary>
    public interface IDot
    {
        /// <summary>
        /// Получить тип точки <see cref="e_Dot"/>.
        /// </summary>
        /// <returns>Тип точки.</returns>
        e_Dot GetDotType();
        /// <summary>
        /// Получить дату точки.
        /// </summary>
        /// <returns>Дата точки.</returns>
        DateTime GetDate();
        /// <summary>
        /// Событие срабатывающее при изменении даты точки.
        /// </summary>
        event EventHandler<ea_ValueChange<DateTime>> event_DateChanged;
    }
    /// <summary>
    /// Интерфейс датируемого отрезка.
    /// </summary>
    public interface ILine
    {
        /// <summary>
        /// Получить ссылку на точку заданного типа.
        /// </summary>
        /// <param name="type">Тип точки.</param>
        /// <returns></returns>
        IDot GetDot(e_Dot type);
        /// <summary>
        /// Длина датируемого отрезка измеряемая в днях.
        /// </summary>
        /// <returns>Длина датируемого отрезка в днях.</returns>
        double GetDuration();
        /// <summary>
        /// Событие срабатывающее при изменении длины датируемого отрезка.
        /// </summary>
        event EventHandler<ea_ValueChange<double>> event_DurationChanged;
    }
    #endregion
    #endregion
    #region dependencies
    /// <summary>
    /// Интерфейс характеристик зависимости.
    /// </summary>
    public interface IDependence : IFncInfo
    {
        /// <summary>
        /// Получить тип зависимой точки.
        /// </summary>
        /// <returns></returns>
        e_Dot GetDependDot();
        /// <summary>
        /// Событие при изменении типа зависимой точки.
        /// </summary>
        event EventHandler<ea_ValueChange<e_Dot>> event_DependDotChanged;
    }
    #endregion
    #region link interfaces
    /// <summary>
    /// Интерфейс взаимодействия с объектами связи.
    /// </summary>
    public interface IDock : IId, IRemovable
    {
        /// <summary>
        /// Подключение к объекту связи <paramref name="link"/> в качестве участника, с типом зависимости <paramref name="dType"/>.
        /// </summary>
        /// <param name="dType">Тип зависимости объекта в связи.</param>
        /// <param name="link">Ссылка на связь.</param>
        /// <returns>Возвращает точку объекта зависимую от связи (для реализации рекомендуется <see cref="alter.Service.iface.IDotAdapter"/>).</returns>
        IDot Subscribe(e_DependType dType, ILink link);
    }
    #endregion
    
}
