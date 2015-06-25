using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Interfaces;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using DrumbleApp.Shared.Infrastructure.Extensions;
using System.Linq;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class TripKarmaViewModel : ViewModelBase
    {
        private IUnitOfWork unitOfWork;

        public TripKarmaViewModel(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
    }
}
