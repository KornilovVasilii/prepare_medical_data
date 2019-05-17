using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoFullData
{
    class LogsIO
    {
        /// <summary>
        /// Объект соединения с базой данных
        /// </summary>
        //private static LoggingDataContext loggingDC = new LoggingDataContext(Properties.Settings.Default.LogsConnectionString);
        private static LoggingDataContext loggingDC = new LoggingDataContext("Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename=\"C:\\Users\\Simon Riley\\Documents\\Study\\Нейросети\\Дипломная\\NoFullData\\NoFullData\\Logs.mdf\";Integrated Security = True");

        /// <summary>
        /// Предоставляет записи из БД по полю имя,
        /// если записи с таким значением поля существуют
        /// </summary>
        /// <param name="name">Имя</param>
        /// <returns></returns>
        private static IQueryable<NeuroLog> GetQueryable(string name)
        {
            return (from c in loggingDC.NeuroLogs
                    where c.Name == name
                    select c);
        }

        /// <summary>
        /// Возвращает строку логов
        /// для нейронной сети с именем Name
        /// </summary>
        /// <param name="name">Имя</param>
        /// <returns></returns>
        public static string GetLog(string name)
        {
            string result = "";
            foreach (NeuroLog str in  GetQueryable(name))
            {
                result = str.Log;
                break;
            }
            return result.Trim();
        }

        /// <summary>
        /// Добавляет или перезаписывает запись в БД
        /// с именем Name и строкой логов Log
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="log">Строка логов</param>
        public static void SetLog(string name, string log)
        {
            var query = GetQueryable(name);
            if (query.Count() == 0)
                loggingDC.NeuroLogs.InsertOnSubmit(new NeuroLog { Name = name, Log = log });
            else
            {
                foreach(NeuroLog logs in query)
                {
                    if (logs.Name == name)
                        logs.Log = log;
                }
                loggingDC.SubmitChanges();
            }
        }
    }
}
