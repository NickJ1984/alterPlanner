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
    class LinkManager : ILinkManager
    {
        #region vars
        private readonly string _ownerId;
        private ILinkFactory _factory;
        private NeighborVault _neighbor;
        private HashSet<Elem> _uniqLinks;
        #endregion
        #region events
        public event EventHandler<ea_Value<ILink>> event_LinkAdded;
        public event EventHandler<ea_Value<ILink>> event_LinkDeleted;
        #endregion
        #region constructors
        public LinkManager(ILinkFactory linkFactory, string ownerId)
        {
            this._ownerId = ownerId;
            _factory = linkFactory;
            _neighbor = new NeighborVault(ownerId);
            _uniqLinks = new HashSet<Elem>(new CmpElem());
        }
        #endregion
        #region handlers
        private void OnLinkAdd(ea_Value<ILink> args)
        {
            EventHandler<ea_Value<ILink>> handler = event_LinkAdded;
            if (handler != null) handler(this, args);
        }
        private void OnLinkDelete(ea_Value<ILink> args)
        {
            EventHandler<ea_Value<ILink>> handler = event_LinkDeleted;
            if (handler != null) handler(this, args);
        }
        private void OnLinkDelete(object sender, ea_IdObject e)
        {
            DelLink(e.Id.Id);
        }
        #endregion
        #region methods
        #region delete
        public bool DelLink()
        {
            Elem[] current = _uniqLinks.ToArray();

            for(int i = 0; i < _uniqLinks.Count; i++)
            {
                DelLink(current[i]);
            }
            return (_uniqLinks.Count == 0) ? true : false;
        }
        public bool DelLink(string linkId)
        {
            Elem delElem = _uniqLinks.Where((v, i) => v.Id == linkId).ElementAt(0);

            return DelLink(delElem);
        }
        public bool DelLink(e_DependType dependType)
        {
            bool result = true;
            Elem[] indexes = _uniqLinks.Where(v => v.Type == dependType).ToArray();
            if (indexes == null || indexes.Length == 0) return false;

            for (int i = 0; i < indexes.Length; i++) result = result && DelLink(indexes[i]);
            return result;
        }
        private bool DelLink(Elem delElement)
        {
            ILink link = _factory.GetLink(delElement.Id);

            if (_uniqLinks.Remove(delElement))
            {
                link.event_ObjectDeleted -= OnLinkDelete;
                _neighbor.Del(delElement.Type, link);

                OnLinkDelete(new ea_Value<ILink>(link));
                //link.Unsuscribe(_ownerId);
                return true;
            }
            else return false;
        }
        #endregion
        #region get
        public string[] GetLinks()
        {
            return _uniqLinks.Select(s => s.Id).ToArray();
        }
        public string[] GetLinks(e_DependType dependType)
        {
            return _uniqLinks.Where(s => s.Type == dependType).Select(e => e.Id).ToArray();
        }
        #endregion
        public bool AddLink(e_DependType type, ILink newLink)
        {
            Elem newElem = new Elem(newLink, type);
            if (!_uniqLinks.Add(newElem)) return false;
            if (!_neighbor.Add(type, newLink))
            {
                _uniqLinks.Remove(newElem);
                return false;
            }

            newLink.event_ObjectDeleted += OnLinkDelete;

            return true;
        }
        public int GetLinksCount(e_DependType dependType)
        {
            return _uniqLinks.Where(s => s.Type == dependType).Count();
        }
        public bool LinkExist(string linkId)
        {
            Elem e = new Elem();
            e.Id = linkId;
            e.Type = e_DependType.Master;
            return _uniqLinks.Contains(e);
        }
        #endregion
        #region inner entities
        protected struct Elem
        {
            public string Id;
            public e_DependType Type;

            public Elem(ILink link, e_DependType type)
            {
                this.Id = link.GetId();
                this.Type = type;
            }
        }
        protected class CmpElem : IEqualityComparer<Elem>
        {
            public bool Equals(Elem x, Elem y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(Elem obj)
            {
                return obj.Id.GetHashCode();
            }
        }
        protected class NeighborVault
        {
            private HashSet<string> _uniq;

            public NeighborVault(string ownerId)
            {
                _uniq = new HashSet<string>();
                _uniq.Add(ownerId);
            }
            public bool Add(e_DependType type, ILink link)
            {
                string nId = link.GetInfoMember(Invert(type)).GetMemberId().GetId();
                return _uniq.Add(nId);
            }
            public bool Del(e_DependType type, ILink link)
            {
                string nId = link.GetInfoMember(Invert(type)).GetMemberId().GetId();
                return _uniq.Remove(nId);
            }
            public void Clear()
            {
                _uniq.Clear();
            }
            private e_DependType Invert(e_DependType type)
            {
                return (type == e_DependType.Master) ? e_DependType.Slave : e_DependType.Master;
            }
        }
        #endregion
    }
}
