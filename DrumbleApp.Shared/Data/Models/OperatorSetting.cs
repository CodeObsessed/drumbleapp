using GalaSoft.MvvmLight;
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace DrumbleApp.Shared.Data.Models
{
    [Table]
    public sealed class OperatorSetting : ViewModelBase
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

        private string operatorName;
        [Column]
        public string OperatorName
        {
            get { return operatorName; }
            set
            {
                if (operatorName != value)
                {
                    RaisePropertyChanging("OperatorName");
                    operatorName = value;
                    RaisePropertyChanged("OperatorName");
                }
            }
        }

        private bool isEnabled;
        [Column]
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (this.isEnabled != value)
                {
                    RaisePropertyChanging("IsEnabled");
                    this.isEnabled = value;
                    RaisePropertyChanged("IsEnabled");
                }
            }
        }

        private bool hasBeenModified;
        [Column]
        public bool HasBeenModified
        {
            get { return hasBeenModified; }
            set
            {
                if (this.hasBeenModified != value)
                {
                    RaisePropertyChanging("HasBeenModified");
                    this.hasBeenModified = value;
                    RaisePropertyChanged("HasBeenModified");
                }
            }
        }

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary version;

        public OperatorSetting(Guid id, string operatorName, bool isEnabled, bool hasBeenModified)
        {
            this.Id = id;
            this.OperatorName = operatorName;
            this.IsEnabled = isEnabled;
            this.HasBeenModified = hasBeenModified;
        }

        public OperatorSetting()
        {
        }
    }
}
