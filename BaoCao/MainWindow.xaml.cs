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
        private MediaElement _media;


        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _media = new MediaElement() { Width = 0, Height = 0 };
            _media.Source = new Uri(System.IO.Path.Combine(Constants.MusicPath, "Eyes.mp3"), UriKind.Relative);
            _media.LoadedBehavior = MediaState.Manual;
            _media.UnloadedBehavior = MediaState.Stop;

            _media.MediaEnded += (sender, e) =>
            {
                _media.Position = TimeSpan.Zero;
                _media.Play();
            };

            _stackPanel.Children.Add(_media);
            _media.Play();

            this.Closing += Exit_MainWindowClosingEvent;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_MainWindowClosingEvent(object sender, System.ComponentModel.CancelEventArgs e)
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
        private void Exit_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// open setting window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Setting_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            SettingWindow settingWindow = new SettingWindow(_media);
            settingWindow.ShowDialog();
        }


        /// <summary>
        /// open new game window --- open PlayBoardWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            this.Hide();

            LevelWindow levelWindow = new LevelWindow();
            levelWindow.ShowDialog();
            //MessageBox.Show(levelWindow.LevelFilePath);

            if (System.IO.File.Exists(levelWindow.LevelFilePath))
            {
                PlayBoardWindow playBoardWindow = new PlayBoardWindow(
                        isDesignMode: false,
                        levelFilePath: levelWindow.LevelFilePath);
                playBoardWindow.ShowDialog();
            }

            this.Show();
        }


        /// <summary>
        /// design game board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Design_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            this.Hide();

            PlayBoardWindow playBoardWindow = new PlayBoardWindow(isDesignMode: true);
            playBoardWindow.ShowDialog();

            this.Show();
        }
    }
}
