using System;
using System.Windows.Media.Imaging;

namespace ImageResizerApp
{
    public class ImageDetail
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Rotation { get; set; }
        public string ImagePath { get; set; }

        public BitmapImage ImageSource
        {
            get
            {
                if (!string.IsNullOrEmpty(ImagePath))
                {
                    return new BitmapImage(new Uri(ImagePath));
                }
                return null;
            }
        }
    }
}
