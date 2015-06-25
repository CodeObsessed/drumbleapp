using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Resources;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DrumbleApp.Shared.Infrastructure.Extensions;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class MapsViewModel : AnalyticsBase, IDisposable
    {
        public MapsViewModel(IAggregateService aggregateService)
            : base(ApplicationPage.Maps, aggregateService)
        {
        
        }

        #region Overrides

        protected override void PageLoaded()
        {
            base.PageLoaded();

            PageTitleMessage.Send(AppResources.HeaderMaps);
        }

        protected override void PageUnloaded()
        {
            base.PageUnloaded();

            PublicTransportOperators.Clear();
        }

        #endregion

        #region Local Functions

        private void SelectOperator()
        {
            NavigationService.NavigateTo("/Views/OperatorSelection.xaml");
        }

        private void OperatorSelectionPageLoaded()
        {
            PageTitleMessage.Send(AppResources.HeaderChooseOperator);

            PublicTransportOperators.AddRange(UnitOfWork.PublicTransportOperatorRepository.GetAll().OrderBy(x => x.DisplayName));
        }

        private void OperatorSelectionPageUnloaded()
        {
            
        }

        #endregion

        #region Properties

        private ObservableCollection<PublicTransportOperator> publicTransportOperators = new ObservableCollection<PublicTransportOperator>();
        public ObservableCollection<PublicTransportOperator> PublicTransportOperators
        {
            get { return publicTransportOperators; }
        }

        private string operatorName = AppResources.MapsSelectOperatorText;
        public string OperatorName
        {
            get { return operatorName; }
            set
            {
                operatorName = value;
                RaisePropertyChanged("OperatorName");
            }
        }

        private string operatorMapUrl;
        public string OperatorMapUrl
        {
            get { return operatorMapUrl; }
            set
            {
                operatorMapUrl = value;
                RaisePropertyChanged("OperatorMapUrl");
            }
        }

        private PublicTransportOperator selectedPublicTransportOperator;
        public PublicTransportOperator SelectedPublicTransportOperator
        {
            get { return selectedPublicTransportOperator; }
            set
            {
                selectedPublicTransportOperator = value;

                if (value != null)
                {
                    OperatorName = value.DisplayName;
                    OperatorMapUrl = value.RouteMapUrl;
                    NavigationService.GoBack();
                }
                else
                {
                    OperatorName = AppResources.MapsSelectOperatorText;
                }

                RaisePropertyChanged("SelectedPublicTransportOperator");
            }
        }

        #endregion

        #region Commands

        public RelayCommand SelectOperatorCommand
        {
            get { return new RelayCommand(SelectOperator); }
        }

        public RelayCommand OperatorSelectionPageLoadedCommand
        {
            get { return new RelayCommand(OperatorSelectionPageLoaded); }
        }

        public RelayCommand OperatorSelectionPageUnloadedCommand
        {
            get { return new RelayCommand(OperatorSelectionPageUnloaded); }
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
                    UnitOfWork.Dispose();
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
