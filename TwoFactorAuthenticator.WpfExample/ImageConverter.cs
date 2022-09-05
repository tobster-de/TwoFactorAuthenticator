using System.IO;
using System.Windows.Media.Imaging;

namespace WpfExample
{
    public static class ImageConverter
    {
        /// <remarks>
        /// taken from https://stackoverflow.com/a/9564425/2131959
        /// </remarks>
        public static BitmapImage CreateImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
            {
                return null;
            }
            
            var image = new BitmapImage();
            using (var ms = new MemoryStream(imageData))
            {
                ms.Seek(0, SeekOrigin.Begin);
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = ms;
                image.EndInit();
            }
            
            image.Freeze();
            return image;
        }        
    }
}