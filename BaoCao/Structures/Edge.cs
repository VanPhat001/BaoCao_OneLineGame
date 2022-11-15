using System;
using System.Collections.Generic;

namespace BaoCao.Structures
{
    public class Edge
    {
        public int U { get; set; }
        public int V { get; set; }

        public Edge(int u, int v)
        {
            U = Math.Min(u, v);
            V = Math.Max(u, v);
        }

        public override string ToString()
        {
            return $"({U},{V})";
        }
    }

    class EdgeComparer : IEqualityComparer<Edge>
    {
        public bool Equals(Edge x, Edge y)
        {
            return x.ToString() == y.ToString();
        }

        public int GetHashCode(Edge obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
