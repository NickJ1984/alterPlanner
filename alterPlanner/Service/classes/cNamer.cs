using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.iface;
using alter.types;

namespace alter.Service.classes
{
    public class cNamer : IName
    {
        #region Переменные
        protected readonly IId _owner;
        protected string _name;
        #endregion
        #region Свойства
        public string name
        {
            get { return _name; }
            set
            {
                if(value == _name) return;

                string temp = _name;
                _name = value;

                event_nameChanged?.Invoke(_owner, new ea_ValueChange<string>(temp, _name));
            }
        }
        #endregion
        #region События
        public event EventHandler<ea_ValueChange<string>> event_nameChanged;
        #endregion
        #region Конструктор
        public cNamer(IId owner, string name)
        {
            if(owner == null) throw new ArgumentNullException(nameof(owner));
            _owner = owner;
            _name = name;
        }
        ~cNamer()
        {
            _name = null;
        }
        public cNamer(IId owner)
            :this(owner, String.Empty)
        { }
        #endregion
        #region Методы
        public string GetName()
        {
            return name;
        }
        public void SetName(string name)
        {
            this.name = name;
        }
        public string GetId()
        {
            return _owner.GetId();
        }
        public e_Entity GetType()
        {
            return _owner.GetType();
        }
        #endregion
        #region Перегрузки
        public static implicit operator string(cNamer instance)
        {
            return instance.name;
        }
        #endregion
    }
}
