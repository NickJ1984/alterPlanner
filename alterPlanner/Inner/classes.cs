using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.types;
using alter.iface;

namespace alter.classes
{
    #region ID
    /// <summary>
    /// Класс реализующий механизм идентификации объектов.
    /// </summary>
    public class identity : IId
    {
        /// <summary>
        /// Уникальный идентификатор объекта, является константой для экземпляра к которому принадлежит.
        /// </summary>
        private Guid _ID;
        /// <summary>
        /// Тип объекта, является константой для экземпляра к которому принадлежит.
        /// </summary>
        private eEntity _type;

        /// <summary>
        /// Получить уникальный идентификатор объекта.
        /// </summary>
        public string ID { get { return _ID.ToString(); } }
        /// <summary>
        /// Получить тип объекта.
        /// </summary>
        public eEntity type { get { return _type; } }

        /// <summary>
        /// Конструктор класса <see cref="identity"/>.
        /// </summary>
        /// <param name="type">Тип идентифицируемого объекта.</param>
        public identity(eEntity type)
        {
            _ID = Guid.NewGuid();
            _type = type;
        }
        /// <summary>
        /// Конструктор класса <see cref="identity"/>, используется для утилитарных нужд.
        /// </summary>
        /// <param name="type">Тип идентифицируемого объекта.</param>
        /// <param name="ID">Уникальный идентификатор объекта.</param>
        public identity(eEntity type, string ID)
        {
            _ID = new Guid(ID);
            _type = type;
        }
        /// <summary>
        /// Конструктор класса <see cref="identity"/>, используемый для полного копирования экземпляра класса <see cref="identity"/>.
        /// </summary>
        /// <param name="IDobject">Экземпляр копируемого класса <see cref="identity"/>.</param>
        public identity(identity IDobject)
            : this(IDobject.type, IDobject.ID)
        { }
        /// <summary>
        /// Получить уникальный идентификатор объекта.
        /// </summary>
        /// <param name="ID">Уникальный идентификатор объекта.</param>
        public void setID(string ID)
        {
            _ID = new Guid(ID);
        }
        /// <summary>
        /// Задать тип объекта.
        /// </summary>
        /// <param name="type">Тип объекта.</param>
        public void setType(eEntity type)
        {
            _type = type;
        }
        /// <summary>
        /// Скопировать все значения экземпляра <paramref name="IDobject"/>.
        /// </summary>
        /// <param name="IDobject">Экземпляр копируемого класса <see cref="identity"/>.</param>
        public void copy(identity IDobject)
        {
            _ID = new Guid(IDobject._ID.ToString());
            _type = IDobject.type;
        }
        /// <summary>
        /// Возвращает уникальный идентификатор объекта
        /// (для генерации используется класс Guid).
        /// </summary>
        /// <returns>Уникальный идентификатор объекта.</returns>
        public string getID() { return ID; }
        /// <summary>
        /// Возвращает тип объекта.
        /// </summary>
        /// <returns>Тип объекта.</returns>
        public eEntity getType() { return type; }
    }
    #endregion
    #region Static
    /// <summary>
    /// Статический класс утилитарных функций
    /// </summary>
    public static class __hlp
    {
        /// <summary>
        /// Стандартное значение для инициализации дат сборки.
        /// </summary>
        public static readonly DateTime initDate = new DateTime(1900, 1, 1);
        /// <summary>
        /// Получить тип зависимой точки предшественника, из значения связи типа <see cref="eTskLim"/>
        /// </summary>
        /// <param name="Type">Значение ограничения типа "предшественник-последователь"</param>
        /// <returns></returns>
        public static eDot getPrecursor(eTskLim Type)
        {
            eTskLimChunk TC = (eTskLimChunk)Type;
            return ((TC & eTskLimChunk.Finish_) == eTskLimChunk.Finish_) ? eDot.finish : eDot.start;
        }
        /// <summary>
        /// Получить тип зависимой точки последователя, из значения связи типа <see cref="eTskLim"/>
        /// </summary>
        /// <param name="Type">Значение ограничения типа "предшественник-последователь"</param>
        /// <returns></returns>
        public static eDot getFollower(eTskLim Type)
        {
            eTskLimChunk TC = (eTskLimChunk)Type;
            return ((TC & eTskLimChunk._Finish) == eTskLimChunk._Finish) ? eDot.finish : eDot.start;
        }
    }
    #endregion
}
