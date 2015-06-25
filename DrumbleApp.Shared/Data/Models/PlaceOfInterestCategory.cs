using GalaSoft.MvvmLight;
using System;
using System.Data.Linq.Mapping;

namespace DrumbleApp.Shared.Data.Models
{
    [Table]
    public sealed class PlaceOfInterestCategory : ViewModelBase
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

        private string category;
        [Column]
        public string Category
        {
            get { return category; }
            set
            {
                if (category != value)
                {
                    RaisePropertyChanging("Category");
                    category = value;
                    RaisePropertyChanged("Category");
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

        private bool isChecked;
        [Column]
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (this.isChecked != value)
                {
                    RaisePropertyChanging("IsChecked");
                    this.isChecked = value;
                    RaisePropertyChanged("IsChecked");
                }
            }
        }

        public PlaceOfInterestCategory() { }

        public PlaceOfInterestCategory(Guid id, string category, byte[] base64EncodeImage)
        {
            this.Id = id;
            this.Category = category;
            this.ImageBinary = base64EncodeImage;
            this.IsChecked = true;
        }
    }
}
