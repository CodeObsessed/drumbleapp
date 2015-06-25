using GalaSoft.MvvmLight;
using System;
using System.Data.Linq.Mapping;

namespace DrumbleApp.Shared.Data.Models
{
    [Table]
    public sealed class SearchItemCategory : ViewModelBase
    {
        private Guid id;
        [Column(IsPrimaryKey = true, IsDbGenerated = false, CanBeNull = false)]
        public Guid Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    RaisePropertyChanging("Id");
                    id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }

        private string text;
        [Column]
        public string Text
        {
            get { return text; }
            set
            {
                if (text != value)
                {
                    RaisePropertyChanging("Text");
                    text = value;
                    RaisePropertyChanged("Text");
                }
            }
        }

        private byte[] imageBinary;
        [Column(DbType = "image", UpdateCheck = UpdateCheck.Never)]
        public byte[] ImageBinary
        {
            get { return imageBinary; }
            set
            {
                if (this.imageBinary != value)
                {
                    RaisePropertyChanging("ImageBinary");
                    this.imageBinary = value;
                    RaisePropertyChanged("ImageBinary");
                }
            }
        }

        public SearchItemCategory() { }

        public SearchItemCategory(Guid id, byte[] base64EncodeImage, string text)
        {
            this.Text = text;
            this.Id = id;
            this.ImageBinary = base64EncodeImage;
        }
    }
}
