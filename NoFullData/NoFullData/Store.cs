using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoFullData
{
    class Store
    {
        /// <summary>
        /// Имя объекта, ключ
        /// </summary>
        private string BLObj;
        /// <summary>
        /// Тип объекта
        /// </summary>
        private TypeBLO type;
        /// <summary>
        /// Минимальное значение
        /// Только для числового типа
        /// </summary>
        private double minValue;
        /// <summary>
        /// Максимальное значение
        /// Только для числового типа
        /// </summary>
        private double maxValue;
        /// <summary>
        /// Славарь, содержащий в качестве ключа значение ячейки,
        /// а в качестве значения, вектор целых чисел,
        /// для подачи в нейронную сеть
        /// </summary>
        private Dictionary<string, double[]> map;
        /// <summary>
        /// Нейронная сеть для обработки значений
        /// Для строкового типа на входе вектор, на выходе значение
        /// Для числового типа на выходе значение из интервала
        /// </summary>
        private NeuroNetWorker netWorker;
        /// <summary>
        /// Перечисление типов объекта
        /// </summary>
        private enum TypeBLO { None, Number, String, Boolean, Percent };
        
        public Store(string name)
        {
            if(String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is not empty");
            BLObj = name;
            type = TypeBLO.None;
        }

        public string Name
        {
            get
            {
                return this.BLObj;
            }
        }

        public int Type
        {
            get
            {
                return (int)this.type;
            }
        }

        /// <summary>
        /// Определяет тип входного объекта,
        /// если таковой еще не определен
        /// </summary>
        /// <param name="value">Объект</param>
        /// <returns>Возвращает False, если числовой тип
        /// True, если строковый</returns>
        private void DeclareTypeBLO(string value)
        {
            if (type != TypeBLO.None && type != TypeBLO.Boolean)
                return;
            if (String.IsNullOrEmpty(value))
                return;
            double res;
            if (double.TryParse(value, out res))
            {
                if (type == TypeBLO.None)
                {
                    if (res != 1 && res != 0)
                        type = TypeBLO.Number;
                    else
                        type = TypeBLO.Boolean;
                }
                else
                {
                    if (res != 1 && res != 0)
                        type = TypeBLO.Number;
                }
            }
            else
            {
                if (double.TryParse(value.Remove(value.Length - 1), out res))
                    type = TypeBLO.Percent;
                else
                {
                    type = TypeBLO.String;
                    map = new Dictionary<string, double[]>();
                    map["None"] = null;
                }
            }
        }

        /// <summary>
        /// Устанавливает значение объекта в структуре
        /// в зависимости от его типа
        /// </summary>
        /// <param name="value">Объект</param>
        public void SetValue(string value)
        {
            DeclareTypeBLO(value);
            if (type == TypeBLO.String)
                SetString(value.ToString());
            if (type == TypeBLO.Number && !String.IsNullOrEmpty(value))
                SetNumeric(double.Parse(value.ToString()));
            if (type == TypeBLO.Boolean && !String.IsNullOrEmpty(value))
                SetNumeric(double.Parse(value.ToString()));
        }

        /// <summary>
        /// Устанавливает значение для объекта строкового типа
        /// </summary>
        /// <param name="value">Объект</param>
        private void SetString(string value)
        {
            if (!map.ContainsKey(value))
                map[value] = null;
        }

        /// <summary>
        /// Устанавливает значение для объекта числового типа
        /// </summary>
        /// <param name="value">Объект</param>
        private void SetNumeric(double value)
        {
            if (value < minValue)
                minValue = value;
            if (value > maxValue)
                maxValue = value;
        }

        /// <summary>
        /// Подготавливает входные данные для нейронных
        /// сетей и сами нейронные сети
        /// </summary>
        public void PrepareValue()
        {
            if (type == TypeBLO.String)
            {
                PrepareString();
                netWorker = new NeuroNetWorker(BLObj, map.Count);
            }
            if(type == TypeBLO.Number)
                netWorker = new NeuroNetWorker(BLObj, 5, lowerEdge: minValue, upperEdge: maxValue);
            if (type == TypeBLO.Boolean)
            {
                PrepareBoolean();
                netWorker = new NeuroNetWorker(BLObj, 3);
            }
            if (type == TypeBLO.Percent)
                netWorker = new NeuroNetWorker(BLObj, 5, lowerEdge: 0, upperEdge: 100);
        }

        /// <summary>
        /// Продготавливает вертора для каждого ключа в map для строкового типа
        /// </summary>
        private void PrepareString()
        {
            int count = map.Count;
            int index = 0;
            foreach(var val in map.ToList())
            {
                map[val.Key] = new double[count];
                map[val.Key][index] = 1;
                index++;
            }
        }

        /// <summary>
        /// Подготавливает вектора для логического типа
        /// </summary>
        private void PrepareBoolean()
        {
            map = new Dictionary<string, double[]>();
            map["-1"] = new double[] { 1, 0, 0 };
            map["0"] = new double[] { 0, 1, 0 };
            map["1"] = new double[] { 0, 0, 1 };
        }

        /// <summary>
        /// Возвращает новое значение для объекта,
        /// опираясь на старое
        /// </summary>
        /// <param name="value">Старое значение объекта</param>
        /// <returns></returns>
        public double GetValue(string value)
        {
            string trimValue = value.ToString();
            if (type == TypeBLO.String)
                return GetString(trimValue);
            if (type == TypeBLO.Number)
                return GetNumber(trimValue);
            if (type == TypeBLO.Boolean)
                return GetBoolean(trimValue);
            if (type == TypeBLO.Percent)
                return GetPercent(trimValue);
            return 0;
        }

        /// <summary>
        /// Возвращает новое значение для объекта строкового типа
        /// </summary>
        /// <param name="value">Старое значение</param>
        /// <returns></returns>
        private double GetString(string value)
        {
            string key = String.IsNullOrEmpty(value) ? "None" : value;
            double[] result = netWorker.Process(map[key]);
            return result[0];
        }

        /// <summary>
        /// Возвращает новое значение для объекта числового типа
        /// </summary>
        /// <param name="value">Старое значение</param>
        /// <returns></returns>
        private double GetNumber(string value)
        {
            if (!String.IsNullOrWhiteSpace(value))
                return double.Parse(value);
            double[] input = { 1, 0, 1, 0, 1 };
            double[] result = netWorker.Process(input);
            return result[0];
        }

        /// <summary>
        /// Возвращает новое значение для обекта логического типа
        /// </summary>
        /// <param name="value">Старое значение</param>
        /// <returns></returns>
        private double GetBoolean(string value)
        {
            string key = String.IsNullOrEmpty(value) ? "-1" : value;
            double[] result = netWorker.Process(map[key]);
            return result[0];
        }

        /// <summary>
        /// Возвращает новое значение для объекта типа процент
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double GetPercent(string value)
        {
            if (!String.IsNullOrWhiteSpace(value))
                return double.Parse(value.Remove(value.Length - 1));
            double[] input = { 1, 0, 1, 0, 1 };
            double[] result = netWorker.Process(input);
            return result[0];
        }

        //TODO: обучение netWorker
    }
}
