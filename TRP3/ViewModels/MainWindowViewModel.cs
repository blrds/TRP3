using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TRP3.Infrastructure.Commands;
using TRP3.Models;
using TRP3.ViewModels.Base;

namespace TRP3.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        #region StartCommand

        public ICommand StartCommand { get; }

        /// <summary>
        /// Проверка, можно ли выполнить вычисление(сумма вероятностей переходов из состояния=1, сумма вероятностей вхождений =1 и шагов >1
        /// </summary>
        /// <returns></returns>
        private bool CanStartCommnadExecute(object p)
        {

            double check = 0;
            foreach (var a in Matrix)
            {
                check = 0;
                foreach (var b in a)
                {
                    if (b < 0) return false;
                    check += b;
                }
                if (check < 0.998 || check > 1.001) return false;
            }
            check = 0;
            foreach (var a in T0)
            {
                if (a < 0) return false;
                check += a;
            }
            if (check < 0.998 || check > 1.001) return false;
            return N > 1;
        }


        /// <summary>
        /// Кнопка пуск
        /// </summary>
        private void OnStartCommandExecuted(object p)
        {
            Graph g = new Graph(Matrix, T0, N);
            if (Model)
            {//моделируем, пока точность не равна 0.01
                int i = 3;
                PlotClearance();
                List<double> Ta = g.Start(1), Tb = g.Start(2);
                AddPoints(Ta, 1);
                AddPoints(Tb, 2);
                while (true)
                {
                    if (g.Compare(Ta, Tb, E)) break;//проверка на точность
                    Ta = Tb;
                    Tb = g.Start(i);
                    AddPoints(Tb, i);
                    i++;
                }
                TN = Tb;
            }
            else
            {
                List<double> a = null; ;
                for (int i = 0; i < N; i++)
                {
                    a = g.Start();
                    AddPoints(a, i+1);
                }
                TN = a;
            }
            OnPropertyChanged("TN");
            
        }

        #endregion


        /// <summary>
        /// Матрица вероятностей
        /// </summary>
        public List<List<double>> Matrix { get; set; } = new List<List<double>>(4);

        /// <summary>
        /// P(t0)
        /// </summary>
        public List<double> T0 { get; set; } = new List<double>(4);

        /// <summary>
        /// Кол-во шагов
        /// </summary>
        public int N { get; set; } = 0;

        /// <summary>
        /// P(n)
        /// </summary>
        public List<double> TN { get; set; } = new List<double>(4);

        /// <summary>
        /// Флаг моделирования
        /// </summary>
        public bool Model { get; set; }
        /// <summary>
        /// График
        /// </summary>
        public PlotModel PlotModel { get; private set; }

        /// <summary>
        /// Точность
        /// </summary>
        public double E { get; set; } = 0.01;
        public MainWindowViewModel()
        {
            #region Генерирование начальных значений в1
            for (int i = 0; i < 4; i++)
            {
                Matrix.Add(new List<double>(4));
                for (int j = 0; j < 4; j++)
                    Matrix[i].Add(0);
                T0.Add(0);
            }
            N = 2;
            Matrix[0][0] = 0.1;
            Matrix[0][1] = 0.8;
            Matrix[0][2] = 0.1;
            Matrix[0][3] = 0.0;
            Matrix[1][0] = 0.2;
            Matrix[1][1] = 0.6;
            Matrix[1][2] = 0.1;
            Matrix[1][3] = 0.1;
            Matrix[2][0] = 0.1;
            Matrix[2][1] = 0.6;
            Matrix[2][2] = 0;
            Matrix[2][3] = 0.3;
            Matrix[3][0] = 0;
            Matrix[3][1] = 0.7;
            Matrix[3][2] = 0.1;
            Matrix[3][3] = 0.2;
            T0[0] = 0.2;
            T0[1] = 0.6;
            T0[2] = 0.1;
            T0[3] = 0.1;
            #endregion

            StartCommand = new LambdaCommand(OnStartCommandExecuted, CanStartCommnadExecute);//инициализация команды 

            PlotModel = new PlotModel();//график
            #region Номер расчета
            PlotModel.Axes.Add(new LinearAxis());
            PlotModel.Axes.Last().Position = AxisPosition.Bottom;
            PlotModel.Axes.Last().Minimum = 1;
            #endregion
            #region Вероятность
            PlotModel.Axes.Add(new LinearAxis());
            PlotModel.Axes.Last().Position = AxisPosition.Left;
            PlotModel.Axes.Last().Minimum = 0;
            #endregion

            PlotModel.Series.Add(new LineSeries());
            PlotModel.Series.Add(new LineSeries());
            PlotModel.Series.Add(new LineSeries());
            PlotModel.Series.Add(new LineSeries());
        }
        /// <summary>
        /// чистка графика
        /// </summary>
        private void PlotClearance() {
            for (int i = 0; i < PlotModel.Series.Count; i++)
                (PlotModel.Series[i] as LineSeries).Points.Clear();
        }
        /// <summary>
        /// добавление точек
        /// </summary>
        /// <param name="a">набор точек</param>
        /// <param name="n">значение по x</param>
        private void AddPoints(List<double> a, int n) {
            for (int i = 0; i < a.Count; i++) {
                (PlotModel.Series[i] as LineSeries).Points.Add(new DataPoint(n, a[i]));
                
            }
            OnPropertyChanged("PlotModel");
            PlotModel.InvalidatePlot(true);
        }
    }
}
