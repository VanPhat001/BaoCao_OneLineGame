using BaoCao.GeneticAlgorithm;
using BaoCao.Objects;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


/** TODO:
 * [x] chọn nút ===> click nút
 * [x] vẽ nút ===> ctrl + click chuột trên canvas
 * [x] di chuyển nút ===> alt + click chuột trên nút + di chuột
 * [x] xóa nút ==> chọn nút + del
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
        #region fields
        private bool _isDesignMode;
        private Node _selectedNode;
        private Edge _selectedEdge;
        private List<Node> _nodeList;
        private List<Edge> _edgeList; // danh sách Edge chứa các cung trong chế độ design và chế độ cung cố định trong chế độ chơi game
        private List<Edge> _edgeGameDrawList; // danh sách Edge chứa các cung được vẽ trong chế độ chơi game
        private List<int> _path; // luu tru duong di theo node index
        private Stack<int> _history; // lưu trữ lịch sử đường đi khi dùng tính năng Next hoặc Prev
        #endregion


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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="isDesignMode"></param>
        public PlayBoardWindow(bool isDesignMode, string levelFilePath = "")
        {
            InitializeComponent();

            _isDesignMode = isDesignMode;
            _path = new List<int>();
            _history = new Stack<int>();
            _canvasGameBoard.Background = Constants.BoardBackgroundColor;


            _nodeList = new List<Node>();
            _edgeList = new List<Edge>();
            _edgeGameDrawList = new List<Edge>();
            SelectedNode = null;
            SelectedEdge = null;

            _canvasGameBoard.MouseDown += CanvasGameBoard_MouseDownEvent;
            _canvasGameBoard.MouseMove += CanvasGameBoard_MouseMoveEvent;

            if (isDesignMode) // che do thiet ke man choi
            {
                InitDesignMode();
            }
            else // isDesignMode == false ---> che do choi game
            {
                InitGameMode(levelFilePath);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasGameBoard_MouseDownEvent(object sender, MouseButtonEventArgs e)
        {
            if (IsDesignMode && IsLeftCtrlDown && IsMouseLeftButtonDown)
            {
                var pos = e.GetPosition(_canvasGameBoard);
                var node = CreateNode(pos.X, pos.Y);
                _nodeList.Add(node);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasGameBoard_MouseMoveEvent(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(_canvasGameBoard);

            // di chuyển node
            if (IsDesignMode && SelectedNode != null && IsLeftAltDown
                && SelectedNode.CheckPointIn(pos)
                && IsMouseLeftButtonDown)
            {
                SelectedNode.SetCenterLocation(pos);
            }

            // vẽ cung :))
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
                        // tạo và liên kết edge(uNode, vNode) thành công
                        Node uNode = SelectedEdge.UNode;
                        Node vNode = node;

                        SelectedEdge.RemoveParent();
                        SelectedEdge = CreateEdge(uNode, vNode);

                        if (SelectedEdge != null)
                        {
                            if (IsDesignMode)
                            {
                                _edgeList.Add(SelectedEdge);
                            }
                            else // IsDesignMode==false ===> gameplay
                            {
                                // tạo cung thành công (đã qua ktra điều kiện)
                                //  - thêm cung đó vào danh sách 
                                //  - xóa lịch sử prev, next trong _history
                                SelectedEdge.EdgeColor = Constants.EdgeColorPlay;
                                _edgeGameDrawList.Add(SelectedEdge);
                                _history.Clear();
                            }
                            SelectedEdge = null;
                        }

                        IsAccept = true;
                        break;
                    }
                }
                // hủy cung do không tìm được node để liên kết edge(uNode, null)
                if (!IsAccept)
                {
                    SelectedEdge.RemoveParent();
                    SelectedEdge = null;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Home_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Prev_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            if (_path.Count == 0)
            {
                return;
            }
            else // _path.Count >= 2  <=> _edgeGameDraw.Count >= 1
            {
                Edge edge = _edgeGameDrawList[_edgeGameDrawList.Count - 1];
                //Node uNode = edge.UNode;
                Node vNode = edge.VNode;

                edge.RemoveParent();

                _path.RemoveAt(_path.Count - 1);
                _edgeGameDrawList.RemoveAt(_edgeGameDrawList.Count - 1);
                _history.Push((int)vNode.Tag);

                if (_path.Count == 1) // đường đi chỉ có 1 đỉnh không tạo thành cung ---> trạng thái đầu (prev max)
                {
                    _history.Push(_path[0]); // thêm vào để xử lí tình huống prev max sau đó chọn next
                    _path.Clear();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Next_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            // TODO: nhớ xóa lịch sử trước khi mọi điều tồi tệ hơn 🙂
            // phải có lịch sử mới xóa (triển khai) được =)
            if (_history.Count > 0)
            {
                int uNodeIndex = _path.Count - 1 >= 0 ? _path[_path.Count - 1] : _history.Pop();
                int vNodeIndex = _history.Pop();
                //int uNodeIndex, vNodeIndex;
                //if (_path.Count - 1 >= 0) // các trạng thái sau
                //{
                //    uNodeIndex = _path[_path.Count - 1];
                //    vNodeIndex = _history.Pop();
                //}
                //else // trạng thái đầu game
                //{
                //    uNodeIndex = _history.Pop();
                //    vNodeIndex = _history.Pop();
                //}

                Node uNode = _nodeList[uNodeIndex];
                Node vNode = _nodeList[vNodeIndex];

                // bên trong hàm CreateEdge() có gọi hàm _path.Add() để thêm vNodeIndex vào _path
                Edge edge = CreateEdge(uNode, vNode);
                edge.EdgeColor = Constants.EdgeColorPlay;
                _edgeGameDrawList.Add(edge);
                // như giải thích ở trên, xóa dòng này hoặc là giữ nguyên dòn này nhưng xóa phần tử cuối của _path trước khi Add
                //_path.Add(vNodeIndex);   
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reset_ButtonEventClick(object sender, RoutedEventArgs e)
        {
            for (int i = _edgeGameDrawList.Count - 1; i >= 0; i--)
            {
                _edgeGameDrawList[i].RemoveParent();
            }
            _edgeGameDrawList.Clear();
            _path.Clear();
            _history.Clear();
        }


        /// <summary>
        /// Solve and show animation 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Solve_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            _edgeGameDrawList.ForEach(item => item.RemoveParent());
            _edgeGameDrawList.Clear();
            _path.Clear();
            _history.Clear();
            await System.Threading.Tasks.Task.Delay(0);

            // !FIXME: giao diện bị block bởi ảnh hưởng của dòng while true bên trong hàm ExecuteAsync(). Đặt await thì giảm tốc độ, không đặt thì bị block 🙄🙃
            GAExcuter gaExcuter = new GAExcuter();
            var path = await gaExcuter.ExcuteAsync(_nodeList, _edgeList);

            BaoCao.Utils.Animation animation = new Utils.Animation(_nodeList, _edgeList, path);
            await animation.ShowAnimationAsync();

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
             *      + cung đầu tiên luôn hợp lệ (phải thõa mãn có cung (uNode, vNode))
             */
            Edge edge;
            if (IsDesignMode)
            {
                edge = new Edge(_canvasGameBoard, u, v);
                edge.EdgeRemove += (sender1, e1) => _edgeList.Remove(sender1 as Edge);
                Debug.WriteLine($"tao edge: ({u.GetCenterLocation()},{u.NodeText})   ({v.GetCenterLocation()},{v.NodeText})");
                return edge;
            }


            // TODO: xử lí dữ kiện đầu bài về chế độ gameplay
            #region IsDesignMode==false // GamePlay process
            int uNodeIndex = (int)u.Tag;
            int vNodeIndex = (int)v.Tag;
            bool canCreateEdge = true;
            bool foundEdgeInMap = false;

            #region kiểm tra cung có trên bản đồ gốc
            foreach (Edge edgeItem in _edgeList)
            {
                int uItemIndex = (int)edgeItem.UNode.Tag;
                int vItemIndex = (int)edgeItem.VNode.Tag;
                if ((uItemIndex == uNodeIndex && vItemIndex == vNodeIndex) || (uItemIndex == vNodeIndex && vItemIndex == uNodeIndex))
                {
                    foundEdgeInMap = true;
                    break;
                }
            }
            #endregion

            if (!foundEdgeInMap) // không có trên bản đồ gốc --> không thể vẽ
            {
                canCreateEdge = false;
            }
            else if (_path.Count == 0) // cung đầu tiên luôn hợp lệ và tồn tại cung trên bản đồ gốc (đã xét ở trên)
            {
                _path.Add(uNodeIndex);
                //_path.Add(vNodeIndex);
                // ở khúc cuối có add con vNodeIndex rồi, đừng mở comment :)
            }
            else // _path.Count != 0
            {
                int back = _path[_path.Count - 1];

                #region kiểm tra vẽ tiếp
                if (back != uNodeIndex)
                {
                    canCreateEdge = false;
                }
                #endregion

                #region kiểm tra quay đầu
                if (back == vNodeIndex)
                {
                    canCreateEdge = false;
                }
                #endregion

                #region cung tồn tại trong màn chơi (đã xét ở trên) và chưa đi qua
                for (int i = 1; i < _path.Count; i++)
                {
                    int p1 = _path[i];
                    int p2 = _path[i - 1];
                    if ((p1 == uNodeIndex && p2 == vNodeIndex) || (p1 == vNodeIndex && p2 == uNodeIndex))
                    {
                        canCreateEdge = false;
                        break;
                    }
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
                    MessageBox.Show("Bạn thắng rồi đó! Cừ cái xem nào 😎");
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


        /// <summary>
        /// 
        /// </summary>
        private void ReadGraphFeature(string filePath)
        {
            #region xóa dữ liệu node và edge cũ
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


        /// <summary>
        /// 
        /// </summary>
        private void InitDesignMode()
        {
            // TODO: chinh sua giao dien, them nut export vao giao dien
            Button btnExport = new Button()
            {
                Content = "Export",
                Height = 45,
                Width = 120,
                FontFamily = new FontFamily("Mistral"),
                FontWeight = FontWeight.FromOpenTypeWeight(500),
                Style = Application.Current.FindResource("ToolBarButtonStyle") as Style
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
                Height = 45,
                Width = 120,
                FontFamily = new FontFamily("Mistral"),
                FontWeight = FontWeight.FromOpenTypeWeight(500),
                Style = Application.Current.FindResource("ToolBarButtonStyle") as Style
            };
            btnRead.Click += (sender, e) =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    ReadGraphFeature(openFileDialog.FileName);
                }
            };

            // TODO: chinh sua giao dien, them nut home vao giao dien
            Button btnHome = new Button()
            {
                Content = "Home",
                Height = 45,
                Width = 120,
                FontFamily = new FontFamily("Mistral"),
                FontWeight = FontWeight.FromOpenTypeWeight(500),
                Style = Application.Current.FindResource("ToolBarButtonStyle") as Style
            };
            btnHome.Click += Home_ButtonClickEvent;

            // TODO: chinh sua giao dien, them nut clear vao giao dien
            Button btnClear = new Button()
            {
                Content = "Clear",
                Height = 45,
                Width = 120,
                FontFamily = new FontFamily("Mistral"),
                FontWeight = FontWeight.FromOpenTypeWeight(500),
                Style = Application.Current.FindResource("ToolBarButtonStyle") as Style
            };
            btnClear.Click += async (sender, e) =>
            {
                for (int i = _edgeList.Count - 1; i >= 0; i--)
                {
                    _edgeList[i].RemoveParent();
                    await System.Threading.Tasks.Task.Delay(20);
                }
                _edgeList.Clear();

                for (int i = _nodeList.Count - 1; i >= 0; i--)
                {
                    _nodeList[i].RemoveParent();
                    await System.Threading.Tasks.Task.Delay(20);
                }
                _nodeList.Clear();
            };

            _toolBar.Items.Clear();
            _toolBar.Items.Add(btnHome);
            _toolBar.Items.Add(btnExport);
            _toolBar.Items.Add(btnRead);
            _toolBar.Items.Add(btnClear);
        }


        /// <summary>
        /// 
        /// </summary>
        private void InitGameMode(string levelFilePath)
        {
            // đường dẫn luôn đúng, không cần kiểm tra
            //bool exists = System.IO.File.Exists(fileLevelPath);
            //if (!exists)
            //{
            //    MessageBox.Show("Đường dẫn không hợp lệ!");
            //    return;
            //}

            ReadGraphFeature(levelFilePath);
            // Ngăn chặn việc xóa node, edge trong lúc chơi
            _nodeList.ForEach(node => node.RemoveMenu());
            _edgeList.ForEach(edge => edge.RemoveMenu());
            _nodeList.ForEach(item => item.NodeText = item.Tag.ToString());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HelpBoard helpBoard = new HelpBoard();
            helpBoard.ShowDialog();
        }
    }
}
