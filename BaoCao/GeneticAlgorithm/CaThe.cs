using BaoCao.Structures;
using System;
using System.Collections.Generic;

namespace BaoCao.GeneticAlgorithm
{
    public class CaThe
    {
        private string _genString;
        public List<int> Gen { get; set; }
        private List<int> _errorIndexList;
        public List<int> ErrorIndexList { get => _errorIndexList; private set => _errorIndexList = value; }
        public int DoThichNghi => ErrorIndexList.Count;

        public CaThe()
        {
            Gen = new List<int>();
            ErrorIndexList = new List<int>();
            _genString = "";
        }

        public void Show()
        {
            Console.WriteLine("Gen: " + _genString);
        }

        public List<int> UpdatePath(Graph graph)
        {
            Edge e = graph.EdgeList[Gen[0]];

            List<int> path1 = BuildPath(graph, e.U, e.V, out _errorIndexList);

            List<int> errList = null;
            List<int> path2 = BuildPath(graph, e.V, e.U, out errList);

            if (errList.Count < ErrorIndexList.Count)
            {
                ErrorIndexList = errList;
                return path2;
            }
            else
            {
                return path1;
            }
        }

        public void UpdateGenString()
        {
            _genString = "";
            foreach (var item in Gen)
            {
                _genString += item + ",";
            }
        }

        private List<int> BuildPath(Graph graph, int u, int v, out List<int> errList)
        {
            // giaTriThichNghi = 0;
            errList = new List<int>();
            List<int> newPath = new List<int>() { u, v };
            HashSet<Edge> set = new HashSet<Edge>(new EdgeComparer());
            set.Add(new Edge(u, v));

            int genSize = Gen.Count;
            for (int i = 1; i < genSize; i++)
            {
                int back = newPath[newPath.Count - 1];
                Edge e = graph.EdgeList[Gen[i]];

                if (back == e.U)
                {
                    newPath.Add(e.V);
                    if (!set.Add(e))
                    {
                        // giaTriThichNghi++;
                        errList.Add(i);
                    }
                }
                else if (back == e.V)
                {
                    newPath.Add(e.U);
                    if (!set.Add(e))
                    {
                        // giaTriThichNghi++;
                        errList.Add(i);
                    }
                }
                else
                {
                    newPath.Add(e.U);
                    newPath.Add(e.V);
                    Edge temp = new Edge(back, e.U);

                    if (!graph.Adjacent(temp.U, temp.V) || !set.Add(temp))
                    {
                        // giaTriThichNghi++;
                        errList.Add(i);
                    }
                    if (!set.Add(e))
                    {
                        // giaTriThichNghi++;
                        errList.Add(i);
                    }
                }
            }

            return newPath;
        }

        public override string ToString()
        {
            return _genString;
        }
    }

    public class CaTheComparer : IEqualityComparer<CaThe>
    {
        public bool Equals(CaThe x, CaThe y)
        {
            return x.ToString() == y.ToString();
        }

        public int GetHashCode(CaThe obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
