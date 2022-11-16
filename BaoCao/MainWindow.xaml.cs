﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BaoCao
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SettingWindow _stWindow = null;
        private MediaElement _media = null;


        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            _media = new MediaElement();
            _media.Source = new Uri("nhacnen2.mp3", UriKind.Relative);
            _media.LoadedBehavior = MediaState.Manual;
            _media.UnloadedBehavior = MediaState.Stop;
            _media.MediaEnded += (sender, e) =>
            {
                _media.Position = TimeSpan.Zero;
                _media.Play();
            };

            InitializeComponent();

            _stackPanel.Children.Add(_media);
            //_media.Play();


            this.Closing += Exit_MainWindowClosing;        
        }
               

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_MainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //MessageBoxResult r =  MessageBox.Show("Ban co muon thoat khoi chuong trinh?", "...", MessageBoxButton.YesNo);
            //if (r == MessageBoxResult.No)
            //{
            //    e.Cancel = true;
            //}
        }


        /// <summary>
        /// exit window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// open setting window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Setting_ButtonClick(object sender, RoutedEventArgs e)
        {
            
            _stWindow = new SettingWindow();
            _stWindow.ShowDialog();
            
        }


        /// <summary>
        /// open new game window --- open PlayBoardWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Hide();

            PlayBoardWindow playBoardWindow = new PlayBoardWindow(isDesignMode: false);
            playBoardWindow.ShowDialog();

            this.Show();
        }


        /// <summary>
        /// design game board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Design_ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Hide();

            PlayBoardWindow playBoardWindow = new PlayBoardWindow(isDesignMode: true);
            playBoardWindow.ShowDialog();

            this.Show();
        }
    }
}
