using BaoCao.GeneticAlgorithm;
using BaoCao.Objects;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


/**
 * [x] chọn nút ===> click nút
 * [x] vẽ nút ===> ctrl + click chuột trên canvas
 * [x] di chuyển nút ===> alt + click chuột trên nút + di chuột
 * [x]xóa nút ==> chọn nút + del
 * [x] xóa cung
 * [x] vẽ cung ===> (shift + click trên nút) (di chuột + đè shift + nút chưa được thả)
 * [x] hủy cung ===> vẽ cung --> thả chuột không trên nút nào
 * [x] xóa cung, xóa node ---> xóa nó ra khỏi list
 * 
 * [x] xuất dữ liệu ra file
 * [x] đọc dữ liệu từ file
 * [x] khóa xóa nút trên chế độ chơi
 * [x] khóa xóa cung trên chế độ chơi
 * 
 */

namespace BaoCao
{
    /// <summary>
    /// Interaction logic for PlayBoardWindow.xaml
    /// </summary>
    public partial class PlayBoardWindow : Window
    {
        private bool _isDesignMode;
        private Node _selectedNode;
        private Edge _selectedEdge;
        private List<Node> _nodeList;
        private List<Edge> _edgeList;
        private List<int> _path; // luu tru duong di theo node index


        #region properties
        public Node SelectedNode
        {
            get => _selectedNode;
            private set
            {
                _selectedNode?.Default();
                _selectedNode?.ClearFocus();
                _selectedNode = value;
                _selectedNode?.Select();
                _selectedNode?.SetFocus();
            }
        }
        public Edge SelectedEdge
        {
            get => _selectedEdge;
            private set => _selectedEdge = value;
        }

        public bool IsDesignMode { get => _isDesignMode; set => _isDesignMode = value; }
        public bool IsLeftCtrlDown => Keyboard.IsKeyDown(Key.LeftCtrl);
        public bool IsMouseLeftButtonDown => Mouse.LeftButton == MouseButtonState.Pressed;
        public bool IsLeftAltDown => Keyboard.IsKeyDown(Key.LeftAlt);
        public bool IsLeftShiftDown => Keyboard.IsKeyDown(Key.LeftShift);
        public bool IsDelKeyDown => Keyboard.IsKeyDown(Key.Delete);
        #endregion



        public PlayBoardWindow(bool isDesignMode)
        {
            InitializeComponent();

            _isDesignMode = isDesignMode;
            _path = new List<int>();
            _canvasGameBoard.Background = Constants.BoardBackgroundColor;


            _nodeList = new List<Node>();
            _edgeList = new List<Edge>();
            SelectedNode = null;
            SelectedEdge = null;

            _canvasGameBoard.MouseDown += CanvasGameBoard_MouseDownEvent;
            _canvasGameBoard.MouseMove += CanvasGameBoard_MouseMoveEvent;

            if (isDesignMode) // che do thiet ke man choi
            {
                // TODO: chinh sua giao dien, them nut export vao giao dien
                Button btnExport = new Button()
                {
                    Content = "Export",
                    Margin = new Thickness(0),
                    Height = 45,
                    Width = 120,
                    FontSize = 24,
                    FontFamily = new FontFamily("Mistral"),
                    FontWeight = FontWeight.FromOpenTypeWeight(500),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                btnExport.Click += (sender, e) =>
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string filePath = saveFileDialog.FileName;
                        Tool.ExportGraphToFile(filePath, _nodeList, _edgeList);
                    }
                };

                // TODO: chinh sua giao dien, them nut read vao giao dien
                Button btnRead = new Button()
                {
                    Content = "Read",
                    Margin = new Thickness(0),
                    Height = 45,
                    Width = 120,
                    FontSize = 24,
                    FontFamily = new FontFamily("Mistral"),
                    FontWeight = FontWeight.FromOpenTypeWeight(500),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                btnRead.Click += (sender, e) =>
                {
                    ReadGraphFeature();
                    
                };
                _toolBar.Items.Clear();
                _toolBar.Items.Add(btnExport);
                _toolBar.Items.Add(btnRead);
            }
            else // isDesignMode == false ---> che do choi game
            {
                ReadGraphFeature();
                // Ngăn chặn việc xóa node, edge trong lúc chơi
                _nodeList.ForEach(node => node.RemoveMenu());
                _edgeList.ForEach(edge => edge.RemoveMenu());
            }
        }

