using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Helpers;
using System;
using System.Windows.Media.Imaging;

namespace DrumbleApp.Shared.Entities
{
    public sealed class SearchItemCategory : Imagable
    {
        public Guid Id { get; private set; }
        public string Text { get; private set; }
        public byte[] ImageBinary { get; private set; }
        public SearchCategory SearchCategory { get; private set; }

        public BitmapImage Image
        {
            get
            {
                return GetImage(this.ImageBinary);
            }
        }


        public SearchItemCategory(Guid id, string base64EncodeImage, string text)
            :this(id, Convert.FromBase64String(base64EncodeImage), text)
        {

        }

        public SearchItemCategory(Guid id, byte[] base64EncodeImage, string text)
        {
            switch (text.ToLower())
            {
                case "stop":
                    this.SearchCategory = Enums.SearchCategory.Stop;
                    break;
                case "address":
                    this.SearchCategory = Enums.SearchCategory.Address;
                    break;
                case "point of interest":
                    this.SearchCategory = Enums.SearchCategory.PlaceOfInterest;
                    break;
            }
            this.Text = text;
            this.Id = id;
            this.ImageBinary = base64EncodeImage;
        }
    }
}
