using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alter.args;
using alter.iface;
using alter.Service.Extensions;
using alter.types;

namespace alter.Service.classes
{
    public interface ILimitAggregator
    {
        
    }

    public partial class cLimitAggregator
    {

    }
    #region Приоритет
    public partial class cLimitAggregator
    {
        public const int PRIORITY_MAX_VALUE = 100000;
        public const int PRIORITY_MIN_VALUE = 0;
        protected Dictionary<e_Entity, int> priority;

        protected void init_priority()
        {
            priority = new Dictionary<e_Entity, int>()
            {
                {e_Entity.Project, 0},
                {e_Entity.Group, 1},
                {e_Entity.Link, 2}
            };
        }

        public int getPriority(e_Entity entity)
        {
            if (!priority.Keys.Contains(entity)) return -1;

            return priority[entity];
        }

        public bool setPriority(e_Entity entity, int priority)
        {
            if (priority < PRIORITY_MIN_VALUE || priority > PRIORITY_MAX_VALUE) return false;
            if (!Enum.IsDefined(typeof (e_Entity), entity)) return false;
            if (!entity.isEqual(e_Entity.Project, e_Entity.Group, e_Entity.Link)) return false;
            if (this.priority.Values.Contains(priority)) return false;

            this.priority[entity] = priority;

            return true;
        }
        #region Служебные
        protected e_Entity nextPriority(e_Entity entity)
        {
            int current = priority[entity];
            int[] expected = priority.Values.Where(v => v < current).ToArray();

            if (expected.Length == 0) return e_Entity.None;
            else
            {
                int next = expected.Max();
                return priority.Where(v => v.Value == next).First().Key;
            }
        }
        protected e_Entity nextPriority()
        {
            int max = priority.Values.Max();
            return priority.Where(v => v.Value == max).First().Key;
        }
        #endregion
    }
    #endregion
    #region Хранение зависимостей
    public partial class cLimitAggregator
    {
        #region Переменные
        protected depData self;
        protected Dictionary<e_Entity, depData> dependences;
        #endregion
        #region Методы
        #region Инициализатор
        protected void init_DependenceVault()
        {
            dependences = new Dictionary<e_Entity, depData>()
            {
                {e_Entity.Project, new depData(handler_dateChanged, handler_directionChanged, handler_dependDotChanged)},
                {e_Entity.Group, new depData(handler_dateChanged, handler_directionChanged, handler_dependDotChanged)},
                {e_Entity.Link, new depData(handler_dateChanged, handler_directionChanged, handler_dependDotChanged)}
            };

            self = new depData(handler_dateChanged, handler_directionChanged, handler_dependDotChanged);
        }
        #endregion
        #region Зависимости
        public void setSelf(IDependence depend)
        {
            if (depend == null) throw new ArgumentNullException(nameof(depend));

            self.subscribe(e_Entity.Task, depend);
        }
        public void clearSelf()
        {
            self.unsuscribe();
        }
        public void setDependence(e_Entity entity, IDependence depend)
        {
            if (depend == null) throw new ArgumentNullException(nameof(depend));
            if (!entity.isEqual(e_Entity.Project, e_Entity.Group, e_Entity.Link)) throw new ArgumentException("Неверное значение entity");

            dependences[entity].subscribe(entity, depend);
        }
        public void clearDependence(e_Entity entity)
        {
            if (!Enum.IsDefined(typeof(e_Entity), entity)) throw new ArgumentException(nameof(entity));
            if (!entity.isEqual(e_Entity.Project, e_Entity.Group, e_Entity.Link)) throw new ArgumentException("Неверное значение entity");

            dependences[entity].unsuscribe();
        } 
        #endregion
        #endregion
        #region Обработчики событий
        protected void handler_directionChanged(object sender, ea_ValueChange<e_Direction> e, e_Entity entity)
        {
            throw new NotImplementedException();
        }
        protected void handler_dateChanged(object sender, ea_ValueChange<DateTime> e, e_Entity entity)
        {
            throw new NotImplementedException();
        }
        protected void handler_dependDotChanged(object sender, ea_ValueChange<e_Dot> e, e_Entity entity)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Служебные
        protected e_Entity getMinimumDependence()
        {
            throw new NotImplementedException();
        }
        protected e_Entity getMaximumDependence()
        {
            throw new NotImplementedException();
        }

        protected int compareDependences(IDependence first, IDependence second)
            //first [sign] second
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Структура хранения зависимостей
        protected struct depData
        {
            #region Свойства
            public e_Entity entity { get; private set; }
            public IDependence dependence { get; private set; }
            #endregion
            #region Делегаты
            public Action<object, ea_ValueChange<DateTime>, e_Entity> handler_dateChanged;
            public Action<object, ea_ValueChange<e_Direction>, e_Entity> handler_directionChanged;
            public Action<object, ea_ValueChange<e_Dot>, e_Entity> handler_dependDotChanged;
            #endregion
            #region Конструктор

            public depData(Action<object, ea_ValueChange<DateTime>, e_Entity> handler_dateChanged,
                           Action<object, ea_ValueChange<e_Direction>, e_Entity> handler_directionChanged,
                           Action<object, ea_ValueChange<e_Dot>, e_Entity> handler_dependDotChanged)
            {
                this.handler_dateChanged = handler_dateChanged;
                this.handler_dependDotChanged = handler_dependDotChanged;
                this.handler_directionChanged = handler_directionChanged;

                entity = e_Entity.None;
                dependence = null;
            }
            #endregion
            #region Методы
            public void subscribe(e_Entity entity, IDependence dependence)
            {
                if (this.dependence != null) unsuscribe();

                this.dependence = dependence;
                this.entity = entity;

                this.dependence.event_DependDotChanged += changeDependDotHandler;
                this.dependence.event_DateChanged += changeDateHandler;
                this.dependence.event_DirectionChanged += changeDirectionHandler;
            }

            public bool unsuscribe()
            {
                if (dependence == null) return false;

                dependence.event_DependDotChanged -= changeDependDotHandler;
                dependence.event_DateChanged -= changeDateHandler;
                dependence.event_DirectionChanged -= changeDirectionHandler;

                entity = e_Entity.None;
                dependence = null;

                return true;
            }

            public void clear()
            {
                unsuscribe();
                handler_dateChanged = null;
                handler_dependDotChanged = null;
                handler_directionChanged = null;
            }
            #endregion
            #region Служебные
            private void changeDateHandler(object sender, ea_ValueChange<DateTime> e)
            {
                handler_dateChanged?.Invoke(sender, e, entity);
            }
            private void changeDirectionHandler(object sender, ea_ValueChange<e_Direction> e)
            {
                handler_directionChanged?.Invoke(sender, e, entity);
            }
            private void changeDependDotHandler(object sender, ea_ValueChange<e_Dot> e)
            {
                handler_dependDotChanged?.Invoke(sender, e, entity);
            }
            #endregion
        }
        #endregion
    }
    #endregion
}
