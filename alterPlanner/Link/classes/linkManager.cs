using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.iface;
using alter.Link.iface;
using alter.Service.classes;
using alter.Service.iface;
using alter.types;

namespace alter.Link.classes
{
    #region Реализация интерфейса
    public partial class linkManager : ILinkManager
    {
        

        public bool connect(ILink link)
        {
            throw new NotImplementedException();
        }

        public bool delLink()
        {
            throw new NotImplementedException();
        }

        public bool delLink(e_DependType dependType)
        {
            throw new NotImplementedException();
        }

        public bool delLink(string linkId)
        {
            throw new NotImplementedException();
        }

        public ILink getActiveLink()
        {
            throw new NotImplementedException();
        }

        public ILink getLink(string linkID)
        {
            throw new NotImplementedException();
        }

        public string[] getLinks()
        {
            throw new NotImplementedException();
        }

        public string[] getLinks(e_DependType dependType)
        {
            throw new NotImplementedException();
        }

        public int GetLinksCount(e_DependType dependType)
        {
            throw new NotImplementedException();
        }

        public bool LinkExist(string linkID)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
    #region Основные переменные
    public partial class linkManager : ILinkManager
    {
        protected IId owner;
        protected Vault links;
        protected Watcher watcher;
    }
    #endregion
    #region Основные события и их инвокеры
    public partial class linkManager : ILinkManager
    {
        #region Внешние события
        public event EventHandler<ea_Value<ILink>> event_ActiveLink;
        public event EventHandler<ea_Value<ILink>> event_LinkAdded;
        public event EventHandler<ea_Value<ILink>> event_LinkDeleted;
        #endregion
        #region Инвокеры внешних событий
        protected void onLinkAdded(ILink link)
        {
            event_LinkAdded?.Invoke(this, new ea_Value<ILink>(link));
        }
        #endregion
    }
    #endregion
    #region Активная связь
    public partial class linkManager : ILinkManager
    {
        protected class activeLinkManager : Watcher.IDependSubscriber
        {
            #region Переменные
            protected linkManager parent;
            protected ILink _activeLink;
            protected DateTime lastDate;
            #endregion
            #region Свойства
            public ILink activeLink
            {
                get { return _activeLink; }
                protected set
                {
                    DateTime dOld = lastDate;
                    ILink oldLink = _activeLink;
                    string Old = _activeLink != null ? activeLink.GetId() : string.Empty;

                    _activeLink = value;
                    string New = _activeLink != null ? activeLink.GetId() : string.Empty;

                    updateLastDate();
                    if(oldLink != _activeLink) event_newActive?.Invoke(parent, new ea_ValueChange<string>(Old, New));

                    DateTime dNew = lastDate;
                    if (dNew != dOld) event_dateUpdated?.Invoke(parent, new ea_ValueChange<DateTime>(dOld, dNew));
                }
            }
            #endregion
            #region Делегаты
            protected Func<ILink[]> delegate_getSlaveLinks = () => null;
            protected Func<DateTime, DateTime, int> fComparator;
            #endregion
            #region События
            public event EventHandler<ea_ValueChange<string>> event_newActive; //если активные связи отсутствуют, отсылаем string.Empty
            public event EventHandler<ea_ValueChange<DateTime>> event_dateUpdated;
            #endregion
            #region Конструктор
            public activeLinkManager(linkManager parent, Func<ILink[]> delegate_getSlaveLinks)
            {
                this.parent = parent;
                if(delegate_getSlaveLinks == null) throw new NullReferenceException();
                this.delegate_getSlaveLinks = delegate_getSlaveLinks;
                fComparator = (current, newDate) => newDate > current ? 1 : -1;
                _activeLink = null;
                updateLastDate();
            }

            ~activeLinkManager()
            {
                parent = null;
                _activeLink = null;
                delegate_getSlaveLinks = null;
                fComparator = null;
            }
            #endregion
            #region IDependSubscriber
            public void handler_dateChanged(object sender, ea_ValueChange<DateTime> e)
            {
                ILink lSender = sender as ILink;
                if(lSender == null) throw new NullReferenceException();
                if(lSender.GetId() != activeLink.GetId()) compareWithActive(lSender);
            }

            public void handler_directionChanged(object sender, ea_ValueChange<e_Direction> e)
            {
                throw new NotImplementedException();
            }

            public void handler_dotChanged(object sender, ea_ValueChange<e_Dot> e)
            { }

