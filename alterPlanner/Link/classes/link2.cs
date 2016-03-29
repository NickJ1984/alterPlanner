using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.classes;
using alter.iface;
using alter.Link.iface.Base;
using alter.Service.classes;
using alter.types;

namespace alter.Link.classes
{
    public partial class link_2 : ILink_2
    {
        #region Константы
        public const double DELAY_MINIMUM_VALUE = 0;
        public const e_TskLim LIMIT_DEFAULT_VALUE = e_TskLim.FinishStart;
        #endregion
        #region Переменные
        protected double _delay;
        #endregion
        #region Делегаты
        protected Action unsubscribeLimit;
        protected Action unsubscribePDate;
        protected Action unsubscribeStorage;
        #endregion
        #region Классы
        protected Identity _id;
        protected memberStorage storage;
        protected precursorDate pDate;
        protected cLimit<e_TskLim> _limit;
        #endregion
        #region Свойства
        public e_TskLim limit
        {
            get { return _limit; }
            set { _limit.limit = value; }
        }
        public DateTime date { get; protected set; }
        public double delay
        {
            get { return _delay; }
            set
            {
                if(value == _delay || value < DELAY_MINIMUM_VALUE) return;

                double temp = _delay;
                _delay = value;

                event_delayChanged?.Invoke(this, new ea_ValueChange<double>(temp, _delay));

                updateDate();
            }
        }
        #endregion
        #region События
        public event EventHandler<ea_ValueChange<double>> event_delayChanged;
        public event EventHandler<ea_ValueChange<DateTime>> event_dateChanged;
        public event EventHandler<ea_ValueChange<e_TskLim>> event_LimitChanged;
        public event EventHandler<ea_IdObject> event_ObjectDeleted;
        #endregion
        #region Конструктор
        public link_2(IConnectible precursor, IConnectible follower, e_TskLim limit, double delay)
        {
            if(delay < DELAY_MINIMUM_VALUE) throw new ArgumentException(nameof(delay));

            init_Identity();
            init_Limit(limit);
            init_Storage(follower, precursor);
            init_PrecursorDate(storage.precursor, storage.dotPrecursor);
        }
        public link_2(IConnectible precursor, IConnectible follower, e_TskLim limit)
            :this(precursor, follower, limit, DELAY_MINIMUM_VALUE)
        { }
        public link_2(IConnectible precursor, IConnectible follower)
            :this(precursor, follower, LIMIT_DEFAULT_VALUE, DELAY_MINIMUM_VALUE)
        { }
        #endregion
        #region Деструктор
        ~link_2()
        {
            unsubscribeLimit?.Invoke();
            unsubscribePDate?.Invoke();
            unsubscribeStorage?.Invoke();
            unsubscribeLimit = null;
            unsubscribePDate = null;
            unsubscribeStorage = null;
            _id = null;
            storage?.clear();
            storage = null;
            pDate?.clear();
            pDate = null;
            _limit = null;
        }
        #endregion
        #region Инициализаторы
        protected void init_Identity()
        {
            _id = new Identity(e_Entity.Link);
        }
        protected void init_Limit(e_TskLim limit)
        { 
            _limit = new cLimit<e_TskLim>(this, limit);

            _limit.event_LimitChanged += limitChangedHandler;

            unsubscribeLimit = () => _limit.event_LimitChanged -= limitChangedHandler;
        }
        protected void init_Storage(IConnectible follower, IConnectible precursor)
        {
            storage = new memberStorage(precursor, follower, _limit);

            storage.event_memberRemoved += memberRemovedHandler;

            unsubscribeStorage = () => storage.event_memberRemoved -= memberRemovedHandler;
        }
        protected void init_PrecursorDate(ILine precursor, e_Dot dependDot)
        {
            pDate = new precursorDate(precursor, dependDot);

            pDate.event_dateChanged += dateChangedHandler;

            unsubscribePDate = () => pDate.event_dateChanged -= dateChangedHandler;
        }
        #endregion
        #region Внутренние обработчики
        protected void memberRemovedHandler(object sender, ea_IdObject e)
        {
            DeleteObject();
        }
        protected void limitChangedHandler(object sender, ea_ValueChange<e_TskLim> e)
        {
            storage.limitChanged(e.NewValue);

            event_LimitChanged?.Invoke(this, e);

            pDate.setDot(storage.dotPrecursor);
        }
        protected void dateChangedHandler(object sender, ea_ValueChange<DateTime> e)
        {
            updateDate();
        }
        #endregion
        #region Методы
        public ILine getMember(e_DependType dependType)
        {
            return storage.getMember(dependType);
        }
        public ILine getMember(string memberID)
        {
            return storage.getMember(memberID);
        }

