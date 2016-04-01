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
using alter.Service.Extensions;

namespace alter.Link.classes
{
    public partial class linkManager2 : ILinkManager2
    {
        #region Переменные
        protected IConnectible owner;
        #endregion
        #region Классы
        protected linkStorage storage;
        protected activeLinkClass aLink;
        #endregion
        #region Делегаты
        protected Action unsubscribeALink;
        protected Action unsubscribeStorage;
        #endregion
        #region Свойства
        public ILink_2 activeLink => aLink.activeLink != null ? aLink.activeLink.link : null;
        public int linkCount => storage.count;
        #endregion
        #region События
        public event EventHandler<ea_ValueChange<ILink_2>> event_activeLinkChanged;
        public event EventHandler<ILink_2> event_linkAdded;
        public event EventHandler<ILink_2> event_linkUnsubscribed;
        #endregion
        #region Конструктор
        public linkManager2(IConnectible owner)
        {
            if(owner == null) throw new ArgumentNullException(nameof(owner));

            this.owner = owner;

            init_LinkStorage();
            init_ActiveLinkClass();
        }
        ~linkManager2()
        {
            clear();
            owner = null;
        }
        #endregion
        #region Инициализаторы
        protected void init_LinkStorage()
        {
            storage = new linkStorage(owner);

            storage.event_linkAdded += handler_linkAdded;
            storage.event_linkRemoved += handler_linkRemoved;

            unsubscribeStorage = () =>
            {
                storage.event_linkAdded -= handler_linkAdded;
                storage.event_linkRemoved -= handler_linkRemoved;
            };
        }
        protected void init_ActiveLinkClass()
        {
            aLink = new activeLinkClass(this);
            aLink.setDuartion(owner.GetDuration());

            storage.event_linkAdded += aLink.handler_linkAdded;
            storage.event_linkRemoved += aLink.handler_linkRemoved;
            owner.event_DurationChanged += aLink.handler_durationChanged;
            aLink.event_activeLinkChanged += handler_activeLinkChanged;

            unsubscribeALink = () =>
            {
                storage.event_linkAdded -= aLink.handler_linkAdded;
                storage.event_linkRemoved -= aLink.handler_linkRemoved;
                owner.event_DurationChanged -= aLink.handler_durationChanged;
                aLink.event_activeLinkChanged -= handler_activeLinkChanged;
            };
        }
        #endregion
        #region Обработчики
        protected void handler_linkAdded(object sender, linkInfo e)
        {
            event_linkAdded?.Invoke(this, e.link);
        }
        protected void handler_linkRemoved(object sender, linkInfo e)
        {
            event_linkUnsubscribed?.Invoke(this, e.link);
        }
        protected void handler_activeLinkChanged(object sender, ea_ValueChange<linkInfo> e)
        {
            event_activeLinkChanged?.Invoke(this, new ea_ValueChange<ILink_2>(e.OldValue.link, e.NewValue.link));
        }
        #endregion
        #region Методы
        public bool addToLink(ILink_2 link)
        {
            return storage.add(link);
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
            unsubscribeStorage?.Invoke();
            unsubscribeALink?.Invoke();
            unsubscribeStorage = null;
            unsubscribeALink = null;
            aLink.clear();
            storage.clear();
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
            protected IConnectible owner;
            #endregion
            #region Свойства
            public linkInfo[] links => vault.Values.ToArray();
            public int count => vault.Count;
            #endregion
            #region События
            public EventHandler<linkInfo> event_linkAdded;
            public EventHandler<linkInfo> event_linkRemoved;
            #endregion
            #region Конструктор
            public linkStorage(IConnectible owner)
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
            protected void linkInfoRemovedHandler(object sender, ea_IdObject e)
            {
                removeLink(e.Id.Id);
            }
            #endregion
            #region Методы
            public bool add(ILink_2 link)
            {
                if(link == null) throw new ArgumentNullException(nameof(link));
                if (vault.Keys.Contains(link.GetId())) return false;

                linkInfo newLink = new linkInfo(owner, link);

                if (!neighbours.Add(newLink.idNeighbour.GetId()))
                {
                    newLink.clear();
                    return false;
                }
                
                vault.Add(newLink.ID, newLink);

                newLink.link.event_ObjectDeleted += linkInfoRemovedHandler;

                event_linkAdded?.Invoke(this, newLink);

                return true;
            }
            public bool remove(string linkID)
            {
                if (!vault.Keys.Contains(linkID)) return false;

                vault[linkID].link.unsubscribe(owner.GetId());
                return true;
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

                return vault.Values.Where(v => v.roleOwner == depend).Select(v => v.link).ToArray();
            }
            public ILink_2[] getLinks()
            {
                if (count > 0) return vault.Values.Select(v => v.link).ToArray();
                return new ILink_2[0];
            }
            public linkInfo[] getLinksInfo(params string[] linksID)
            {
                if (linksID.isNullOrEmpty()) return null;

                return vault.Where(v => linksID.Contains(v.Key)).Select(v => v.Value).ToArray();
            }
            public linkInfo getLinksInfo(string linkID)
            {
                if (string.IsNullOrEmpty(linkID)) return null;

                return vault[linkID];
            }
            #endregion
            #region Служебные
            protected void removeLink(string linkID)
            {
                linkInfo link = vault[linkID];

                if (!neighbours.Remove(link.idNeighbour.GetId()))
                    throw new ApplicationException(nameof(neighbours));

                if (!vault.Remove(link.ID))
                    throw new ApplicationException(nameof(vault));

                link.link.event_ObjectDeleted -= linkInfoRemovedHandler;

                event_linkRemoved?.Invoke(this, link);

                link.clear();
                vault.Remove(linkID);
            }
            #endregion
        }
        #endregion
        #region Активная связь
        protected class activeLinkClass
        {
            #region Флаги
            protected bool flag_relativeDateChangedPause = false;
            #endregion
            #region Переменные
            protected linkManager2 owner;
            protected Dictionary<string, Action> linksID;
            protected linkInfo _activeLink;
            protected double _duration;
            #endregion
            #region Свойства
            public linkInfo activeLink
            {
                get { return _activeLink; }
                protected set
                {
                    if(value == _activeLink) return;

                    linkInfo old = _activeLink;
                    _activeLink = value;

                    event_activeLinkChanged?.Invoke(this, new ea_ValueChange<linkInfo>(old, _activeLink));
                }
            }
            #endregion
            #region События
            public EventHandler<ea_ValueChange<linkInfo>> event_activeLinkChanged;
            #endregion
            #region Конструктор
            public activeLinkClass(linkManager2 owner)
            {
                this.owner = owner;
                _activeLink = null;
                _duration = 0;
                linksID = new Dictionary<string, Action>();
            }

