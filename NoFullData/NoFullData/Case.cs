using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NoFullData
{
    class Case
    {
        private Preparator objPreparator;
        private NeuroNetWorker NetWorker;
        private int P;
        private int N;
        private int TP;
        private int FP;
        private int TN;
        private int FN;

        public Case(XLSReader Reader)
        {
            objPreparator = new Preparator();
            Init(Reader);
            NetWorker = new NeuroNetWorker("CaseMethod" + Reader.FileName, objPreparator.Count, edge: false);
            P = 0;
            N = 0;
            TP = 0;
            FP = 0;
            TN = 0;
            FN = 0;
        }

        /// <summary>
        /// Метод подготовки преобразователя
        /// </summary>
        /// <param name="Reader"></param>
        private void Init(XLSReader Reader)
        {
            foreach (var obj in Reader.ReadHeaders(4))
                objPreparator.SetHeader(obj.Item1, obj.Item2);
            foreach (int i in Reader.ReadRow())
                foreach (var obj in Reader.ReadColumn(i, 4))
                    objPreparator.SetValue(obj.Item1, obj.Item2);
            objPreparator.PrepareStores();
        }

        public List<Viewer> Analyze(XLSReader Reader, List<int> sample)
        {
            P = 0;
            N = 0;
            TP = 0;
            FP = 0;
            TN = 0;
            FN = 0;
            List<Viewer> resList = new List<Viewer>();
            StringBuilder inputString = new StringBuilder();
            StringBuilder storesString = new StringBuilder();
            double[] input = new double[objPreparator.Count];
            double[] result;
            int index;
            foreach (int i in Reader.ReadRow(sample: sample))
            {
                inputString.Clear();
                storesString.Clear();
                foreach (var obj in Reader.ReadColumn(i, 4))
                {
                    inputString.Append(obj.Item2 + ", ");
                    input[obj.Item1 - 4] = objPreparator.GetValue(obj.Item1, obj.Item2);
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

        public void Train(XLSReader Reader, List<int> sample)
        {
            double[,] xy = new double[sample.Count, objPreparator.Count + 1];
            int i = 0, j = 0;
            foreach (int index in Reader.ReadRow(true, sample))
            {
                foreach (var obj in Reader.ReadColumn(index, 4))
                {
                    xy[i, j] = objPreparator.GetValue(obj.Item1, obj.Item2);
                    j++;
                }
                foreach (var obj in Reader.ReadColumn(index, columnStop: 1))
                    xy[i, j] = double.Parse(obj.Item2);
                i++;
                j = 0;
            }
            NetWorker.Training(xy, sample.Count);
        }

        public void Statistic()
        {
            double accuracy = 0;
            double precision = 0;
            double recall = 0;
            double fmeasure = 0;
            if (N != 0)
                accuracy = Math.Round(P / (double)N, 2);
            if (TP + FP != 0)
                precision = Math.Round(TP / (double)(TP + FP), 2);
            if (TP + FN != 0)
                recall = Math.Round(TP / (double)(TP + FN), 2);
            if (precision + recall != 0)
                fmeasure = Math.Round(2 * precision * recall / (precision + recall), 2);
            MessageBox.Show("Accuracy " + accuracy + "; Precision " + precision + "; Recall " + recall + "; F-Measure " + fmeasure);
        }

        private void CalculateStat(XLSReader Reader, int row, int result)
        {
            N++;
            foreach (var obj in Reader.ReadColumn(row, columnStop: 1))
            {
                if (result.ToString() == obj.Item2)
                    P++;
                if (result == 1 && obj.Item2 == "1")
                    TP++;
                if (result == 0 && obj.Item2 == "0")
                    TN++;
                if (result == 1 && obj.Item2 == "0")
                    FP++;
                if (result == 0 && obj.Item2 == "1")
                    FN++;
            }
        }

        public void Clear()
        {
            objPreparator.Clear();
        }
    }
}
