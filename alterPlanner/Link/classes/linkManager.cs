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
        protected IDock owner;
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
        public linkManager(IDock owner)
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
            /// <summary>
            /// Ссылка на экземпляр родитель
            /// </summary>
            protected linkManager parent;
            /// <summary>
            /// Ссылка на активную связь, Null если активная связь отсутствует
            /// </summary>
            protected ILink _activeLink;
            /// <summary>
            /// Последняя обновленная дата активной связи.
            /// </summary>
            protected DateTime lastDate;
            #endregion
            #region Свойства
            /// <summary>
            /// Свойство ссылающееся на активную связь, Null если активная связь отсутствует
            /// </summary>
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
            /// <summary>
            /// Делегат метода получения ссылок на связи где владелец является подчиненным
            /// </summary>
            protected Func<ILink[]> delegate_getSlaveLinks = () => null;
            /// <summary>
            /// Делегат метода сравнения двух связей по их зависимостям;
            /// </summary>
            protected Func<ILink, ILink, int> fLinkCompare;
            /// <summary>
            /// Делегат метода подготовки связи для сравнения (перевод от зависимых точек к общей величине)
            /// </summary>
            protected Func<ILink, double> fLinkCmpPrepare;

            protected Func<double, double, int> fLinkUValueCompare;
            #endregion
            #region События
            /// <summary>
            /// Событие срабатывает при объявлении активной новой связи, либо при объявлении отсутствия активной связи (Null)
            /// </summary>
            public event EventHandler<ea_ValueChange<ILink>> event_newActive; 
            /// <summary>
            /// Событие срабатывает при изменении даты активной связи
            /// </summary>
            public event EventHandler<ea_ValueChange<DateTime>> event_dateUpdated;
            #endregion
            #region Конструктор
            /// <summary>
            /// Конструктор экземпляра класса ответственного за управление и выбор активной связи
            /// </summary>
            /// <param name="parent">Ссылка на родителя</param>
            /// <param name="delegate_getSlaveLinks">Метод получения ссылок на связи где владелец является подчиненным</param>
            public activeLinkManager(linkManager parent, Func<ILink[]> delegate_getSlaveLinks)
            {
                this.parent = parent;
                if(delegate_getSlaveLinks == null) throw new NullReferenceException();
                this.delegate_getSlaveLinks = delegate_getSlaveLinks;
                
                init_Functions();

                _activeLink = null;
                updateLastDate();
            }
            /// <summary>
            /// Инициализация делегатов экземпляра класса
            /// </summary>
            protected virtual void init_Functions()
            {
                fLinkCmpPrepare = pLink =>
                    getOwnerDotDate(pLink.GetSlaveDependence().GetDependDot())
                        .Subtract(pLink.GetSlaveDependence().GetDate())
                        .Days;

                fLinkUValueCompare = (First, Second) =>
                {
                    if (Second > First) return 1;
                    else if (Second < First) return -1;
                    else return 0;
                };

                fLinkCompare = (First, Second) =>
                {
                    double dFirst = First != null ? 
                        fLinkCmpPrepare(First)  : -1000000000000;
                    double dSecond = Second != null ? 
                        fLinkCmpPrepare(Second) : -1000000000000;

                    return fLinkUValueCompare(dFirst, dSecond);
                };
            }
            /// <summary>
            /// Деструктор
            /// </summary>
            ~activeLinkManager()
            {
                parent = null;
                _activeLink = null;
                delegate_getSlaveLinks = null;
                fLinkCmpPrepare = null;
                fLinkUValueCompare = null;
                fLinkCompare = null;
            }
            #endregion
            #region IDependSubscriber
            /// <summary>
            /// Обработчик события изменения даты активной связи, при отсутствии активной связи передает дату 1.1.1
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public void handler_dateChanged(object sender, ea_ValueChange<DateTime> e)
            {
                ILink lSender = sender as ILink;
                if(lSender == null) throw new NullReferenceException();
                compareWithActive(lSender);
            }
            /// <summary>
            /// Обработчик события изменения направления ограничения связи, в данной версии не реализован, так как направление связи всегда имеет фиксированное значение и не меняется.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public void handler_directionChanged(object sender, ea_ValueChange<e_Direction> e)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Обработчик события изменения подчиненной точки владельца в активной связи
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public void handler_dotChanged(object sender, ea_ValueChange<e_Dot> e)
            {

                throw new NotImplementedException();
            }
            /// <summary>
            /// Обработчик события удаления связи
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public void handler_linkRemoved(object sender, ea_ValueChange<string> e)
            {
                ILink lSender = sender as ILink;
                linkRemoved(lSender.GetId());
            }
            #endregion
            #region Методы
            /// <summary>
            /// Обработчик события удаления связи с идентификатором <paramref name="linkID"/>
            /// </summary>
            /// <param name="linkID">Идентификатор удаляемой связи</param>
            /// <returns>Вовзращает истину, если удаляемая связь являлась активной</returns>
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
            /// <summary>
            /// Метод установки новой активной связи
            /// </summary>
            /// <param name="link"></param>
            public void setNewLink(ILink link)
            {
                if (_activeLink == null) activeLink = link;
                else compareWithActive(link);
            }
            /// <summary>
            /// Метод установки ссылки на активную связь в значение Null
            /// </summary>
            public void resetActiveLink()
            {
                activeLink = null;
            }
            #endregion
            #region Утилитарные
            /// <summary>
            /// Метод сравнения <paramref name="newLink"/> с текущей активной связью
            /// </summary>
            /// <param name="newLink"></param>
            protected virtual void compareWithActive(ILink newLink)
            {
                int result = fLinkCompare(_activeLink, newLink);

                if (result > 0) activeLink = newLink;
                else if (result < 0 && newLink.GetId() == _activeLink.GetId())
                    activeLink = findActive();
                else if (result == 0)
                {
                    if (activeLink != null) activeLink = _activeLink;
                }
                else throw new ApplicationException(string.Format("Unexpected function result {0}", nameof(fLinkCompare)));
            }
            /// <summary>
            /// Метод поиска активной связи, опциональный параметр <paramref name="exceptLink"/> исключает определенную связь из списка выбора
            /// </summary>
            /// <param name="exceptLink">Опциональный параметр, связь исключаемая из списка выбора активной связи</param>
            /// <returns>Возвращает ссылку на выбранную активную связь, либо null если список выбора пуст</returns>
            protected ILink findActive(ILink exceptLink = null)
            {
                ILink[] slaves = new ILink[0];

                if (exceptLink != null)
                    slaves = delegate_getSlaveLinks()
                        .Where(val => val.GetId() != exceptLink.GetId()).ToArray();
                else slaves = delegate_getSlaveLinks();



                if (slaves == null || slaves.Length == 0) return null;

                ILink result = slaves[0];

                if (slaves == null || slaves.Length == 0) return null;
                for (int i = 0; i < slaves.Length; i++)
                {
                    if (fLinkCompare(result, slaves[i]) > 0)
                    {
                        result = slaves[i];
                    }
                }

                return result;
            }
            /// <summary>
            /// Метод обновления последней даты (<seealso cref="lastDate"/>) активной связи 
            /// </summary>
            protected virtual void updateLastDate()
            {
                lastDate = _activeLink != null ? _activeLink.GetSlaveDependence().GetDate() : new DateTime(1, 1, 1);
            }
            #endregion
            #region Служебные
            /// <summary>
            /// Получить дату точки <param name="dot"></param> владельца
            /// </summary>
            /// <param name="dot">Точка владельца дату которой получаем</param>
            /// <returns>Дата точки владельца</returns>
            protected DateTime getOwnerDotDate(e_Dot dot)
            {
                return parent.owner.GetDot(dot).GetDate();
            }
            #endregion
        }
    }
    #endregion
    #region Отслеживание изменений в связях
    public partial class linkManager : ILinkManager
    {
        /// <summary>
        /// Класс управляющий подписками на связи
        /// </summary>
        public class Watcher
        {
            #region Переменные
            /// <summary>
            /// Словарь делегатов подписанных на событие изменения направления, ключом служит ID связи, значением список ссылок на делегаты подписанные на событие
            /// </summary>
            protected Dictionary<string, HashSet<onChange<e_Direction>>> hndsDir;
            /// <summary>
            /// Словарь делегатов подписанных на событие изменения подчиненной точки владельца, ключом служит ID связи, значением список ссылок на делегаты подписанные на событие
            /// </summary
            protected Dictionary<string, HashSet<onChange<e_Dot>>> hndsDot;
            /// <summary>
            /// Словарь делегатов подписанных на событие изменения даты ограничения связи, ключом служит ID связи, значением список ссылок на делегаты подписанные на событие
            /// </summary>
            protected Dictionary<string, HashSet<onChange<DateTime>>> hndsDate;
            /// <summary>
            /// Словарь делегатов подписанных на событие удаления связи, ключом служит ID связи, значением список ссылок на делегаты подписанные на событие
            /// </summary>
            protected Dictionary<string, HashSet<onChange<string>>> hndsRemove;
            /// <summary>
            /// Словарь делегатов отписки от всех событий связи, ключом служит ID связи, значением делегат отписки от связи
            /// </summary>
            protected Dictionary<string, Action> aUnsuscribe;
            #endregion
            #region Свойства
            /// <summary>
            /// Количество записей словаря отписки <seealso cref="aUnsuscribe"/>
            /// </summary>
            public int count => aUnsuscribe.Count;
            #endregion
            #region Делегаты
            /// <summary>
            /// Делегат генерик для подписки на события связей
            /// </summary>
            /// <typeparam name="T">Могут выступать следующие типы: <seealso cref="e_Direction"/>, <seealso cref="e_Dot"/>, <seealso cref="DateTime"/>, <seealso cref="string"/></typeparam>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void onChange<T>(object sender, ea_ValueChange<T> e);
            #endregion
            #region Интерфейс
            /// <summary>
            /// Интерфейс для автоматиченской подписки класса
            /// </summary>
            public interface IDependSubscriber
            {
                /// <summary>
                /// Обработчик события изменения направления ограничения связи
                /// </summary>
                /// <param name="sender"></param>
                /// <param name="e"></param>
                void handler_directionChanged(object sender, ea_ValueChange<e_Direction> e);
                /// <summary>
                /// Обработчик события изменения зависимой точки владельца
                /// </summary>
                /// <param name="sender"></param>
                /// <param name="e"></param>
                void handler_dotChanged(object sender, ea_ValueChange<e_Dot> e);
                /// <summary>
                /// Обработчик события изменения даты зависимости связи
                /// </summary>
                /// <param name="sender"></param>
                /// <param name="e"></param>
                void handler_dateChanged(object sender, ea_ValueChange<DateTime> e);
                /// <summary>
                /// Обработчик события удаления связи
                /// </summary>
                /// <param name="sender"></param>
                /// <param name="e"></param>
                void handler_linkRemoved(object sender, ea_ValueChange<string> e);
            }
            #endregion
            #region Конструктор
            /// <summary>
            /// Конструктор экземпляра класса управляющего подписками на связи
            /// </summary>
            public Watcher()
            {
                hndsDir = new Dictionary<string, HashSet<onChange<e_Direction>>>();
                hndsDate = new Dictionary<string, HashSet<onChange<DateTime>>>();
                hndsDot = new Dictionary<string, HashSet<onChange<e_Dot>>>();
                hndsRemove = new Dictionary<string, HashSet<onChange<string>>>();
                aUnsuscribe = new Dictionary<string, Action>();
            }
            /// <summary>
            /// Деструктор
            /// </summary>
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
            /// <summary>
            /// Метод старта наблюдения за новой связью с зависимостью владельца <paramref name="dependType"/>
            /// </summary>
            /// <param name="newLink">Новая связь на события которой подписываемся</param>
            /// <param name="dependType">Тип зависимости владельца в новой связи</param>
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
            /// <summary>
            /// Метод подписки делегата на связь с идентификатором <paramref name="linkID"/>
            /// </summary>
            /// <typeparam name="T">Могут выступать следующие типы: <seealso cref="e_Direction"/>, <seealso cref="e_Dot"/>, <seealso cref="DateTime"/>, <seealso cref="string"/></typeparam>
            /// <param name="linkID">Идентификатор наблюдаемой связи</param>
            /// <param name="handler">Ссылка на делегат обработчик</param>
            /// <returns></returns>
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
            /// <summary>
            /// Метод автоматической подписки класса на все события связи с идентификатором <paramref name="linkID"/>
            /// </summary>
            /// <param name="linkID">Идентификатор наблюдаемой связи</param>
            /// <param name="subscriber">Ссылка на экземпляр класса подписчика</param>
            /// <returns>Вовзвращает истину если подписка прошла успешщно</returns>
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
            /// <summary>
            /// Метод отписки делегата от связи с идентификатором <paramref name="linkID"/>
            /// </summary>
            /// <typeparam name="T">Могут выступать следующие типы: <seealso cref="e_Direction"/>, <seealso cref="e_Dot"/>, <seealso cref="DateTime"/>, <seealso cref="string"/></typeparam>
            /// <param name="linkID">Идентификатор наблюдаемой связи</param>
            /// <param name="handler">Ссылка на делегат обработчик</param>
            /// <returns>Истина если отписка произведена успешно</returns>
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
            /// <summary>
            /// Метод отписки экземпляра класса от всех событий связи с идентификатором <paramref name="linkID"/>
            /// </summary>
            /// <param name="linkID">Идентификатор наблюдаемой связи</param>
            /// <param name="subscriber">Ссылка на экземпляр класса подписчика</param>
            /// <returns>Вовзвращает истину если отписка прошла успешщно</returns>
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
            /// <summary>
            /// Отписаться и прекратить наблюдать связь с идентификатором <paramref name="linkID"/>
            /// </summary>
            /// <param name="linkID">Идентификатор связи</param>
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
            /// <summary>
            /// Отписаться и прекратить наблюдать от всех наблюдаемых связей
            /// </summary>
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
            /// <summary>
            /// Обработчик события изменения направления ограничения связи
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            protected void handler_directionChanged(object sender, ea_ValueChange<e_Direction> e)
            {
                ILink link = sender as ILink;
                if (link == null) throw new NullReferenceException();

                pushDelegates(sender, e, hndsDir[link.GetId()]);
            }
            /// <summary>
            /// Обработчик события изменения подчиненной точки владельца
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            protected void handler_dotChanged(object sender, ea_ValueChange<e_Dot> e)
            {
                ILink link = sender as ILink;
                if (link == null) throw new NullReferenceException();

                if (hndsDot.ContainsKey(link.GetId()))
                    pushDelegates(sender, e, hndsDot[link.GetId()]);
            }
            /// <summary>
            /// Обработчик события изменения даты зависимости связи
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            protected void handler_dateChanged(object sender, ea_ValueChange<DateTime> e)
            {
                ILink link = sender as ILink;
                if (link == null) throw new NullReferenceException();
                if(hndsDate.ContainsKey(link.GetId()))
                    pushDelegates(sender, e, hndsDate[link.GetId()]);
            }
            /// <summary>
            /// Обработчик события удаления связи
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
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
            /// <summary>
            /// Вызвать все делегаты обработчиков <paramref name="delegates"/>, с параметрами <paramref name="sender"/> и <paramref name="e"/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            /// <param name="delegates">Список вызываемых делегатов</param>
            protected void pushDelegates<T>(object sender, ea_ValueChange<T> e, HashSet<onChange<T>> delegates)
            {
                if(delegates == null || delegates.Count == 0) return;

                foreach (onChange<T> dlg in delegates) dlg(sender, e);
            }
            /// <summary>
            /// Добавить делегат обработчика <paramref name="handler"/> в словарь делегатов <paramref name="vault"/> по ключу-идентификатору связи <paramref name="linkID"/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="vault"></param>
            /// <param name="linkID"></param>
            /// <param name="handler"></param>
            /// <returns>Истина если делегат обработчик добавлен успешно</returns>
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
            /// <summary>
            /// Удалить делегат обработчика <paramref name="handler"/> из словаря делегатов <paramref name="vault"/> по ключу-идентификатору связи <paramref name="linkID"/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="vault"></param>
            /// <param name="linkID"></param>
            /// <param name="handler"></param>
            /// <returns>Истина если делегат удален успешно</returns>
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
        /// <summary>
        /// Класс хранения связей взаимодействующих с владельцем
        /// </summary>
        public class Vault
        {
            #region Индексатор
            /// <summary>
            /// Свойство индексатора возвращающее по идентификатору связи, хранимую экземпляром класса связь
            /// </summary>
            /// <param name="linkID">Идентификатор связи</param>
            /// <returns></returns>
            public storedLink this[string linkID]
            {
                get { return vault[linkID]; }
                set { vault[linkID] = value; }
            } 
            #endregion
            #region Переменные
            /// <summary>
            /// Ссылка на родителя
            /// </summary>
            protected linkManager parent;
            /// <summary>
            /// Ссылка на идентификатор владельца
            /// </summary>
            protected IId ownerIID => parent.owner;
            /// <summary>
            /// Словарь хранения взаимодействующих с владельцем связей, ключ - идентификатор связи, значение - ссылка на связь
            /// </summary>
            protected Dictionary<string, storedLink> vault;
            /// <summary>
            /// Соседи учавствующие во взаимодействующих с владельцем связях
            /// </summary>
            protected HashSet<string> neighbours;
            #endregion
            #region Свойства
            /// <summary>
            /// Количество взаимодействующих связей
            /// </summary>
            public int count => vault.Count;
            #endregion
            #region Конструктор
            /// <summary>
            /// Конструктор экземпляра класса хранения связей взаимодействующих с владельцем
            /// </summary>
            /// <param name="parent">Ссылка на родителя</param>
            public Vault(linkManager parent)
            {
                this.parent = parent;
                vault = new Dictionary<string, storedLink>();
                neighbours = new HashSet<string>();
            }
            /// <summary>
            /// Деструктор
            /// </summary>
            ~Vault()
            {
                parent = null;
                vault = null;
                neighbours = null;
            }
            #endregion
            #region Методы
            #region Добавление
            /// <summary>
            /// Добавление новой связи в хранилище
            /// </summary>
            /// <param name="newLink">Ссылка на добавляемую связь</param>
            /// <returns>Истина если ссылка на связь была добавлена</returns>
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
            /// <summary>
            /// Удаление ссылки на связь с идентификатором <paramref name="linkID"/> из хранилища
            /// </summary>
            /// <param name="linkID">Идентификатор связи</param>
            /// <returns>Истина, если ссылка была удалена</returns>
            public bool Remove(string linkID)
            {
                if (!vault.Keys.Contains(linkID)) return false;

                if(!neighbours.Remove(vault[linkID].Neighbour.GetMemberId().GetId())) return false;
                if(!vault.Remove(linkID)) return false;
                return true;
            }
            /// <summary>
            /// Удалить все ссылки на связи из хранилища
            /// </summary>
            /// <returns></returns>
            public bool Remove()
            {
                if (vault.Count == 0) return false;

                vault.Clear();
                neighbours.Clear();
                return true;
            }
            #endregion
            #region Утилитарные
            /// <summary>
            /// Добавить нового соседа
            /// </summary>
            /// <param name="neighbourID">Идентификатор соседа</param>
            /// <returns>Истина если сосед был добавлен, ложь - если он уже существует в хранилище</returns>
            private bool neighbourAdd(string neighbourID)
            {
                return neighbours.Add(neighbourID);
            }
            #endregion
            #region Доступ
            /// <summary>
            /// Метод получения всех соседей по связям, владельца
            /// </summary>
            /// <returns></returns>
            public string[] getNeighbours()
            {
                return neighbours.ToArray();
            }
            /// <summary>
            /// Метод получения все связит в которых учавствует владелец
            /// </summary>
            /// <returns></returns>
            public ILink[] getLinks()
            {
                return vault.Select(v => v.Value.Link).ToArray();
            }
            /// <summary>
            /// Метод получения массива связей в которых владелец учавствует с зависимостью <paramref name="depend"/>
            /// </summary>
            /// <param name="depend"></param>
            /// <returns></returns>
            public ILink[] getLinks(e_DependType depend)
            {
                return (depend == e_DependType.Master) ? getMasterDependences() : getSlaveDependences();
            }
            /// <summary>
            /// Метод получения массива связей в которых владелец учавствует с подчиненной зависимостью
            /// </summary>
            /// <returns></returns>
            public ILink[] getSlaveDependences()
            {
                return vault.Where(v => v.Value.dependType == e_DependType.Slave).Select(v => v.Value.Link).ToArray();
            }
            /// <summary>
            /// Метод получения массива связей в которых владелец учавствует с основной зависимостью
            /// </summary>
            /// <returns></returns>
            public ILink[] getMasterDependences()
            {
                return vault.Where(v => v.Value.dependType == e_DependType.Master).Select(v => v.Value.Link).ToArray();
            }
            #endregion
            #region Информационные
            /// <summary>
            /// Метод проверки связи с идентификатором <paramref name="linkID"/> на ее взаимодействие с владельцем
            /// </summary>
            /// <param name="linkID"></param>
            /// <returns></returns>
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
        /// <summary>
        /// Структура для хранения информации взаимодействующей с владельцем связи
        /// </summary>
        public struct storedLink
        {
            /// <summary>
            /// Ссылка на связь
            /// </summary>
            public readonly ILink Link;
            /// <summary>
            /// Зависимость с которой владлелец учавствует в связи
            /// </summary>
            public readonly e_DependType dependType;
            /// <summary>
            /// Зависимость соседа по связи
            /// </summary>
            private readonly e_DependType dtNeighbour;
            /// <summary>
            /// Ссылка на соседа по связи
            /// </summary>
            public ILMember Neighbour
                =>
                Link
                ?.GetInfoMember(dependType == e_DependType.Master ?
                    e_DependType.Slave : e_DependType.Master);

            /// <summary>
            /// Конструктор экземпляра структуры хранения информации взаимодействующей с владельцем связи
            /// </summary>
            /// <param name="link"></param>
            /// <param name="dType"></param>
            public storedLink(ILink link, e_DependType dType)
            {
                Link = link;
                dependType = dType;
                dtNeighbour = 
                    dependType == e_DependType.Master ? 
                    e_DependType.Slave : e_DependType.Master;
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
