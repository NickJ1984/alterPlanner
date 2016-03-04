using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.classes;
using alter.iface;
using alter.Link.iface;
using alter.Project.iface;
using alter.Service.Extensions;
using alter.types;

namespace alter.Link.classes
{
    public partial class linkFactory : ILinkFactory
    {
        #region Переменные
        protected IProject parent;
        protected Identity _id;
        protected linkStorage _storage;
        #endregion
        #region Свойства
        public int count => _storage.Count;
        #endregion
        #region События
        public event EventHandler<IId> event_linkFactoryRemoved;
        public event EventHandler<IId> event_createdLink;
        public event EventHandler<IId> event_removedLink;
        #endregion
        #region Конструктор
        public linkFactory(IProject parent)
        {
            if(parent == null) throw new ArgumentNullException();

            this.parent = parent;
            _id = new Identity(e_Entity.Factory);

            _storage = new linkStorage(this);
        }
        ~linkFactory()
        {
            parent = null;
            _id = null;
            _storage = null;
        }
        #endregion
        #region Обработчики
        public void handler_ownerDelete(object sender, ea_IdObject objectId)
        {
            remove();
        }
        #endregion
        #region Методы
        #region Создание связи
        public ILink CreateLink(e_TskLim type, IDock master, IDock slave)
        {
            ILink link = _storage.Add(master, slave, type);

            event_createdLink?.Invoke(this, link);

            return link;
        }
        #endregion
        #region Удаление связи
        public bool deleteLink()
        {
            for (int i = 0; i < _storage.Count; i++)
                event_removedLink?.Invoke(this, _storage[i]);

            _storage.Clear();

            return true;
        }
        public bool deleteLink(string dlink)
        {
            ILink link = _storage.getLink(dlink);

            if (_storage.Remove(dlink))
            {
                event_removedLink?.Invoke(this, link);

                return true;
            }
            return false;
        }
        public void unsuscribe(string subscriberID, string linkID)
        {
            if(string.IsNullOrEmpty(subscriberID) || string.IsNullOrEmpty(linkID))
                throw new ArgumentNullException();

            ILink link = _storage.getLink(linkID);

            if(!link.isItMember(subscriberID))
                throw new ArgumentException(string.Format("Члена связи {0} с идентификатором {1} не существует", linkID, subscriberID));

            _storage.Remove(linkID);
        }
        #endregion
        #region Доступ связи
        public ILink GetLink(string dlink)
        {
            return _storage.getLink(dlink);
        }
        #endregion
        #region Методы фабрики
        #region Идентификация
        public string GetId()
        {
            return _id.Id;
        }
        e_Entity IId.GetType()
        {
            return _id.Type;
        }
        #endregion
        #region Удаление
        protected void remove()
        {
            parent.event_ObjectDeleted -= handler_ownerDelete;
            _storage.Clear();
            parent = null;
            _storage = null;
            _id = null;

            event_linkFactoryRemoved?.Invoke(this, this);
        }
        #endregion
        #endregion
        #endregion
    }

    #region Хранилище связей
    public partial class linkFactory : ILinkFactory
    {
        protected class linkStorage : ICollection<ILink>, IEnumerator<ILink>
        {
            #region Переменные
            protected linkFactory parent;
            protected Dictionary<string, ILink> storage;
            #endregion
            #region Индексатор
            public ILink this[int index]
            {
                get
                {
                    if(index < 0 || index >= Count) throw new ArgumentOutOfRangeException();
                    return storage.Values.ElementAt(index);
                }
            }
            #endregion
            #region Конструктор
            public linkStorage(linkFactory Parent)
            {
                parent = Parent;
                storage = new Dictionary<string, ILink>();
            }

            ~linkStorage()
            {
                parent = null;
                storage = null;
            }
            #endregion
            #region Методы
            #region Добавление связи
            public ILink Add(IDock precursor, IDock follower, e_TskLim limitType)
            {
                if (!Enum.IsDefined(typeof(e_TskLim), limitType)) throw new ArgumentException("Неверное значение аргумента limitType");
                if (precursor == null || follower == null) throw new ArgumentNullException();
                if (!precursor.GetType().isEqual(e_Entity.Group, e_Entity.Task) ||
                   !follower.GetType().isEqual(e_Entity.Group, e_Entity.Task))
                    throw new ArgumentException("Аргумент имеет неверный тип");
                if (parent.isLoop(precursor, follower)) throw new ArgumentException("Обнаружено зацикливание при создании новой связи");

                link Link = new link(precursor, follower, limitType);
                Add(Link);

                return Link;
            }
            #endregion
            #region Доступ к связи
            public ILink getLink(string linkID)
            {
                if (storage.Keys.Contains(linkID)) return storage[linkID];
                throw new ArgumentException(string.Format("Связь с идентификатором {0} отсутствует в хранилище", linkID));
            }
            public bool Contains(string linkID)
            {
                return storage.Keys.Contains(linkID);
            }
            #endregion
            #region Удаление связи
            public bool Remove(string linkID)
            {
                if (string.IsNullOrEmpty(linkID)) throw new ArgumentNullException();
                if (!storage.Keys.Contains(linkID)) return false;

                return Remove(storage[linkID]);
            }
            #endregion
            #endregion
            #region ICollection
            public int Count => storage.Count;
            public bool IsReadOnly => false;
            


            public void Add(ILink item)
            {
                if (item == null) throw new NullReferenceException();

                storage.Add(item.GetId(), item);
            }
            public void Clear()
            {
                if (Count > 0)
                {
                    foreach (var value in storage.Values)
                    {
                        value.DeleteObject();
                    }
                    storage.Clear();
                }
            }
            public bool Contains(ILink item)
            {
                return storage.Values.Contains(item);
            }
            public void CopyTo(ILink[] array, int arrayIndex)
            {
                if(Count == 0) return;
                if(array.isNullOrEmpty()) throw new NullReferenceException();
                int memorySpace = array.Length - arrayIndex;
                if(!array.isInRange(arrayIndex) || memorySpace < Count) throw new ArgumentOutOfRangeException();

                ILink[] links = storage.Values.ToArray();

                Array.Copy(links, 0, array, arrayIndex, Count);
            }
            public IEnumerator<ILink> GetEnumerator()
            {
                return this;
            }
            public bool Remove(ILink item)
            {
                if(item == null) throw new NullReferenceException();
                if (!storage.Values.Contains(item)) return false;

                item.DeleteObject();

                return storage.Remove(item.GetId());
            }
            #endregion
            #region IEnumerator<ILink>
            protected ILink _current;
            protected int _index;
            object IEnumerator.Current => _current;
            public ILink Current => _current;
            public void Reset()
            {
                _index = -1;
                _current = null;
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }
            public bool MoveNext()
            {
                if (Count == 0) return false;
                if (_index < Count - 1)
                {
                    _index++;
                    _current = storage.Values.ElementAt(_index);
                    return true;
                }
                else return false;
            }
            public void Dispose()
            {
                Clear();
            }
            #endregion
        }
    }
    #endregion
    #region Алгоритм антизацикливания связей
    public partial class linkFactory : ILinkFactory
    {
        protected bool isLoop(IDock precursor, IDock follower)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
