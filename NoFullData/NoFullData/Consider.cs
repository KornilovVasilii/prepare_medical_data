using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoFullData
{
    class Consider
    {
        /// <summary>
        /// Словарь объектов, распределенных по индексам
        /// </summary>
        private Dictionary<int, Capacitor> capasitor;
        /// <summary>
        /// Типы данных
        /// </summary>
        private enum TypeBLO {None, Number, String, Boolean };

        public Consider()
        {
            capasitor = new Dictionary<int, Capacitor>();
        }

        /// <summary>
        /// Возвращает количество обработанных объектов
        /// </summary>
        public int Column
        {
            get
            {
                return capasitor.Count;
            }
        }

        /// <summary>
        /// Определяет тип входного объекта,
        /// если таковой еще не определен
        /// </summary>
        /// <param name="value">Объект</param>
        /// <returns>Возвращает False, если числовой тип
        /// True, если строковый</returns>
        private TypeBLO DeclareTypeBLO(string value)
        {
            if (String.IsNullOrEmpty(value))
                return TypeBLO.None;
            double res;
            if (double.TryParse(value, out res))
            {
                if (res != 1 && res != 0)
                    return TypeBLO.Number;
                return TypeBLO.Boolean;
            }
            else
            {
                return TypeBLO.String;
            }
        }

        /// <summary>
        /// Просчитывает новое значение для текущего столбца
        /// </summary>
        /// <param name="index">Индекс текущего столбца</param>
        /// <param name="value">Значение текущего столбца</param>
        public void Set(int index, string value)
        {
            TypeBLO type = DeclareTypeBLO(value);
            if (!capasitor.Keys.Contains(index))
                capasitor.Add(index, new Capacitor(type));
            if ((capasitor[index].type == TypeBLO.Boolean || capasitor[index].type == TypeBLO.None) && type == TypeBLO.Number)
                capasitor[index].type = type;
            switch (type)
            {
                case TypeBLO.Boolean:
                    if (!String.IsNullOrEmpty(value))
                    {
                        capasitor[index].notNullRows++;
                        capasitor[index].sum += double.Parse(value);
                    }
                    break;
                case TypeBLO.Number:
                    if (!String.IsNullOrEmpty(value))
                    {
                        capasitor[index].notNullRows++;
                        capasitor[index].sum += double.Parse(value);
                    }
                    break;
                case TypeBLO.String:
                    if (!String.IsNullOrEmpty(value))
                    {
                        capasitor[index].notNullRows++;
                        if (!capasitor[index].saturation.Keys.Contains(value))
                            capasitor[index].saturation.Add(value, 0);
                        capasitor[index].saturation[value]++;
                    }
                    break;
            }
            capasitor[index].rows++;
        }

        /// <summary>
        /// Возвращает новое значение столбца опираясь на старое
        /// </summary>
        /// <param name="index">Индекс стобца</param>
        /// <param name="value">Значение ячейки</param>
        /// <returns></returns>
        public double Get(int index, string value)
        {
            switch (capasitor[index].type)
            {
                case TypeBLO.Boolean:
                    if (String.IsNullOrEmpty(value))
                        return 0;
                    else
                    {
                        if (value == "1")
                            return 1;
                        return -1;
                    }
                case TypeBLO.Number:
                    if (String.IsNullOrEmpty(value))
                        return capasitor[index].sum / (double)capasitor[index].notNullRows;
                    return double.Parse(value);
                case TypeBLO.String:
                    if (String.IsNullOrEmpty(value))
                    {
                        return capasitor[index].saturation.Sum(a => a.Value) / (double)capasitor[index].saturation.Count;
                    }
                    return capasitor[index].saturation[value] / (double)capasitor[index].notNullRows;
                case TypeBLO.None:
                    return 0;
            }
            return 0;
        }

        /// <summary>
        /// Очищает данные
        /// </summary>
        public void Clear()
        {
            capasitor.Clear();
        }

        /// <summary>
        /// Объект хранения всех возможных вариантов данных
        /// </summary>
        private class Capacitor
        {
            public TypeBLO type;
            public double sum;
            public Dictionary<string, int> saturation;
            public int notNullRows;
            public int rows;

            public Capacitor(TypeBLO type)
            {
                this.type = type;
                sum = 0;
                saturation = new Dictionary<string, int>();
                notNullRows = 0;
                rows = 0;
            }
        }
    }
}
