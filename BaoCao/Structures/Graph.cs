using System.Collections.Generic;

namespace BaoCao.Structures
{
    public class Graph
    {
        private List<List<bool>> _matrix;
        private List<Edge> _edgeList;

        public int N { get; private set; }
        public int M { get; private set; }
        public List<Edge> EdgeList { get => _edgeList; private set => _edgeList = value; }
        public List<List<bool>> Matrix { get => _matrix; private set => _matrix = value; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        public Graph(int n)
        {
            N = n;
            M = 0;
            EdgeList = new List<Edge>();
            Matrix = new List<List<bool>>();
            for (int i = 0; i < n + 1; i++)
            {
                Matrix.Add(new List<bool>());
                for (int j = 0; j < n + 1; j++)
                {
                    Matrix[i].Add(false);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public bool AddEdge(int u, int v)
        {
            if (!Adjacent(u, v))
            {
                _matrix[u][v] = true;
                _matrix[v][u] = true;
                M++;
                EdgeList.Add(new Edge(u, v));
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public bool Adjacent(int u, int v)
        {
            return _matrix[u][v];
        }
    }
}
