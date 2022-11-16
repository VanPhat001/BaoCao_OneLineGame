using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BaoCao
{
    /// <summary>
    /// Interaction logic for LevelWindow.xaml
    /// </summary>
    public partial class LevelWindow : Window
    {
        public string LevelFilePath { get; private set; }

        public LevelWindow()
        {
            InitializeComponent();

            _stackPnlLevels.Children.Clear();
            LevelFilePath = "";
            var directoryInfo = new DirectoryInfo(Constants.LevelPath);
            var infoList = directoryInfo.GetFiles();
            //MessageBox.Show(infoList.Length.ToString());

            int counter = 0;
            foreach (var info in infoList)
            {
                string path = info.FullName;
                string fileName = info.Name;
                //MessageBox.Show(path);
                //MessageBox.Show(fileName);

                Button btn = new Button()
                {
                    Content = $"Level {++counter}",
                    Tag = path
                };
                btn.Click += (sender, e) =>
                {
                    LevelFilePath = btn.Tag.ToString();
                    this.Close();
                };
                _stackPnlLevels.Children.Add(btn);
            }
        }
    }
}
