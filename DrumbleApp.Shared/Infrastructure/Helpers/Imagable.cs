using System.IO;
using System.Windows.Media.Imaging;

namespace DrumbleApp.Shared.Infrastructure.Helpers
{
    public abstract class Imagable
    {
        protected static BitmapImage GetImage(byte[] imageBinary)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream ms = new MemoryStream(imageBinary))
            {
                bitmapImage.SetSource(ms);
            }
            bitmapImage.CreateOptions = BitmapCreateOptions.DelayCreation;

            return bitmapImage;
        }
    }
}
