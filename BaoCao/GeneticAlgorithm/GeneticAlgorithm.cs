using BaoCao.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BaoCao.GeneticAlgorithm
{
    public class GeneticAlgorithm
    {
        private readonly Graph _graph;
        private Population _population;
        private List<Individual> _children;
        private int _quanTheMax;
        private double _survivalPercent;
        private double _couplePercent;

        public int QuanTheSize => _population.Count;

        /// <summary>
        /// O(quanTheSize * genSize) === O(quanTheSize * graph.M)
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="quanTheSize">số lượng cá thể ban đầu</param>
        /// <param name="quanTheMax">số lượng cá thể tối đa trong quần thể</param>
        /// <param name="survivalPercent">tỉ lệ sống xót sau 1 đợt chọn lọc tự nhiên</param>
        /// <param name="couplePercent">tỉ lệ số cặp tham gia giao phối</param>
        public GeneticAlgorithm(Graph graph, int quanTheSize, int quanTheMax, double survivalPercent, double couplePercent)
        {
            _graph = graph;
            _children = new List<Individual>();
            _population = new Population(graph.M + 1);
            _quanTheMax = quanTheMax;
            _survivalPercent = survivalPercent;
            _couplePercent = couplePercent;

            int genSize = _graph.M;
            List<int> temp = new List<int>();
            for (int i = 0; i < genSize; i++)
            {
                temp.Add(i);
            }

            for (int i = 0; i < quanTheSize; i++)
            {
                Individual a = new Individual();

                for (int cur = 0; cur < genSize; cur++)
                {
                    int index = Tool.Random(genSize - cur);
                    a.Gen.Add(temp[index]);

                    int t = temp[index];
                    temp[index] = temp[genSize - cur - 1];
                    temp[genSize - cur - 1] = t;
                }

                _children.Add(a);
                // a.Show();
            }
        }


        /// <summary>
        /// O(Max(_children.Count, N, graph.M))
        /// </summary>
        /// <param name="goal"></param>
        public void ScoreFitness(out Individual goal)
        {
            goal = null;
            foreach (Individual child in _children)
            {
                child.UpdatePath(_graph);
                child.UpdateGenString();
                int floor = child.Fitness;
                _population.Add(floor, child);
            }

            for (int floor = 0; floor < _population.Floors; floor++)
            {
                int size = _population.CountIndividual(floor);
                if (size > 0)
                {
                    Debug.WriteLine($"Min Fitness =  {floor}  --- Size = {size}");
                    break;
                }
            }
            if (_population.CountIndividual(0) > 0)
            {
                goal = _population[0, 0];
                return;
            }

            _children.Clear();
        }


        /// <summary>
        /// (*) O(Max(N, _chilren.Count * graph.M)) ~ O(N * graph.M)        
        /// </summary>
        /// <param name="goal"></param>
        public void ScoreFitness2(out Individual goal)
        {
            goal = null;
            int childrenSize = _children.Count;
            int middle = childrenSize / 2;

            Action<object> ScoreFitnessAction = (obj) =>
            {
                // (Item1, Item2)  <=>  (firstChildIndex, lastChildIndex)
                var data = obj as Tuple<int, int>;
                for (int i = data.Item1; i <= data.Item2; i++)
                {
                    Individual child = _children[i];
                    child.UpdatePath(_graph);
                    child.UpdateGenString();
                    // int phanTang = child.DoThichNghi;
                    // _quanThe.Add(phanTang, child);
                }
            };

            List<Tuple<int, int>> dataList = new List<Tuple<int, int>>()
            {
                new Tuple<int, int>(0, middle - 1),
                new Tuple<int, int>(middle, childrenSize - 1)
            };

            List<Task> tasks = new List<Task>();
            // O(tasks.Count * (_children.Count * graph.M)) ~ O(_children.Count * graph.M)
            foreach (var data in dataList)
            {
                // O(middle * graph.M) === O(_children.Count/2 * graph.M) ~ O(_children.Count * graph.M)
                Task task = new Task(ScoreFitnessAction, data);
                task.Start();
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());

            // O(_children.Count)
            foreach (var child in _children)
            {
                _population.Add(child.Fitness, child);
            }
            // O(graph.M)
            for (int floor = 0; floor < _population.Count; floor++)
            {
                int size = _population.CountIndividual(floor);
                if (size != 0)
                {
                    Debug.WriteLine($"Min Fitness = {floor} --- Size: {size}");
                    break;
                }
            }
            // O(N)
            if (_population.CountIndividual(0) != 0)
            {
                goal = _population[0, 0];
                return;
            }
            _children.Clear();
        }


        /// <summary>
        /// O(Max( deadNumber * N, graph.M * N ))
        /// </summary>
        public void Selection()
        {
            int floors = _population.Floors;
            int survivalNumber = Math.Min(_quanTheMax, (int)(_population.Count * _survivalPercent));

            // O(deadNumber * N)
#if false
            #region old version
            for (int floor = floors - 1; _population.Count > survivalNumber && floor >= 0; floor--)
            {
                int size = _population.CountIndividual(floor);
                while (size > 0)
                {
                    _population.Remove(floor, size - 1);
                    size--;
                    if (_population.Count <= survivalNumber)
                    {
                        break;
                    }
                }
            }
            #endregion
#endif

            // O(M * N)
#if true
            #region new version
            int deadNumber = _population.Count - survivalNumber;
            for (int floor = floors - 1; deadNumber > 0 && floor >= 0; floor--)
            {
                int size = _population.CountIndividual(floor);
                if (size == 0)
                {
                    continue;
                }
                else if (deadNumber >= size)
                {
                    _population.RemoveFloor(floor);
                    deadNumber -= size;
                }
                else // deadNumber < size
                {
                    _population.RemoveRange(floor, size - deadNumber);
                    deadNumber = 0;
                }
            }
            #endregion
#endif
        }


        /// <summary>
        /// O(Max( graph.M, N * N ))
        /// </summary>
        public void CrossOver()
        {
            int floors = _population.Floors;
            int coupleNumber = (int)(_couplePercent * _population.Count);

            int floor2 = floors - 1;
            int size2 = _population.CountIndividual(floor2);
            // O(graph.M)
            while (size2 == 0)
            {
                floor2--;
                size2 = _population.CountIndividual(floor2);
            }
            int index2 = size2 - 1;

            // O(N * N)
            for (int floor1 = 0; floor1 < floors; floor1++)
            {
                int size = _population.CountIndividual(floor1);
                if (size == 0) continue;

                for (int index1 = 0; index1 < size; index1++)
                {
                    if (index2 < 0)
                    {
                        floor2--;
                        size2 = _population.CountIndividual(floor2);
                        while (size2 == 0)
                        {
                            floor2--;
                            size2 = _population.CountIndividual(floor2);
                        }
                        index2 = size2 - 1;
                    }

                    // O(N)
                    Individual a = _population[floor1, index1];
                    Individual b = _population[floor2, index2];
                    _children.Add(GiaoPhoi(a, b));
                    _children.Add(GiaoPhoi(b, a));

                    index2--;
                }
            }
        }


        /// <summary>
        /// O(Max(coupleNumber * Max(N, graph.M), _children.Count))
        /// </summary>
        public void CrossOver2()
        {
            int floors = _population.Floors;
            int coupleNumber = (int)(_couplePercent * _population.Count);

            // O(coupleNumber)
            #region create index child list 1
            List<Tuple<int, int>> indexParent1 = new List<Tuple<int, int>>();
            for (int floor = 0; floor < floors && indexParent1.Count < coupleNumber; floor++)
            {
                int size = _population.CountIndividual(floor);
                for (int index = 0; index < size && indexParent1.Count < coupleNumber; index++)
                {
                    indexParent1.Add(new Tuple<int, int>(floor, index));
                }
            }
            #endregion

            #region create index child list 2
            List<Tuple<int, int>> indexParent2 = new List<Tuple<int, int>>();
            for (int floor = floors - 1; floor >= 0 && indexParent2.Count < coupleNumber; floor--)
            {
                for (int index = _population.CountIndividual(floor) - 1; index >= 0 && indexParent2.Count < coupleNumber; index--)
                {
                    indexParent2.Add(new Tuple<int, int>(floor, index));
                }
            }
            #endregion

            Action<object> GiaoPhoiAction = (mess) =>
            {
                // (Item1, Item2, Item3)  <=>  (firstParentIndex, lastParentIndex, childrenList)
                var data = mess as Tuple<int, int, List<Individual>>;
                data.Item3.Clear();
                for (int i = data.Item1; i <= data.Item2; i++)
                {
                    Individual a = _population[indexParent1[i].Item1, indexParent1[i].Item2];
                    Individual b = _population[indexParent2[i].Item1, indexParent2[i].Item2];
                    Individual child1 = GiaoPhoi(a, b);
                    Individual child2 = GiaoPhoi(b, a);
                    data.Item3.Add(child1);
                    data.Item3.Add(child2);
                }
            };

            var dataList = new List<Tuple<int, int, List<Individual>>>()
            {
                new Tuple<int, int, List<Individual>>(0, coupleNumber/2 - 1, new List<Individual>()),
                new Tuple<int, int, List<Individual>>(coupleNumber/2, coupleNumber-1, new List<Individual>()),
			    // new Tuple<int, int, List<CaThe>>(0, coupleNumber/3 - 1, new List<CaThe>()),
			    // new Tuple<int, int, List<CaThe>>(coupleNumber/3, coupleNumber*2/3-1, new List<CaThe>()),
			    // new Tuple<int, int, List<CaThe>>(coupleNumber*2/3, coupleNumber-1, new List<CaThe>()),
		    };

            List<Task> tasks = new List<Task>();
            // O(tasks.Count * (coupleNumber * Max(N, graph.M))) ~ O(coupleNumber * Max(N, graph.M))
            foreach (var data in dataList)
            {
                Task task = new Task(GiaoPhoiAction, data);
                task.Start();
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
            // O(_children.Count)
            foreach (var item in dataList)
            {
                item.Item3.ForEach(child => _children.Add(child));
                // foreach (var child in item.Item3)
                // {
                // 	_children.Add(child);
                // }
            }
        }


        /// <summary>
        /// O(graph.M)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private Individual GiaoPhoi(Individual a, Individual b)
        {
            int genSize = a.Gen.Count;
            int indexCut = Tool.Random(genSize);
            Individual child = new Individual();
            List<bool> pick = Enumerable.Repeat(false, genSize).ToList();

            if (a.ErrorIndexList[0] + 1 < indexCut)
            {
                child.ErrorIndexList.Add(a.ErrorIndexList[0]);
            }

            for (int i = 0; i < indexCut; i++)
            {
                child.Gen.Add(a.Gen[i]);
                pick[a.Gen[i]] = true;
            }
            for (int i = indexCut; i < genSize; i++)
            {
                if (!pick[b.Gen[i]])
                {
                    child.Gen.Add(b.Gen[i]);
                    pick[b.Gen[i]] = true;
                }
            }
            if (child.Gen.Count != genSize)
            {
                for (int i = indexCut; i < genSize; i++)
                {
                    if (!pick[a.Gen[i]])
                    {
                        child.Gen.Add(a.Gen[i]);
                        pick[a.Gen[i]] = true;
                        if (child.Gen.Count == genSize) break;
                    }
                }
            }
            return child;
        }


        /// <summary>
        /// O(_children.Count)        
        /// </summary>
        public void Mutation()
        {
            int genSize = _graph.M;
            foreach (var child in _children)
            {
                // if (Tool.Random(2) == 0) continue;

                int index2, index1 = child.ErrorIndexList.Count == 0 ? Tool.Random(_graph.M) : child.ErrorIndexList[0];
                do
                {
                    index2 = Tool.Random(_graph.M);
                } while (index1 == index2);

                int t = child.Gen[index1];
                child.Gen[index1] = child.Gen[index2];
                child.Gen[index2] = t;
            }
        }
    }
}
