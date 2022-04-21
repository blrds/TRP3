﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRP3.Models
{
    class Graph
    {
        public Graph(List<List<double>> matrix, List<double> t0, int n)
        {
            Matrix = matrix;
            T0 = t0;
            N = n;
        }

        public List<List<double>> Matrix { get; private set; }

        public List<double> T0 { get; private set; }

        public int N { get; private set; }


        /// <summary>
        /// Суммирует соответсвующие элементы последовательности
        /// </summary>
        private List<double> ProbabilitySum(List<double> a, List<double> b)
        {
            var answer = new List<double>();
            for (int i = 0; i < 4; i++)
            {
                answer.Add(a[i] + b[i]);
            }
            return answer;
        }
        /// <summary>
        /// Умножает каждый элемент последовательности а на b
        /// </summary>
        private List<double> ProbabilityProduct(List<double> a, double b)
        {
            var answer = new List<double>();
            for (int i = 0; i < 4; i++)
            {
                answer.Add(a[i] * b);
            }
            return answer;
        }

        /// <summary>
        /// Новый шаг в рекурсии
        /// </summary>
        /// <param name="curentPosition">текущая позиция в графе</param>
        /// <param name="moveTillEnd">сколько шагов осталось</param>
        /// <returns>вероятность перехода во все позиции за оставшиеся шаги</returns>
        private List<double> Split(int curentPosition, int moveTillEnd)
        {
            List<double> answer = new List<double>();
            for (int i = 0; i < 4; i++)
                answer.Add(0);
            if (moveTillEnd > 1)
            {
                for (int i = 0; i < 4; i++)
                    if (Matrix[curentPosition][i] > 0)//если вероятность перехода есть
                    {
                        var a = Split(i, moveTillEnd - 1);//уходим в другую позицию, забирая шаг
                        a = ProbabilityProduct(a, Matrix[curentPosition][i]);//умножаем найденные веротности на вероятность уйти в другую позицию
                        answer = ProbabilitySum(answer, a);//сохраняем вероятности
                    }

            }
            else
            {   //если последний шаг, то возвращаем вероятности с матрицы переходов без изменений
                for (int i = 0; i < 4; i++)
                    answer[i] = Matrix[curentPosition][i];
            }
            return answer;
        }

        public List<double> Start()
        {
            List<double> answer = new List<double>(4);
            for (int i = 0; i < 4; i++)
                answer.Add(0);
            for(int i=0;i<4;i++)
            {
                if (T0[i] > 0) {//если есть возможность входа с данной позиции, запускаем рекурсию
                    var b = Split(i, N);
                    answer = ProbabilitySum(answer, b);
                }
            }

            return answer;
        }
    }
}