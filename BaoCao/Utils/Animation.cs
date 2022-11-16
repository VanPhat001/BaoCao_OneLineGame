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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="gap"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public async Task ShowAnimationTask(int gap=50, int delay=5)
        {
            Canvas parent = _edgeList[0].Parent;
            for (int nodeIndex = 1; nodeIndex < _path.Count; nodeIndex++)
            {
                var uNode = _nodeList[_path[nodeIndex - 1]];
                var vNode = _nodeList[_path[nodeIndex]];
                //var edge = _edgeList.Find(e => ((e.UNode == uNode && e.VNode == vNode) || (e.UNode == vNode && e.VNode == uNode)));
                //edge.EdgeColor = Constants.EdgeColorPlay;
                //await Task.Delay(400);

                var edge = await DrawLineTask(uNode, vNode, parent, gap, delay);
                _newEdge.Add(edge);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void ClearAnimation()
        {
            _newEdge.ForEach(item => item.RemoveParent());
            _newEdge.Clear();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="uNode"></param>
        /// <param name="vNode"></param>
        /// <param name="parent"></param>
        /// <param name="gap"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private async Task<Edge> DrawLineTask(Node uNode, Node vNode, Canvas parent, int gap, int delay)
        {
            Edge edge = new Edge(parent, uNode);
            edge.EdgeColor = Constants.EdgeColorPlay;
            var curPoint = uNode.GetCenterLocation(); // curPoint === startPoint
            var endPoint = vNode.GetCenterLocation();
            var delta = new Point(
                x: (endPoint.X - curPoint.X) / gap,
                y: (endPoint.Y - curPoint.Y) / gap);

            for (int i = 1; i <= gap; i++)
            {
                curPoint.X += delta.X;
                curPoint.Y += delta.Y;
                edge.SetEndPoint(curPoint);
                await Task.Delay(delay);
            }

            return edge;
        }
    }
}
