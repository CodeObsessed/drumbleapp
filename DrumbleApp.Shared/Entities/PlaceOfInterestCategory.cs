using DrumbleApp.Shared.Infrastructure.Helpers;
using System;
using System.Windows.Media.Imaging;

namespace DrumbleApp.Shared.Entities
{
    public sealed class PlaceOfInterestCategory : Imagable
    {
        public Guid Id { get; private set; }
        public string Category { get; private set; }
        public bool IsChecked { get; set; }
        public byte[] ImageBinary { get; private set; }
        
        public BitmapImage Image
        {
            get
            {
                return GetImage(this.ImageBinary);
            }
        }


        public PlaceOfInterestCategory(Guid id, string category, string base64EncodeImage, bool isChecked)
            : this(id, category, Convert.FromBase64String(base64EncodeImage), isChecked)
        {
        }

        public PlaceOfInterestCategory(Guid id, string category, byte[] base64EncodeImage, bool isChecked)
        {
            this.Id = id;
            this.Category = category;
            this.ImageBinary = base64EncodeImage;
            this.IsChecked = isChecked;
        }
    }
}