            public void handler_linkRemoved(object sender, ea_ValueChange<string> e)
            {
                ILink lSender = sender as ILink;
                if (lSender.GetId() == activeLink.GetId()) activeLink = findActive();
            }
            #endregion
            #region Методы
            public void setNewLink(ILink link)
            {
                if (_activeLink == null) activeLink = link;
                else compareWithActive(link);
            }

            public void resetActiveLink()
            {
                activeLink = null;
            }
            #endregion
            #region Утилитарные
            protected virtual void compareWithActive(ILink newLink)
            {
                int result = fComparator(lastDate, newLink.GetSlaveDependence().GetDate());
                if (result > 0) activeLink = newLink;
                if (result < 0 && newLink.GetId() == _activeLink.GetId()) activeLink = findActive();
            }
            protected ILink findActive()
            {
                ILink[] slaves = delegate_getSlaveLinks();
                ILink result = _activeLink;
                DateTime dResult = lastDate;
                DateTime tempDate;

                if (slaves == null || slaves.Length == 0) return null;
                for (int i = 0; i < slaves.Length; i++)
                {
                    tempDate = slaves[i].GetSlaveDependence().GetDate();
                    if (fComparator(dResult, tempDate) > 0)
                    {
                        dResult = tempDate;
                        result = slaves[i];
                    }
                }

                return result;
            }
            protected virtual void updateLastDate()
            {
                lastDate = _activeLink != null ? _activeLink.GetSlaveDependence().GetDate() : new DateTime(1, 1, 1);
            }
            #endregion
        }
    }
    #endregion
    #region Отслеживание изменений в связях
    public partial class linkManager : ILinkManager
    {
        protected class Watcher
        {
            #region Переменные
            protected Dictionary<string, HashSet<onChange<e_Direction>>> hndsDir;
            protected Dictionary<string, HashSet<onChange<e_Dot>>> hndsDot;
            protected Dictionary<string, HashSet<onChange<DateTime>>> hndsDate;
            protected Dictionary<string, HashSet<onChange<string>>> hndsRemove;
            protected Dictionary<string, Action> aUnsuscribe;
            #endregion
            #region Свойства
            public int count => aUnsuscribe.Count;
            #endregion
            #region Делегаты
            public delegate void onChange<T>(object sender, ea_ValueChange<T> e);
            #endregion
            #region Интерфейс
            public interface IDependSubscriber
            {
                void handler_directionChanged(object sender, ea_ValueChange<e_Direction> e);
                void handler_dotChanged(object sender, ea_ValueChange<e_Dot> e);
                void handler_dateChanged(object sender, ea_ValueChange<DateTime> e);
                void handler_linkRemoved(object sender, ea_ValueChange<string> e);
            }
            #endregion
            #region Конструктор
            public Watcher()
            {
                hndsDir = new Dictionary<string, HashSet<onChange<e_Direction>>>();
                hndsDate = new Dictionary<string, HashSet<onChange<DateTime>>>();
                hndsDot = new Dictionary<string, HashSet<onChange<e_Dot>>>();
                hndsRemove = new Dictionary<string, HashSet<onChange<string>>>();
                aUnsuscribe = new Dictionary<string, Action>();
            }

            ~Watcher()
            {
                hndsDir = null;
                hndsDate = null;
                hndsDot = null;
                hndsRemove = null;
                aUnsuscribe = null;
            }
            #endregion
            #region Методы
            public void watchSlaveDependence(ILink newLink)
            {
                IDependence depend = newLink.GetSlaveDependence();

                depend.event_DependDotChanged += handler_dotChanged;
                depend.event_DateChanged += handler_dateChanged;
                depend.event_DirectionChanged += handler_directionChanged;

                aUnsuscribe.Add(newLink.GetId(), () =>
                {
                    depend.event_DependDotChanged -= handler_dotChanged;
                    depend.event_DateChanged -= handler_dateChanged;
                    depend.event_DirectionChanged -= handler_directionChanged;
                });
            }
            public bool subscribe<T>(string linkID, onChange<T> handler)
            {
                Type tp = typeof (T);

                if (tp == typeof (e_Direction))
                {
                    onChange<e_Direction> hnd = handler as onChange<e_Direction>;
                    return subscribe(ref hndsDir, linkID, hnd);
                }
                else if (tp == typeof(e_Dot))
                {
                    onChange<e_Dot> hnd = handler as onChange<e_Dot>;
                    return subscribe(ref hndsDot, linkID, hnd);
                }
                else if (tp == typeof(DateTime))
                {
                    onChange<DateTime> hnd = handler as onChange<DateTime>;
                    return subscribe(ref hndsDate, linkID, hnd);
                }

                return false;
            }

