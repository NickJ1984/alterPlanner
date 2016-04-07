using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.classes;
using alter.iface;
using alter.Link.classes;
using alter.Link.iface.Base;
using alter.Project.iface;
using alter.Service.classes;
using alter.Service.Extensions;
using alter.types;

namespace alter.Task.classes
{
    public interface ITask2 : IId, IRemovable, ILine, IName, ILimit<e_TLLim>, IConnectible
    {
        event EventHandler<ea_ValueChange<DateTime>> event_LimitDateChanged;
    }

    #region Основная логика
    public partial class task2
    {
        #region Константы
        public const e_TLLim DEFAULT_VALUE_LOCAL_LIMIT = e_TLLim.Earlier;
        #endregion
        #region Переменные
        protected e_Dot _dependDot;
        protected DateTime _limitDate;
        protected e_Entity _limitEntity;
        #endregion
        #region Классы
        protected Identity _id;
        protected cNamer _name;
        
        protected IProject project;
        protected linkManager2 mgrLink;
        protected cLocalLimit localLimit;
        protected line lineTask;
        #endregion
        #region Делегаты
        protected Action unsubscribeProject;
        protected Action unsubscribeLink;
        protected Action unsubscribeActiveLink;
        protected Action unsubscribeLocalLimit;
        #endregion
        #region Свойства
        public string name
        {
            get { return _name; }
            set { _name.name = value; }
        } 
        public double duration => lineTask.duration;
        public DateTime start => lineTask.start;
        public DateTime finish => lineTask.finish;
        public e_TLLim limit
        {
            get { return localLimit.limit; }
            set { localLimit.limit = value; }
        }
        public DateTime dateLimit
        {
            get { return localLimit.date; }
            set { localLimit.date = value; }
        }
        protected e_Dot dependDot
        {
            get { return _dependDot; }
            set
            {
                if (_dependDot == value) return;

                _dependDot = value;
            }
        }
        protected DateTime limitDate
        {
            get { return _limitDate; }
            set
            {
                if(_limitDate == value) return;

                _limitDate = value;
            }
        }
        protected e_Entity limitEntity
        {
            get { return _limitEntity; }
            set
            {
                if(_limitEntity == value) return;

                _limitEntity = value;
            }
        }
        #endregion
        #region События
        public event EventHandler<ea_ValueChange<double>> event_DurationChanged
        {
            add { lineTask.event_DurationChanged += value; }
            remove { lineTask.event_DurationChanged -= value; }
        }
        public event EventHandler<ea_ValueChange<e_TLLim>> event_LimitChanged
        {
            add { localLimit.event_limitChanged += value; }
            remove { localLimit.event_limitChanged -= value; }
        }
        public event EventHandler<ea_ValueChange<DateTime>> event_LimitDateChanged
        {
            add { localLimit.event_dateChanged += value; }
            remove { localLimit.event_dateChanged -= value; }
        }
        public event EventHandler<ea_IdObject> event_ObjectDeleted;
        #endregion
        #region Конструктор
        public task2(IProject project, DateTime limitDate, e_TLLim limit)
        {
            this.project = project;

            init_Identity();
            init_cNamer();
            init_Line();
            init_LinkManager();
            init_LocalLimit(limit, limitDate);

            limitCalculate();
        }
        ~task2()
        {
            clear();
        }
        #endregion
        #region Инициализаторы

