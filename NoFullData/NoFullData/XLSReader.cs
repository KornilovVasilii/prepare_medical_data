using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace NoFullData
{
    class XLSReader
    {
        /// <summary>
        /// Объект приложения Excel
        /// </summary>
        private Excel.Application ExcelApp;
        /// <summary>
        /// Excel книга из открытого xls-файла
        /// </summary>
        private Excel.Workbook WorkBook;
        /// <summary>
        /// Страница из книги, таблица
        /// </summary>
        private Excel.Worksheet WorkSheet;
        /// <summary>
        /// Ячейка таблицы
        /// </summary>
        private Excel.Range WorkCells;
        /// <summary>
        /// Количество столбцов в таблице
        /// </summary>
        private int CountColumns;
        /// <summary>
        /// Имя открытого файла
        /// </summary>
        private string fileName;
        /// <summary>
        /// Флаг открытия файла
        /// </summary>
        private bool isOpen;

        public XLSReader(string Name)
        {
            fileName = Name;
            ExcelApp = new Excel.Application();
            WorkBook = ExcelApp.Workbooks.Open(fileName, 0, true, 5, "", "", Excel.XlPlatform.xlWindows, true, false, 0, true, false, false);
            WorkSheet = (Excel.Worksheet)WorkBook.Sheets[1];
            WorkCells = WorkSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell);
            CountColumns = 0;
            isOpen = true;
        }

        /// <summary>
        /// Имя открытого файла
        /// </summary>
        public string FileName
        {
            get
            {
                return fileName;
            }
        }

        /// <summary>
        /// Флаг открытия файла
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return isOpen;
            }
        }

        /// <summary>
        /// Итератор для считывания шапки таблицы
        /// Одновременно запоминает количество столбцов
        /// </summary>
        /// <returns>Индекс столбца и значение ячейки</returns>
        public IEnumerable<Tuple<int, string>> ReadHeaders(int columnStart = 1)
        {
            CountColumns = columnStart - 1;
            for(int j = columnStart; ; j++)
            {
                string obj = WorkSheet.Cells[1, j].Text.ToString().Trim();
                if (String.IsNullOrEmpty(obj))
                    break;
                CountColumns++;
                yield return new Tuple<int, string>(j, obj);
            }
        }

        /// <summary>
        /// Итератор для обхода строк таблицы
        /// </summary>
        /// <returns>Индекс текущей строки</returns>
        public IEnumerable<int> ReadRow(bool isSample = false, List<int> sample = null)
        {
            if (sample == null)
                sample = new List<int>();
            if (!isSample)
                for(int i = 2; i <= WorkCells.Row; i++)
                {
                    if(sample.IndexOf(i) == -1)
                        yield return i;
                }
            else
                foreach (int i in sample)
                {
                        yield return i;
                }
        }

        /// <summary>
        /// Итератор для обхода стоблцов текущей строки
        /// </summary>
        /// <param name="i">Индекс текущей строки</param>
        /// <returns>Индекс столбца и значение ячейки</returns>
        public IEnumerable<Tuple<int, string>> ReadColumn(int i, int columnStart = 1, int columnStop = 0)
        {
            if (columnStop == 0 || columnStop > CountColumns)
                columnStop = CountColumns;
            for (int j = columnStart; j <= columnStop; j++)
                yield return new Tuple<int, string>(j, WorkSheet.Cells[i, j].Text.ToString().Trim().ToLower());
        }

        /// <summary>
        /// Проверяет строку на целостность
        /// </summary>
        /// <param name="index">Индекс строки</param>
        /// <param name="step">Первый столбец</param>
        /// <returns></returns>
        public bool IsValidIndex(int index, int step)
        {
            foreach (var obj in ReadColumn(index, step))
                if (String.IsNullOrEmpty(obj.Item2))
                    return false;
            return true;
        }

        /// <summary>
        /// Закрывает файл
        /// </summary>
        public void Close()
        {
            WorkBook.Close(false, fileName);
            ExcelApp.Quit();
            isOpen = false;
        }
    }
}
