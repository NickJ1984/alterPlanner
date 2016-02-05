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
        #region property
        public bool isTail => previous == null;
        public bool isHead => next == null;
        public string ID => nodeID;
        public bool isConnected { get; private set; }
        public T data { get { return _data; } }
        public node<T> next
        {
            get { return _next; }
            private set
            {
                _next = value;
            }
        }
        public node<T> previous
        {
            get { return _previous; }
            private set
            {
                _previous = value;
            }
        }
        #endregion
        #region Events
        public event EventHandler<node<T>> event_nextNull;
        public event EventHandler<node<T>> event_previousNull;
        public event EventHandler<node<T>> event_dataChanged;
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
        protected void onNextNull()
        { event_nextNull?.Invoke(this, this); }
        protected void onPreviousNull()
        { event_previousNull?.Invoke(this, this); }
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
            moveBefore(getHead());
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
            setMeNext(previousNode);
        }
        public void moveBefore(node<T> nextNode)
        {
            if (nextNode == null) return;
            if (!isConnected) return;
            unset();
            setMeNext(nextNode);
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
            if (this == nextNode || _next == nextNode || _previous == nextNode) return false;
            next = nextNode;
            checkNullNeighbours();
            return true;
        }
        protected bool setPreviousNode(node<T> previousNode)
        {
            if (this == previousNode || _next == previousNode || _previous == previousNode) return false;
            previous = previousNode;
            checkNullNeighbours();
            return true;
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
        public bool processData(Action<T> aDataProcess)
        {
            if (data != null)
            {
                aDataProcess(data);
                return true;
            }
            return false;
        }
        public bool processData(Action<node<T>> aDataProcess)
        {
            if (data != null)
            {
                aDataProcess(this);
                return true;
            }
            return false;
        }
        public void processDataNext(Action<node<T>> aDataProcess)
        {
            if (data != null) aDataProcess(this);
            if (isHead) next.processDataNext(aDataProcess);
        }
        public void processDataPrevious(Action<node<T>> aDataProcess)
        {
            if (data != null) aDataProcess(this);
            if (isTail) previous.processDataPrevious(aDataProcess);
        }
        public void processDataNext(Action<T> aDataProcess)
        {
            if (data != null) aDataProcess(data);
            if (isHead) next.processDataNext(aDataProcess);
        }
        public void processDataPrevious(Action<T> aDataProcess)
        {
            if (data != null) aDataProcess(data);
            if (isTail) previous.processDataPrevious(aDataProcess);
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
        private void checkNullNeighbours()
        {
            if (next == null) onNextNull();
            if (previous == null) onPreviousNull();
            if (next == null && previous == null && isConnected) isConnected = false;
            if ((next != null || previous != null) && !isConnected) isConnected = true;
        }
        private void unset()
        {
            if (next != null) next.setPreviousNode(previous);
            if (previous != null) previous.setNextNode(next);
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
        #endregion


    }



}