        private void CanvasGameBoard_MouseDownEvent(object sender, MouseButtonEventArgs e)
        {
            if (IsLeftCtrlDown && IsMouseLeftButtonDown)
            {
                var pos = e.GetPosition(_canvasGameBoard);
                var node = CreateNode(pos.X, pos.Y);
                _nodeList.Add(node);
            }
        }

        private void CanvasGameBoard_MouseMoveEvent(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(_canvasGameBoard);

            // doi node
            if (IsDesignMode && SelectedNode != null && IsLeftAltDown
                && SelectedNode.CheckPointIn(pos)
                && IsMouseLeftButtonDown)
            {
                SelectedNode.SetCenterLocation(pos);
            }

            // ve cung :))
            if (SelectedEdge != null && IsLeftShiftDown && IsMouseLeftButtonDown)
            {
                SelectedEdge.SetEndPoint(pos);
            }
            else if (SelectedEdge != null)
            {
                bool IsAccept = false;
                foreach (var node in _nodeList)
                {
                    if (node.CheckPointIn(pos) && node != SelectedEdge.UNode)
                    {
                        // accept edge 
                        Node uNode = SelectedEdge.UNode;
                        Node vNode = node;

                        SelectedEdge.RemoveParent();
                        SelectedEdge = CreateEdge(uNode, vNode);

                        if (SelectedEdge != null)
                        {
                            SelectedEdge.EdgeColor = Constants.EdgeColorPlay;
                            if (IsDesignMode)
                            {
                                _edgeList.Add(SelectedEdge);
                            }
                            SelectedEdge = null;
                        }

                        IsAccept = true;
                        break;
                    }
                }
                // huy cung
                if (!IsAccept)
                {
                    SelectedEdge.RemoveParent();
                    SelectedEdge = null;
                }
            }
        }

        private void Node_ClickEvent(object sender, MouseButtonEventArgs e)
        {
            SelectedNode = sender as Node;
            e.Handled = true;
            // bắt đầu vẽ cung
            if (IsLeftShiftDown)
            {
                Edge edge = new Edge(_canvasGameBoard, SelectedNode);
                edge.SetEndPoint(SelectedNode.GetCenterLocation()); // !IMPORTANT : khong duoc xoa dong nay
                edge.EdgeColor = Constants.EdgeColorPlay;
                SelectedEdge = edge;
            }
        }


        /// <summary>
        /// Solve and show animation 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Solve_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            GAExcuter gaExcuter = new GAExcuter();
            var path = gaExcuter.Excute(_nodeList, _edgeList);
            BaoCao.Utils.Animation animation = new Utils.Animation(_nodeList, _edgeList, path);
            await animation.ShowAnimation();
            MessageBox.Show("Complete");
            animation.ClearAnimation();
        }


        /// <summary>
        /// tạo node mới có tọa độ (x,y) và hiển thị trên _canvasGameBoard
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Node CreateNode(double x, double y)
        {
            Node node = new Node();
            node.SetParent(_canvasGameBoard);
            node.SetCenterLocation(new Point(x, y));


            node.Click += Node_ClickEvent;
            if (IsDesignMode)
            {
                node.NodeRemove += (sender, e) =>
                {
                    Stack<int> edgeRemoveIndex = new Stack<int>();
                    for (int i = 0; i < _edgeList.Count; i++)
                    {
                        Edge edge = _edgeList[i];
                        if (edge.UNode == node || edge.VNode == node)
                        {
                            edgeRemoveIndex.Push(i);
                        }
                    }
                    while (edgeRemoveIndex.Count > 0)
                    {
                        _edgeList[edgeRemoveIndex.Pop()].RemoveParent();
                    }
                    _nodeList.Remove(node);
                };
            }

            Debug.WriteLine($"tao node: {x} {y}");
            return node;
        }


