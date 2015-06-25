using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using System;
using System.Collections.ObjectModel;
using DrumbleApp.Shared.Infrastructure.Extensions;
using GalaSoft.MvvmLight.Command;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class PlaceOfInterestCategoriesViewModel : AnalyticsBase, IDisposable
    {
        public PlaceOfInterestCategoriesViewModel(IUnitOfWork unitOfWork, AppUse appUse, INavigationService navigationService)
            : base(appUse, ApplicationPage.FilterPoiCategories, unitOfWork, navigationService)
        {

        }

        #region Local Functions

        private void SaveFilterButton()
        {
            unitOfWork.PlaceOfInterestCategoryRepository.UpdateRange(PlaceOfInterestCategories);
            unitOfWork.Save();

            navigationService.GoBack();
        }

        private void CancelFilterButton()
        {
            navigationService.GoBack();
        }

        #endregion

        #region Overides

        protected override void PageLoaded()
        {
            base.PageLoaded();

            PlaceOfInterestCategories.Clear();
            PlaceOfInterestCategories.AddRange(unitOfWork.PlaceOfInterestCategoryRepository.GetAll());
        }

        protected override void PageUnloaded()
        {
            base.PageUnloaded();

            PlaceOfInterestCategories.Clear();
        }

        #endregion

        #region Properties

        private ObservableCollection<Entities.PlaceOfInterestCategory> placeOfInterestCategories = new ObservableCollection<Entities.PlaceOfInterestCategory>();
        public ObservableCollection<Entities.PlaceOfInterestCategory> PlaceOfInterestCategories
        {
            get { return placeOfInterestCategories; }
        }

        #endregion

        #region Commands

        public RelayCommand SaveFilterButtonCommand
        {
            get { return new RelayCommand(SaveFilterButton); }
        }

        public RelayCommand CancelFilterButtonCommand
        {
            get { return new RelayCommand(CancelFilterButton); }
        }

        #endregion

        #region Cleanup

        private bool disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    unitOfWork.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}