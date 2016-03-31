using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.iface;
using alter.Link.iface.Base;
using alter.types;
using alter.classes;
using alter.Service.classes;

namespace alter.Link.classes
{
    public partial class linkManager2 : ILinkManager2
    {
        #region Переменные
        protected IConnectible owner;
        #endregion
        #region Классы
        protected linkStorage storage;
        #endregion
        #region Свойства
        public ILink_2 activeLink
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public int linkCount => storage.count;
        #endregion
        #region События
        public event EventHandler<ea_ValueChange<ILink_2>> event_activeLinkChanged;
        public event EventHandler<ILink_2> event_linkAdded;
        public event EventHandler<ILink_2> event_linkUnsubscribed;
        #endregion
        #region Конструктор

        #endregion
        #region Инициализаторы
        protected void init_LinkStorage()
        {
            storage = new linkStorage(owner);
        }
        #endregion
        #region Обработчики

        #endregion
        #region Методы
        public bool addToLink(e_DependType role, ILink_2 link)
        {
            throw new NotImplementedException();
        }
        public ILink_2[] getLinks()
        {
            return storage.getLinks();
        }
        public ILink_2[] getLinks(e_DependType depend)
        {
            return storage.getLinks(depend);
        }
        public ILink_2 getLink(string linkID)
        {
            return storage.getLink(linkID);
        }
        public bool isLinkExist(string linkID)
        {
            return storage.isLinkExist(linkID);
        }
        public void clear()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    #region Внутренние сущности
    public partial class linkManager2 : ILinkManager2
    {
        #region Хранилище
        protected class linkStorage
        {
            #region Переменные
            protected Dictionary<string, linkInfo> vault;
            protected HashSet<string> neighbours;
            protected IId owner;
            #endregion
            #region Свойства
            public linkInfo[] links => vault.Values.ToArray();
            public int count => vault.Count;
            #endregion
            #region События
            public EventHandler<KeyValuePair<e_DependType, ILink_2>> event_linkAdded;
            public event EventHandler<ILink_2> event_linkRemoved;
            #endregion
            #region Конструктор
            public linkStorage(IId owner)
            {
                vault = new Dictionary<string, linkInfo>();
                neighbours = new HashSet<string>();
            }
            ~linkStorage()
            {
                vault = null;
                neighbours = null;
                owner = null;
            }
            #endregion
            #region Обработчики

            protected void linkRemovedHandler(object sender, ea_IdObject e)
            {
                removeLink(e.Id.Id);
            }
            #endregion
            #region Методы
            public bool add(ILink_2 link)
            {
                if(link == null) throw new ArgumentNullException(nameof(link));
                if (vault.Keys.Contains(link.GetId())) return false;

                e_DependType ownerDepend = getOwnerDepend(link);
                e_DependType neighbourDepend = ownerDepend == e_DependType.Slave
                    ? e_DependType.Master
                    : e_DependType.Slave;
                string NID = link.getMemberID(neighbourDepend).GetId();


                if (!neighbours.Add(NID)) return false;
                
                linkInfo newLink = new linkInfo(link);
                vault.Add(newLink.ID, newLink);

                link.event_ObjectDeleted += linkRemovedHandler;

                event_linkAdded?.Invoke(this, new KeyValuePair<e_DependType, ILink_2>(ownerDepend, link));

                return true;
            }
            public bool remove(string linkID)
            {
                if (!vault.Keys.Contains(linkID)) return false;

                vault[linkID].link.unsubscribe(owner.GetId());
                return true;
            }
            public void setDuration(double duration)
            {
                if(count == 0) return;

                string[] keys = vault.Keys.ToArray();
                linkInfo temp;

                for (int i = 0; i < keys.Length; i++)
                {
                    temp = vault[keys[i]];
                    temp.duration = duration;
                    vault[keys[i]] = temp;
                }
            }
            public void clear()
            {
                if (count > 0)
                {
                    foreach (linkInfo val in vault.Values)
                    {
                        val.link.unsubscribe(owner.GetId());
                    }
                }
            }
            #endregion
            #region Интерфейс
            public bool isLinkExist(string linkID)
            {
                if (vault.Keys.Contains(linkID)) return true;
                return false;
            }
            public ILink_2 getLink(string linkID)
            {
                if (!isLinkExist(linkID)) throw new ApplicationException(nameof(linkID));
                return vault[linkID].link;
            }
            public ILink_2[] getLinks(e_DependType depend)
            {
                if(!Enum.IsDefined(typeof(e_DependType), depend)) throw new ArgumentException(nameof(depend));

                return vault.Values.Where(v => v.dependOwner == depend).Select(v => v.link).ToArray();
            }
            public ILink_2[] getLinks()
            {
                if (count > 0) return vault.Values.Select(v => v.link).ToArray();
                return new ILink_2[0];
            }