        protected void init_Identity()
        {
            _id = new Identity(e_Entity.Task);
        }
        protected void init_cNamer()
        {
            _name = new cNamer(this);
        }
        protected void init_Line()
        {
            lineTask = new line(this, project.GetDot(e_Dot.Start).GetDate());
            lineTask.sender = this;
        }
        protected void init_LocalLimit(e_TLLim limit, DateTime date)
        {
            localLimit = new cLocalLimit();
            localLimit.limit = limit;
            localLimit.date = date;
        }
        protected void init_LinkManager()
        {
            mgrLink = new linkManager2(this);
        }
        #endregion
        #region Обработчики
        #region Проект
        protected void handler_projectStartChanged(object sender, ea_ValueChange<DateTime> e)
        {
            switch (localLimit.limit)
            {
                case e_TLLim.Earlier:
                    if (limitEntity == e_Entity.Project) setLimitProject();
                    else limitCalculate();
                    break;

                case e_TLLim.Later:
                case e_TLLim.FinishFixed:
                case e_TLLim.StartFixed:
                    return;

                case e_TLLim.FinishNotEarlier:
                case e_TLLim.FinishNotLater:
                case e_TLLim.StartNotEarlier:
                case e_TLLim.StartNotLater:
                    limitCalculate();
                    break;

                default:
                    throw new ApplicationException(nameof(handler_projectStartChanged));
            }
        }
        protected void handler_projectFinishChanged(object sender, ea_ValueChange<DateTime> e)
        {
            switch (localLimit.limit)
            {
                case e_TLLim.Later:
                    if (limitEntity == e_Entity.Project) setLimitProject();
                    else limitCalculate();
                    break;

                case e_TLLim.Earlier:
                case e_TLLim.FinishFixed:
                case e_TLLim.StartFixed:
                    return;

                case e_TLLim.FinishNotEarlier:
                case e_TLLim.FinishNotLater:
                case e_TLLim.StartNotEarlier:
                case e_TLLim.StartNotLater:
                    limitCalculate();
                    break;

                default:
                    throw new ApplicationException(nameof(handler_projectStartChanged));
            }
        }
        #endregion
        #region Менеджер связей
        protected void handler_activeLinkChanged(object sender, ea_ValueChange<ILink_2> e)
        {
            subscribe_activeLink(e);
            
            if(localLimit.limit == e_TLLim.FinishFixed ||
               localLimit.limit == e_TLLim.StartFixed) return;
            
            limitCalculate();            
        }
        #endregion
        #region Активная связь
        protected void handler_activeLinkLimitChanged(object sender, ea_ValueChange<e_TskLim> e)
        {
            switch (localLimit.limit)
            {
                case e_TLLim.Earlier:
                case e_TLLim.Later:
                    if (limitEntity == e_Entity.Link)
                    {
                        setLimitActiveLink();
                        return;
                    }
                    break;                    

                case e_TLLim.StartFixed:
                case e_TLLim.FinishFixed:
                    return;
            }

            limitCalculate();
        }
        protected void handler_activeLinkDateChanged(object sender, ea_ValueChange<DateTime> e)
        {
            switch (localLimit.limit)
            {
                case e_TLLim.Earlier:
                case e_TLLim.Later:
                    if (limitEntity == e_Entity.Link)
                    {
                        setLimitActiveLink();
                        return;
                    }
                    break;

                case e_TLLim.StartFixed:
                case e_TLLim.FinishFixed:
                    return;
            }

            limitCalculate();
        }
        #endregion
        #region Локальная зависимость
        protected void handler_localLimit_limitChanged(object sender, ea_ValueChange<e_TLLim> e)
        {
            limitCalculate();
        }
        protected void handler_localLimitDateChanged(object sender, ea_ValueChange<DateTime> e)
        {
            switch (localLimit.limit)
            {
                case e_TLLim.FinishFixed:
                case e_TLLim.StartFixed:
                    setLimitLocal();
                    return;

                case e_TLLim.Later:
                case e_TLLim.Earlier:
                    return;
            }

            limitCalculate();
        }
        #endregion
        #endregion
        #region Методы
        public void clear()
        {
            event_ObjectDeleted?.Invoke(this, new ea_IdObject(this));

            unsubscribeActiveLink?.Invoke();
            unsubscribeLink?.Invoke();
            unsubscribeProject?.Invoke();
            unsubscribeLocalLimit?.Invoke();

            unsubscribeActiveLink = null;
            unsubscribeLink = null;
            unsubscribeProject = null;
            unsubscribeLocalLimit = null;

            mgrLink?.clear();
            localLimit?.clear();

            project = null;
            mgrLink = null;
            localLimit = null;
            lineTask = null;
            _name = null;
            _id = null;
        }
        #endregion
        #region Служебные
        #region Подсчет и выборка ограничения
        protected void limitCalculate()
        {
            switch (getLimitEntity())
            {
                case e_Entity.Project:
                    setLimitProject();
                    break;
                case e_Entity.Link:
                    setLimitActiveLink();
                    break;
                case e_Entity.Task:
                    setLimitLocal();
                    break;
                default:
                    throw new ApplicationException(nameof(getLimitEntity));
            }
        }
        protected e_Entity getLimitEntity()
        {
            switch (localLimit.limit)
            {
                case e_TLLim.FinishFixed:
                case e_TLLim.StartFixed:
                    return e_Entity.Task;



                case e_TLLim.Earlier:
                case e_TLLim.Later:
                    if (mgrLink.activeLink != null) return e_Entity.Link;
                    return e_Entity.Project;



                case e_TLLim.FinishNotEarlier:
                    if (mgrLink.activeLink != null)
                    {
                        e_Dot alDot = Hlp.GetFollower(mgrLink.activeLink.GetLimit());
                        DateTime alDate = mgrLink.activeLink.date;
                        if (alDot != e_Dot.Finish) alDate = alDate.AddDays(lineTask.duration);

                        if (alDate >= localLimit.date) return e_Entity.Link;
                    }

                    if (localLimit.date.AddDays(-lineTask.duration) < project.GetDot(e_Dot.Start).GetDate())
                        return e_Entity.Project;
                    else
                        return e_Entity.Task;
                    


                case e_TLLim.StartNotEarlier:
                    if (mgrLink.activeLink != null)
                    {
                        e_Dot alDot = Hlp.GetFollower(mgrLink.activeLink.GetLimit());
                        DateTime alDate = mgrLink.activeLink.date;
                        if (alDot != e_Dot.Start) alDate = alDate.AddDays(-lineTask.duration);

                        if (alDate >= localLimit.date) return e_Entity.Link;
                    }

                    if (localLimit.date < project.GetDot(e_Dot.Start).GetDate())
                        return e_Entity.Project;
                    else
                        return e_Entity.Task;



                case e_TLLim.FinishNotLater:
                    if (mgrLink.activeLink != null)
                    {
                        e_Dot alDot = Hlp.GetFollower(mgrLink.activeLink.GetLimit());
                        DateTime alDate = mgrLink.activeLink.date;
                        if (alDot != e_Dot.Finish) alDate = alDate.AddDays(lineTask.duration);

                        if (alDate <= localLimit.date) return e_Entity.Link;
                        else return e_Entity.Task;
                    }

                    if(localLimit.date.AddDays(-lineTask.duration) < project.GetDot(e_Dot.Start).GetDate())
                        return e_Entity.Task;
                    else 
                        return e_Entity.Project;



                case e_TLLim.StartNotLater:
                    if (mgrLink.activeLink != null)
                    {
                        e_Dot alDot = Hlp.GetFollower(mgrLink.activeLink.GetLimit());
                        DateTime alDate = mgrLink.activeLink.date;
                        if (alDot != e_Dot.Start) alDate = alDate.AddDays(-lineTask.duration);

                        if (alDate <= localLimit.date) return e_Entity.Link;
                        else return e_Entity.Task;
                    }

                    if (localLimit.date < project.GetDot(e_Dot.Start).GetDate())
                        return e_Entity.Task;
                    else
                        return e_Entity.Project;

                default:
                    throw new ApplicationException(nameof(localLimit.limit));
            }
        }
        protected void setLimitProject()
        {
            
            if (localLimit.limit == e_TLLim.Earlier)
            {
                dependDot = e_Dot.Start;
                limitDate = project.GetDot(e_Dot.Start).GetDate();
                limitEntity = e_Entity.Project;
                lineTask.move(dependDot, limitDate);
            }
            else if(localLimit.limit == e_TLLim.Later)
            {
                dependDot = e_Dot.Finish;
                limitDate = project.GetDot(e_Dot.Finish).GetDate();
                limitEntity = e_Entity.Project;
                lineTask.move(dependDot, limitDate);
            }
            else throw new ApplicationException("Локальное ограничение задачи не содержит ограничения по проекту");
        }
        protected void setLimitActiveLink()
        {
            dependDot = Hlp.GetFollower(mgrLink.activeLink.GetLimit());
            limitDate = mgrLink.activeLink.date;
            limitEntity = e_Entity.Link;
            lineTask.move(dependDot, limitDate);
        }

