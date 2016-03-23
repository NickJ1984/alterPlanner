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
using alter.Project.iface;
using alter.Service.Extensions;
using alter.types;

namespace alter.Service.classes
{
    public interface ILimitAgregator
    {
        e_Entity dependEntity { get; }

        void setProject(IProject project);
        void setGroup(IGroup group);
        void setLink(ILink link);

        bool updateDependEntity();

        event EventHandler<ea_ValueChange<e_Entity>> event_dependEntityChanged;
    }

    public partial class cLimitAgregator  : ILimitAgregator
    {
        #region Переменные
        protected IProject _project;
        protected IGroup _group;
        protected ILink _link;
        protected ILine _owner;

        protected e_Entity _dependEntity;
        #endregion
        #region Свойства
        public e_Entity dependEntity
        {
            get { return _dependEntity; }
            protected set
            {
                if (value == _dependEntity) return;

                e_Entity temp = _dependEntity;
                _dependEntity = value;

                event_dependEntityChanged?.Invoke(this, new ea_ValueChange<e_Entity>(temp, _dependEntity));
            }
        }
        #endregion
        #region События
        public event EventHandler<ea_ValueChange<e_Entity>> event_dependEntityChanged;
        #endregion
        #region Конструктор
        public cLimitAgregator(ILine owner, IProject project)
        {
            if(project == null || owner == null) throw new ArgumentNullException();

            _owner = owner;
            _project = project;
        }
        #endregion
        #region Методы
        public bool updateDependEntity()
        {
            e_Entity temp = dependEntity;

            dependEntity = calculateDependEntity();

            return temp != dependEntity ? true : false;
        }
        public void setLink(ILink link)
        {
            if (_link == link) return;

            _link = link;

            dependEntity = calculateDependEntity();
        }
        public void setGroup(IGroup group)
        {
            if (_group == group) return;

            _group = group;

            dependEntity = calculateDependEntity();
        }

        public void setProject(IProject project)
        {
            if (project == null || project == _project) return;

            _project = project;

            dependEntity = calculateDependEntity();
        }
        #endregion
        #region Служебные
        protected e_Entity calculateDependEntity()
        {
            //Все даты расчитываются по финишу
            DateTime result = new DateTime(1,1,1);
            e_Entity dEntity = e_Entity.None;

            if (_link != null)
            {
                if (_link.GetSlaveDependence().GetDependDot() == e_Dot.Start)
                    result = _link.GetSlaveDependence().GetDate().AddDays(_owner.GetDuration());
                else
                    result = _link.GetSlaveDependence().GetDate();
                dEntity = e_Entity.Link;
            }


            if (_group != null)
            {
                e_GrpLim gLimit = _group.GetLimit();
                DateTime groupDate = new DateTime(1,1,1);

                switch (gLimit)
                {
                    case e_GrpLim.Earlier:
                        groupDate = _group.GetDot(e_Dot.Start).GetDate().AddDays(_owner.GetDuration());
                        break;
                    case e_GrpLim.Later:
                        groupDate = _group.GetDot(e_Dot.Finish).GetDate();
                        break;
                    case e_GrpLim.NotLater:
                        groupDate = _group.getGroupDepend().GetDate();
                        break;
                    case e_GrpLim.NotEarlier:
                        groupDate = _group.getGroupDepend().GetDate().AddDays(_owner.GetDuration());
                        break;
                    default:
                        throw new ApplicationException(nameof(gLimit));
                }


                if (groupDate > result)
                {
                    if (gLimit != e_GrpLim.NotLater)
                    {
                        result = groupDate;
                        dEntity = e_Entity.Group;
                    }
                }
            }


            if (dEntity == e_Entity.None) return e_Entity.Project;
            else return dEntity;
        }
        #endregion
    }
}
