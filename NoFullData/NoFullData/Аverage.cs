using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NoFullData
{
    class АverageMethod
    {
        private Consider consider;
        private NeuroNetWorker NetWorker;
        private int statCount;
        private int statAll;

        public АverageMethod(XLSReader Reader)
        {
            Init(Reader);
            NetWorker = new NeuroNetWorker("AverageMethod" + Reader.FileName, consider.Column, edge: false);
            statCount = 0;
            statAll = 0;
        }

        private void Init(XLSReader Reader)
        {
            consider = new Consider();
            foreach (int i in Reader.ReadRow())
                foreach (var obj in Reader.ReadColumn(i, 4))
                    consider.Set(obj.Item1, obj.Item2);
        }

        public List<Viewer> Analyze(XLSReader Reader, List<int> sample)
        {
            statCount = 0;
            statAll = 0;
            List<Viewer> resList = new List<Viewer>();
            StringBuilder inputString = new StringBuilder();
            StringBuilder storesString = new StringBuilder();
            double[] input = new double[consider.Column];
            double[] result;
            int index;
            foreach (int i in Reader.ReadRow(sample: sample))
            {
                inputString.Clear();
                storesString.Clear();
                foreach (var obj in Reader.ReadColumn(i, 4))
                {
                    inputString.Append(obj.Item2 + ", ");
                    input[obj.Item1 - 4] = consider.Get(obj.Item1, obj.Item2);
                    storesString.Append(input[obj.Item1 - 4] + ", ");
                }
                result = NetWorker.Process(input);
                index = result[0] >= 0.5 ? 0 : 1;
                CalculateStat(Reader, i, index);
                inputString.Remove(inputString.Length - 2, 2);
                storesString.Remove(storesString.Length - 2, 2);

                resList.Add(new Viewer { Input = inputString.ToString(), Prep = storesString.ToString(), Res = index.ToString() });
            }
            return resList;
        }

        public void Train(XLSReader reader, List<int> sample)
        {
            double[,] xy = new double[sample.Count, consider.Column + 1];
            int i = 0, j = 0;
            foreach (int index in reader.ReadRow(true, sample))
            {
                foreach (var obj in reader.ReadColumn(index, 4))
                {
                    xy[i, j] = consider.Get(obj.Item1, obj.Item2);
                    j++;
                }
                foreach (var obj in reader.ReadColumn(index, columnStop: 1))
                    xy[i, j] = double.Parse(obj.Item2);
                i++;
                j = 0;
            }
            NetWorker.Training(xy, sample.Count);
        }

        public void Statistic()
        {
            double result = 0;
            if (statAll != 0)
                result = statCount / (double)statAll * 100;
            MessageBox.Show("Правильно проанализированных данных " + Math.Round(result, 2) + "%");
        }

        private void CalculateStat(XLSReader Reader, int row, int result)
        {
            statAll++;
            foreach (var obj in Reader.ReadColumn(row, columnStop: 1))
                if (result.ToString() == obj.Item2)
                    statCount++;
        }

        public void Clear()
        {
            consider.Clear();
        }
    }
}
