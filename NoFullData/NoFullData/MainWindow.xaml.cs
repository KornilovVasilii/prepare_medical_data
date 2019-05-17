using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace NoFullData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private XLSReader Reader;
        private List<int> Sample;

        private Simple simple;
        private Case caseMethod;
        private АverageMethod average;

        public MainWindow()
        {
            InitializeComponent();
            FileClose.IsEnabled = false;
            SimpleMethod.IsEnabled = false;
            MiddleMethod.IsEnabled = false;
            CaseMethod.IsEnabled = false;
            CreateSample.IsEnabled = false;
            this.Closing += MainWindow_Closing;
            Sample = new List<int>();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Reader != null && Reader.IsOpen)
                Reader.Close();
            if (caseMethod != null)
                caseMethod.Clear();
        }

        private void FileLoad_Click(object sender, RoutedEventArgs e)
        {
            string fileName = OpenFile();
            if (String.IsNullOrWhiteSpace(fileName))
            {
                MessageBox.Show("Ошибка выбора файла");
                return;
            }
            FileLoad.IsEnabled = false;
            FileClose.IsEnabled = true;
            SimpleMethod.IsEnabled = true;
            MiddleMethod.IsEnabled = true;
            CaseMethod.IsEnabled = true;
            Reader = new XLSReader(fileName);
            CreateSampleList(400);

            simple = new Simple(Reader);
            //average = new АverageMethod(Reader);
            //caseMethod = new Case(Reader);
            CreateSampleList(400);
            CreateSample.IsEnabled = true;
        }

        private string OpenFile()
        {
            OpenFileDialog choiceFile = new OpenFileDialog();
            choiceFile.DefaultExt = ".xls";
            choiceFile.Filter = "Excel files (*.xls;*.xlsx)|*.xls;*.xlsx";
            choiceFile.ShowDialog();
            return choiceFile.FileName;
        }

        /// <summary>
        /// Выборка индексов для обучения
        /// </summary>
        /// <param name="count">Необходимое количество</param>
        private void CreateSampleList(int count)
        {
            Random rand = new Random();
            int index;
            while(Sample.Count != count)
            {
                index = rand.Next(2, 1000);
                if (Sample.IndexOf(index) == -1 && Reader.IsValidIndex(index, 4))
                    Sample.Add(index);
            }
            Sample.Sort();
        }

        private void CreateSample_Click(object sender, RoutedEventArgs e)
        {
            CreateSampleList(400);
        }

        private void FileClose_Click(object sender, RoutedEventArgs e)
        {
            FileLoad.IsEnabled = true;
            FileClose.IsEnabled = false;
            SimpleMethod.IsEnabled = false;
            MiddleMethod.IsEnabled = false;
            CaseMethod.IsEnabled = false;
            CreateSample.IsEnabled = false;
            Reader.Close();
            Sample.Clear();
            //average.Clear();
            //caseMethod.Clear();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = null;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (Reader != null && Reader.IsOpen)
                Reader.Close();
            if(caseMethod != null)
                caseMethod.Clear();
            this.Close();
        }

        private void AnalyzeSM_Click(object sender, RoutedEventArgs e)
        {
            MiddleMethod.IsEnabled = false;
            CaseMethod.IsEnabled = false;
            AnalyzeSM.IsEnabled = false;
            TrainSM.IsEnabled = false;

            dataGrid.ItemsSource = simple.Analyze(Reader, Sample);

            MiddleMethod.IsEnabled = true;
            CaseMethod.IsEnabled = true;
            AnalyzeSM.IsEnabled = true;
            TrainSM.IsEnabled = true;
        }

        private void TrainSM_Click(object sender, RoutedEventArgs e)
        {
            MiddleMethod.IsEnabled = false;
            CaseMethod.IsEnabled = false;
            AnalyzeSM.IsEnabled = false;
            TrainSM.IsEnabled = false;

            simple.Train(Reader, Sample);

            MiddleMethod.IsEnabled = true;
            CaseMethod.IsEnabled = true;
            AnalyzeSM.IsEnabled = true;
            TrainSM.IsEnabled = true;
        }

        private void StatisticSM_Click(object sender, RoutedEventArgs e)
        {
            simple.Statistic();
        }

        private void AnalyzeMM_Click(object sender, RoutedEventArgs e)
        {
            SimpleMethod.IsEnabled = false;
            CaseMethod.IsEnabled = false;
            AnalyzeMM.IsEnabled = false;
            TrainMM.IsEnabled = false;

            dataGrid.ItemsSource = average.Analyze(Reader, Sample);

            SimpleMethod.IsEnabled = true;
            CaseMethod.IsEnabled = true;
            AnalyzeMM.IsEnabled = true;
            TrainMM.IsEnabled = true;
        }

        private void TrainMM_Click(object sender, RoutedEventArgs e)
        {
            SimpleMethod.IsEnabled = false;
            CaseMethod.IsEnabled = false;
            AnalyzeMM.IsEnabled = false;
            TrainMM.IsEnabled = false;

            average.Train(Reader, Sample);

            SimpleMethod.IsEnabled = true;
            CaseMethod.IsEnabled = true;
            AnalyzeMM.IsEnabled = true;
            TrainMM.IsEnabled = true;
        }

        private void StatisticMM_Click(object sender, RoutedEventArgs e)
        {
            average.Statistic();
        }

        private void AnalyzeCM_Click(object sender, RoutedEventArgs e)
        {
            SimpleMethod.IsEnabled = false;
            MiddleMethod.IsEnabled = false;
            AnalyzeCM.IsEnabled = false;
            TrainCM.IsEnabled = false;

            dataGrid.ItemsSource = caseMethod.Analyze(Reader, Sample);

            SimpleMethod.IsEnabled = true;
            MiddleMethod.IsEnabled = true;
            AnalyzeCM.IsEnabled = true;
            TrainCM.IsEnabled = true;
        }

        private void TrainCM_Click(object sender, RoutedEventArgs e)
        {
            SimpleMethod.IsEnabled = false;
            MiddleMethod.IsEnabled = false;
            AnalyzeCM.IsEnabled = false;
            TrainCM.IsEnabled = false;

            caseMethod.Train(Reader, Sample);

            SimpleMethod.IsEnabled = true;
            MiddleMethod.IsEnabled = true;
            AnalyzeCM.IsEnabled = true;
            TrainCM.IsEnabled = true;
        }

        private void StatisticCM_Click(object sender, RoutedEventArgs e)
        {
            caseMethod.Statistic();
        }

    }

    /// <summary>
    /// Класс отображения данных
    /// </summary>
    public class Viewer
    {
        public string Input { get; set; }
        public string Prep { get; set; }
        public string Res { get; set; }
    }
}
