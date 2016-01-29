using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.Link.iface;
using alter.types;

namespace alter.Link.classes
{
    class linkManager : ILinkManager
    {
        #region vars
        private readonly string ownerID;
        private ILinkFactory factory;
        private neighborVault neighbor;
        private HashSet<elem> uniqLinks;
        #endregion
        #region events
        public event EventHandler<EA_value<ILink>> event_linkAdded;
        public event EventHandler<EA_value<ILink>> event_linkDeleted;
        #endregion
        #region constructors
        public linkManager(ILinkFactory linkFactory, string ownerID)
        {
            this.ownerID = ownerID;
            factory = linkFactory;
            neighbor = new neighborVault(ownerID);
            uniqLinks = new HashSet<elem>(new cmpElem());
        }
        #endregion
        #region handlers
        private void onLinkAdd(EA_value<ILink> args)
        {
            EventHandler<EA_value<ILink>> handler = event_linkAdded;
            if (handler != null) handler(this, args);
        }
        private void onLinkDelete(EA_value<ILink> args)
        {
            EventHandler<EA_value<ILink>> handler = event_linkDeleted;
            if (handler != null) handler(this, args);
        }
        private void onLinkDelete(object sender, EA_IDObject e)
        {
            delLink(e.ID.ID);
        }
        #endregion
        #region methods
        #region delete
        public bool delLink()
        {
            elem[] current = uniqLinks.ToArray();

            for(int i = 0; i < uniqLinks.Count; i++)
            {
                delLink(current[i]);
            }
            return (uniqLinks.Count == 0) ? true : false;
        }
        public bool delLink(string linkID)
        {
            elem delElem = uniqLinks.Where((v, i) => v.ID == linkID).ElementAt(0);

            return delLink(delElem);
        }
        public bool delLink(eDependType dependType)
        {
            bool result = true;
            elem[] indexes = uniqLinks.Where(v => v.type == dependType).ToArray();
            if (indexes == null || indexes.Length == 0) return false;

            for (int i = 0; i < indexes.Length; i++) result = result && delLink(indexes[i]);
            return result;
        }
        private bool delLink(elem delElement)
        {
            ILink link = factory.getLink(delElement.ID);

            if (uniqLinks.Remove(delElement))
            {
                link.event_objectDeleted -= onLinkDelete;
                neighbor.del(delElement.type, link);

                onLinkDelete(new EA_value<ILink>(link));
                link.unsuscribe(ownerID);
                return true;
            }
            else return false;
        }
        #endregion
        #region get
        public string[] getLinks()
        {
            return uniqLinks.Select(s => s.ID).ToArray();
        }
        public string[] getLinks(eDependType dependType)
        {
            return uniqLinks.Where(s => s.type == dependType).Select(e => e.ID).ToArray();
        }
        #endregion
        public bool addLink(eDependType type, ILink newLink)
        {
            elem newElem = new elem(newLink, type);
            if (!uniqLinks.Add(newElem)) return false;
            if (!neighbor.add(type, newLink))
            {
                uniqLinks.Remove(newElem);
                return false;
            }

            newLink.event_objectDeleted += onLinkDelete;

            return true;
        }
        public int getLinksCount(eDependType dependType)
        {
            return uniqLinks.Where(s => s.type == dependType).Count();
        }
        public bool linkExist(string linkID)
        {
            elem e = new elem();
            e.ID = linkID;
            e.type = eDependType.master;
            return uniqLinks.Contains(e);
        }
        #endregion
        #region inner entities
        protected struct elem
        {
            public string ID;
            public eDependType type;

            public elem(ILink link, eDependType type)
            {
                this.ID = link.getID();
                this.type = type;
            }
        }
        protected class cmpElem : IEqualityComparer<elem>
        {
            public bool Equals(elem x, elem y)
            {
                return x.ID == y.ID;
            }

            public int GetHashCode(elem obj)
            {
                return obj.ID.GetHashCode();
            }
        }
        protected class neighborVault
        {
            private HashSet<string> uniq;

            public neighborVault(string ownerID)
            {
                uniq = new HashSet<string>();
                uniq.Add(ownerID);
            }
            public bool add(eDependType type, ILink link)
            {
                string nID = link.getInfoMember(invert(type)).getMemberID().getID();
                return uniq.Add(nID);
            }
            public bool del(eDependType type, ILink link)
            {
                string nID = link.getInfoMember(invert(type)).getMemberID().getID();
                return uniq.Remove(nID);
            }
            public void clear()
            {
                uniq.Clear();
            }
            private eDependType invert(eDependType type)
            {
                return (type == eDependType.master) ? eDependType.slave : eDependType.master;
            }
        }
        #endregion
    }
}
