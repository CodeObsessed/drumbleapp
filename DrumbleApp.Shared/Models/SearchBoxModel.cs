using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Windows;
using System.Threading;
using DrumbleApp.Shared.ViewModels;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Messages.Enums;
using GalaSoft.MvvmLight.Ioc;

namespace DrumbleApp.Shared.Models
{
    public sealed class SearchBoxModel : ViewModelBase
    {
        #region Private Properties

        private IBumbleApiService BumbleApiService;
        private INavigationService navigationService;
        private IUnitOfWork unitOfWork;
        private IInMemoryApplicationSettingModel inMemoryApplicationSettingModel;
        private User user;
        private bool sentCoordinateRequest = false;
        private CancellationTokenSource cancellationTokenSource;
        private WhereToViewModel whereToViewModel;

        #endregion

        #region Public Properties

        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler Changed;

        public SearchType Type { get; private set; }
        public TripOptionModel TripOptions { get; private set; }

        private string text;
        public string TextLocation
        {
            get { return text; }
            set
            {
                text = value;
                RaisePropertyChanged("TextLocation");
            }
        }

        public string TextDestination
        {
            get { return text; }
            set
            {
                text = value;
                RaisePropertyChanged("TextDestination");
            }
        }

        private Visibility clearButtonVisibility = Visibility.Collapsed;
        public Visibility ClearButtonVisibilityLocation
        {
            get { return clearButtonVisibility; }
            set
            {
                clearButtonVisibility = value;
                RaisePropertyChanged("ClearButtonVisibilityLocation");
            }
        }
        public Visibility ClearButtonVisibilityDestination
        {
            get { return clearButtonVisibility; }
            set
            {
                clearButtonVisibility = value;
                RaisePropertyChanged("ClearButtonVisibilityDestination");
            }
        }
        
        #endregion

        #region Constructors

        public SearchBoxModel(IUnitOfWork unitOfWork, IInMemoryApplicationSettingModel inMemoryApplicationSettingModel, WhereToViewModel whereToViewModel, SearchType searchType, IBumbleApiService BumbleApiService, User user, INavigationService navigationService)
        {
            this.Type = searchType;
            this.TripOptions = new TripOptionModel();
            this.BumbleApiService = BumbleApiService;
            this.navigationService = navigationService;
            this.user = user;
            this.whereToViewModel = whereToViewModel;
            this.unitOfWork = unitOfWork;
            this.inMemoryApplicationSettingModel = inMemoryApplicationSettingModel;

            switch (searchType)
            {
                case SearchType.Location:
                    this.TextLocation = AppResources.WhereToLocationTextBoxWaterMark;
                    break;
                case SearchType.Destination:
                    this.TextDestination = AppResources.WhereToDestinationTextBoxWaterMark;
                    break;
            }
        }

        #endregion

        #region Local Functions

        public void GpsError()
        {
            if (sentCoordinateRequest)
            {
                sentCoordinateRequest = false;
                ClearButton();
            }
        }

