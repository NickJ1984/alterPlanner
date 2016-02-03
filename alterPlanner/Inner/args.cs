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
    /// Аргумент события передающий идентификатор объекта <see cref="Identity"/> со свойством readonly.
    /// </summary>
    public class ea_IdObject : System.EventArgs
    {
        /// <summary>
        /// Идентификатор объекта.
        /// </summary>
        public readonly Identity Id;

        /// <summary>
        /// Конструктор класса аргументов.
        /// </summary>
        /// <param name="IDobject">Задать идентификатор объекта передаваемый аргументом события.</param>
        public ea_IdObject(IId dobject)
        {
            Id = new Identity(dobject.GetType(), dobject.GetId());
        }
    }
    /// <summary>
    /// Аргумент события передающий объект типа <see cref="ea_Value{T}(T)"/>.
    /// </summary>
    /// <typeparam name="T">Тип передаваемого объекта.</typeparam>
    public class ea_Value<T> : System.EventArgs
    {
        /// <summary>
        /// Передаваемый объект типа <typeparamref name="T"/>.
        /// </summary>
        public T Value;

        /// <summary>
        /// Конструктор класса аргумента события.
        /// </summary>
        /// <param name="value">Задать передаваемый объект типа <typeparamref name="T"/>.</param>
        public ea_Value(T value)
        {
            this.Value = value;
        }
    }

    /// <summary>
    /// Аргумент события передающий объект типа <see cref="ea_Value{T}(T)"/>, до и после его изменения.
    /// </summary>
    /// <typeparam name="T">Тип передаваемого объекта.</typeparam>
    public class ea_ValueChange<T> : System.EventArgs
    {
        /// <summary>
        /// Старое значение объекта типа <typeparamref name="T"/>.
        /// </summary>
        public T OldValue;
        /// <summary>
        /// Новое значение объекта типа <typeparamref name="T"/>.
        /// </summary>
        public T NewValue;

        /// <summary>
        /// Конструктор класса аргумента события.
        /// </summary>
        /// <param name="old">Задать старое значение передаваемого объекта типа <typeparamref name="T"/>.</param>
        /// <param name="New">Задать новое значение передаваемого объекта типа <typeparamref name="T"/>.</param>
        public ea_ValueChange(T old, T New)
        {
            OldValue = old;
            NewValue = New;
        }
    }

    /// <summary>
    /// Аргумент события передающий информацию о делегате
    /// </summary>
    public class ea_DelegateInfo : System.EventArgs
    {
        /// <summary>
        /// Возвращает экземпляр класса, метод которого вызывает текущий делегат.
        /// </summary>
        public readonly object Target;
        /// <summary>
        /// Возвращает метод представленный делегатом.
        /// </summary>
        public readonly System.Reflection.MethodInfo Method;

        /// <summary>
        /// Конструктор экземпляра класса <see cref="ea_DelegateInfo"/>
        /// </summary>
        /// <param name="method">Ссылка на метод представляемый делегатом.</param>
        /// <param name="target">Ссылка экземпляр класса, метод которого вызывает текущий делегат.</param>
        public ea_DelegateInfo(System.Reflection.MethodInfo method, object target)
        {
            this.Target = target;
            this.Method = method;
        }

        /// <summary>
        /// Конструктор экземпляра класса <see cref="ea_DelegateInfo"/>
        /// </summary>
        /// <param name="_delegate">Ссылка на делегат метод и источник которого будет передаваться данными аргументами.</param>
        public ea_DelegateInfo(Delegate _delegate)
            :this(_delegate.Method, _delegate.Target)
        { }
    }
}