            #endregion
            #region Служебные
            protected void removeLink(string linkID)
            {
                ILink_2 link = vault[linkID].link;

                event_linkRemoved?.Invoke(this, link);

                if (!neighbours.Remove(link.getMemberID(getNeighbourDepend(link)).GetId()))
                    throw new ApplicationException(nameof(neighbours));

                vault[linkID].clear();

                if (!vault.Remove(linkID))
                    throw new ApplicationException(nameof(vault));
            }
            protected e_DependType getOwnerDepend(ILink_2 link)
            {
                return link.getDependType(owner.GetId());
            }
            protected e_DependType getNeighbourDepend(ILink_2 link)
            {
                return getOwnerDepend(link) == e_DependType.Master ? e_DependType.Slave : e_DependType.Master;
            }
            #endregion
        }
        #endregion
        #region Активная связь
        protected class activeLinkClass
        {
            #region Переменные
            protected linkManager2 owner;
            protected Dictionary<string, Action> linksID;
            protected string _activeLink;
            protected double _duration;
            #endregion
            #region Свойства
            
            #endregion
            #region События

            #endregion
            #region Конструктор

            #endregion
            #region Обработчики
            protected void handler_linkAdded(object sender, KeyValuePair<e_DependType, ILink_2> e)
            {
                throw new NotImplementedException();
            }
            protected void handler_linkRemoved(object sender, ILink_2 e)
            {
                throw new NotImplementedException();
            }
            protected void handler_durationChanged(object sender, ea_ValueChange<double> e)
            {
                throw new NotImplementedException();
            }
            protected void handler_dateChanged(object sender, ea_ValueChange<DateTime> e)
            {
                throw new NotImplementedException();
            }
            protected void handler_limitChanged(object sender, ea_ValueChange<e_TskLim> e)
            {
                throw new NotImplementedException();
            }
            #endregion
            #region Служебные
            #region Подписки отписки
            protected void unsubscribeDuration()
            {
                owner.owner.event_DurationChanged -= handler_durationChanged;
            }
            protected void subscribeDuration()
            {
                owner.owner.event_DurationChanged += handler_durationChanged;
            }
            protected void unsubscribeStorage()
            {
                owner.storage.event_linkAdded -= handler_linkAdded;
                owner.storage.event_linkRemoved -= handler_linkRemoved;
            }
            protected void subscribeStorage()
            {
                owner.storage.event_linkAdded += handler_linkAdded;
                owner.storage.event_linkRemoved += handler_linkRemoved;
            }
            protected Action subscribeLink(ILink_2 link)
            {
                link.event_dateChanged += handler_dateChanged;
                link.event_LimitChanged += handler_limitChanged;

                return () =>
                {
                    link.event_dateChanged -= handler_dateChanged;
                    link.event_LimitChanged -= handler_limitChanged;
                };
            }
            protected void unsubscribeLink(string linkID)
            {
                linksID[linkID]();
                linksID.Remove(linkID);
            }
            #endregion
            #region Длительность
            protected void durationChanged(double duration)
            {
                if (duration != _duration)
                {
                    _duration = duration;
                    owner.storage.setDuration(_duration);
                }
            }
            #endregion
            #region Активная связь
            
            #endregion
            #endregion
        }
        #endregion
        #region Информация о связи
        protected class linkInfo : IComparable<linkInfo>
        {
            #region Константы
            public const e_Dot calculateDot = e_Dot.Finish;
            #endregion
            #region Переменная
            protected ILink_2 _link;

            protected readonly e_DependType _ownerRole;
            protected readonly e_DependType _neighbourRole;

            protected e_Dot _dependDot;