        protected void setLimitLocal()
        {
            dependDot = localLimit.dependDot;
            limitDate = localLimit.date;
            lineTask.move(dependDot, limitDate);
        }
        #endregion
        #region Подписки
        #region Проект
        protected void subscribe_Project()
        {
            project.GetDot(e_Dot.Start).event_DateChanged += handler_projectStartChanged;
            project.GetDot(e_Dot.Finish).event_DateChanged += handler_projectFinishChanged;

            unsubscribeProject = () =>
            {
                project.GetDot(e_Dot.Start).event_DateChanged -= handler_projectStartChanged;
                project.GetDot(e_Dot.Finish).event_DateChanged -= handler_projectFinishChanged;
            };
        }
        #endregion
        #region Связь
        protected void subscribe_linkManager()
        {
            mgrLink.event_activeLinkChanged += handler_activeLinkChanged;

            unsubscribeLink = () =>
                mgrLink.event_activeLinkChanged -= handler_activeLinkChanged;
        }
        protected void subscribe_activeLink(ea_ValueChange<ILink_2> links)
        {
            unsubscribeActiveLink?.Invoke();

            if (links.NewValue != null)
            {
                links.NewValue.event_dateChanged += handler_activeLinkDateChanged;
                links.NewValue.event_LimitChanged += handler_activeLinkLimitChanged;

                unsubscribeActiveLink = () =>
                {
                    links.NewValue.event_dateChanged -= handler_activeLinkDateChanged;
                    links.NewValue.event_LimitChanged -= handler_activeLinkLimitChanged;
                };
            }
            else unsubscribeActiveLink = null;
        }
        #endregion
        #region Локальная зависимость
        protected void subscribe_localLimit()
        {
            localLimit.event_dateChanged += handler_localLimitDateChanged;
            localLimit.event_limitChanged += handler_localLimit_limitChanged;

            unsubscribeLocalLimit = () =>
            {
                localLimit.event_dateChanged -= handler_localLimitDateChanged;
                localLimit.event_limitChanged -= handler_localLimit_limitChanged;
            };
        }
        #endregion
        #endregion
        #endregion
        #region Интерфейсы
        #region 

