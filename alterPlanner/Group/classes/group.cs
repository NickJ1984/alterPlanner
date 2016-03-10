using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.classes;
using alter.Group.iface;
using alter.iface;
using alter.Link.iface;
using alter.types;
using alter.Task.iface;

namespace alter.Group.classes
{
    public class Group : IGroup
    {
        #region Переменные
        protected string _groupName;
        protected Identity _id;
        protected int _enclosureCount;

        protected Dictionary<string, IGroup> _groups;
        protected Dictionary<string, ITask> _tasks;

        protected IGroup _owner;
        #endregion
        #region Свойства
        public int countGroups => _groups.Count;
        public int countTasks => _tasks.Count;
        public int countEnclosure => _enclosureCount;
        #endregion
        #region События
        public event EventHandler<ea_ValueChange<double>> event_DurationChanged;
        public event EventHandler<ea_ValueChange<e_GrpLim>> event_LimitChanged;
        public event EventHandler<ea_IdObject> event_ObjectDeleted;
        #endregion
        #region Конструкторы
        protected Group()
        {
            _id = new Identity(e_Entity.Group);
            _groups = new Dictionary<string, IGroup>();
            _tasks = new Dictionary<string, ITask>();

            _enclosureCount = 0;

            _owner = null;
        }
        #endregion
        #region Методы запуска событий
        protected void durationChangedPushEvent(double Old, double New)
        {
            event_DurationChanged?.Invoke(this, new ea_ValueChange<double>(Old, New));
        }
        protected void limitChangedPushEvent(e_GrpLim Old, e_GrpLim New)
        {
            event_LimitChanged?.Invoke(this, new ea_ValueChange<e_GrpLim>(Old, New));
        }
        protected void objectDeletedPushEvent()
        {
            event_ObjectDeleted?.Invoke(this, new ea_IdObject(this));
        }
        #endregion
        #region Методы
        #region Манипуляции с объектами
        public bool addInGroup(IDock newObject)
        {
            throw new NotImplementedException();
        }
        public bool removeFromGroup(string objectId)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Объект
        public void DeleteObject()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Информация
        public int GroupCount()
        {
            return countGroups;
        }
        public int TaskCount()
        {
            return countTasks;
        }
        public int EnclosureCount()
        {
            return countEnclosure;
        }
        public IGroup GetGroupOwner()
        {
            return _owner;
        }
        public bool InGroup(string id)
        {
            if(string.IsNullOrEmpty(id)) throw new ArgumentNullException();

            if (_owner != null)
            {
                if (_owner.GetId() == id) return true;
                return _owner.InGroup(id);
            }

            return false;
        }
        public IDependence getGroupDepend()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
        #region Интерфейсы
        #region IDock
        public bool connect(ILink link)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region ILine
        public IDot GetDot(e_Dot type)
        {
            throw new NotImplementedException();
        }
        public double GetDuration()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region IName
        public string GetName()
        {
            return _groupName;
        }
        public void SetName(string name)
        {
            if (name == _groupName) return;

            _groupName = name;
        }
        #endregion
        #region ILimit
        public e_GrpLim GetLimit()
        {
            throw new NotImplementedException();
        }
        public bool SetLimit(e_GrpLim limitType)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region IId
        public string GetId()
        {
            return _id.Id;
        }
        e_Entity IId.GetType()
        {
            return _id.Type;
        }
        #endregion
        #endregion
    }

}