            protected double duration;

            protected DateTime _relativeDate;

            protected Action unsubscribeLink;
            protected Action unsubscribeOwner;
            #endregion
            #region Свойства
            public ILink_2 link => _link;

            public string ID => _link.GetId();

            public DateTime date => _link.date;
            public DateTime relativeDate => _relativeDate;

            public e_Dot dependDot => _dependDot;

            public e_DependType roleOwner => _ownerRole;
            public e_DependType roleNeighbour => _neighbourRole;

            public IId idNeighbour => _link.getMemberID(_neighbourRole);
            #endregion
            #region События
            public event EventHandler<ea_ValueChange<DateTime>> event_relativeDateChanged;
            public event EventHandler<e_Dot> event_dependDotChanged;
            public event EventHandler<linkInfo> event_linkRemoved;
            #endregion
            #region Конструктор
            public linkInfo(IConnectible owner, ILink_2 link)
            {
                _ownerRole = link.getDependType(owner.GetId());
                _neighbourRole = _ownerRole == e_DependType.Master ? e_DependType.Slave : e_DependType.Master;

                if (_ownerRole == e_DependType.Master)
                {
                    unsubscribeOwner = () => { };
                    duration = 0;
                }
                else
                {
                    owner.event_DurationChanged += handler_durationChanged;

                    duration = owner.GetDuration();

                    unsubscribeOwner = () => owner.event_DurationChanged -= handler_durationChanged;
                }

                unsubscribeLink = subscribeLink(link);
            }
            ~linkInfo()
            {
                clear();
            }
            #endregion
            #region Обработчики
            private void handler_durationChanged(object sender, ea_ValueChange<double> e)
            {
                duration = e.NewValue;

                update_relativeDate();
            }
            private void handler_limitChanged(object sender, ea_ValueChange<e_TskLim> e)
            {
                update_dependDot(e.NewValue);
            }
            private void handler_dateChanged(object sender, ea_ValueChange<DateTime> e)
            {
                update_relativeDate();
            }
            private void handler_linkRemoved(object sender, ea_IdObject e)
            {
                event_linkRemoved?.Invoke(this, this);
                clear();
            }
            #endregion
            #region Методы
            public void clear()
            {
                unsubscribeOwner?.Invoke();
                unsubscribeLink?.Invoke();
                unsubscribeOwner = null;
                unsubscribeLink = null;
                _link = null;
            }
            #endregion
            #region Служебные
            protected Action subscribeLink(ILink_2 link)
            {
                link.event_LimitChanged += handler_limitChanged;
                link.event_dateChanged += handler_dateChanged;
                link.event_ObjectDeleted += handler_linkRemoved;

                return () =>
                {
                    link.event_LimitChanged -= handler_limitChanged;
                    link.event_dateChanged -= handler_dateChanged;
                    link.event_ObjectDeleted -= handler_linkRemoved;
                };
            }
            protected void update_dependDot(e_TskLim limit)
            {
                e_Dot dot = Hlp.GetDepenDot(limit, _ownerRole);
                if (dot != _dependDot)
                {
                    _dependDot = dot;

                    update_relativeDate();

                    event_dependDotChanged?.Invoke(this, _dependDot);
                }
            }
            protected void update_relativeDate()
            {
                DateTime rdate = new DateTime(1,1,1);

                if (calculateDot == e_Dot.Finish)
                {
                    if (_dependDot == e_Dot.Finish) rdate = link.date;
                    else rdate = link.date.AddDays(duration);
                }
                else
                {
                    if (_dependDot == e_Dot.Start) rdate = link.date;
                    else rdate = link.date.AddDays(-duration);
                }

                if (_relativeDate != rdate)
                {
                    DateTime old = _relativeDate;
                    _relativeDate = rdate;

                    event_relativeDateChanged?.Invoke(this, new ea_ValueChange<DateTime>(old, rdate));
                }
            }
            #endregion
            #region Интерфейс
            public int CompareTo(linkInfo other)
            {
                if (relativeDate > other.relativeDate) return 1;
                else if (relativeDate == other.relativeDate) return 0;
                else return -1;
            }
            #endregion
        }
        #endregion
    }
    #endregion
    public partial class linkManager2 : ILinkManager2
    {
    }
}
