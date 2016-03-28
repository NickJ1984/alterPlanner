using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.classes;
using alter.Group.iface;
using alter.iface;
using alter.Link.classes;
using alter.Link.iface;
using alter.Project.iface;
using alter.Service.classes;
using alter.types;
using alter.Task.iface;

namespace alter.Group.classes
{
    public partial class Group : IGroup
    {
        #region Переменные
        protected IProject project;
        
        #region Экземпляры классов
        protected Identity _id;
        protected line _line;
        protected linkManager mgrLink;
        protected groupManager mgrGroup;
        protected cNamer _name;
        protected cLimit<e_GrpLim> _limit;
        protected Dependence _depend;
        protected cLimitAgregator lAgregator;
        #endregion
        #endregion
        #region Свойства
        public int count
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public int enclosureCount => mgrGroup.ownerEnclosureCount;
        #endregion
        #region События
        public event EventHandler<ea_ValueChange<double>> event_DurationChanged
        {
            add { _line.event_DurationChanged += value; }
            remove { _line.event_DurationChanged -= value; }
        }
        public event EventHandler<ea_ValueChange<e_GrpLim>> event_LimitChanged
        {
            add { _limit.event_LimitChanged += value; }
            remove { _limit.event_LimitChanged -= value; }
        }
        public event EventHandler<ea_IdObject> event_ObjectDeleted;
        #endregion
        #region Конструктор
        public Group(IProject projectOwner)
        {
            if(projectOwner == null) throw new ArgumentNullException(nameof(projectOwner));

            project = projectOwner;

            init_ID();
            init_cNamer();
            init_Limit();
            init_Line();
            init_GroupManager();
            init_LinkManager();
            init_LimitAgregator();
        }
        #region Методы инициализаторы классов и переменных
        protected void init_ID()
        {
            _id = new Identity(e_Entity.Group);
        }
        protected void init_Line()
        {
            _line = new line(this, project.GetDot(e_Dot.Start).GetDate());
        }
        protected void init_LinkManager()
        {
            mgrLink = new linkManager(this);   
        }

        protected void init_GroupManager()
        {
            mgrGroup = new groupManager(this);
        }
        protected void init_cNamer()
        {
            _name = new cNamer(this);
        }
        protected void init_Limit()
        {
            _limit = new cLimit<e_GrpLim>(this, e_GrpLim.Earlier);
        }
        protected void init_LimitAgregator()
        {
            lAgregator = new cLimitAgregator(this, project);            
        }
        protected void init_Dependence()
        {
            _depend = new Dependence(project.GetDot(e_Dot.Start).GetDate(), 
                                     e_Direction.Right, 
                                     e_Dot.Start);
        }
        #endregion
        #endregion
        #region Методы
        #region Добавление и удаление из группы
        public bool addInGroup(IGroupable newObject)
        {
            throw new NotImplementedException();
        }
        public bool removeFromGroup(string objectId)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Информационные
        public IDependence getGroupDepend()
        {
            throw new NotImplementedException();
        }
        public IGroupManager getGroupManager()
        {
            throw new NotImplementedException();
        }
        public IGroup GetGroupOwner()
        {
            throw new NotImplementedException();
        }
        public bool InGroup(string id)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Методы экземпляра
        public void DeleteObject()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
        #region Реализация интерфейсов
        #region IDock
        public bool connect(ILink link)
        {
            return mgrLink.connect(link);
        }
        #endregion
        #region ILine
        public IDot GetDot(e_Dot type)
        {
            return _line.GetDot(type);
        }
        public double GetDuration()
        {
            return _line.duration;
        }
        #endregion
        #region IId
        public string GetId()
        {
            return _id.Id;
        }
        public e_Entity GetType()
        {
            return _id.Type;
        }
        #endregion
        #region ILimit
        public e_GrpLim GetLimit()
        {
            return _limit;
        }
        public bool SetLimit(e_GrpLim limitType)
        {
            if (limitType == _limit.limit) return false;

            _limit.limit = limitType;

            return true;
        }
        #endregion
        #region IName
        public string GetName()
        {
            return _name;
        }
        public void SetName(string name)
        {
            _name.name = name;
        }
        #endregion
        #endregion
        #region Служебные
        
        protected void limitToDependence()
        {
            switch (_limit.limit)
            {
                case e_GrpLim.Earlier:

                    break;
                case e_GrpLim.Later:

                    break;
                case e_GrpLim.NotEarlier:

                    break;
                case e_GrpLim.NotLater:

                    break;
            }            
        }
        #endregion
    }
    #region Класс хранения и обработки объектов группы
    public partial class Group : IGroup
    {
        internal interface IGroupElements
        {
            DateTime dateMax { get; }
            DateTime dateMin { get; }

            event EventHandler<ea_ValueChange<DateTime>> event_dateMinChanged;
            event EventHandler<ea_ValueChange<DateTime>> event_dateMaxChanged;
            event EventHandler<ea_Value<IGroupable>> event_itemAdded;
            event EventHandler<ea_Value<IGroupable>> event_itemRemoved;

            bool addItem(IGroupable item);
            bool remove(IGroupable item);
            bool remove(string ID);
            bool remove();
        }
        internal class groupElements : IGroupElements
        {
            #region Переменные
            protected Dictionary<string, IGroupable> items;
            protected DateTime min;
            protected DateTime max;
            #endregion
            #region Свойства
            public DateTime dateMax
            {
                get { return max; }
                protected set
                {
                    throw new NotImplementedException();
                }
            }
            public DateTime dateMin
            {
                get { return min; }
                protected set
                {
                    throw new NotImplementedException();
                }
            }
            #endregion
            #region События
            public event EventHandler<ea_ValueChange<DateTime>> event_dateMaxChanged;
            public event EventHandler<ea_ValueChange<DateTime>> event_dateMinChanged;
            public event EventHandler<ea_Value<IGroupable>> event_itemAdded;
            public event EventHandler<ea_Value<IGroupable>> event_itemRemoved;
            #endregion
            #region Конструктор

            public groupElements()
            {
                items = new Dictionary<string, IGroupable>();
                min = new DateTime(1, 1, 1);
                max = new DateTime(1, 1, 1);
            }
            #endregion
            #region Обработчики

            #endregion
            #region Методы
            #region Добавление
            public bool addItem(IGroupable item)
            {
                if (items.Keys.Contains(item.GetId())) return false;

                items.Add(item.GetId(), item);

                throw new NotImplementedException();
            }
            #endregion
            #region Удаление
            public bool remove()
            {
                throw new NotImplementedException();
            }
            public bool remove(string ID)
            {
                throw new NotImplementedException();
                return items.Remove(ID);
            }
            public bool remove(IGroupable item)
            {
                throw new NotImplementedException();
            }
            #endregion
            #endregion
            #region Служебные

            #endregion
        }
    }
    #endregion  
}
