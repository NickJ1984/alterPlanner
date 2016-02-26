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
using alter.Service.Extensions;
using alter.Service.iface;
using alter.types;

//Протестировано
namespace alter.Link.classes
{
    #region Реализация класса
    /// <summary>
    /// Класс управления связями взаимодействующими с объектом
    /// </summary>
    public partial class linkManager : ILinkManager
    {
        #region Переменные
        /// <summary>
        /// Идентификатор владельца экземпляра класса
        /// </summary>
        protected IId owner;
        /// <summary>
        /// Хранилище связей
        /// </summary>
        protected Vault links;
        /// <summary>
        /// Управление подписками на связи
        /// </summary>
        protected Watcher cWatcher;
        /// <summary>
        /// Менеджер активных связей
        /// </summary>
        protected activeLinkManager alManager;

        /// <summary>
        /// Отписка от внутренних объектов
        /// </summary>
        private Action unsuscribe;
        #endregion
        #region Делегаты
        /// <summary>
        /// Делегат для подписки на событие удаления связи экземпляра Watcher
        /// </summary>
        protected Watcher.onChange<string> onRemoveLink;
        #endregion
        #region События
        /// <summary>
        /// Событие срабатывающее при установке новой активной связи
        /// </summary>
        public event EventHandler<ea_ValueChange<ILink>> event_newActiveLink;
        /// <summary>
        /// Событие срабатывающее при изменении даты активной связи
        /// </summary>
        public event EventHandler<ea_ValueChange<DateTime>> event_activeLinkDateChanged;
        /// <summary>
        /// Событие при добавлении связи
        /// </summary>
        public event EventHandler<ea_Value<ILink>> event_linkAdded;
        /// <summary>
        /// Событие при удалении связи
        /// </summary>
        public event EventHandler<ea_Value<ILink>> event_linkDeleted;
        #endregion
        #region Конструкторы
        /// <summary>
        /// Конструктор экземпляра класса управления связями взаимодействующими с объектом владельцем
        /// </summary>
        /// <param name="owner">Интерфейс идентификатор владельца экземпляра класса</param>
        public linkManager(IId owner)
        {
            this.owner = owner;
            links = new Vault(this);
            cWatcher = new Watcher();
            alManager = new activeLinkManager(this, links.getSlaveDependences);

            alManager.event_newActive += handler_activeLinkNew;
            alManager.event_dateUpdated += handler_activeLinkDateChanged;

            unsuscribe = () =>
            {
                alManager.event_newActive -= handler_activeLinkNew;
                alManager.event_dateUpdated -= handler_activeLinkDateChanged;
            };

            onRemoveLink = handler_linkRemoved;
        }
        /// <summary>
        /// Деструктор
        /// </summary>
        ~linkManager()
        {
            unsuscribe();
            unsuscribe = null;
            owner = null;
            links = null;
            cWatcher = null;
            alManager = null;
            onRemoveLink = null;
        }
        #endregion
        #region Обработчики событий
        /// <summary>
        /// Обработчик события установки новой активной связи
        /// </summary>
        /// <param name="sender">Объект инициатор</param>
        /// <param name="e">Аргумент содержит ссылку на старую активную связь и на новую, одним из аргументов может являться значение Null при условии отсутствия активной связи</param>
        protected void handler_activeLinkNew(object sender, ea_ValueChange<ILink> e)
        {
            event_newActiveLink?.Invoke(this, e);
        }
        /// <summary>
        /// Обработчик события изменения даты активной связи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Аргумент изменения даты (старое и новое значение)</param>
        protected void handler_activeLinkDateChanged(object sender, ea_ValueChange<DateTime> e)
        {
            event_activeLinkDateChanged?.Invoke(this, e);
        }
        /// <summary>
        /// Обработчик события удаления связи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Используемый тип ea_ValueChange вместо ea_Value, обусловлен механизмом подписки Watcher'а. (В обоих переменных одно и то же значение, удаляемой связи)</param>
        protected void handler_linkRemoved(object sender, ea_ValueChange<string> e)
        {
            delLink(e.NewValue);
        }
        #endregion
        #region Методы
        #region Добавление связи
        /// <summary>
        /// Метод добавление связи, прототип метода обусловлен интерфейсом IDock, возвращает истину если связь добавлена
        /// </summary>
        /// <param name="link">Возвращает истину если связь добавлена</param>
        /// <returns></returns>
        public bool connect(ILink link)
        {
            if(link.isNull()) throw new NullReferenceException(nameof(link));
            if (!links.Add(link)) return false;

            event_linkAdded?.Invoke(this, new ea_Value<ILink>(link));

            e_DependType dType = link.GetInfoMember(owner).GetDependType();


            cWatcher.watchLink(link, dType);

            if (dType == e_DependType.Slave)
            {
                alManager.setNewLink(link);
                if (!cWatcher.subscribe(link.GetId(), alManager)) throw new ArgumentException(nameof(cWatcher));
            }

            cWatcher.subscribe(link.GetId(), onRemoveLink);

            return true;
        }
        #endregion
        #region Удаление связи
        /// <summary>
        /// Отписка от всех взаимодействующих с объектом связей, и удаление ссылок на них
        /// </summary>
        /// <returns></returns>
        public bool delLinks()
        {
            if (links.count == 0) return false;
            ILink[] rmLinks = links.getLinks();
            cWatcher.removeLinks();
            alManager.resetActiveLink();
            links.Remove();
            if (event_linkDeleted != null)
            {
                for (int i = 0; i < rmLinks.Length; i++)
                    event_linkDeleted?.Invoke(this, new ea_Value<ILink>(rmLinks[i]));
            }
            return true;
        }
        /// <summary>
        /// Отписка и удаление связей взаимодействующих связей где объект владелец выступает в роли <paramref name="dependType"/>
        /// </summary>
        /// <param name="dependType">Роль объекта владельца в связях</param>
        /// <returns></returns>
        public bool delLink(e_DependType dependType)
        {
            if (links.count == 0) return false;

            ILink[] rmLinks = links.getLinks(dependType);
            if (rmLinks.isNullOrEmpty()) return false;

            for (int i = 0; i < rmLinks.Length; i++)
            {
                delLink(rmLinks[i]);
                event_linkDeleted?.Invoke(this, new ea_Value<ILink>(rmLinks[i]));
            }

            return true;
        }
        /// <summary>
        /// Отписка и удаление связи по ее идентификатору, возвращает истину если ссылка удалена
        /// </summary>
        /// <param name="linkID">Идентификатор связи</param>
        /// <returns></returns>
        public bool delLink(string linkID)
        {
            if (links.count == 0) return false;

            ILink rmLink = links[linkID].Link;

            cWatcher.removeLink(linkID);
            alManager.linkRemoved(linkID);
            if (!links.Remove(linkID)) throw new ArgumentException(nameof(links));

            event_linkDeleted?.Invoke(this, new ea_Value<ILink>(rmLink));

            return true;
        }
        /// <summary>
        /// Отписка и удаление связи по ее ссылке, возвращает истину если ссылка удалена
        /// </summary>
        /// <param name="Link">Ссылка на связь</param>
        /// <returns></returns>
        public bool delLink(ILink Link)
        {
            return delLink(Link.GetId());
        }
        #endregion
        #region Доступ к связям
        /// <summary>
        /// Получить ссылку на активную связь, возвращает null если активная связь отсутствует
        /// </summary>
        /// <returns>Вовзращает ссылку на активную связь, возвращает null если активная связь отсутствует</returns>
        public ILink getActiveLink()
        {
            return alManager.activeLink;
        }
        /// <summary>
        /// Возвращает ссылку на хранимую связь по ее идентификатору
        /// </summary>
        /// <param name="linkID"></param>
        /// <returns></returns>
        public ILink getLink(string linkID)
        {
            return links[linkID].Link;
        }
        /// <summary>
        /// Возвращает массив ссылок на все хранящиеся связи
        /// </summary>
        /// <returns></returns>
        public ILink[] getLinks()
        {
            return links.getLinks();
        }
        /// <summary>
        /// Возвращает массив ссылок на хранящиеся связи где объект владелец выступает в роли <paramref name="dependType"/>
        /// </summary>
        /// <param name="dependType">Роль объекта владельца в связях</param>
        /// <returns></returns>
        public ILink[] getLinks(e_DependType dependType)
        {
            if(!Enum.IsDefined(typeof(e_DependType), dependType)) throw new ArgumentNullException(nameof(dependType));
            return links.getLinks(dependType);
        }
        #endregion
        #region Информационные методы
        /// <summary>
        /// Метод предоставляющих количество связей где объект владелец выступает в роли <paramref name="dependType"/>
        /// </summary>
        /// <param name="dependType">Роль объекта владельца в связях</param>
        /// <returns>Количество связей</returns>
        public int GetLinksCount(e_DependType dependType)
        {
            return links.count;
        }
        /// <summary>
        /// Метод возвращает истину если экземпляр класса хранит в себе связь с идентификатором эквивалентным <paramref name="linkID"/>
        /// </summary>
        /// <param name="linkID">Идентификатор связи</param>
        /// <returns></returns>
        public bool LinkExist(string linkID)
        {
            return links.isExist(linkID);
        }
        #endregion
        #region Методы объекта
        /// <summary>
        /// Очистить экземпляр класса
        /// </summary>
        public void clear()
        {
            delLinks();
        }
        #endregion
        #endregion
    }
    #endregion
    #region Активная связь
    public partial class linkManager : ILinkManager
    {
        /// <summary>
        /// Класс ответственный за управление и выбор активной связи
        /// </summary>
        public class activeLinkManager : Watcher.IDependSubscriber
        {
            #region Переменные
            protected linkManager parent;
            protected ILink _activeLink;
            protected DateTime lastDate;
            #endregion
            #region Свойства
            public DateTime LDate => lastDate;
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
                    if(oldLink != _activeLink) event_newActive?.Invoke(parent, new ea_ValueChange<ILink>(oldLink, activeLink));

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
            public event EventHandler<ea_ValueChange<ILink>> event_newActive; //если активные связи отсутствуют, отсылаем string.Empty
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
                compareWithActive(lSender);
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
                linkRemoved(lSender.GetId());
            }
            #endregion
            #region Методы
            public bool linkRemoved(string linkID)
            {
                string activeLinkID = (_activeLink != null) ? activeLink.GetId() : string.Empty;

                if (linkID == activeLinkID)
                {
                    activeLink = findActive(activeLink);
                    return true;
                }
                return false;
            }
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
                if (result < 0 && newLink.GetId() == _activeLink.GetId())
                    activeLink = findActive();
            }
            protected ILink findActive(ILink exceptLink = null)
            {
                ILink[] slaves = new ILink[0];

                if (exceptLink != null)
                    slaves = delegate_getSlaveLinks()
                        .Where(val => val.GetId() != exceptLink.GetId()).ToArray();
                else slaves = delegate_getSlaveLinks();

                if (slaves == null || slaves.Length == 0) return null;


                ILink result = slaves[0];
                DateTime dResult = slaves[0].GetSlaveDependence().GetDate();
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
        public class Watcher
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

            public void watchLink(ILink newLink, e_DependType dependType)
            {
                if(!Enum.IsDefined(typeof(e_DependType), dependType)) throw new ArgumentException(nameof(dependType));

                newLink.event_ObjectDeleted += handler_linkRemoved;

                if (dependType == e_DependType.Slave)
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

                        newLink.event_ObjectDeleted -= handler_linkRemoved;
                    });
                }
                else
                {
                    aUnsuscribe.Add(newLink.GetId(), () =>
                    {
                        newLink.event_ObjectDeleted -= handler_linkRemoved;
                    });
                }
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
                else if (tp == typeof(string))
                {
                    onChange<string> hnd = handler as onChange<string>;
                    return subscribe(ref hndsRemove, linkID, hnd);
                }
                return false;
            }

            public bool subscribe(string linkID, Watcher.IDependSubscriber subscriber)
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
            public bool unsubscribe(string linkID, Watcher.IDependSubscriber subscriber)
            {
                bool result = true;
                Action<bool> aRes = (bl) => result = (!result) ? result : bl;

                aRes(unsubscribe(ref hndsDir, linkID, subscriber.handler_directionChanged));
                aRes(unsubscribe(ref hndsDate, linkID, subscriber.handler_dateChanged));
                aRes(unsubscribe(ref hndsDot, linkID, subscriber.handler_dotChanged));
                aRes(unsubscribe(ref hndsRemove, linkID, subscriber.handler_linkRemoved));

                return result;
            }

            public void removeLink(string linkID)
            {
                if(!aUnsuscribe.Keys.Contains(linkID)) return;
                aUnsuscribe[linkID]();
                aUnsuscribe.Remove(linkID);

                hndsDir.Remove(linkID);
                hndsDate.Remove(linkID);
                hndsDot.Remove(linkID);
                hndsRemove.Remove(linkID);
            }
            public void removeLinks()
            {
                if(aUnsuscribe.Count == 0) return;
                var aUns = aUnsuscribe.Values.ToArray();

                for (int i = 0; i < aUnsuscribe.Count; i++)
                    aUns[i]();

                hndsDir.Clear();
                hndsDate.Clear();
                hndsDot.Clear();
                hndsRemove.Clear();
                aUnsuscribe.Clear();
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

                if (hndsDot.ContainsKey(link.GetId()))
                    pushDelegates(sender, e, hndsDot[link.GetId()]);
            }

            protected void handler_dateChanged(object sender, ea_ValueChange<DateTime> e)
            {
                ILink link = sender as ILink;
                if (link == null) throw new NullReferenceException();
                if(hndsDate.ContainsKey(link.GetId()))
                    pushDelegates(sender, e, hndsDate[link.GetId()]);
            }

            protected void handler_linkRemoved(object sender, ea_IdObject e)
            {
                ILink link = sender as ILink;
                if (link == null) throw new NullReferenceException();

                if (hndsRemove.ContainsKey(link.GetId()))
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

                if (HSet.Count == 0) vault.Remove(linkID);

                return true;
            }
            #endregion
        }
    }
    #endregion
    #region Хранение связей
    public partial class linkManager : ILinkManager
    {
        public class Vault
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
                ILMember mbr = newLink.GetInfoMember(ownerIID);
                if (mbr == null) return false;

                e_DependType dtOwner = mbr.GetDependType();
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
            public bool Remove()
            {
                if (vault.Count == 0) return false;

                vault.Clear();
                neighbours.Clear();
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

            public ILink[] getLinks(e_DependType depend)
            {
                return (depend == e_DependType.Master) ? getMasterDependences() : getSlaveDependences();
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
            #region Информационные
            public bool isExist(string linkID)
            {
                return vault.Keys.Contains(linkID);
            }
            #endregion
            #endregion
        }
        #endregion
    }
    #region Внутренние сущности

    public partial class linkManager : ILinkManager
    {
        #region Структуры
        public struct storedLink
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
