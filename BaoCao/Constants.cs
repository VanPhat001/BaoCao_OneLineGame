using System;
using System.IO;
using System.Windows.Media;

namespace BaoCao
{
    static public class Constants
    {
        public static readonly string RootPath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string LevelPath = Path.Combine(RootPath, "Resources", "Levels");
        public static readonly string MusicPath = Path.Combine(RootPath, "Resources", "Musics");


        public static readonly double NodeSize = 50;
        public static readonly double NodeFontSize = 16;
        public static readonly Brush BoardBackgroundColor = Tool.ColorRGB(188, 184, 177);


        public static readonly Brush NodeBackgroundColorSelect = Tool.ColorRGB(0,175,185);
        public static readonly Brush EdgeColorPlay = Brushes.Blue;



        public static readonly Brush NodeBackgroundColorDefault = Brushes.OrangeRed;
        public static readonly Brush EdgeColorDefault = Brushes.Black;
        public static readonly double EdgeThickness = 8;
    }
}
