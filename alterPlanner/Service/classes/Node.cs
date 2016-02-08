using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.Service.iface;

namespace alter.Service.classes
{
    public class node<T> : INode<T>
        where T : IComparable
    {
        #region variables
        private string nodeID;
        protected T _data;
        protected node<T> _next;
        protected node<T> _previous;
        #endregion
        #region flag
        private bool bIsHead;
        private bool bIsTail;
        #endregion
        #region property
        public string debug_information => __debug_information();
        public bool isTail => bIsTail;
        public bool isHead => bIsHead;
        public string ID => nodeID;
        public bool isConnected { get; private set; }
        public T data { get { return _data; } }
        public node<T> next
        {
            get { return _next; }
            private set
            {
                _next = value;
                checkConnected();
                checkHead();
            }
        }
        public node<T> previous
        {
            get { return _previous; }
            private set
            {
                _previous = value;
                checkConnected();
                checkTail();
            }
        }
        #endregion
        #region Events
        public event EventHandler<node<T>> event_dataChanged;
        public event EventHandler<node<T>> event_Tail;
        public event EventHandler<node<T>> event_Head;
        public event EventHandler<node<T>> event_Remove;
        #endregion
        #region Constructor
        public node()
        {
            nodeID = Guid.NewGuid().ToString();
            _data = default(T);
            _next = null;
            _previous = null;
            isConnected = false;
            bIsHead = true;
            bIsTail = true;

        }
        public node(T data) : this()
        {
            this._data = data;
        }
        public node(T data, node<T> nextNode) : this(data)
        {
            if (!connectTo(nextNode)) throw new ArgumentException();
        }
        ~node()
        {
            remove();
        }
        #endregion
        #region Handlers
        public void handler_dataChange(object sender, EventArgs e)
        {
            event_dataChanged?.Invoke(sender, this);
        }
        protected void onRemove()
        { event_Remove?.Invoke(this, this); }
        #endregion
        #region Methods
        #region Compare
        public int comparePrevious()
        {
            if (previous == null) return -1;
            if (previous.data == null) return 1;
            return data.CompareTo(previous.data);
        }
        public int compareNext()
        {
            if (next == null) return -1;
            if (next.data == null) return 1;
            return data.CompareTo(next.data);
        }
        #endregion
        #region connect
        public bool connectTo(node<T> node)
        {
            if (node == null || node == this) return false;
            if (isConnected) return false;
            setNextNode(node);
            setMePrevious(node);
            isConnected = true;
            return true;
        }
        #endregion
        #region Move
        public bool moveToHead()
        {
            if (!isConnected || next == null) return false;
            moveAfter(getHead());
            return true;
        }
        public bool moveToTail()
        {
            if (!isConnected || previous == null) return false;
            moveBefore(getTail());
            return true;
        }
        public bool moveNext()
        {
            if (next == null || !isConnected) return false;
            moveAfter(next);
            return true;
        }
        public bool movePrevious()
        {
            if (previous == null || !isConnected) return false;
            moveBefore(previous);
            return true;
        }
        public void moveAfter(node<T> previousNode)
        {
            if (previousNode == null) return;
            if (!isConnected) return;
            unset();
            setNextNode(previousNode.next);
            setMeNext(previousNode);
            setPreviousNode(previousNode);
        }
        public void moveBefore(node<T> nextNode)
        {
            if (nextNode == null) return;
            if (!isConnected) return;
            unset();
            setPreviousNode(nextNode.previous);
            setMePrevious(nextNode);
            setNextNode(nextNode);
        }
        #endregion
        #region Find
        public node<T> getTail()
        {
            if (previous == null) return this;
            node<T> tmpPosition = this;
            do
            {
                tmpPosition = tmpPosition.previous;
            } while (tmpPosition.previous != null);
            return tmpPosition;
        }
        public node<T> getHead()
        {
            if (next == null) return this;
            node<T> tmpPosition = this;
            do
            {
                tmpPosition = tmpPosition.next;
            } while (tmpPosition.next != null);
            return tmpPosition;
        }
        #endregion
        #region Set
        protected bool setNextNode(node<T> nextNode)
        {
            if (this == nextNode || next == nextNode || previous == nextNode) return false;
            next = nextNode;
            return true;
        }
        protected bool setPreviousNode(node<T> previousNode)
        {
            if (this == previousNode || next == previousNode || previous == previousNode) return false;
            previous = previousNode;
            return true;
        }
        private void setMePrevious(node<T> connectNode)
        {
            if (connectNode.previous != null) connectNode.previous.setNextNode(this);
            connectNode.setPreviousNode(this);
        }
        private void setMeNext(node<T> connectNode)
        {
            if (connectNode.next != null) connectNode.next.setPreviousNode(this);
            connectNode.setNextNode(this);
        }
        private void unset()
        {
            if (next != null) next.setPreviousNode(previous);
            if (previous != null) previous.setNextNode(next);
        }
        #endregion
        #region object
        public void remove()
        {
            unset();
            _next = null;
            _previous = null;
            isConnected = false;
            _data = default(T);
            onRemove();
        }
        #endregion
        #region Data
        #region delegateProcessing
        public object process(Func<node<T>,object,object> aDataProcess, object Object)
        {
            if (data != null) return aDataProcess(this, Object);
            return Object;
        }
        public bool process(Action<T> aDataProcess)
        {
            if (data != null)
            {
                aDataProcess(data);
                return true;
            }
            return false;
        }
        public bool process(Action<node<T>> aDataProcess)
        {
            if (data != null)
            {
                aDataProcess(this);
                return true;
            }
            return false;
        }
        public object processNext(Func<node<T>, object, object> aDataProcess, object Object)
        {
            object result = Object;
            if (data != null) result  = aDataProcess(this, Object);
            if (!isHead) result = next.processNext(aDataProcess, result);
            return result;
        }
        public void processNext(Action<node<T>> aDataProcess)
        {
            if (data != null) aDataProcess(this);
            if (!isHead) next.processNext(aDataProcess);
        }
        public object processPrevious(Func<node<T>, object, object> aDataProcess, object Object)
        {
            object result = Object;
            if (data != null) result = aDataProcess(this, Object);
            if (!isTail) result = next.processPrevious(aDataProcess, result);
            return result;
        }
        public void processPrevious(Action<node<T>> aDataProcess)
        {
            if (data != null) aDataProcess(this);
            if (!isTail) previous.processPrevious(aDataProcess);
        }
        public void processNext(Action<T> aDataProcess)
        {
            if (data != null) aDataProcess(data);
            if (!isHead) next.processNext(aDataProcess);
        }
        public void processPrevious(Action<T> aDataProcess)
        {
            aDataProcess(data);
            if (!isTail) previous.processPrevious(aDataProcess);
        }
        #endregion
        #region manipulation
        public bool setData(T data)
        {
            if (!_data.Equals(default(T))) return false;
            _data = data;
            return true;
        }
        #endregion
        #endregion
        #endregion
        #region Service

        private string __debug_information()
        {
            string bound = new string('-', 40);
            string nextID = (next != null) ? next.ID : "null";
            string previousID = (previous != null) ? previous.ID : "null";
            string result =
                string.Format
                    ("{0}\nNode ID: {1}\nConnected: {2} isHead: {4} isTail: {3}\nData: {5}" +
                     "\nNext node: {6}\nPrevious node: {7}\n{0}",
                        bound, ID.ToString(), isConnected, isTail, isHead,
                        data.ToString(), nextID, previousID);

            return result;
        }
        private void checkHead()
        {
            if (bIsHead && next != null) bIsHead = false;
            if (!bIsHead && next == null)
            {
                bIsHead = true;
                event_Head?.Invoke(this, this);
            }
        }

        private void checkTail()
        {
            if (bIsTail && previous != null) bIsTail = false;
            if (!bIsTail && previous == null)
            {
                bIsTail = true;
                event_Tail?.Invoke(this, this);
            }
        }
        private void checkConnected()
        {
            if (next == null && previous == null && isConnected) isConnected = false;
            if ((next != null || previous != null) && !isConnected) isConnected = true;
        }
        #endregion
    }

}