            public bool subscribe<T>(string linkID, Watcher.IDependSubscriber subscriber)
            {
                bool result = true;
                Action<bool> aRes = (bl) => result = (!result) ? result : bl;

                aRes(subscribe(ref hndsDir, linkID, subscriber.handler_directionChanged));
                aRes(subscribe(ref hndsDate, linkID, subscriber.handler_dateChanged));
                aRes(subscribe(ref hndsDot, linkID, subscriber.handler_dotChanged));
                aRes(subscribe(ref hndsRemove, linkID, subscriber.handler_linkRemoved));

                return result;
            }
            public bool unsubscribe<T>(string linkID, onChange<T> handler)
            {
                Type tp = typeof(T);

                if (tp == typeof(e_Direction))
                {
                    onChange<e_Direction> hnd = handler as onChange<e_Direction>;
                    return unsubscribe(ref hndsDir, linkID, hnd);
                }
                else if (tp == typeof(e_Dot))
                {
                    onChange<e_Dot> hnd = handler as onChange<e_Dot>;
                    return unsubscribe(ref hndsDot, linkID, hnd);
                }
                else if (tp == typeof(DateTime))
                {
                    onChange<DateTime> hnd = handler as onChange<DateTime>;
                    return unsubscribe(ref hndsDate, linkID, hnd);
                }

                return false;
            }
            public bool unsubscribe<T>(string linkID, Watcher.IDependSubscriber subscriber)
            {
                bool result = true;
                Action<bool> aRes = (bl) => result = (!result) ? result : bl;

                aRes(unsubscribe(ref hndsDir, linkID, subscriber.handler_directionChanged));
                aRes(unsubscribe(ref hndsDate, linkID, subscriber.handler_dateChanged));
                aRes(unsubscribe(ref hndsDot, linkID, subscriber.handler_dotChanged));
                aRes(unsubscribe(ref hndsRemove, linkID, subscriber.handler_linkRemoved));

                return result;
            }
            #endregion
            #region Обработчики
            protected void handler_directionChanged(object sender, ea_ValueChange<e_Direction> e)
            {
                ILink link = sender as ILink;
                if (link == null) throw new NullReferenceException();

                pushDelegates(sender, e, hndsDir[link.GetId()]);
            }

            protected void handler_dotChanged(object sender, ea_ValueChange<e_Dot> e)
            {
                ILink link = sender as ILink;
                if (link == null) throw new NullReferenceException();

                pushDelegates(sender, e, hndsDot[link.GetId()]);
            }

            protected void handler_dateChanged(object sender, ea_ValueChange<DateTime> e)
            {
                ILink link = sender as ILink;
                if (link == null) throw new NullReferenceException();

                pushDelegates(sender, e, hndsDate[link.GetId()]);
            }

            protected void handler_linkRemoved(object sender, ea_IdObject e)
            {
                ILink link = sender as ILink;
                if (link == null) throw new NullReferenceException();

                pushDelegates(sender, new ea_ValueChange<string>(e.Id.Id, e.Id.Id), hndsRemove[link.GetId()]);
                removeLink(e.Id.Id);
            }
            #endregion
            #region Утилитарные
            protected void pushDelegates<T>(object sender, ea_ValueChange<T> e, HashSet<onChange<T>> delegates)
            {
                if(delegates == null || delegates.Count == 0) return;

                foreach (onChange<T> dlg in delegates) dlg(sender, e);
            }

            protected bool subscribe<T>(ref Dictionary<string, HashSet<onChange<T>>> vault, string linkID, onChange<T> handler)
            {
                HashSet<onChange<T>> HSet = new HashSet<onChange<T>>();

                try
                {
                    HSet = vault[linkID];
                }
                catch (KeyNotFoundException)
                {
                    HSet.Add(handler);
                    vault.Add(linkID, HSet);
                    return true;
                }

                if (HSet.Contains(handler)) return false;
                HSet.Add(handler);
                vault[linkID] = HSet;

                return true;
            }

            protected bool unsubscribe<T>(ref Dictionary<string, HashSet<onChange<T>>> vault, string linkID, onChange<T> handler)
            {
                if (vault.Count == 0) return false;

                HashSet<onChange<T>> HSet = new HashSet<onChange<T>>();

                try
                {
                    HSet = vault[linkID];
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }

                if (!HSet.Contains(handler)) return false;
                HSet.Remove(handler);
                vault[linkID] = HSet;

                return true;
            }

