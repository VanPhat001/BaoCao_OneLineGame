using BaoCao.Objects;
using BaoCao.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace BaoCao.GeneticAlgorithm
{
    public class GAExcuter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public void Excute(string filePath)
        {
            Debug.WriteLine("Start");
            #region read data - build graph
            Graph graph;
            ReadData(filePath, out graph);
            #endregion

            Debug.WriteLine("Running...");
            #region GA_Algorithm
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Individual goal = null;
            GeneticAlgorithm ga = new GeneticAlgorithm(
                        graph,
                        quanTheSize: 30,
                        quanTheMax: 3000,
                        survivalPercent: 0.7f,
                        couplePercent: 0.4f);
            int generation = 0;

            // O(k * N * graph.M)
            while (true)
            {
                Debug.WriteLine("Generation: " + (++generation));

                // (*) O(Max(N, _chilren.Count * graph.M)) ~ O(N * graph.M)
                ga.ScoreFitness2(out goal);
                if (goal != null)
                {
                    Debug.WriteLine("found");
                    break;
                }

                // O(Max( deadNumber * N, graph.M * N ))
                ga.Selection();

                // O(Max(coupleNumber * Max(N, graph.M), _children.Count))
                ga.CrossOver2();

                // O(_children.Count)
                ga.Mutation();
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="edgeList"></param>
        /// <returns></returns>
        public async Task<List<int>> ExcuteAsync(List<Node> nodeList, List<Objects.Edge> edgeList)
        {
            await Task.Delay(1);
            Debug.WriteLine("Start");
            #region read data - build graph
            var graph = new Structures.Graph(nodeList.Count);
            edgeList.ForEach(edge => graph.AddEdge((int)edge.UNode.Tag, (int)edge.VNode.Tag));
            #endregion

            //await Task.Delay(1);
            Debug.WriteLine("Running...");
            #region GA_Algorithm
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Individual goal = null;
            GeneticAlgorithm ga = new GeneticAlgorithm(
                        graph,
                        quanTheSize: 30,
                        quanTheMax: 3000,
                        survivalPercent: 0.7f,
                        couplePercent: 0.4f);
            int generation = 0;

            // O(k * N * graph.M)
            while (true)
            {
                Debug.WriteLine("Generation: " + (++generation));

                // (*) O(Max(N, _chilren.Count * graph.M)) ~ O(N * graph.M)
                ga.ScoreFitness2(out goal);
                if (goal != null)
                {
                    Debug.WriteLine("found");
                    break;
                }

                // O(Max( deadNumber * N, graph.M * N ))
                ga.Selection();

                // O(Max(coupleNumber * Max(N, graph.M), _children.Count))
                ga.CrossOver2();

                // O(_children.Count)
                ga.Mutation();

                //await Task.Delay(1);
            }

            watch.Stop();
            #endregion
                        
            Debug.WriteLine("End");
            goal?.Show();
            List<int> path = goal?.UpdatePath(graph);
            Debug.Write("Path: ");
            path?.ForEach(nodeIndex => Debug.Write($"({nodeIndex}:{nodeList[nodeIndex].NodeText})  "));
            Debug.WriteLine($"\nTotal time: {watch.ElapsedMilliseconds}ms");

            return path;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="graph"></param>
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <exception cref="Exception"></exception>
        private void ConvertToNumbers(string s, out int a, out int b)
        {
            var args = s.Split(' ');
            if (!Int32.TryParse(args[0], out a) || !Int32.TryParse(args[1], out b))
            {
                throw new Exception("can not convert string to int");
            }
        }
    }

}