        public void UserLocationFound(Coordinate coordinate)
        {
            if (sentCoordinateRequest)
            {
                sentCoordinateRequest = false;

                if (cancellationTokenSource == null || cancellationTokenSource.Token.IsCancellationRequested)
                {
                    ClearButton();
                    return;
                }

                // Change text to finding address.
                switch (this.Type)
                {
                    case SearchType.Location:
                        // Check if still finding location (there is a chance that a favourite was loaded in this time).
                        if (TextLocation == AppResources.WhereToFindingLocationTextBoxWaterMark)
                            TextLocation = AppResources.WhereToFindingAddressWaterMark;
                        else
                            return;
                        break;
                    case SearchType.Destination:
                        // Check if still finding location (there is a chance that a favourite was loaded in this time).
                        if (TextDestination == AppResources.WhereToFindingLocationTextBoxWaterMark)
                            TextDestination = AppResources.WhereToFindingAddressWaterMark;
                        else
                            return;
                        break;
                }

                Action getAddress = async () =>
                {
                    LoadingBarMessage.Send(LoadingBarMessageReason.Show);

                    try
                    {
                        Address userAddress = await BumbleApiService.ReverseGeoCode(cancellationTokenSource.Token, user, coordinate);

                        switch (this.Type)
                        {
                            case SearchType.Location:
                                // Check if still finding location (there is a chance that a favourite was loaded in this time).
                                if (TextLocation == AppResources.WhereToFindingAddressWaterMark)
                                    TextLocation = userAddress.ShortAddressText;
                                else
                                {
                                    LoadingBarMessage.Send(LoadingBarMessageReason.Hide);
                                    return;
                                }
                                break;
                            case SearchType.Destination:
                                // Check if still finding location (there is a chance that a favourite was loaded in this time).
                                if (TextDestination == AppResources.WhereToFindingAddressWaterMark)
                                    TextDestination = userAddress.ShortAddressText;
                                else
                                {
                                    LoadingBarMessage.Send(LoadingBarMessageReason.Hide);
                                    return;
                                }
                                break;
                        }

                        // Don't lose the user's location, so create a new address object.
                        TripOptions.SetAsAddress(userAddress);

                        OnChanged(EventArgs.Empty);
                    }
                    catch (Exception e)
                    {
                        ClearButton();

                        if (e.Message != "Cancelled")
                        {
                            Messenger.Default.Send<CustomPopupMessage>(new CustomPopupMessage(CustomPopupMessageType.Error, e.Message, AppResources.CustomPopupGenericOkMessage, null));
                        }
                    }

                    LoadingBarMessage.Send(LoadingBarMessageReason.Hide);
                };

                DispatcherHelper.CheckBeginInvokeOnUI(getAddress);
            }
        }

        private void ButtonTap()
        {
            switch (this.Type)
            {
                case SearchType.Location:
                    SimpleIoc.Default.Unregister<SearchTypeModel>();

                    SimpleIoc.Default.Register<SearchTypeModel>(() =>
                    {
                        return new SearchTypeModel(SearchType.Location);
                    });

                    navigationService.NavigateTo("/Views/Search.xaml");
                    break;
                case SearchType.Destination:
                    SimpleIoc.Default.Unregister<SearchTypeModel>();

                    SimpleIoc.Default.Register<SearchTypeModel>(() =>
                    {
                        return new SearchTypeModel(SearchType.Destination);
                    });

                    navigationService.NavigateTo("/Views/Search.xaml");
                    break;
            }
        }

        private void SetSearchChoice(SearchItem searchItem)
        {
            TripOptions.SetAsAddress(new Address(searchItem.Name, searchItem.Name, searchItem.Location));

            switch (this.Type)
            {
                case SearchType.Location:
                    // Change text to the search item name.
                    TextLocation = searchItem.Name;

                    ClearButtonVisibilityLocation = Visibility.Visible;
                    break;
                case SearchType.Destination:
                    // Change text to the search item name.
                    TextDestination = searchItem.Name;

                    ClearButtonVisibilityDestination = Visibility.Visible;;
                    break;
            }

            OnChanged(EventArgs.Empty);
        }

        private void SetAsCurrentLocation()
        {
            if (this.inMemoryApplicationSettingModel.GetSetting(ApplicationSetting.AllowLocation).Value)
            {
                switch (this.Type)
                {
                    case SearchType.Location:
                        // Show the clear button.
                        ClearButtonVisibilityLocation = Visibility.Visible;
                        // Change text to finding location.
                        TextLocation = AppResources.WhereToFindingLocationTextBoxWaterMark;
                        break;
                    case SearchType.Destination:
                        // Show the clear button.
                        ClearButtonVisibilityDestination = Visibility.Visible;
                        // Change text to finding location.
                        TextDestination = AppResources.WhereToFindingLocationTextBoxWaterMark;
                        break;
                }

                sentCoordinateRequest = true;
                cancellationTokenSource = new CancellationTokenSource();
                // Locate user position.
                if (!user.IsLocationUptodate)
                {
                    whereToViewModel.RegisterWatcher();
                }
                else
                {
                    UserLocationFound(user.LastKnownGeneralLocation);
                }
            }
            else
            {
                Messenger.Default.Send<CustomPopupMessage>(new CustomPopupMessage(CustomPopupMessageType.Error, AppResources.CustomPopupGenericLocationDisabledMessage, AppResources.CustomPopupGenericOkMessage, null));
            }
        }

