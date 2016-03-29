using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.iface;
using alter.Link.iface.Base;
using alter.types;

namespace alter.Link.classes
{
    public partial class linkManager2 : ILinkManager2
    {
        #region Переменные
        protected IId owner;
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
        public int linkCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }
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
            protected Dictionary<string, ILink_2> vault;
            protected HashSet<string> neighboursID;
            protected IId owner;
            #endregion
            #region Свойства
            public int count => vault.Count;
            #endregion
            #region События
            public EventHandler<KeyValuePair<e_DependType, ILink_2>> event_linkAdded;
            public event EventHandler<ILink_2> event_linkRemoved;
            #endregion
            #region Конструктор
            public linkStorage(IId owner)
            {
                this.owner = owner;
                vault = new Dictionary<string, ILink_2>();
                neighboursID = new HashSet<string>();
            }
            #endregion
            #region Методы
            public bool add(e_DependType depend, ILink_2 link)
            {
                if(!Enum.IsDefined(typeof(e_DependType), depend)) throw new ArgumentException(nameof(depend));
                if(link == null) throw new ArgumentNullException(nameof(link));
                if (vault.Keys.Contains(link.GetId())) return false;
                string NID =
                    link.getMemberID(depend == e_DependType.Master ? 
                                     e_DependType.Slave : e_DependType.Master).GetId();
                if (owner.GetId() == NID) return false;
                if (neighboursID.Contains(NID)) return false;

                neighboursID.Add(NID);
                vault.Add(link.GetId(), link);

                link.event_ObjectDeleted += linkRemovedHandler;

                event_linkAdded?.Invoke(this, new KeyValuePair<e_DependType, ILink_2>(depend, link));

                return true;
            }
            public ILink_2[] getLinks()
            {
                return vault.Values.ToArray();
            }
            public ILink_2[] getLinks(e_DependType depend)
            {
                return vault.Values.Where(v => v.getDependType(owner.GetId()) == depend).ToArray();
            }
            public ILink_2 getLink(string linkID)
            {
                return vault[linkID];
            }
            public bool isLinkExist(string linkID)
            {
                return vault.Keys.Contains(linkID);
            }
            public bool remove(string linkID)
            {
                if (!vault.Keys.Contains(linkID)) return false;

                vault[linkID].unsubscribe(owner.GetId());

                return true;
            }
            #endregion
            #region Обработчики
            protected void linkRemovedHandler(object sender, ea_IdObject e)
            {
                removeLink(e.Id.Id);    
            }
            #endregion
            #region Служебные
            protected void removeLink(string linkID)
            {
                ILink_2 link = vault[linkID];

                link.event_ObjectDeleted -= linkRemovedHandler;

                event_linkRemoved?.Invoke(this, link);
                
                neighboursID.Remove(NID(link));
                vault.Remove(linkID);
            }
            protected string NID(ILink_2 link)
            {
                e_DependType neighbourDepend = link.getDependType(owner.GetId()) == e_DependType.Master
                    ? e_DependType.Slave
                    : e_DependType.Master;

                return link.getMemberID(neighbourDepend).GetId();
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
