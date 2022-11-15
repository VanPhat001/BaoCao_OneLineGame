using BaoCao.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BaoCao.Utils
{
    public class Animation
    {
        private readonly List<Node> _nodeList;
        private readonly List<Edge> _edgeList;
        private readonly List<int> _path;
        private List<Edge> _newEdge;

        public Animation(List<Node> nodeList, List<Edge> edgeList, List<int> path)
        {
            _nodeList = nodeList;
            _edgeList = edgeList;
            _path = path;
            _newEdge = new List<Edge>();
        }

        public async Task ShowAnimation()
        {
            Canvas parent = _edgeList[0].Parent;
            for (int nodeIndex = 1; nodeIndex < _path.Count; nodeIndex++)
            {
                var uNode = _nodeList[_path[nodeIndex - 1]];
                var vNode = _nodeList[_path[nodeIndex]];
                //var edge = _edgeList.Find(e => ((e.UNode == uNode && e.VNode == vNode) || (e.UNode == vNode && e.VNode == uNode)));
                //edge.EdgeColor = Constants.EdgeColorPlay;
                //await Task.Delay(400);

                var edge = await DrawLine(uNode, vNode, parent);
                _newEdge.Add(edge);
            }
        }

        public void ClearAnimation()
        {
            _newEdge.ForEach(item => item.RemoveParent());
            _newEdge.Clear();
        }

        private async Task<Edge> DrawLine(Node uNode, Node vNode, Canvas parent)
        {
            Edge edge = new Edge(parent, uNode);
            edge.EdgeColor = Constants.EdgeColorPlay;
            var curPoint = uNode.GetCenterLocation(); // curPoint === startPoint
            var endPoint = vNode.GetCenterLocation();
            int gap = 50;
            var delta = new Point(
                x: (endPoint.X - curPoint.X) / gap,
                y: (endPoint.Y - curPoint.Y) / gap);

            for (int i = 1; i <= gap; i++)
            {
                curPoint.X += delta.X;
                curPoint.Y += delta.Y;
                edge.SetEndPoint(curPoint);
                await Task.Delay(5);
            }

            return edge;
        }
    }
}
