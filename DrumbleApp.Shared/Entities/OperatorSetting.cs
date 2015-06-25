using GalaSoft.MvvmLight;
using System;

namespace DrumbleApp.Shared.Entities
{
    public sealed class OperatorSetting : ViewModelBase
    {
        public Guid Id { get; private set; }
        public string OperatorName { get; private set; }

        private bool isEnabled;
        public bool IsEnabled 
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
                RaisePropertyChanged("IsEnabled");
            }

        }
        public bool HasBeenModified { get; set; }

        public OperatorSetting(string operatorName, bool isEnabled)
        {
            this.Id = Guid.NewGuid();
            this.OperatorName = operatorName;
            this.IsEnabled = isEnabled;
            this.HasBeenModified = false;
        }

        public OperatorSetting(Guid id, string operatorName, bool isEnabled, bool hasBeenModified)
            : this(operatorName, isEnabled)
        {
            this.Id = id;
            this.HasBeenModified = hasBeenModified;
        }
    }
}
