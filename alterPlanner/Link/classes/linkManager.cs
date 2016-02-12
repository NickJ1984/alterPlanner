using System;
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
        protected class activeLink
        {
            public bool isActive(ILink link)
            {
                throw new NotImplementedException();
            }
        }
    }
    #endregion
    #region Отслеживание изменений в связях
    public partial class linkManager : ILinkManager
    {
        protected class Watcher
        {
            protected linkManager parent;

            public delegate void onLinkUpdated(string linkID, linkDependenceInfo cLink);
            public event onLinkUpdated event_linkChanged;

            public void subscribe(storedLink LinkUnit)
            {
                ILink link = LinkUnit.Link;
                Action Unsuscribe;
                link.GetSlaveDependence().event_DateChanged += handler_linkDateChanged;
                link.GetSlaveDependence().event_DependDotChanged += handler_linkDotChanged;
                link.GetSlaveDependence().event_DirectionChanged += handler_linkDirectionChanged;
                Unsuscribe = () =>
                {
                    link.GetSlaveDependence().event_DateChanged -= handler_linkDateChanged;
                    link.GetSlaveDependence().event_DependDotChanged -= handler_linkDotChanged;
                    link.GetSlaveDependence().event_DirectionChanged -= handler_linkDirectionChanged;
                };
                LinkUnit.unsuscribe = Unsuscribe;
            }
            protected void handler_linkDateChanged(object sender, ea_ValueChange<DateTime> e)
            {
                throw new NotImplementedException();
            }

            protected void handler_linkDirectionChanged(object sender, ea_ValueChange<e_Direction> e)
            {
                throw new NotImplementedException();
            }

            protected void handler_linkDotChanged(object sender, ea_ValueChange<e_Dot> e)
            {
                throw new NotImplementedException();
            }
            protected void onLinkChanged(ILink changedLink, chng whatChanges)
            {
                linkDependenceInfo info = new linkDependenceInfo()
                {
                    changed = whatChanges,
                    link = changedLink
                };
                
                event_linkChanged.Invoke(changedLink.GetId(), info);
            }
        }
    }
    #endregion
    #region Хранение связей
    public partial class linkManager : ILinkManager
    {
        #region Реализация класса хранилища
        protected class Vault
        {
            #region Переменные
            protected linkManager parent;
            protected HashSet<string> storedLinks;
            protected Dictionary<string, storedLink> slaveLinks;
            protected Dictionary<string, storedLink> masterLinks;
            #endregion
            #region Свойства
            public int count => storedLinks.Count;
            public int countSlave => slaveLinks.Count;
            public int countMaster => masterLinks.Count;
            #endregion
            #region Конструктор
            public Vault()
            {
                storedLinks = new HashSet<string>();
                slaveLinks = new Dictionary<string, storedLink>();
                masterLinks = new Dictionary<string, storedLink>();
            }
            #endregion
            #region Методы
            #region Доступ
            public storedLink getSlave(ILink link)
            {
                return getSlave(link.GetId());
            }
            public storedLink getSlave(string linkID)
            {
                return slaveLinks[linkID];
            }
            public storedLink getMaster(ILink link)
            {
                return getMaster(link.GetId());
            }
            public storedLink getMaster(string linkID)
            {
                return masterLinks[linkID];
            }
            #endregion
            #region Добавление
            protected bool addToStorage(ILink link)
            {
                if (link == null) throw new ArgumentNullException();

                return storedLinks.Add(link.GetId());
            }
            public bool storeSlave(ILink link)
            {
                if (!addToStorage(link)) return false;

                storedLink sLink = new storedLink()
                {
                    Link = link
                };

                slaveLinks.Add(link.GetId(), sLink);

                return true;
            }

            public bool storeMaster(ILink link)
            {
                if (!addToStorage(link)) return false;

                storedLink sLink = new storedLink()
                {
                    Link = link
                };

                masterLinks.Add(link.GetId(), sLink);

                return true;
            } 
            #endregion
            #region Удаление
            public void clear()
            {
                storedLinks.Clear();
                slaveLinks.Clear();
                masterLinks.Clear();
            }
            public void deleteSlaveStore()
            {
                var keys = slaveLinks.Select(v => v.Key).ToArray();

                slaveLinks.Clear();

                if (keys == null || keys.Length == 0) return;

                for (int i = 0; i < keys.Length; i++) storedLinks.Remove(keys[i]);
            }

            public void deleteMasterStore()
            {
                var keys = masterLinks.Select(v => v.Key).ToArray();

                masterLinks.Clear();

                if (keys == null || keys.Length == 0) return;

                for (int i = 0; i < keys.Length; i++) storedLinks.Remove(keys[i]);
            }
            public void deleteLink(ILink link)
            {
                deleteLink(link.GetId());
            }
            public void deleteLink(string linkID)
            {
                bool isSlave = slaveLinks.ContainsKey(linkID);
                bool isMaster = masterLinks.ContainsKey(linkID);

                if (!isSlave && !isMaster) throw new noThisLinkInCollectionException();
                if (!storedLinks.Remove(linkID)) throw new noThisLinkInCollectionException();

                if (isMaster) masterLinks.Remove(linkID);
                else slaveLinks.Remove(linkID);
            }
            #endregion
            #region Объект
            public void remove()
            {
                clear();
                parent = null;
                storedLinks = null;
                slaveLinks = null;
                masterLinks = null;
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
        #region Перечисления
        protected enum chng
        {
            date = 1,
            direction = 2,
            dot = 3
        }
        #endregion
        #region Структуры
        protected struct linkDependenceInfo
        {
            public chng changed;
            public ILink link;
        }
        protected struct storedLink
        {
            public ILink Link;
            public Action unsuscribe;

            public void clear()
            {
                Link = null;
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
