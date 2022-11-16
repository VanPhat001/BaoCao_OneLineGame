﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace BaoCao.Objects
{
    public class Edge
    {
        private Line _line;
        private Canvas _parent;
        private Node _uNode;
        private Node _vNode;


        #region edge remove (remove parent) event
        private event EventHandler _edgeRemove;
        public event EventHandler EdgeRemove { add => _edgeRemove += value; remove => _edgeRemove -= value; }
        private void OnEdgeRemove() => _edgeRemove?.Invoke(this, new EventArgs());
        #endregion


        public object Tag { get; set; }
        public Node UNode { get => _uNode; private set => _uNode = value; }
        public Node VNode { get => _vNode; private set => _vNode = value; }
        public Canvas Parent => _parent;
        public Point StartPoint => new Point(_line.X1, _line.Y1);
        public Point EndPoint => new Point(_line.X2, _line.Y2);
        public Brush EdgeColor { get => _line.Stroke; set => _line.Stroke = value; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="uNode"></param>
        /// <param name="vNode"></param>
        public Edge(Canvas parent, Node uNode = null, Node vNode = null)
        {
            //< Line X1 = "20" X2 = "150" Y1 = "13" Y2 = "200"
            //      Stroke = "Black" StrokeThickness = "2" />

            _line = new Line()
            {
                Stroke = Constants.EdgeColorDefault,
                StrokeThickness = Constants.EdgeThickness
            };
            SetUNode(uNode);
            SetVNode(vNode);
            _parent = null;
            if (parent != null)
            {
                AddParent(parent);
            }

            Canvas.SetZIndex(_line, -1);

            ContextMenu contextMenu = new ContextMenu();
            Button btn = new Button()
            {
                Content = "remove"
            };
            btn.Click += (sender, e) =>
            {
                RemoveParent();
                contextMenu.IsOpen = false;
            };
            contextMenu.Items.Add(btn);

            _line.ContextMenu = contextMenu;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        private void SetStartPoint(Point startPoint)
        {
            _line.X1 = startPoint.X;
            _line.Y1 = startPoint.Y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPoint"></param>
        public void SetEndPoint(Point endPoint)
        {
            _line.X2 = endPoint.X;
            _line.Y2 = endPoint.Y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SetUNode(Node value)
        {
            if (value == VNode && VNode != null) return;
            UNode = value;
            if (UNode == null) return;

            SetStartPoint(value.GetCenterLocation());
            UNode.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Location")
                {
                    SetStartPoint(UNode.GetCenterLocation());
                }
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SetVNode(Node value)
        {
            if (value == UNode && UNode != null) return;
            VNode = value;
            if (value == null) return;

            SetEndPoint(value.GetCenterLocation());
            VNode.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Location")
                {
                    SetEndPoint(VNode.GetCenterLocation());
                }
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        public void AddParent(Canvas canvas)
        {
            _parent = canvas;
            _parent.Children.Add(_line);
        }


        /// <summary>
        /// 
        /// </summary>
        public void RemoveParent()
        {
            if (_parent != null)
            {
                _parent.Children.Remove(_line);
                _parent = null;
                OnEdgeRemove();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void RemoveMenu()
        {
            _line.ContextMenu = null;
        }
    }
}
