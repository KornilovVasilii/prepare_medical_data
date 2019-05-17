using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoFullData
{
    class NeuroNetWorker
    {
        /// <summary>
        /// Имя объекта нейронной сети
        /// </summary>
        private string name;
        /// <summary>
        /// Нейронная сеть
        /// </summary>
        private alglib.multilayerperceptron net;

        public NeuroNetWorker(string bisLogObj, int input, bool edge = true, double lowerEdge = -10, double upperEdge = 10)
        {
            if (String.IsNullOrWhiteSpace(bisLogObj))
                throw new ArgumentException("Name is not empty");
            if (input <= 0)
                throw new ArgumentException("Input layer is less zero");
            Create(bisLogObj, input, edge, lowerEdge, upperEdge);
            Init();
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Создает нейронную сеть с одним внутренним слоем
        /// с границами и без взависимомти от флага границ
        /// </summary>
        /// <param name="bisLogObj">Имя объекта</param>
        /// <param name="input">Количество входных нейронов</param>
        /// <param name="edge">Флаг границ</param>
        /// <param name="lowerEdge">Нижняя граница</param>
        /// <param name="upperEdge">Верхняя граница</param>
        private void Create(string bisLogObj, int input, bool edge, double lowerEdge, double upperEdge)
        {
            int layer = (int)(input * 1.2);
            name = String.Format("{0}.{1}.{2}.{3}.{4}", bisLogObj, input, layer, lowerEdge, upperEdge);
            net = new alglib.multilayerperceptron();
            if (edge)
                alglib.mlpcreater1(input, layer, 1, lowerEdge, upperEdge, out net);
            else
                alglib.mlpcreatec1(input, layer, 2, out net);
        }

        /// <summary>
        /// Десериализует нейронную сеть из базы данных
        /// Если в БД ее нет, то сериализует созданную
        /// нейронную сеть с рандомными весами в БД
        /// </summary>
        private void Init()
        {
            string log = LogsIO.GetLog(name) + " ";
            if (String.IsNullOrWhiteSpace(log))
            {
                alglib.mlpserialize(net, out log);
                LogsIO.SetLog(name, log);
                return;
            }
            alglib.mlpunserialize(log, out net);
        }

        /// <summary>
        /// Обработка входных данных нейронной сетью
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public double[] Process(double[] input)
        {
            double[] result;
            alglib.mlpprocessi(net, input, out result);
            return result;
        }

        /// <summary>
        /// Обучение нейронной сети
        /// </summary>
        /// <param name="xy">Обучающее множество</param>
        /// <param name="sizeSet">Размер обучающего множества</param>
        public void Training(double [,] xy, int sizeSet)
        {
            int info;
            alglib.mlpreport rep;
            alglib.mlptrainlbfgs(net, xy, sizeSet, 0.001, 21, 0, 1, out info, out rep);//(20, 21), 0.05, (1, 19)
            string serial = null;
            alglib.mlpserialize(net, out serial);
            LogsIO.SetLog(name, serial);
        }
    }
}