        public IId getMemberID(e_DependType dependType)
        {
            return storage.getMemberID(dependType);
        }
        public IId getMemberID(string memberID)
        {
            return storage.getMemberID(memberID);
        }


        public bool isMemberExist(IId identity)
        {
            return storage.isMemberExist(identity);
        }
        public bool isMemberExist(string memberID)
        {
            return isMemberExist(memberID);
        }

        public e_DependType getDependType(string memberID)
        {
            return storage.getDependType(memberID);
        }
        public void unsubscribe(string memberID)
        {
            DeleteObject();
        }
        #endregion
        #region Служебные
        protected void updateDate()
        {
            DateTime newDate = pDate.date.AddDays(delay);

            if (date != newDate)
            {
                DateTime old = date;
                date = newDate;

                event_dateChanged?.Invoke(this, new ea_ValueChange<DateTime>(old, date));
            }
        }
        #endregion
        #region Интерфейсы
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
        #region ILimit
        public e_TskLim GetLimit()
        {
            return _limit;
        }
        public bool SetLimit(e_TskLim limitType)
        {
            return _limit.SetLimit(limitType);
        }
        #endregion
        #region IRemovable
        public void DeleteObject()
        {
            event_ObjectDeleted?.Invoke(this, new ea_IdObject(this));

            unsubscribeLimit();
            unsubscribePDate();
            unsubscribeStorage();
            unsubscribeLimit = null;
            unsubscribePDate = null;
            unsubscribeStorage = null;

            storage.clear();
            pDate.clear();
        }
        #endregion
        #endregion
    }
    #region Внутренние сущности
    public partial class link_2 : ILink_2
    {
        #region структура члена связи
        protected struct memberData
        {
            public e_DependType depend;
            public e_Dot dependDot;
            public DateTime date => member.GetDot(dependDot).GetDate();

            public IConnectible member;

            public memberData(IConnectible member, e_DependType type, e_Dot dot)
            {
                this.member = member;
                depend = type;
                dependDot = dot;
            }
            public memberData(IConnectible member, e_DependType type, e_TskLim limit)
            {
                this.member = member;
                depend = type;
                dependDot = depend == e_DependType.Master ? 
                    Hlp.GetPrecursor(limit) : Hlp.GetFollower(limit);
            }

            public void updateDependDot(e_TskLim limit)
            {
                dependDot = depend == e_DependType.Master ? Hlp.GetPrecursor(limit) : Hlp.GetFollower(limit);
            }

            public void clear()
            {
                member = null;
            }
        }
        #endregion
        #region Хранилище членов
        protected class memberStorage
        {
            protected memberData _follower;
            protected memberData _precursor;

            protected DateTime pDate;

            protected Action unsubscribeFollower;
            protected Action unsubscribePrecursor;


            public IConnectible follower => _follower.member;
            public IConnectible precursor => _precursor.member;
            public e_Dot dotFollower => _follower.dependDot;
            public e_Dot dotPrecursor => _precursor.dependDot;

            public event EventHandler<ea_IdObject> event_memberRemoved;

            public memberStorage(IConnectible precursor, IConnectible follower, e_TskLim limit)
            {
                if(precursor == null || follower == null) throw new ArgumentNullException();
                if(!Enum.IsDefined(typeof(e_TskLim), limit)) throw new ArgumentException(nameof(limit));

                _follower = new memberData(follower, e_DependType.Slave, limit);
                _precursor = new memberData(precursor, e_DependType.Master, limit);

                follower.event_ObjectDeleted += memberRemovedHandler;
                precursor.event_ObjectDeleted += memberRemovedHandler;

                unsubscribeFollower = () => follower.event_ObjectDeleted -= memberRemovedHandler;
                unsubscribePrecursor = () => precursor.event_ObjectDeleted -= memberRemovedHandler;
            }

            
            public void limitChanged(e_TskLim limit)
            {
                _follower.updateDependDot(limit);
                _precursor.updateDependDot(limit);
            }
            public void clear()
            {
                unsubscribeFollower?.Invoke();
                unsubscribePrecursor?.Invoke();
                unsubscribeFollower = null;
                unsubscribePrecursor = null;

                _follower.clear();
                _precursor.clear();
            }