            ~activeLinkClass()
            {
                owner = null;
                _activeLink = null;
                if (linksID != null)
                {
                    foreach (Action val in linksID.Values) val();
                    linksID.Clear();
                    linksID = null;
                }
            }
            #endregion
            #region Методы

            public void clear()
            {
                owner = null;
                _activeLink = null;
                if (linksID != null)
                {
                    foreach (Action val in linksID.Values) val();
                    linksID.Clear();
                    linksID = null;
                }
            }
            public void setDuartion(double duration)
            {
                if (duration == _duration) return;

                _duration = duration;

                if (linksID.Count > 0)
                {
                    flag_relativeDateChangedPause = true;
                    linkInfo[] links = owner.storage.getLinksInfo(linksID.Keys.ToArray());

                    for (int i = 0; i < links.Length; i++) links[i].duration = _duration;

                    flag_relativeDateChangedPause = false;
                    activeLink = findMax();
                }
            }
            #endregion
            #region Обработчики
            public void handler_linkAdded(object sender, linkInfo e)
            {
                if (e.roleOwner == e_DependType.Slave)
                {
                    addLink(e);
                    newLinkCheck(e);
                }
            }
            public void handler_linkRemoved(object sender, linkInfo e)
            {
                removeLink(e);
            }
            public void handler_durationChanged(object sender, ea_ValueChange<double> e)
            {
                setDuartion(e.NewValue);
            }
            protected void handler_relativeDateChanged(object sender, ea_ValueChange<DateTime> e)
            {
                if(flag_relativeDateChangedPause) return;
                relativeChanged((linkInfo)sender, e.NewValue);
            }
            #endregion
            #region Служебные
            #region Связи
            protected bool addLink(linkInfo linkValue)
            {
                if (linksID.ContainsKey(linkValue.ID)) return false;

                linkValue.duration = _duration;
                linksID.Add(linkValue.ID, subscribeLink(linkValue));

                return true;
            }
            protected void removeLink(linkInfo linkValue)
            {
                if(!linksID.ContainsKey(linkValue.ID)) return;
                
                unsubscribeLink(linkValue.ID);

                if (linkValue.ID == activeLink.ID) activeLink = findMax();
            }
            #endregion
            #region Подписки отписки
            protected Action subscribeLink(linkInfo link)
            {
                link.event_relativeDateChanged += handler_relativeDateChanged;

                return () =>
                {
                    link.event_relativeDateChanged -= handler_relativeDateChanged;
                };
            }
            protected void unsubscribeLink(string linkID)
            {
                linksID[linkID]();
                linksID.Remove(linkID);
            }
            #endregion
            #region Активная связь
            protected void newLinkCheck(linkInfo linkValue)
            {
                if (_activeLink == null) activeLink = linkValue;
                else if (linkValue.relativeDate > _activeLink.relativeDate) activeLink = linkValue;
            }
            protected void relativeChanged(linkInfo linkValue, DateTime rdate)
            {
                if (activeLink.ID == linkValue.ID) activeLink = findMax();
                else if (linkValue.relativeDate > activeLink.relativeDate) activeLink = linkValue;
            }
            protected linkInfo findMax()
            {
                if (linksID.Count == 0) return null;
                if (linksID.Count == 1) return owner.storage.getLinksInfo(linksID.Keys.First());

                linkInfo[] links = owner.storage.getLinksInfo(linksID.Keys.ToArray());
                return links.Max();
            }
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

