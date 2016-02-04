using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.Group.iface;
using alter.iface;
using alter.Service.iface;
using alter.types;
using alter.Task.iface;

namespace alter.Service.classes
{
    public class dependDotAdapter : IDependDotAdapter
    {
        #region Variables

        protected bool cleared = false;
        protected object sender;
        protected IId parentID;
        protected IDot selectedDot;
        protected ILine line;
        #endregion
        #region Delegates
        protected Action unsubscribe = () => { };
        #endregion
        #region Properties

        public e_Dot dotType => selectedDot.GetDotType();
        public DateTime date => selectedDot.GetDate();
        #endregion
        #region Events
        public event EventHandler<ea_ValueChange<DateTime>> event_DateChanged;
        public event EventHandler<ea_ValueChange<e_Dot>> event_dotTypeChanged;
        #endregion
        #region Constructors
        public dependDotAdapter(object ownerSender, IId IDinterface, ILine LineInterface)
        {
            if(ownerSender == null || IDinterface == null || LineInterface == null) 
                throw new ArgumentNullException();
            sender = ownerSender;
            parentID = IDinterface;
            line = LineInterface;
        }
        public dependDotAdapter(ITask taskInterface)
            :this(taskInterface, taskInterface, taskInterface)
        { }
        public dependDotAdapter(IGroup groupInterface)
            :this(groupInterface, groupInterface, groupInterface)
        { }

        ~dependDotAdapter()
        {
            if(!cleared) clear();
        }
        #endregion
        #region Handlers

        protected void handler_dotDateChanged(object sender, ea_ValueChange<DateTime> e)
        {
            eventDateChangedInvoke(sender, e);
        }

        protected void onDotTypeChange(e_Dot Old, e_Dot New)
        {
            event_dotTypeChanged?.Invoke(sender,new ea_ValueChange<e_Dot>(Old, New));
        }

        protected void eventDateChangedInvoke(object sender, ea_ValueChange<DateTime> e)
        {
            event_DateChanged?.Invoke(sender, e);
        }
        #endregion
        #region Methods
        #region Dot
        public DateTime GetDate()
        {
            return date;
        }

        public e_Dot GetDotType()
        {
            return dotType;
        }
        #endregion
        #region ID
        public IId getParentId()
        {
            return parentID;
        }
        #endregion
        #region DependDot
        public bool setDependDot(e_Dot dependDot)
        {
            return subscribeDot(dependDot);
        }
        #endregion
        #region Object

        public void clear()
        {
            unsubscribe();
            unsubscribe();
            unsubscribe = null;
            sender = null;
            parentID = null;
            selectedDot = null;
            line = null;
            cleared = true;
        }
        #endregion
        #endregion
        #region Service
        protected void subscribeHandler(IDot dot)
        {
            if(dot == null) throw new ArgumentNullException();
            unsubscribe();
            dot.event_DateChanged += handler_dotDateChanged;
            unsubscribe = () =>
            {
                dot.event_DateChanged -= handler_dotDateChanged;
                unsubscribe = () => { };
            };
        }

        protected void checkChangedValues(DateTime OldDate, e_Dot OldType)
        {
            if(OldType != dotType) onDotTypeChange(OldType, dotType);
            if(OldDate != date) eventDateChangedInvoke(sender, new ea_ValueChange<DateTime>(OldDate, date));
        }
        protected bool subscribeDot(e_Dot type)
        {
            if (type == selectedDot.GetDotType() && !Enum.IsDefined(typeof (e_Dot), type)) return false;

            DateTime oldDate = date;
            e_Dot oldType = dotType;
            IDot dot = line.GetDot(type);

            subscribeHandler(dot);
            selectedDot = dot;
            checkChangedValues(oldDate, oldType);
            return true;
        }

        #endregion
    }
}
