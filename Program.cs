using System;
using System.Collections.Generic;

namespace AntColony
{
    internal class AntColonyProgram
    {

        private static readonly Random random = new Random(0);
        // влияние феромона на направление
        private static readonly int alpha = 3;
        // влияние расстояния между соседними узлами
        private static readonly int beta = 4;

        // коэффициент ослабления феромонов
        private static readonly double rho = 0.01;
        // коэффициент усиления феромонов
        private static readonly double Q = 2.0;

        public static void Main()
        {
            try
            {
                Console.WriteLine("\nМуравьиный алгоритм\n");

                int numCities = 30;
                int numAnts = 15;
                int maxTime = 1000;

                Console.WriteLine("Количество точек = " + numCities);

                Console.WriteLine("\nКоличество муравьев = " + numAnts);
                Console.WriteLine("Максимальное время работы муравьев = " + maxTime);

                Console.WriteLine("\nВлияние феромона на направление = " + alpha);
                Console.WriteLine("Влияние расстояния между соседними узлами = " + beta);
                Console.WriteLine("Коэффициент ослабления феромонов = " + rho.ToString("F2"));
                Console.WriteLine("Коэффициент усиления феромонов = " + Q.ToString("F2"));

                Console.WriteLine("\nСоздание путей в графе");
                Console.WriteLine("Выбор генерации пути:" +
                    "\n1.Случайная генерация" +
                    "\n2.Контрольный пример");
                int sw = Convert.ToInt32(Console.ReadLine());

                int[][] dists = MakeGraphDistances(numCities); 
                if(sw == 2)
                {
                    dists = MakePrivateDistances(numCities);
                    DisplayPrivateEx(dists, numCities);
                }
                
                Console.WriteLine("\nВывод муравьев на случайные тропы\n");
                int[][] ants = InitAnts(numAnts, numCities);
                // инициализируйте муравьев на случайных маршрутахize ants to random trails
                ShowAnts(ants, dists);

                int[] bestTrail = AntColonyProgram.BestTrail(ants, dists);
                // определение наилучшего начального маршрута
                double bestLength = Length(bestTrail, dists);
                // Длинна лучшего маршрута

                Console.Write("\nНаилучшая начальная длина трассы: " + bestLength.ToString("F1") + "\n");

                Console.WriteLine("\nСоздание феромонов на тропах");
                double[][] pheromones = InitPheromones(numCities);

                int time = 0;
                Console.WriteLine("\nВвод обновляющих муравьев (цикл обнавления ферамонов)\n");
                while (time < maxTime)
                {
                    UpdateAnts(ants, pheromones, dists);
                    UpdatePheromones(pheromones, ants, dists);

                    int[] currBestTrail = AntColonyProgram.BestTrail(ants, dists);
                    double currBestLength = Length(currBestTrail, dists);
                    if (currBestLength < bestLength)
                    {
                        bestLength = currBestLength;
                        bestTrail = currBestTrail;
                        Console.WriteLine("Наилучшая новая длинна " + bestLength.ToString("F1") + " найдено за время " + time);
                    }
                    time += 1;
                }

                Console.WriteLine("\nВремя завершилось");

                Console.WriteLine("\nЛучший найденный путь:");
                Display(bestTrail);
                Console.WriteLine("\nДлинна лучшего найденного пути: " + bestLength.ToString("F1"));

                Console.WriteLine("\nКонец алгоритма\n");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

        }

        private static int[][] MakePrivateDistances(int numCities)
        {
            int[][] dists = new int[numCities][];
            for (int i = 0; i <= dists.Length - 1; i++)
            {
                dists[i] = new int[numCities];
            }
            dists[0][0] = 0;
            dists[0][1] = 5;
            dists[0][2] = int.MaxValue;
            dists[0][3] = int.MaxValue;
            dists[0][4] = 1;
            dists[0][5] = 8;

            dists[1][0] = 7;
            dists[1][1] = 0;
            dists[1][2] = 1;
            dists[1][3] = int.MaxValue;
            dists[1][4] = int.MaxValue;
            dists[1][5] = 5;

            dists[2][0] = int.MaxValue;
            dists[2][1] = 3;
            dists[2][2] = 0;
            dists[2][3] = 6;
            dists[2][4] = int.MaxValue;
            dists[2][5] = 1;

            dists[3][0] = int.MaxValue;
            dists[3][1] = int.MaxValue;
            dists[3][2] = 8;
            dists[3][3] = 0;
            dists[3][4] = 7;
            dists[3][5] = 6;

            dists[4][0] = 7;
            dists[4][1] = int.MaxValue;
            dists[4][2] = int.MaxValue;
            dists[4][3] = 5;
            dists[4][4] = 0;
            dists[4][5] = int.MaxValue;

            dists[5][0] = 7;
            dists[5][1] = 7;
            dists[5][2] = 3;
            dists[5][3] = 8;
            dists[5][4] = 7;
            dists[5][5] = 0;
            return dists;
            
        }

        // Main

        // --------------------------------------------------------------------------------------------

        private static int[][] InitAnts(int numAnts, int numCities)
        {
            int[][] ants = new int[numAnts][];
            for (int k = 0; k <= numAnts - 1; k++)
            {
                int start = random.Next(0, numCities);
                ants[k] = RandomTrail(start, numCities);
            }
            return ants;
        }

        private static int[] RandomTrail(int start, int numCities)
        {

            int[] trail = new int[numCities];


            for (int i = 0; i <= numCities - 1; i++)
            {
                trail[i] = i;
            }

            // Тасовка Фишера-Йейтса
            for (int i = 0; i <= numCities - 1; i++)
            {
                int r = random.Next(i, numCities);
                int tmp = trail[r];
                trail[r] = trail[i];
                trail[i] = tmp;
            }

            int idx = IndexOfTarget(trail, start);
            // Ставим в начало [0]
            int temp = trail[0];
            trail[0] = trail[idx];
            trail[idx] = temp;

            return trail;
        }

        private static int IndexOfTarget(int[] trail, int target)
        {
            // помощник для RandomTrail
            for (int i = 0; i <= trail.Length - 1; i++)
            {
                if (trail[i] == target)
                {
                    return i;
                }
            }
            throw new Exception("Target not found in IndexOfTarget");
        }
        //Длинна пути когда муравей прошел все города один раз
        private static double Length(int[] trail, int[][] dists)
        {
            // общая протяженность пути
            double result = 0.0;
            for (int i = 0; i <= trail.Length - 2; i++)
            {
                result += Distance(trail[i], trail[i + 1], dists);
            }
            return result;
        }

        // -------------------------------------------------------------------------------------------- 

        private static int[] BestTrail(int[][] ants, int[][] dists)
        {
            // лучшая трасса имеет самую короткую общую протяженность
            double bestLength = Length(ants[0], dists);
            int idxBestLength = 0;
            for (int k = 1; k <= ants.Length - 1; k++)
            {
                double len = Length(ants[k], dists);
                if (len < bestLength)
                {
                    bestLength = len;
                    idxBestLength = k;
                }
            }
            int numCities = ants[0].Length;
            int[] bestTrail_Renamed = new int[numCities];
            ants[idxBestLength].CopyTo(bestTrail_Renamed, 0);
            return bestTrail_Renamed;
        }

        // --------------------------------------------------------------------------------------------

        private static double[][] InitPheromones(int numCities)
        {
            double[][] pheromones = new double[numCities][];
            for (int i = 0; i <= numCities - 1; i++)
            {
                pheromones[i] = new double[numCities];
            }
            for (int i = 0; i <= pheromones.Length - 1; i++)
            {
                for (int j = 0; j <= pheromones[i].Length - 1; j++)
                {
                    pheromones[i][j] = 0.01;
                    //  противном случае первый вызов UpdateAnts -> BuiuldTrail -> nextNode -> MoveProbs => all 0.0 => выдает
                }
            }
            return pheromones;
        }

        // --------------------------------------------------------------------------------------------

        private static void UpdateAnts(int[][] ants, double[][] pheromones, int[][] dists)
        {
            int numCities = pheromones.Length;
            for (int k = 0; k <= ants.Length - 1; k++)
            {
                int start = random.Next(0, numCities);
                int[] newTrail = BuildTrail(k, start, pheromones, dists);
                ants[k] = newTrail;
            }
        }

        private static int[] BuildTrail(int k, int start, double[][] pheromones, int[][] dists)
        {
            int numCities = pheromones.Length;
            int[] trail = new int[numCities];
            bool[] visited = new bool[numCities];
            trail[0] = start;
            visited[start] = true;
            for (int i = 0; i <= numCities - 2; i++)
            {
                int cityX = trail[i];
                int next = NextCity(k, cityX, visited, pheromones, dists);
                trail[i + 1] = next;
                visited[next] = true;
            }
            return trail;
        }

        private static int NextCity(int k, int cityX, bool[] visited, double[][] pheromones, int[][] dists)
        {
            // для ant k (с посещенным[]), в nodeX, каков следующий узел в trail?
            double[] probs = MoveProbs(k, cityX, visited, pheromones, dists);
            double[] cumul = new double[probs.Length + 1];
            for (int i = 0; i <= probs.Length - 1; i++)
            {
                cumul[i + 1] = cumul[i] + probs[i];
                // рассматриваем возможность установки cumul[cuml.Длина-от 1] до 1,00
            }

            double p = random.NextDouble();

            for (int i = 0; i <= cumul.Length - 2; i++)
            {
                if (p >= cumul[i] && p < cumul[i + 1])
                {
                    return i;
                }
            }
            throw new Exception("Failure to return valid city in NextCity");
        }

        private static double[] MoveProbs(int k, int cityX, bool[] visited, double[][] pheromones, int[][] dists)
        {
            // для ant k, расположенного в nodeX, с помощью visited[] возвращаем пути перемещения в каждый город
            int numCities = pheromones.Length;
            double[] taueta = new double[numCities];
            // включает cityX и посещенные города
            double sum = 0.0;
            // сумма tauetas
            // i - соседний город
            for (int i = 0; i <= taueta.Length - 1; i++)
            {
                if (i == cityX)
                {
                    taueta[i] = 0.0;
                    // вероятность самостоятельного перехода равна 0
                }
                else if (visited[i] == true)
                {
                    taueta[i] = 0.0;
                    // вероятность переезда в посещаемый город равна 0
                }
                else
                {
                    taueta[i] = Math.Pow(pheromones[cityX][i], alpha) * Math.Pow((1.0 / Distance(cityX, i, dists)), beta);
                    // может быть огромным, когда pheromone[][] большой
                    if (taueta[i] < 0.0001)
                    {
                        taueta[i] = 0.0001;
                    }
                    else if (taueta[i] > (double.MaxValue / (numCities * 100)))
                    {
                        taueta[i] = double.MaxValue / (numCities * 100);
                    }
                }
                sum += taueta[i];
            }

            double[] probs = new double[numCities];
            for (int i = 0; i <= probs.Length - 1; i++)
            {
                probs[i] = taueta[i] / sum;
                //главное чтобы сумма не оказалось 0
            }
            return probs;
        }

        // --------------------------------------------------------------------------------------------

        private static void UpdatePheromones(double[][] pheromones, int[][] ants, int[][] dists)
        {
            for (int i = 0; i <= pheromones.Length - 1; i++)
            {
                for (int j = i + 1; j <= pheromones[i].Length - 1; j++)
                {
                    for (int k = 0; k <= ants.Length - 1; k++)
                    {
                        double length = AntColonyProgram.Length(ants[k], dists);
                        // длина тропы ant k
                        double decrease = (1.0 - rho) * pheromones[i][j];
                        double increase = 0.0;
                        if (EdgeInTrail(i, j, ants[k]) == true)
                        {
                            increase = (Q / length);
                        }

                        pheromones[i][j] = decrease + increase;

                        if (pheromones[i][j] < 0.0001)
                        {
                            pheromones[i][j] = 0.0001;
                        }
                        else if (pheromones[i][j] > 100000.0)
                        {
                            pheromones[i][j] = 100000.0;
                        }

                        pheromones[j][i] = pheromones[i][j];
                    }
                }
            }
        }

        private static bool EdgeInTrail(int cityX, int cityY, int[] trail)
        {
            // находятся ли cityX и Cityyy рядом друг с другом в trail[]?
            int lastIndex = trail.Length - 1;
            int idx = IndexOfTarget(trail, cityX);

            if (idx == 0 && trail[1] == cityY)
            {
                return true;
            }
            else if (idx == 0 && trail[lastIndex] == cityY)
            {
                return true;
            }
            else if (idx == 0)
            {
                return false;
            }
            else if (idx == lastIndex && trail[lastIndex - 1] == cityY)
            {
                return true;
            }
            else if (idx == lastIndex && trail[0] == cityY)
            {
                return true;
            }
            else if (idx == lastIndex)
            {
                return false;
            }
            else if (trail[idx - 1] == cityY)
            {
                return true;
            }
            else if (trail[idx + 1] == cityY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        // --------------------------------------------------------------------------------------------

        private static int[][] MakeGraphDistances(int numCities)
        {
            int[][] dists = new int[numCities][];
            for (int i = 0; i <= dists.Length - 1; i++)
            {
                dists[i] = new int[numCities];
            }
            for (int i = 0; i <= numCities - 1; i++)
            {
                for (int j = i + 1; j <= numCities - 1; j++)
                {
                    int d = random.Next(1, 9);
                    // [1,8]
                    dists[i][j] = d;
                    dists[j][i] = d;
                }
            }
            return dists;
        }

        //Дистанция между двумя городами
        private static double Distance(int cityX, int cityY, int[][] dists)
        {
            return dists[cityX][cityY];
        }

        // --------------------------------------------------------------------------------------------

        //Вывод на экран одномерного массива
        private static void Display(int[] trail)
        {
            for (int i = 0; i <= trail.Length - 1; i++)
            {
                Console.Write(trail[i] + " ");
                if (i > 0 && i % 20 == 0)
                {
                    Console.WriteLine("");
                }
            }
            Console.WriteLine("");
        }


        private static void ShowAnts(int[][] ants, int[][] dists)
        {
            for (int i = 0; i <= ants.Length - 1; i++)
            {
                Console.Write(i + ": [ ");

                for (int j = 0; j <= 3; j++)
                {
                    Console.Write(ants[i][j] + " ");
                }

                Console.Write(". . . ");

                for (int j = ants[i].Length - 4; j <= ants[i].Length - 1; j++)
                {
                    Console.Write(ants[i][j] + " ");
                }

                Console.Write("] len = ");
                double len = Length(ants[i], dists);
                Console.Write(len.ToString("F1"));
                Console.WriteLine("");
            }
        }

        private static void Display(double[][] pheromones)
        {
            for (int i = 0; i <= pheromones.Length - 1; i++)
            {
                Console.Write(i + ": ");
                for (int j = 0; j <= pheromones[i].Length - 1; j++)
                {
                    Console.Write(pheromones[i][j].ToString("F4").PadLeft(8) + " ");
                }
                Console.WriteLine("");
            }

        }
        private static void DisplayPrivateEx(int[][] dist, int numCities)
        {
            List<char> abcdfg = new List<char> { 'a','b','c','d','f','g'};
            Console.Write("  ");
            foreach (var i in abcdfg)
            {
                Console.Write(" | " + i);
            }
            Console.Write(" | ");
            Console.WriteLine("");
            Console.WriteLine("----------------------------");
            for (int i = 0; i <= numCities - 1; i++)
            {
                Console.Write(" ");
                Console.Write(abcdfg[i]+ " | ");
                for (int j = 0; j <= numCities - 1; j++)
                {
                    if (dist[i][j] == int.MaxValue)
                    {
                        Console.Write("0" + "   ");
                    }
                    else
                    {
                        Console.Write(dist[i][j] + "   ");
                    }
                }
                Console.WriteLine("");
                Console.WriteLine("----");
            }
        }

    }

}
