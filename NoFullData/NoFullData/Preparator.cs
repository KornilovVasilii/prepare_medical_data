using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoFullData
{
    class Preparator
    {
        /// <summary>
        /// Словарь, в котором ключ это индекс из исходной таблицы,
        /// а значение - название столбца шапки из исходной таблицы
        /// для этого индекса
        /// </summary>
        private Dictionary<int, string> Indexer;
        /// <summary>
        /// Словарь, в котором ключ это имя шапки в исходной таблице,
        /// а значение - объект Store, предобрабатывающий значения
        /// из столбца с указанным именем шапки таблцы
        /// </summary>
        private Dictionary<string, Store> Stores;

        public Preparator()
        {
            Indexer = new Dictionary<int, string>();
            Stores = new Dictionary<string, Store>();
        }

        /// <summary>
        /// Количество объектов Store
        /// </summary>
        public int Count
        {
            get
            {
                return Stores.Count;
            }
        }

        /// <summary>
        /// Инициализирует словари Indexer и Stores
        /// </summary>
        /// <param name="index">Индекс в исходной таблице</param>
        /// <param name="name">Имя шапки столбца с указанным индексом</param>
        public void SetHeader(int index, string name)
        {
            Indexer[index] = name;
            Stores[name] = new Store(name);
        }

        /// <summary>
        /// Устанавливает значение в объекте Store
        /// по индексу стобца исходной таблицы
        /// </summary>
        /// <param name="index">Индекс столбца</param>
        /// <param name="value">Значение ячейки исходной таблицы</param>
        public void SetValue(int index, string value)
        {
            Stores[Indexer[index]].SetValue(value);
        }

        /// <summary>
        /// Подготавливает объекты Store к
        /// предоставлению новых значений
        /// </summary>
        public void PrepareStores()
        {
            foreach (string key in Stores.Keys)
                Stores[key].PrepareValue();
        }

        /// <summary>
        /// Возвращает новое значение из объекта Store
        /// по индексу из исходной таблицы и
        /// старому значению ячейки
        /// </summary>
        /// <param name="index">Индекс стобца исходной таблицы</param>
        /// <param name="value">Старое значение ячейки</param>
        /// <returns></returns>
        public double GetValue(int index, string value)
        {
            return Stores[Indexer[index]].GetValue(value);
        }

        //TODO: обучение нейросетей

        /// <summary>
        /// Очищает словари объектов
        /// </summary>
        public void Clear()
        {
            Indexer.Clear();
            Stores.Clear();
        }
    }
}
