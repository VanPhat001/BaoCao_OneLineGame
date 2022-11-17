using System;
using System.Collections.Generic;
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
    /// Interaction logic for HelpBoard.xaml
    /// </summary>
    public partial class HelpBoard : Window
    {
        public HelpBoard()
        {
            InitializeComponent();
        }

        private void help_back_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