        private void ClearButton()
        {
            if (cancellationTokenSource != null)
                cancellationTokenSource.Cancel();
            TripOptions.SearchCategory = SearchCategory.Unspecified;
            switch (this.Type)
            {
                case SearchType.Location:
                    ClearButtonVisibilityLocation = Visibility.Collapsed;
                    TextLocation = AppResources.WhereToLocationTextBoxWaterMark;
                    break;
                case SearchType.Destination:
                    ClearButtonVisibilityDestination = Visibility.Collapsed;
                    TextDestination = AppResources.WhereToDestinationTextBoxWaterMark;
                    break;
            }

            OnChanged(EventArgs.Empty);
        }

        #endregion

        #region Public Functions

        public void SetAutoPopulateState()
        {
            if (this.inMemoryApplicationSettingModel.GetSetting(ApplicationSetting.AllowLocation).Value && this.inMemoryApplicationSettingModel.GetSetting(ApplicationSetting.AutoPopulateLocation).Value)
            {
                SetAsCurrentLocation();
            }
            else if (this.inMemoryApplicationSettingModel.GetSetting(ApplicationSetting.AutoPopulateMostRecent).Value)
            {
                Recent mostRecentTrip = unitOfWork.RecentTripRepository.GetMostRecent();

                if (mostRecentTrip != null)
                {
                    RecentTripMessage.Send(mostRecentTrip, RecentTripMessageReason.SetAsWhereToDestination);
                }
                else
                {
                    this.TextLocation = AppResources.WhereToLocationTextBoxWaterMark;
                }
            }
            else if (this.inMemoryApplicationSettingModel.GetSetting(ApplicationSetting.AutoPopulateMostFrequent).Value)
            {
                Recent mostFrequentTrip = unitOfWork.RecentTripRepository.GetMostFrequent();

                if (mostFrequentTrip != null)
                {
                    RecentTripMessage.Send(mostFrequentTrip, RecentTripMessageReason.SetAsWhereToDestination);
                }
                else
                {
                    this.TextLocation = AppResources.WhereToLocationTextBoxWaterMark;
                }
            }
        }

        public void LoadFavourite(string text, Coordinate location)
        {
            this.TripOptions.SetAsAddress(new Address(text, text, location));

            switch (this.Type)
            {
                case SearchType.Location:                   
                    this.TextLocation = text;
                    ClearButtonVisibilityLocation = Visibility.Visible;
                    break;
                case SearchType.Destination:
                    this.TextDestination = text;
                    ClearButtonVisibilityDestination = Visibility.Visible;
                    break;
            }
        }

        public void LoadLocation()
        {
            SetAsCurrentLocation();
        }

        public bool IsValid()
        {
            return TripOptions.IsValid();
        }

        public void UpdateUser(User user)
        {
            this.user = user;
        }

        public void Deregister()
        {
        }

        public void Switch()
        {
            switch (Type)
            {
                case SearchType.Location:
                    Type = SearchType.Destination;
                    ClearButtonVisibilityDestination = ClearButtonVisibilityLocation;    
                    
                    if (TextLocation == AppResources.WhereToLocationTextBoxWaterMark)
                        TextDestination = AppResources.WhereToDestinationTextBoxWaterMark;
                    else
                        TextDestination = TextLocation;
                    break;

                case SearchType.Destination:
                    Type = SearchType.Location;
                    ClearButtonVisibilityLocation = ClearButtonVisibilityDestination;

                    if (TextDestination == AppResources.WhereToDestinationTextBoxWaterMark)
                        TextLocation = AppResources.WhereToLocationTextBoxWaterMark;
                    else
                        TextLocation = TextDestination;

                    break;
            }
        }

        #endregion

        #region Protected Functions

        // Invoke the Changed event; called whenever there is a  change
        private void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        #endregion

        #region Commands

        public RelayCommand ClearButtonCommand
        {
            get { return new RelayCommand(ClearButton); }
        }

        public RelayCommand ButtonTapCommand
        {
            get { return new RelayCommand(ButtonTap); }
        }

        #endregion

    }
}
