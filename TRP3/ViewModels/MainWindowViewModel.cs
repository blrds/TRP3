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
    class MainWindowViewModel:ViewModel
    {
        #region StartCommand

        public ICommand StartCommand { get; }

        /// <summary>
        /// Проверка, можно ли выполнить вычисление(сумма вероятностей переходов из состояния=1, сумма вероятностей вхождений =1 и шагов >1
        /// </summary>
        /// <returns></returns>
        private bool CanStartCommnadExecute(object p) {

            double check = 0;
            foreach (var a in Matrix) {
                check = 0;
                foreach (var b in a)
                {
                    if (b < 0) return false;
                    check += b;
                }
                if (check != 1) return false;
            }
            check = 0;
            foreach (var a in T0) {
                if (a < 0) return false;
                check += a;
            }
            if (check != 1) return false;
            return N > 1;
        }


        /// <summary>
        /// Кнопка пуск
        /// </summary>
        private void OnStartCommandExecuted(object p)
        {
            Graph g = new Graph(Matrix, T0, N);
            TN=g.Start();
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
        public MainWindowViewModel() {
            #region Генерирование начальных значений в1
            for (int i = 0; i < 4; i++)
            {
                Matrix.Add(new List<double>(4));
                for (int j = 0; j < 4; j++)
                    Matrix[i].Add(0);
                T0.Add(0);
            }
            N = 2;
            Matrix[0][0] = 0.7;
            Matrix[0][1] = 0.1;
            Matrix[0][2] = 0.1;
            Matrix[0][3] = 0.1;
            Matrix[1][0] = 0.2;
            Matrix[1][1] = 0.6;
            Matrix[1][2] = 0;
            Matrix[1][3] = 0.2;
            Matrix[2][0] = 0.2;
            Matrix[2][1] = 0;
            Matrix[2][2] = 0.5;
            Matrix[2][3] = 0.3;
            Matrix[3][0] = 0;
            Matrix[3][1] = 0.3;
            Matrix[3][2] = 0;
            Matrix[3][3] = 0.7;
            T0[0] = 0;
            T0[1] = 0.8;
            T0[2] = 0.2;
            T0[3] = 0;
            #endregion

            StartCommand = new LambdaCommand(OnStartCommandExecuted, CanStartCommnadExecute);//инициализация команды 
        }
    }
}
