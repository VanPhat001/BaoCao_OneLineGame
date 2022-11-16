using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BaoCao
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        private MediaElement _media;

        /// <summary>
        /// 
        /// </summary>
        public SettingWindow(MediaElement media)
        {
            _media = media; // !IMPORTANT: không được xóa hay dời dòng này
            InitializeComponent();

            _chbxMusic.IsChecked = Tool.GetMediaState(_media) == MediaState.Play;            

            // _media.Volume <=> _slider.Value
            _slider.SetBinding(Slider.ValueProperty,
                new System.Windows.Data.Binding(nameof(_media.Volume))
                {
                    Source = _media,
                    UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
                });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            System.Windows.Data.BindingOperations.ClearBinding(_slider, Slider.ValueProperty);
            System.Windows.Data.BindingOperations.ClearBinding(_media, MediaElement.VolumeProperty);
            this.Close();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Music_CheckboxCheckedEvent(object sender, RoutedEventArgs e)
        {
            _media.Position = System.TimeSpan.Zero;
            _media.Play();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Music_CheckboxUnCheckedEvent(object sender, RoutedEventArgs e)
        {
            _media.Stop();            
        }
    }
}
