using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.classes;
using alter.iface;
using alter.Link.iface.Base;
using alter.Task.classes;
using alter.types;

namespace alter.Link.classes
{
    public partial class linkFactory2 : ILinkFactory2
    {
        #region Переменные
        protected storageLinks vault;
        protected Identity id;
        protected Action unsubscribeOwner;
        protected Action unsubscribeStorage;
        #endregion
        #region Свойства
        public int count => vault.count;
        #endregion
        #region События
        public event EventHandler<ILink_2> event_linkCreated;
        public event EventHandler<ILink_2> event_linkRemoved;
        #endregion
        #region Конструктор
        public linkFactory2(IRemovable owner)
        {
            init_Identity();
            init_StorageLinks();

            owner.event_ObjectDeleted += handler_ownerDelete;

            unsubscribeOwner = () =>
            {
                if (owner != null)
                {
                    owner.event_ObjectDeleted -= handler_ownerDelete;
                }
                unsubscribeOwner = null;
            };
        }
        ~linkFactory2()
        {
            clear();
        }
        #endregion
        #region Инициализаторы
        protected void init_Identity()
        {
            id = new Identity(e_Entity.Factory);
        }
        protected void init_StorageLinks()
        {
            vault = new storageLinks();
            vault.event_linkRemoved += handler_linkRemoved;

            unsubscribeStorage = () =>
            {
                if (vault != null)
                {
                    vault.event_linkRemoved -= handler_linkRemoved;
                }
                unsubscribeStorage = null;
            };
        }
        #endregion
        #region Обработчики
        protected void handler_linkRemoved(object sender, ILink_2 e)
        {
            event_linkRemoved?.Invoke(this, e);
        }
        #endregion
        #region Методы
        public ILink_2 createLink(IConnectible precursor, IConnectible follower, e_TskLim limit, double delay)
        {
            if(delay < link_2.DELAY_MINIMUM_VALUE) throw new ArgumentException(nameof(delay));
            if(!Enum.IsDefined(typeof(e_TskLim), limit)) throw new ArgumentException(nameof(limit));
            if (!checkMembers(precursor, follower)) return null;

            link_2 newLink = new link_2(precursor, follower, limit, delay);
            vault.addLink(newLink);

            return newLink;
        }
        public bool removeLink(string linkID)
        {
            if (vault.isLinkExists(linkID))
            {
                vault.removeLink(linkID);
                return true;
            }
            else return false;
        }
        public ILink_2[] getInvolved(string memberID)
        {
            return vault.getInvolved(memberID);
        }
        public ILink_2[] getInvolved(string memberID, e_DependType dependType)
        {
            return vault.getInvolved(memberID, dependType);
        }
        public ILink_2 getLink(string linkID)
        {
            return vault.getLink(linkID);
        }
        public ILink_2[] getLinks()
        {
            return vault.getLinks();
        }
        #endregion
        #region Служебные
        protected void clear()
        {
            unsubscribeStorage?.Invoke();
            unsubscribeOwner?.Invoke();
            vault = null;
            id = null;
        }
        protected bool checkMembers(IConnectible precursor, IConnectible follower)
        {
            if(precursor == null) throw new ArgumentNullException(nameof(precursor));
            if (follower == null) throw new ArgumentNullException(nameof(follower));
            if(follower.GetId() == precursor.GetId() && follower.GetType() == precursor.GetType()) 
                throw new ApplicationException("Одна и та же сущность не может являться последователем и предшественником в одном экземпляре связи");

            Func<ILink_2, bool> check = lnk =>
            {
                if (lnk.isMemberExist(precursor) && lnk.isMemberExist(follower)) return false;
                return true;
            };

            ILink_2[] arr = vault.getLinks().Where(v => check(v)).ToArray();

            return arr.Length == 0 ? true : false;
        }
        #endregion
        #region Интерфейс
        #region IId
        public string GetId()
        {
            return id.Id;
        }
        e_Entity IId.GetType()
        {
            return id.Type;
        }
        #endregion
        #region IChild
        public void handler_ownerDelete(object sender, ea_IdObject objectId)
        {
            clear();
        }
        #endregion
        #endregion
    }
    #region Внутренние сущности
    public partial class linkFactory2 : ILinkFactory2
    {
        #region Хранилище связей
        protected class storageLinks
        {
            #region Переменные
            Dictionary<string, ILink_2> _storage;
            #endregion
            #region Свойства
            public int count => _storage.Count;
            #endregion
            #region События
            public event EventHandler<ILink_2> event_linkRemoved;
            #endregion
            #region Конструктор
            public storageLinks()
            {
                _storage = new Dictionary<string, ILink_2>();
            }
            ~storageLinks()
            {
                clear();
                _storage = null;
            }
            #endregion
            #region Обработчики
            protected void handler_linkRemoved(object sender, ea_IdObject e)
            {
                removeLink(e.Id.Id);
            }
            #endregion
            #region Методы
            public bool isLinkExists(string linkID)
            {
                return _storage.ContainsKey(linkID);
            }
            public ILink_2 getLink(string linkID)
            {
                if (!isLinkExists(linkID)) return null;

                return _storage[linkID];
            }
            public ILink_2[] getLinks()
            {
                if (_storage.Count == 0) return null;

                return _storage.Values.ToArray();
            }
            public ILink_2[] getInvolved(string memberID)
            {
                if(_storage.Count == 0) return new ILink_2[0];

                return _storage.Values.Where(v => v.isMemberExist(memberID)).ToArray();
            }
            public ILink_2[] getInvolved(string memberID, e_DependType dependType)
            {
                ILink_2[] result = getInvolved(memberID);
                if (result.Length == 0) return result;
                return result.Where(v => v.getDependType(memberID) == dependType).ToArray();
            }
            public void clear()
            {
                if (_storage != null)
                {
                    string[] keys = _storage.Keys.ToArray();
                    for (int i = 0; i < keys.Length; i++) removeLink(keys[i]);
                }
            }
            #endregion
            #region Служебные
            public void removeLink(string linkID)
            {
                _storage[linkID].event_ObjectDeleted -= handler_linkRemoved;

                _storage.Remove(linkID);
            }
            public void addLink(ILink_2 link)
            {
                _storage.Add(link.GetId(), link);

                link.event_ObjectDeleted += handler_linkRemoved;
            }
            #endregion
        }
        #endregion
    }
    #endregion

}
