using BaoCao.Objects;
using BaoCao.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BaoCao.GeneticAlgorithm
{
    public class GAExcuter
    {
        public void Excute(string filePath)
        {
            System.Console.WriteLine("Start");
            #region read data - build graph
            Graph graph;
            ReadData(filePath, out graph);
            #endregion

            System.Console.WriteLine("Running...");
            #region GA_Algorithm
            Stopwatch watch = new Stopwatch();
            watch.Start();

            CaThe goal = null;
            GA ga = new GA(
                        graph,
                        quanTheSize: 30,
                        quanTheMax: 3000,
                        survivalPercent: 0.7f,
                        couplePercent: 0.4f);
            int theHe = 0;

            // O(k * N * graph.M)
            while (true)
            {
                Debug.WriteLine("The he: " + (++theHe));

                // (*) O(Max(N, _chilren.Count * graph.M)) ~ O(N * graph.M)
                ga.TinhThichNghi2(out goal);
                if (goal != null)
                {
                    Debug.WriteLine("found");
                    break;
                }

                // O(Max( deadNumber * N, graph.M * N ))
                ga.ChonLocTuNhien();

                // O(Max(coupleNumber * Max(N, graph.M), _children.Count))
                ga.LaiTao2();

                // O(_children.Count)
                ga.DotBien();
            }

            watch.Stop();
            #endregion

            Debug.WriteLine("End");
            goal?.Show();
            List<int> path = goal?.UpdatePath(graph);
            Debug.Write("Path: ");
            path?.ForEach(item => Console.Write(item + " "));
            Debug.WriteLine("\nTotal time: " + watch.ElapsedMilliseconds + "ms");
        }

        public void Excute(List<Node> nodeList, List<Objects.Edge> edgeList)
        {
            System.Console.WriteLine("Start");
            #region read data - build graph
            var graph = new Structures.Graph(nodeList.Count);
            edgeList.ForEach(edge => graph.AddEdge((int)edge.UNode.Tag, (int)edge.VNode.Tag));
            #endregion

            System.Console.WriteLine("Running...");
            #region GA_Algorithm
            Stopwatch watch = new Stopwatch();
            watch.Start();

            CaThe goal = null;
            GA ga = new GA(
                        graph,
                        quanTheSize: 30,
                        quanTheMax: 3000,
                        survivalPercent: 0.7f,
                        couplePercent: 0.4f);
            int theHe = 0;

            // O(k * N * graph.M)
            while (true)
            {
                Debug.WriteLine("The he: " + (++theHe));

                // (*) O(Max(N, _chilren.Count * graph.M)) ~ O(N * graph.M)
                ga.TinhThichNghi2(out goal);
                if (goal != null)
                {
                    Debug.WriteLine("found");
                    break;
                }

                // O(Max( deadNumber * N, graph.M * N ))
                ga.ChonLocTuNhien();

                // O(Max(coupleNumber * Max(N, graph.M), _children.Count))
                ga.LaiTao2();

                // O(_children.Count)
                ga.DotBien();
            }

            watch.Stop();
            #endregion

            Debug.WriteLine("End");
            goal?.Show();
            List<int> path = goal?.UpdatePath(graph);
            Debug.Write("Path: ");
            path?.ForEach(nodeIndex => Debug.Write($"({nodeIndex}:{nodeList[nodeIndex].NodeText})  "));
            Debug.WriteLine("\nTotal time: " + watch.ElapsedMilliseconds + "ms");

        }

        private void ReadData(string fileName, out Graph graph)
        {
            var dataLines = File.ReadAllLines(fileName);
            graph = null;

            #region read n, m
            int n, m;
            ConvertToNumbers(dataLines[0], out n, out m);
            graph = new Graph(n);
            #endregion

            #region read edgeList
            int u, v;
            for (int i = 1; i <= m; i++)
            {
                ConvertToNumbers(dataLines[i], out u, out v);
                graph.AddEdge(u, v);
            }
            #endregion
        }

        private void ConvertToNumbers(string s, out int a, out int b)
        {
            var args = s.Split(' ');
            if (!Int32.TryParse(args[0], out a) || !Int32.TryParse(args[1], out b))
            {
                throw new Exception("can not convert to int");
            }
        }
    }

}