        #endregion
        #endregion
    }
    #endregion
    #region Реализация интерфейсов
    public partial class task2 : ITask2
    {
        #region Методы интерфейсов
        #region IRemovable
        public void DeleteObject()
        {
            clear();
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
        #region ILine
        public IDot GetDot(e_Dot type)
        {
            return lineTask.GetDot(type);
        }
        public double GetDuration()
        {
            return lineTask.duration;
        }
        #endregion
        #region ILimit
        public e_TLLim GetLimit()
        {
            return localLimit.limit;
        }
        public bool SetLimit(e_TLLim limitType)
        {
            if (limitType == localLimit.limit) return false;

            localLimit.limit = limitType;
            
            return true;
        }
        #endregion
        #endregion
    }
    #endregion
    #region Внутренние сущности
    public partial class task2
    {
        #region Локальное ограничение
        protected class cLocalLimit
        {
            protected cLimit<e_TLLim> _limit;
            protected DateTime _date;
            protected e_Dot _dependDot;
            protected Action unsubscribe;
            public e_Dot dependDot
            {
                get { return _dependDot; }
                protected set
                {
                    if(value == _dependDot) return;

                    _dependDot = value;
                }
            }
            public e_TLLim limit
            {
                get { return _limit; }
                set { _limit.limit = value; }
            }
            public DateTime date
            {
                get { return _date; }
                set
                {
                    if (value == _date) return;

                    DateTime old = _date;
                    _date = value;

                    event_dateChanged?.Invoke(this, new ea_ValueChange<DateTime>(old, _date));
                }
            }
            public event EventHandler<ea_ValueChange<DateTime>> event_dateChanged;
            public event EventHandler<ea_ValueChange<e_TLLim>> event_limitChanged
            {
                add { _limit.event_LimitChanged += value; }
                remove { _limit.event_LimitChanged -= value; }
            }

            public cLocalLimit()
            {
                _limit = new cLimit<e_TLLim>(this, task2.DEFAULT_VALUE_LOCAL_LIMIT);
                _limit.event_LimitChanged += handler_limitChanged;

                unsubscribe = () =>
                {
                    if (_limit != null)
                    {
                        _limit.event_LimitChanged -= handler_limitChanged;
                        _limit = null;
                    }
                };
            }
            ~cLocalLimit()
            {
                clear();
            }
            private void handler_limitChanged(object sender, ea_ValueChange<e_TLLim> e)
            {
                switch (e.NewValue)
                {
                    case e_TLLim.Earlier:
                    case e_TLLim.StartFixed:
                    case e_TLLim.StartNotEarlier:
                    case e_TLLim.StartNotLater:
                        dependDot = e_Dot.Start;
                    break;
                    case e_TLLim.Later:
                    case e_TLLim.FinishFixed:
                    case e_TLLim.FinishNotEarlier:
                    case e_TLLim.FinishNotLater:
                        dependDot = e_Dot.Finish;
                    break;
                }
            }

            public void clear()
            {
                unsubscribe?.Invoke();
                unsubscribe = null;
            }
        }
        #endregion
    }
    #endregion
}
