using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.classes;
using alter.iface;

namespace alter.args
{
    /// <summary>
    /// Аргумент события передающий идентификатор объекта <see cref="identity"/> со свойством readonly.
    /// </summary>
    public class EA_IDObject : System.EventArgs
    {
        /// <summary>
        /// Идентификатор объекта.
        /// </summary>
        public readonly identity ID;

        /// <summary>
        /// Конструктор класса аргументов.
        /// </summary>
        /// <param name="IDobject">Задать идентификатор объекта передаваемый аргументом события.</param>
        public EA_IDObject(IId IDobject)
        {
            ID = new identity(IDobject.getType(), IDobject.getID());
        }
    }
    /// <summary>
    /// Аргумент события передающий объект типа <see cref="EA_value{T}(T)"/>.
    /// </summary>
    /// <typeparam name="T">Тип передаваемого объекта.</typeparam>
    public class EA_value<T> : System.EventArgs
    {
        /// <summary>
        /// Передаваемый объект типа <typeparamref name="T"/>.
        /// </summary>
        public T Value;

        /// <summary>
        /// Конструктор класса аргумента события.
        /// </summary>
        /// <param name="Value">Задать передаваемый объект типа <typeparamref name="T"/>.</param>
        public EA_value(T Value)
        {
            this.Value = Value;
        }
    }

    /// <summary>
    /// Аргумент события передающий объект типа <see cref="EA_value{T}(T)"/>, до и после его изменения.
    /// </summary>
    /// <typeparam name="T">Тип передаваемого объекта.</typeparam>
    public class EA_valueChange<T> : System.EventArgs
    {
        /// <summary>
        /// Старое значение объекта типа <typeparamref name="T"/>.
        /// </summary>
        public T oldValue;
        /// <summary>
        /// Новое значение объекта типа <typeparamref name="T"/>.
        /// </summary>
        public T newValue;

        /// <summary>
        /// Конструктор класса аргумента события.
        /// </summary>
        /// <param name="Old">Задать старое значение передаваемого объекта типа <typeparamref name="T"/>.</param>
        /// <param name="New">Задать новое значение передаваемого объекта типа <typeparamref name="T"/>.</param>
        public EA_valueChange(T Old, T New)
        {
            oldValue = Old;
            newValue = New;
        }
    }

    public class EA_delegateInfo : System.EventArgs
    {
        /// <summary>
        /// Возвращает экземпляр класса, метод которого вызывает текущий делегат.
        /// </summary>
        public readonly object target;
        /// <summary>
        /// Возвращает метод представленный делегатом.
        /// </summary>
        public readonly System.Reflection.MethodInfo method;

        /// <summary>
        /// Конструктор экземпляра класса <see cref="EA_delegateInfo"/>
        /// </summary>
        /// <param name="method">Ссылка на метод представляемый делегатом.</param>
        /// <param name="target">Ссылка экземпляр класса, метод которого вызывает текущий делегат.</param>
        public EA_delegateInfo(System.Reflection.MethodInfo method, object target)
        {
            this.target = target;
            this.method = method;
        }

        /// <summary>
        /// Конструктор экземпляра класса <see cref="EA_delegateInfo"/>
        /// </summary>
        /// <param name="_delegate">Ссылка на делегат метод и источник которого будет передаваться данными аргументами.</param>
        public EA_delegateInfo(Delegate _delegate)
            :this(_delegate.Method, _delegate.Target)
        { }
    }
}