            protected void removeLink(string linkID)
            {
                aUnsuscribe[linkID]();
                aUnsuscribe.Remove(linkID);

                hndsDir.Remove(linkID);
                hndsDate.Remove(linkID);
                hndsDot.Remove(linkID);
                hndsRemove.Remove(linkID);
            }
            #endregion
        }
    }
    #endregion
    #region Хранение связей
    public partial class linkManager : ILinkManager
    {
        #region Реализация класса хранилища
        protected class Vault
        {
            #region Индексатор
            public storedLink this[string linkID]
            {
                get { return vault[linkID]; }
                set { vault[linkID] = value; }
            } 
            #endregion
            #region Переменные
            protected linkManager parent;
            protected IId ownerIID => parent.owner;
            protected Dictionary<string, storedLink> vault;
            protected HashSet<string> neighbours;
            #endregion
            #region Свойства
            public int count => vault.Count;
            #endregion
            #region Конструктор
            public Vault(linkManager parent)
            {
                this.parent = parent;
                vault = new Dictionary<string, storedLink>();
                neighbours = new HashSet<string>();
            }

            ~Vault()
            {
                parent = null;
                vault = null;
                neighbours = null;
            }
            #endregion
            #region Методы
            #region Добавление
            public bool Add(ILink newLink)
            {
                if (vault.Keys.Contains(newLink.GetId())) return false;
                e_DependType dtOwner = newLink.GetInfoMember(ownerIID).GetDependType();
                storedLink SLink = new storedLink(newLink, dtOwner);

                if (!neighbourAdd(SLink.Neighbour.GetMemberId().GetId())) return false;

                vault.Add(newLink.GetId(), SLink);

                return true;
            }
            #endregion
            #region Удаление
            public bool Remove(string linkID)
            {
                if (!vault.Keys.Contains(linkID)) return false;

                if(!neighbours.Remove(vault[linkID].Neighbour.GetMemberId().GetId())) return false;
                if(!vault.Remove(linkID)) return false;
                return true;
            }
            #endregion
            #region Утилитарные

            private bool neighbourAdd(string neighbourID)
            {
                return neighbours.Add(neighbourID);
            }

            #endregion
            #region Доступ
            public string[] getNeighbours()
            {
                return neighbours.ToArray();
            }

            public ILink[] getLinks()
            {
                return vault.Select(v => v.Value.Link).ToArray();
            }

            public ILink[] getSlaveDependences()
            {
                return vault.Where(v => v.Value.dependType == e_DependType.Slave).Select(v => v.Value.Link).ToArray();
            }
            public ILink[] getMasterDependences()
            {
                return vault.Where(v => v.Value.dependType == e_DependType.Master).Select(v => v.Value.Link).ToArray();
            }
            #endregion
            #endregion
        }
        #endregion

    }
    #endregion
    #region Внутренние сущности

    public partial class linkManager : ILinkManager
    {
        #region Структуры
        protected struct storedLink
        {
            public readonly ILink Link;
            public Action unsuscribe;
            public readonly e_DependType dependType;
            private readonly e_DependType dtNeighbour;
            public ILMember Neighbour
                =>
                Link
                ?.GetInfoMember(dependType == e_DependType.Master ?
                    e_DependType.Slave : e_DependType.Master);

            public storedLink(ILink link, e_DependType dType)
            {
                Link = link;
                dependType = dType;
                dtNeighbour = 
                    dependType == e_DependType.Master ? 
                    e_DependType.Slave : e_DependType.Master;
                unsuscribe = null;
            }
        }
        #endregion
        #region Исключения
        public class noThisLinkInCollectionException : ApplicationException
        {
            public noThisLinkInCollectionException() : base("Связь отсутствует в коллекции")
            { }
        }
        public class notMemberForThisLinkException : ApplicationException
        {
            protected string text =
                "Объект {0} с уникальным номером {1} не является членом связи с уникальным номером {2}";

            public notMemberForThisLinkException(ILink link, IId owner)
                : base(
                     string.Format(
                         "Объект {0} с уникальным номером {1} не является членом связи с уникальным номером {2}",
                     owner.GetType(), owner.GetId(), link.GetId()))
            { }
        }
        #endregion
    }
    #endregion
}
