using BaoCao.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaoCao.GeneticAlgorithm
{
    public class GA
    {
        private readonly Graph _graph;
        private QuanThe _quanThe;
        private List<CaThe> _children;
        private int _quanTheMax;
        private double _survivalPercent;
        private double _couplePercent;

        public int QuanTheSize => _quanThe.Count;

        // O(quanTheSize * genSize) === O(quanTheSize * graph.M)
        public GA(Graph graph, int quanTheSize, int quanTheMax, double survivalPercent, double couplePercent)
        {
            _graph = graph;
            _children = new List<CaThe>();
            _quanThe = new QuanThe(graph.M + 1);
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
                CaThe a = new CaThe();

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

        // O(Max(_children.Count, N, graph.M))
        public void TinhThichNghi(out CaThe goal)
        {
            goal = null;
            foreach (CaThe child in _children)
            {
                child.UpdatePath(_graph);
                child.UpdateGenString();
                int phanTang = child.DoThichNghi;
                _quanThe.Add(phanTang, child);
            }

            for (int phanTang = 0; phanTang < _quanThe.SoTang; phanTang++)
            {
                int size = _quanThe.SoLuongCaThe(phanTang);
                if (size > 0)
                {
                    System.Console.WriteLine("Min Thich Nghi = " + size);
                    break;
                }
            }
            if (_quanThe.SoLuongCaThe(0) > 0)
            {
                goal = _quanThe[0, 0];
                return;
            }

            _children.Clear();
        }

        // (*) O(Max(N, _chilren.Count * graph.M)) ~ O(N * graph.M)
        public void TinhThichNghi2(out CaThe goal)
        {
            goal = null;
            int childrenSize = _children.Count;
            int middle = childrenSize / 2;

            Action<object> TinhThichNghiAction = (obj) =>
            {
                var data = obj as Tuple<int, int>;
                for (int i = data.Item1; i <= data.Item2; i++)
                {
                    CaThe child = _children[i];
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
                Task task = new Task(TinhThichNghiAction, data);
                task.Start();
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());

            // O(_children.Count)
            foreach (var child in _children)
            {
                _quanThe.Add(child.DoThichNghi, child);
            }
            // O(graph.M)
            for (int phanTang = 0; phanTang < _quanThe.Count; phanTang++)
            {
                int size = _quanThe.SoLuongCaThe(phanTang);
                if (size != 0)
                {
                    System.Console.WriteLine($"Min Thich Nghi = {phanTang} --- Size: {size}");
                    break;
                }
            }
            // O(N)
            if (_quanThe.SoLuongCaThe(0) != 0)
            {
                goal = _quanThe[0, 0];
                return;
            }
            _children.Clear();
        }

        // O(Max( deadNumber * N, graph.M * N ))
        public void ChonLocTuNhien()
        {
            int soTang = _quanThe.SoTang;
            int survivalNumber = Math.Min(_quanTheMax, (int)(_quanThe.Count * _survivalPercent));

            // O(deadNumber * N)
#if false
            #region old version
        for (int i = soTang - 1; _quanThe.Count > survivalNumber && i >= 0; i--)
        {
            int size = _quanThe.SoLuongCaThe(i);
            while (size > 0)
            {
                _quanThe.Remove(i, size - 1);
                size--;
                if (_quanThe.Count <= survivalNumber)
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
            int deadNumber = _quanThe.Count - survivalNumber;
            for (int i = soTang - 1; deadNumber > 0 && i >= 0; i--)
            {
                int size = _quanThe.SoLuongCaThe(i);
                if (size == 0)
                {
                    continue;
                }
                else if (deadNumber >= size)
                {
                    _quanThe.RemovePhanTang(i);
                    deadNumber -= size;
                }
                else // deadNumber < size
                {
                    _quanThe.RemoveRange(i, size - deadNumber);
                    deadNumber = 0;
                }
            }
            #endregion
#endif
        }

        // O(Max( graph.M, N * N ))
        public void LaiTao()
        {
            int soTang = _quanThe.SoTang;
            int coupleNumber = (int)(_couplePercent * _quanThe.Count);

            int phanTang2 = soTang - 1;
            int size2 = _quanThe.SoLuongCaThe(phanTang2);
            // O(graph.M)
            while (size2 == 0)
            {
                phanTang2--;
                size2 = _quanThe.SoLuongCaThe(phanTang2);
            }
            int index2 = size2 - 1;

            // O(N * N)
            for (int phanTang = 0; phanTang < soTang; phanTang++)
            {
                int size = _quanThe.SoLuongCaThe(phanTang);
                if (size == 0) continue;

                for (int index = 0; index < size; index++)
                {
                    if (index2 < 0)
                    {
                        phanTang2--;
                        size2 = _quanThe.SoLuongCaThe(phanTang2);
                        while (size2 == 0)
                        {
                            phanTang2--;
                            size2 = _quanThe.SoLuongCaThe(phanTang2);
                        }
                        index2 = size2 - 1;
                    }

                    // O(N)
                    CaThe a = _quanThe[phanTang, index];
                    CaThe b = _quanThe[phanTang2, index2];
                    _children.Add(GiaoPhoi(a, b));
                    _children.Add(GiaoPhoi(b, a));

                    index2--;
                }
            }
        }

        // O(Max(coupleNumber * Max(N, graph.M), _children.Count))
        public void LaiTao2()
        {
            int soTang = _quanThe.SoTang;
            int coupleNumber = (int)(_couplePercent * _quanThe.Count);

            // O(coupleNumber)
            #region create index child list 1
            List<Tuple<int, int>> indexParent1 = new List<Tuple<int, int>>();
            for (int phanTang = 0; phanTang < soTang && indexParent1.Count < coupleNumber; phanTang++)
            {
                int size = _quanThe.SoLuongCaThe(phanTang);
                for (int index = 0; index < size && indexParent1.Count < coupleNumber; index++)
                {
                    indexParent1.Add(new Tuple<int, int>(phanTang, index));
                }
            }
            #endregion
            #region create index child list 2
            List<Tuple<int, int>> indexParent2 = new List<Tuple<int, int>>();
            for (int phanTang = soTang - 1; phanTang >= 0 && indexParent2.Count < coupleNumber; phanTang--)
            {
                for (int index = _quanThe.SoLuongCaThe(phanTang) - 1; index >= 0 && indexParent2.Count < coupleNumber; index--)
                {
                    indexParent2.Add(new Tuple<int, int>(phanTang, index));
                }
            }
            #endregion

            Action<object> GiaoPhoiAction = (mess) =>
            {
                var data = mess as Tuple<int, int, List<CaThe>>;
                data.Item3.Clear();
                for (int i = data.Item1; i <= data.Item2; i++)
                {
                    CaThe a = _quanThe[indexParent1[i].Item1, indexParent1[i].Item2];
                    CaThe b = _quanThe[indexParent2[i].Item1, indexParent2[i].Item2];
                    CaThe child1 = GiaoPhoi(a, b);
                    CaThe child2 = GiaoPhoi(b, a);
                    data.Item3.Add(child1);
                    data.Item3.Add(child2);
                }
            };

            var dataList = new List<Tuple<int, int, List<CaThe>>>()
        {
            new Tuple<int, int, List<CaThe>>(0, coupleNumber/2 - 1, new List<CaThe>()),
            new Tuple<int, int, List<CaThe>>(coupleNumber/2, coupleNumber-1, new List<CaThe>()),
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

        // O(graph.M)
        private CaThe GiaoPhoi(CaThe a, CaThe b)
        {
            int genSize = a.Gen.Count;
            int indexCut = Tool.Random(genSize);
            CaThe child = new CaThe();
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

        // O(_children.Count)
        public void DotBien()
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
