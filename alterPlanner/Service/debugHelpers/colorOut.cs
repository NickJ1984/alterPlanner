using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using alter.Service.Extensions;

namespace alter.Service.debugHelpers
{
    public static class actsExt
    {
        public static string toTxt(this acts self)
        {
            switch (self)
            {
                case acts.Invoke:
                    return "вызван";
                case acts.Add:
                    return "добавлен";
                case acts.Delete:
                    return "удален";
                case acts.CurrentState:
                    return "текущее состояние";
                case acts.Change:
                    return "изменен";
                case acts.Set:
                    return "установлен";
                case acts.Created:
                    return "создан";
                default:
                    return string.Empty;
            }
        }
    }
    public enum acts
    {
        CurrentState = 0,
        Add = 2,
        Delete = 4,
        Change = 8,
        Set = 16,
        Invoke = 32,
        Created = 64
    }

    public class objectMessages
    {
        #region Индексатор
        public string this[acts type]
        {
            get
            {
                if (!messages.ContainsKey(type)) return string.Empty;
                return messages[type];
            }
            set
            {
                if (!messages.ContainsKey(type)) messages.Add(type, value);
                else messages[type] = value;
            }
        }
        #endregion
        #region Переменные
        #region Объекты
        protected object defaultParameterColor = new object();
        protected object defaultSeparatorColor = new object();
        protected object defaultMessageColor = new object();
        #endregion
        protected char sepSign = '-';
        protected Dictionary<acts, string> messages;
        protected paramCollection _parameters;
        protected Dictionary<object, HashSet<acts>> actPrint;
        protected colorOut colors;
        protected ConsoleColor _messageColor;
        protected ConsoleColor _parameterColor;
        protected ConsoleColor _separatorColor;
        #endregion
        #region Свойства

        public ConsoleColor colorMessage
        {
            get { return _messageColor; }
            set
            {
                if(value == _messageColor) return;
                _messageColor = value;
                colors.addObject(defaultMessageColor, _messageColor);
            }
        }
        public ConsoleColor colorParameter
        {
            get { return _parameterColor; }
            set
            {
                if (value == _parameterColor) return;
                _parameterColor = value;
                colors.addObject(defaultParameterColor, _parameterColor);
            }
        }
        public ConsoleColor colorSeparator
        {
            get { return _separatorColor; }
            set
            {
                if (value == _separatorColor) return;
                _separatorColor = value;
                colors.addObject(defaultSeparatorColor, _separatorColor);
            }
        }
        public paramCollection parameters
        {
            get { return _parameters; }
            set
            {
                if (value == null) return;
                _parameters = value;
            }
        }
        public char? separatorSign
        {
            get { return sepSign; }
            set
            {
                if (value == ' ' || value == null) sepSign = '-';
                else sepSign = (char)value;
            }
        }
        public string owner { get; protected set; }
        protected string separator => new string(sepSign, 60) + '\n';
        #endregion
        #region Конструктор
        public objectMessages(string ownerName)
        {
            if (string.IsNullOrEmpty(ownerName)) throw new NullReferenceException();
            _messageColor = ConsoleColor.Green;
            _separatorColor = ConsoleColor.White;
            _parameterColor = ConsoleColor.Gray;
            
            owner = ownerName;

            messages = new Dictionary<acts, string>();
            _parameters = new paramCollection();
            actPrint = new Dictionary<object, HashSet<acts>>();
            colors = new colorOut();
            colors.addObject(owner);
        }
        #endregion
        #region Методы
        #region Вывод
        public void print(acts type)
        {
            string result = string.Empty;

            result += separator;
            result += getMessage(type);
            result += getParameters(type);
            result += separator;
            Console.WriteLine(result);
        }
        #endregion
        #region Параметры
        public bool setParameter(object parameter, string parameterName)
        {
            if (parameter.isNull() || string.IsNullOrEmpty(parameterName)) return false;

            if (!actPrint.ContainsKey(parameter)) actPrint.Add(parameter, new HashSet<acts>());
            _parameters[parameter] = parameterName;

            return true;
        }
        public bool delParameter(object parameter)
        {
            if (!actPrint.ContainsKey(parameter)) return false;
            actPrint.Remove(parameter);
            return _parameters.remove(parameter);
        }
        public bool markParameter(object parameter, params acts[] printInAction)
        {
            if (!actPrint.ContainsKey(parameter) || printInAction.isNullOrEmpty()) return false;

            for (int i = 0; i < printInAction.Length; i++)
                actPrint[parameter].Add(printInAction[i]);

            return true;
        }
        public void unmarkParameter(object parameter, params acts[] printInAction)
        {
            if (parameter.isNull() || !actPrint.ContainsKey(parameter)) return;

            HashSet<acts> temp = actPrint[parameter];

            if (printInAction.isNullOrEmpty())
            {
                temp.Clear();
                actPrint[parameter] = temp;
            }
            else
            {
                acts[] tActs = actPrint[parameter].Where(v => !printInAction.Contains(v)).ToArray();
                if (tActs.isNullOrEmpty())
                {
                    temp.Clear();
                    actPrint[parameter] = temp;
                }
                else actPrint[parameter] = new HashSet<acts>(temp);
            }
        }
        protected string getParameters(acts type)
        {
            if (actPrint.Count == 0) return string.Empty;

            object[] prm = actPrint.Where(v => v.Value.Contains(type)).Select(v => v.Key).ToArray();

            if (prm.isNullOrEmpty()) return string.Empty;
            else
            {
                string result = string.Empty;

                for (int i = 0; i < prm.Length; i++)
                {
                    result += parameters.getString(prm[i]);
                }

                return result;
            }
        }
        #endregion
        #region Сообщения
        public void setMessage(acts type, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                if (messages.ContainsKey(type))
                {
                    messages.Remove(type);
                    return;
                }
                else return;
            }
            if (messages.ContainsKey(type)) messages[type] = message;
            else messages.Add(type, message);
        }
        protected string getMessage(acts type)
        {
            string msg = messages.ContainsKey(type) ? messages[type] : type.toTxt() + '\n';
            return string.Format("[{0}] {1}", owner, msg);
        }
        #endregion
        #region Объект
        public bool remove(acts type)
        {
            setMessage(type, string.Empty);

            HashSet<acts> temp;
            acts[] aact;
            for (int i = 0; i < actPrint.Count; i++)
            {
                temp = actPrint[actPrint.Keys.ElementAt(i)];
                aact = temp.Where(v => v != type).ToArray();
                actPrint[actPrint.Keys.ElementAt(i)] = new HashSet<acts>(aact);
            }

            return true;
        }
        public void clear()
        {
            messages.Clear();
            _parameters.clear();
            actPrint.Clear();
        }
        #endregion
        #endregion
        #region Служебные