        /// <summary>
        /// tạo cung mới liên kết tọa độ với 2 đỉnh uNode và vNode, hiển thị trên _canvasGameBoard
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public Edge CreateEdge(Node u, Node v)
        {
            /** Ý tưởng:
             *  - Ở chế độ design: thoải mái đê =))
             *  - Ở chế độ gameplay: xem xét cạnh(uNode, vNode) có thể thêm vào danh sách đường đi _path hay không?
             *      + nếu path = {} thì cung đó không cần xét gì nữa
             *      + không được quay đầu
             *      + uNode phải khớp với _path.back()
             */
            Edge edge;
            if (IsDesignMode)
            {
                edge = new Edge(_canvasGameBoard, u, v);
                edge.EdgeRemove += (sender1, e1) => _edgeList.Remove(sender1 as Edge);
                Debug.WriteLine($"tao edge: ({u.GetCenterLocation()},{u.NodeText})   ({v.GetCenterLocation()},{v.NodeText})");
                return edge;
            }


            // TODO: xu li du kien dau bai ve viec gameplay
            #region IsDesignMode==false // GamePlay process
            int uNodeIndex = (int)u.Tag;
            int vNodeIndex = (int)v.Tag;
            bool canCreateEdge = true;
            //*cung dau tien luon hop le,
            if (_path.Count == 0)
            {
                _path.Add(uNodeIndex);
                //_path.Add(vNodeIndex);
                // ở khúc cuối có add con vNodeIndex rồi, đừng mở comment :)
            }
            else // _path.Count != 0
            {
                int back = _path[_path.Count - 1];

                #region kiem tra ve tiep
                if (back != uNodeIndex)
                {
                    canCreateEdge = false;
                    //return null;
                }
                #endregion

                #region kiem tra quay dau
                if (back == vNodeIndex)
                {
                    canCreateEdge = false;
                    //return null;
                }
                #endregion

                #region cung ton tai tren man choi va chua di qua
                bool found = false;
                foreach (Edge edgeItem in _edgeList)
                {
                    int uItemIndex = (int)edgeItem.UNode.Tag;
                    int vItemIndex = (int)edgeItem.VNode.Tag;
                    if ((uItemIndex == uNodeIndex && vItemIndex == vNodeIndex) || (uItemIndex == vNodeIndex && vItemIndex == uNodeIndex))
                    {
                        found = true;
                    }
                }
                //_edgeList.ForEach(edgeItem =>
                //{
                //    int uItemIndex = (int)edgeItem.UNode.Tag;
                //    int vItemIndex = (int)edgeItem.VNode.Tag;
                //    if ((uItemIndex == uNodeIndex && vItemIndex == vNodeIndex) || (uItemIndex == vNodeIndex && vItemIndex == uNodeIndex))
                //    {
                //        found = true;
                //    }
                //});
                if (found)
                {
                    for (int i = 1; i < _path.Count; i++)
                    {
                        int p1 = _path[i];
                        int p2 = _path[i - 1];
                        if ((p1 == uNodeIndex && p2 == vNodeIndex) || (p1 == vNodeIndex && p2 == uNodeIndex))
                        {
                            canCreateEdge = false;
                            //return null;
                        }
                    }
                }
                else
                {
                    canCreateEdge = false;
                    //return null;
                }
                #endregion

            }

            if (canCreateEdge)
            {
                _path.Add(vNodeIndex);
                edge = new Edge(_canvasGameBoard, u, v);
                Debug.WriteLine($"tao edge: ({u.GetCenterLocation()},{u.NodeText})   ({v.GetCenterLocation()},{v.NodeText})");
                if (_path.Count == _edgeList.Count + 1)
                {
                    MessageBox.Show("Bạn thắng rồi đó! Cừ cái xem nào =))");
                }
                return edge;
            }
            else
            {
                MessageBox.Show("Edge is not valid!!!");
                return null;
            }
            #endregion
        }

        private void ReadGraphFeature()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                #region xoa du lieu node va edge cu
                for (int i = _edgeList.Count - 1; i >= 0; i--)
                {
                    _edgeList[i].RemoveParent();
                }
                _edgeList.Clear();
                for (int i = _nodeList.Count - 1; i >= 0; i--)
                {
                    _nodeList[i].RemoveParent();
                }
                _nodeList.Clear();
                #endregion


                List<Node> tempNodes;
                List<Edge> tempEdges;
                bool OddDesignModeValue = IsDesignMode;
                IsDesignMode = true; // !IMPORTANT: không được xóa dòng này
                Tool.ReadGraphFromFile(filePath, out tempNodes, out tempEdges, CreateNode, CreateEdge);
                IsDesignMode = OddDesignModeValue; // !IMPORTANT: không được xóa dòng này
                _edgeList = tempEdges;
                _nodeList = tempNodes;
            }
        }
    }
}