            protected double _duration;

            protected DateTime _relativeDate;

            protected Action unsubscribeLink;
            #endregion
            #region Свойства
            public double duration
            {
                get { return _duration; }
                set
                {
                    if (value != _duration)
                    {
                        double old = _duration;
                        _duration = value;

                        update_relativeDate();
                    }
                }
            }
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
            #endregion
            #region Конструктор
            public linkInfo(IId owner, ILink_2 link)
            {
                _ownerRole = link.getDependType(owner.GetId());
                _neighbourRole = _ownerRole == e_DependType.Master ? e_DependType.Slave : e_DependType.Master;
                _duration = 0;

                unsubscribeLink = subscribeLink(link);

                update_dependDot(link.GetLimit());
                update_relativeDate();
            }
            ~linkInfo()
            {
                clear();
            }
            #endregion
            #region Обработчики
            public void handler_durationChanged(object sender, ea_ValueChange<double> e)
            {
                duration = e.NewValue;
            }
            private void handler_limitChanged(object sender, ea_ValueChange<e_TskLim> e)
            {
                update_dependDot(e.NewValue);
            }
            private void handler_dateChanged(object sender, ea_ValueChange<DateTime> e)
            {
                update_relativeDate();
            }
            #endregion
            #region Методы
            public void clear()
            {
                unsubscribeLink?.Invoke();
                unsubscribeLink = null;
                _link = null;
            }
            #endregion
            #region Служебные
            protected Action subscribeLink(ILink_2 link)
            {
                link.event_LimitChanged += handler_limitChanged;
                link.event_dateChanged += handler_dateChanged;

                return () =>
                {
                    link.event_LimitChanged -= handler_limitChanged;
                    link.event_dateChanged -= handler_dateChanged;
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
}