        #endregion
    }

    public class paramCollection
    {
        public string separator = ": ";
        public string this[object Object]
        {
            get
            {
                if (!Params.ContainsKey(Object)) return string.Empty;
                else return Params[Object];
            }
            set
            {
                if (!Params.ContainsKey(Object)) Params.Add(Object, value);
                else Params[Object] = value;
            }
        }
        protected Dictionary<object, string> Params;

        public paramCollection()
        {
            Params = new Dictionary<object, string>();
        }

        public string getString(object Parameter)
        {
            if (!Params.ContainsKey(Parameter)) return string.Empty;
            return string.Format("{0}{1}{2}", Params[Parameter], separator, Parameter);
        }

        public bool remove(object Parameter)
        {
            return Params.Remove(Parameter);
        }

        public void clear()
        {
            separator = ": ";
            Params.Clear();
        }
    }


    public struct debugMessage
    {
        public string text;
        public string instanceName;
        
        public bool print(params object[] parameters)
        {
            if(string.IsNullOrEmpty(text) && string.IsNullOrEmpty(instanceName)) return false;
            string def = string.Format("{0} {1}", instanceName, text);

            if(parameters.isNullOrEmpty()) Console.WriteLine(def);
            else
            {
                try
                {
                    Console.WriteLine(string.Format(def, parameters));
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        public void clear()
        {
            text = instanceName = string.Empty;
        }
    }

    public class colorOut
    {
        #region Индексатор
        public colorPair this[object colorObject]
        {
            get
            {
                if(!isKey(colorObject)) throw new ArgumentException(nameof(colorObject));
                return colors[colorObject];
            }
            set
            {
                if (!isKey(colorObject)) throw new ArgumentException(nameof(colorObject));
                colors[colorObject] = value;
            }
        }
        #endregion
        #region Переменные и свойства
        protected Dictionary<object, colorPair> colors;
        protected KeyValuePair<ConsoleColor, ConsoleColor> defaultColor;
        protected object _current;

        public object current
        {
            get { return _current; }
            set
            {
                if (!isKey(value) || value == _current) return;

                resetCurrentColor();
                _current = value;
                setCurrentColor();
            }
        }
        public bool isSet { get; protected set; }
        public int count => colors.Count;
        #endregion
        #region Конструктор
        public colorOut()
        {
            colors = new Dictionary<object, colorPair>();
            defaultColor = new KeyValuePair<ConsoleColor, ConsoleColor>(Console.ForegroundColor, Console.BackgroundColor);
            _current = null;
            isSet = false;
        }

        ~colorOut()
        {
            colors = null;
            _current = null;
            isSet = false;
        }
        #endregion
        #region Текущий объект
        #region Установка и сброс цвета
        public bool setCurrentColor()
        {
            if (current.isNull()) return false;
            if (isSet) resetCurrentColor();

            colorPair cPair = colors[current];
            cPair.setColors();
            colors[current] = cPair;

            isSet = true;

            return true;
        }
        public bool resetCurrentColor()
        {
            if (!isSet || current.isNull()) return false;

            colorPair cPair = colors[current];
            cPair.resetColors();
            colors[current] = cPair;

            isSet = false;

            return true;
        }
        #endregion
        #region Изменение цвета
        public void backgroundCurrent(ConsoleColor background)
        {
            if (current.isNull()) return;

            bool tmpIsSet = isSet;
            if (isSet) resetCurrentColor();

            colorPair cPair = colors[current];
            cPair.backgroundColor = background;
            colors[current] = cPair;

            if (tmpIsSet) setCurrentColor();
        }
        public void foregroundCurrent(ConsoleColor foreground)
        {
            if (current.isNull()) return;

            bool tmpIsSet = isSet;
            if (isSet) resetCurrentColor();

            colorPair cPair = colors[current];
            cPair.foregroundColor = foreground;
            colors[current] = cPair;

            if (tmpIsSet) setCurrentColor();
        }
        #endregion
        #endregion
        #region Объект
        #region Удаление
        public void clear()
        {
            if(count == 0) return;
            if (isSet) resetCurrentColor();
            current = null;
            colors.Clear();
        }
        public bool remove(object colorObject)
        {
            if (!isKey(colorObject)) return false;

            if (current == colorObject)
            {
                if (isSet) resetCurrentColor();
                current = null;
            }

            return colors.Remove(colorObject);
        }
        #endregion
        #region Добавление
        public bool addObject(object colorObject, colorPair pair)
        {
            if (isKey(colorObject))
            {
                colors[colorObject] = pair;
                return false;
            }
            colors.Add(colorObject, pair);

            return true;
        }
        public bool addObject(object colorObject)
        {
            return addObject(colorObject, newPair());
        }
        public bool addObject(object colorObject, ConsoleColor foreground, ConsoleColor background)
        {
            return addObject(colorObject, new colorPair(foreground, background));
        }
        public bool addObject(object colorObject, ConsoleColor foreground)
        {
            return addObject(colorObject, new colorPair(foreground, defaultColor.Value));
        }
        #endregion
        #region Доступ
        
        #endregion
        #endregion
        #region Служебные
        protected bool isKey(object keyObject)
        {
            return colors.Keys.Contains(keyObject);
        }

        protected colorPair newPair()
        {
            return new colorPair(defaultColor.Key, defaultColor.Value);
        }
        #endregion
    }

    public struct colorPair : IEquatable<colorPair>
    {
        private bool isSet;
        private KeyValuePair<ConsoleColor, ConsoleColor> temp;
        public ConsoleColor foregroundColor;
        public ConsoleColor backgroundColor;

        #region Конструктор
        public colorPair(ConsoleColor foreground, ConsoleColor background)
        {
            isSet = false;
            temp = new KeyValuePair<ConsoleColor, ConsoleColor>();
            foregroundColor = foreground;
            backgroundColor = background;
        }
        public colorPair(ConsoleColor foreground)
            :this(foreground, Console.BackgroundColor)
        { }
        #endregion
        #region Установка значений
        public void setForegroundValue()
        {
            foregroundColor = Console.ForegroundColor;
        }
        public void setBackgroundValue()
        {
            backgroundColor = Console.BackgroundColor;
        }
        public void setColorsValue()
        {
            setBackgroundValue();
            setForegroundValue();
        }
        public void setColorsValue(ConsoleColor foreground, ConsoleColor background)
        {
            foregroundColor = foreground;
            backgroundColor = background;
        }
        public bool setColors()
        {
            if (isSet) return false;

            initTemp();
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;

            isSet = true;

            return true;    
        }
        #endregion
        #region Сброс значений
        public bool resetColors()
        {
            if (!isSet) return false;

            loadTemp();
            isSet = false;

            return true;
        }
        #endregion
        #region Служебные 
        private void initTemp()
        {
            temp = new KeyValuePair<ConsoleColor, ConsoleColor>(Console.ForegroundColor, Console.BackgroundColor);
        }

        private void loadTemp()
        {
            Console.ForegroundColor = temp.Key;
            Console.BackgroundColor = temp.Value;
        }
        #endregion
        #region Перегрузка операторов object, IEquatable<colorPair>
        public override int GetHashCode()
        {
            return (int)foregroundColor + ((int)backgroundColor + 60);
        }
        public override bool Equals(object obj)
        {
            if (!obj.isType(typeof(colorPair))) return false;
            return this.GetHashCode() == ((colorPair)obj).GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("Foreground: {0} Background: {1}", foregroundColor, backgroundColor);
        }
        public bool Equals(colorPair other)
        {
            return this.foregroundColor == other.foregroundColor &&
                   this.backgroundColor == other.backgroundColor;
        }
        #endregion
    }
}