            public ILine getMember(e_DependType dependType)
            {
                if(!Enum.IsDefined(typeof(e_DependType), dependType))
                    throw new ArgumentException(nameof(dependType));

                return dependType == e_DependType.Master ? precursor : follower;
            }
            public IId getMemberID(e_DependType dependType)
            {
                if (!Enum.IsDefined(typeof(e_DependType), dependType))
                    throw new ArgumentException(nameof(dependType));

                return dependType == e_DependType.Master ? precursor : follower;
            }
            

            public ILine getMember(string memberID)
            {
                if(string.IsNullOrEmpty(memberID)) throw new ArgumentNullException(nameof(memberID));

                if (_follower.member.GetId() == memberID) return follower;
                else if (_precursor.member.GetId() == memberID) return precursor;
                else return null;
            }
            public IId getMemberID(string memberID)
            {
                if (string.IsNullOrEmpty(memberID)) throw new ArgumentNullException(nameof(memberID));

                if (_follower.member.GetId() == memberID) return follower;
                else if (_precursor.member.GetId() == memberID) return precursor;
                else return null;
            }

            public e_DependType getDependType(string memberID)
            {
                if (string.IsNullOrEmpty(memberID)) throw new ArgumentNullException(nameof(memberID));

                if (_follower.member.GetId() == memberID) return _follower.depend;
                else if (_precursor.member.GetId() == memberID) return _precursor.depend;
                else throw new ArgumentException(String.Format("Член с идентификатором {0} не содержится в связи", memberID));
            }

            public bool isMemberExist(IId identity)
            {
                if(identity == null) throw new ArgumentNullException(nameof(identity));

                IId flwIID = _follower.member;
                IId prcIID = _precursor.member;

                if (identity == prcIID || identity == flwIID) return true;
                else return false;
            }
            public bool isMemberExist(string memberID)
            {
                if(string.IsNullOrEmpty(memberID)) throw new ArgumentNullException(nameof(memberID));

                if (_follower.member.GetId() == memberID ||
                    _precursor.member.GetId() == memberID) return true;
                else return false;
            }


            protected void memberRemovedHandler(object sender, ea_IdObject e)
            {
                event_memberRemoved?.Invoke(this, e);
            }
        }
        #endregion
        #region Отслеживание даты предшественника
        protected class precursorDate
        {
            protected ILine precursor;

            protected e_Dot dependDot;
            protected DateTime _date;

            protected Action unsubscribe;

            public DateTime date
            {
                get { return _date; }
                private set
                {
                    if(_date == value) return;

                    DateTime old = _date;
                    _date = value;

                    event_dateChanged?.Invoke(this, new ea_ValueChange<DateTime>(old, _date));
                }
            }

            public event EventHandler<ea_ValueChange<DateTime>> event_dateChanged;

            public precursorDate(ILine precursor, e_Dot dependDot)
            {
                this.precursor = precursor;
                this.dependDot = dependDot;

                _date = precursor.GetDot(dependDot).GetDate();
                unsubscribe = subscribe(dependDot);
            }
            ~precursorDate()
            {
                unsubscribe?.Invoke();
                unsubscribe = null;
                precursor = null;
            }

            public void setDot(e_Dot dot)
            {
                if (dependDot == dot) return;
                else dependDot = dot;

                unsubscribe?.Invoke();
                unsubscribe = subscribe(dot);

                date = precursor.GetDot(dot).GetDate();
            }

            public void clear()
            {
                unsubscribe?.Invoke();
            }

            protected Action subscribe(e_Dot dot)
            {
                precursor.GetDot(dot).event_DateChanged += handler_dateChanged;

                return () => precursor.GetDot(dot).event_DateChanged -= handler_dateChanged;
            }

            protected void handler_dateChanged(object sender, ea_ValueChange<DateTime> e)
            {
                date = e.NewValue;
            }

            public static implicit operator DateTime(precursorDate instance)
            {
                return instance._date;
            }
        }
        #endregion
    }
    #endregion
    #region Вспомогательные интерфейсы
    public interface IConnectible : IId, IRemovable, ILine
    {
    }
    #endregion
    public partial class link_2 : ILink_2
    {

    }
}
