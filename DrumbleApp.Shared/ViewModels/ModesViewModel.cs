using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Resources;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using DrumbleApp.Shared.Models;
using System.Collections.ObjectModel;
using DrumbleApp.Shared.Messages.Classes;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class ModesViewModel : AnalyticsBase, IDisposable
    {
        private List<PublicTransportOperator> operators;
        private bool firstLoad = true;
        private bool checkOnly = false;

        public ModesViewModel(IAggregateService aggregateService)
            : base(ApplicationPage.Modes, aggregateService)
        {
            operators = UnitOfWork.PublicTransportOperatorRepository.GetAll().ToList();

            UpdateCheckBoxes();
        }

        #region Overrides

        protected override void PageLoaded()
        {
            base.PageLoaded();

            LoadSettings();
        }

        #endregion

        #region Local Functions

        private void Save()
        {
            // Public
            foreach (OperatorModeModel operatorModeModel in PublicOperators)
            {
                UnitOfWork.OperatorSettingRepository.Update(operatorModeModel.OperatorSetting);
            }

            // Private
            foreach (OperatorModeModel operatorModeModel in PrivateOperators)
            {
                UnitOfWork.OperatorSettingRepository.Update(operatorModeModel.OperatorSetting);
            }
            
            UnitOfWork.Save();

            NavigationService.GoBack();
        }

        private void Cancel()
        {
            UpdateCheckBoxes();

            NavigationService.GoBack();
        }

        private void UpdateOperatorCheckBoxes(bool isChecked, ObservableCollection<OperatorModeModel> operatorsModeModels)
        {
            if (isChecked)
            {
                foreach (OperatorModeModel model in operatorsModeModels)
                {
                    model.Enable();
                }
            }
            else
            {
                foreach (OperatorModeModel model in operatorsModeModels)
                {
                    model.Disable();
                }
            }
        }

        private void PublicTransportShowHide()
        {
            if (PublicOperatorsVisibility == Visibility.Visible)
            {
                PublicOperatorsVisibility = Visibility.Collapsed;
                PublicTransportVisibilityArrowImage = "/Images/64/W/IconArrowExpandUp.png";
            }
            else
            {
                PublicOperatorsVisibility = Visibility.Visible;
                PublicTransportVisibilityArrowImage = "/Images/64/W/IconArrowExpandDown.png";
            }
        }

        private void PrivateTransportShowHide()
        {
            if (PrivateOperatorsVisibility == Visibility.Visible)
            {
                PrivateOperatorsVisibility = Visibility.Collapsed;
                PrivateTransportVisibilityArrowImage = "/Images/64/W/IconArrowExpandUp.png";
            }
            else
            {
                PrivateOperatorsVisibility = Visibility.Visible;
                PrivateTransportVisibilityArrowImage = "/Images/64/W/IconArrowExpandDown.png";
            }
        }

        private void UpdateCheckBoxes()
        {
            PublicOperators.Clear();
            PrivateOperators.Clear();

            List<OperatorSetting> operatorSettings = UnitOfWork.OperatorSettingRepository.GetAll().ToList();

            foreach (PublicTransportOperator publicTransportOperator in operators)
            {
                if (publicTransportOperator.IsPublic)
                    PublicOperators.Add(new OperatorModeModel(publicTransportOperator, operatorSettings.Where(x => x.OperatorName == publicTransportOperator.Name).FirstOrDefault()));
                else
                    PrivateOperators.Add(new OperatorModeModel(publicTransportOperator, operatorSettings.Where(x => x.OperatorName == publicTransportOperator.Name).FirstOrDefault()));
            }

            checkOnly = true;
            PublicTransportIsChecked = PublicOperators.Select(x => x.OperatorSetting).Any(x => x.IsEnabled);
            PrivateTransportIsChecked = PrivateOperators.Select(x => x.OperatorSetting).Any(x => x.IsEnabled);
            checkOnly = false;

            firstLoad = false;
        }

        private void LoadSettings()
        {
            foreach (TransportMode mode in UnitOfWork.TransportModeRepository.GetAll())
            {
                switch (mode.ApplicationTransportMode)
                {
                    case ApplicationTransportMode.Bus:
                        if (mode.IsEnabled)
                            BusImage = "/Images/64/W/ModeBus.png";
                        else
                            BusImage = "/Images/64/W/ModeBus-Off.png";
                        break;
                    case ApplicationTransportMode.Rail:
                        if (mode.IsEnabled)
                            RailImage = "/Images/64/W/ModeRail.png";
                        else
                            RailImage = "/Images/64/W/ModeRail-Off.png";
                        break;
                    case ApplicationTransportMode.Boat:
                        if (mode.IsEnabled)
                            BoatImage = "/Images/64/W/ModeBoat.png";
                        else
                            BoatImage = "/Images/64/W/ModeBoat-Off.png";
                        break;
                    case ApplicationTransportMode.Taxi:
                        if (mode.IsEnabled)
                            TaxiImage = "/Images/64/W/ModeTaxi.png";
                        else
                            TaxiImage = "/Images/64/W/ModeTaxi-Off.png";
                        break;
                }
            }

            UpdateCheckBoxes();
        }

        private void Bus()
        {
            TransportMode mode = UnitOfWork.TransportModeRepository.FindByType(ApplicationTransportMode.Bus);
            if (mode.IsEnabled)
            {
                mode.IsEnabled = false;
                BusImage = "/Images/64/W/ModeBus-Off.png";
            }
            else
            {
                mode.IsEnabled = true;
                BusImage = "/Images/64/W/ModeBus.png";
            }

            UpdateModeSetting(mode);
        }

        private void Rail()
        {
            TransportMode mode = UnitOfWork.TransportModeRepository.FindByType(ApplicationTransportMode.Rail);
            if (mode.IsEnabled)
            {
                mode.IsEnabled = false;
                RailImage = "/Images/64/W/ModeRail-Off.png";
            }
            else
            {
                mode.IsEnabled = true;
                RailImage = "/Images/64/W/ModeRail.png";
            }

            UpdateModeSetting(mode);
        }

        private void Boat()
        {
            TransportMode mode = UnitOfWork.TransportModeRepository.FindByType(ApplicationTransportMode.Boat);
            if (mode.IsEnabled)
            {
                mode.IsEnabled = false;
                BoatImage = "/Images/64/W/ModeBoat-Off.png";
            }
            else
            {
                mode.IsEnabled = true;
                BoatImage = "/Images/64/W/ModeBoat.png";
            }

            UpdateModeSetting(mode);
        }

        private void Taxi()
        {
            TransportMode mode = UnitOfWork.TransportModeRepository.FindByType(ApplicationTransportMode.Taxi);
            if (mode.IsEnabled)
            {
                mode.IsEnabled = false;
                TaxiImage = "/Images/64/W/ModeTaxi-Off.png";
            }
            else
            {
                mode.IsEnabled = true;
                TaxiImage = "/Images/64/W/ModeTaxi.png";
            }
            UpdateModeSetting(mode);
        }

        private void SetOperatorsAsModified()
        {
            List<OperatorSetting> operatorSettings = UnitOfWork.OperatorSettingRepository.GetAll().Where(x => !x.HasBeenModified).ToList();
            foreach (OperatorSetting operatorSetting in operatorSettings)
            {
                operatorSetting.HasBeenModified = true;
                UnitOfWork.OperatorSettingRepository.Update(operatorSetting);
                UnitOfWork.Save();
            }
        }

        private void ShowModePageViaButton()
        {
            if (NavigationService.CurrentPage() != "/Views/Modes.xaml")
            {
                SetOperatorsAsModified();

                PageTitleMessage.Send(AppResources.HeaderMode);

                NavigationService.NavigateTo("/Views/Modes.xaml");
            }
            else
            {
                NavigationService.GoBack();
            }
        }

        private void UpdateModeSetting(TransportMode mode)
        {
            UnitOfWork.TransportModeRepository.Update(mode);
            UnitOfWork.Save();
        }

        #endregion

        #region Properties

        private ObservableCollection<OperatorModeModel> publicOperators = new ObservableCollection<OperatorModeModel>();
        public ObservableCollection<OperatorModeModel> PublicOperators
        {
            get { return publicOperators; }
        }

        private ObservableCollection<OperatorModeModel> privateOperators = new ObservableCollection<OperatorModeModel>();
        public ObservableCollection<OperatorModeModel> PrivateOperators
        {
            get { return privateOperators; }
        }

        private Visibility publicOperatorsVisibility = Visibility.Visible;
        public Visibility PublicOperatorsVisibility
        {
            get { return publicOperatorsVisibility; }
            set
            {
                publicOperatorsVisibility = value;
                RaisePropertyChanged("PublicOperatorsVisibility");
            }
        }

        private Visibility privateOperatorsVisibility = Visibility.Visible;
        public Visibility PrivateOperatorsVisibility
        {
            get { return privateOperatorsVisibility; }
            set
            {
                privateOperatorsVisibility = value;
                RaisePropertyChanged("PrivateOperatorsVisibility");
            }
        }

        private string publicTransportVisibilityArrowImage = "/Images/64/W/IconArrowExpandDown.png";
        public string PublicTransportVisibilityArrowImage
        {
            get { return publicTransportVisibilityArrowImage; }
            set
            {
                publicTransportVisibilityArrowImage = value;
                RaisePropertyChanged("PublicTransportVisibilityArrowImage");
            }
        }

        private string privateTransportVisibilityArrowImage = "/Images/64/W/IconArrowExpandDown.png";
        public string PrivateTransportVisibilityArrowImage
        {
            get { return privateTransportVisibilityArrowImage; }
            set
            {
                privateTransportVisibilityArrowImage = value;
                RaisePropertyChanged("PrivateTransportVisibilityArrowImage");
            }
        }

        private string busImage = "/Images/64/W/ModeBus.png";
        public string BusImage
        {
            get { return busImage; }
            set
            {
                busImage = value;
                RaisePropertyChanged("BusImage");
            }
        }

        private string railImage = "/Images/64/W/ModeRail.png";
        public string RailImage
        {
            get { return railImage; }
            set
            {
                railImage = value;
                RaisePropertyChanged("RailImage");
            }
        }

        private string boatImage = "/Images/64/W/ModeBoat.png";
        public string BoatImage
        {
            get { return boatImage; }
            set
            {
                boatImage = value;
                RaisePropertyChanged("BoatImage");
            }
        }

        private string taxiImage = "/Images/64/W/ModeTaxi.png";
        public string TaxiImage
        {
            get { return taxiImage; }
            set
            {
                taxiImage = value;
                RaisePropertyChanged("TaxiImage");
            }
        }

        private string busText = AppResources.ModeBus;
        public string BusText
        {
            get { return busText; }
            set
            {
                busText = value;
                RaisePropertyChanged("BusText");
            }
        }

        private string railText = AppResources.ModeRail;
        public string RailText
        {
            get { return railText; }
            set
            {
                railText = value;
                RaisePropertyChanged("RailText");
            }
        }

        private string boatText = AppResources.ModeBoat;
        public string BoatText
        {
            get { return boatText; }
            set
            {
                boatText = value;
                RaisePropertyChanged("BoatText");
            }
        }

        private string taxiText = AppResources.ModeTaxi;
        public string TaxiText
        {
            get { return taxiText; }
            set
            {
                taxiText = value;
                RaisePropertyChanged("TaxiText");
            }
        }

        private bool publicTransportIsChecked;
        public bool PublicTransportIsChecked
        {
            get { return publicTransportIsChecked; }
            set
            {
                publicTransportIsChecked = value;
                RaisePropertyChanged("PublicTransportIsChecked");

                if (!firstLoad && !checkOnly)
                {
                    UpdateOperatorCheckBoxes(value, PublicOperators);
                }
            }
        }

        private bool privateTransportIsChecked;
        public bool PrivateTransportIsChecked
        {
            get { return privateTransportIsChecked; }
            set
            {
                privateTransportIsChecked = value;
                RaisePropertyChanged("PrivateTransportIsChecked");

                if (!firstLoad && !checkOnly)
                {
                    UpdateOperatorCheckBoxes(value, PrivateOperators);
                }
            }
        }

        #endregion

        #region Commands

        public RelayCommand CancelButtonCommand
        {
            get { return new RelayCommand(Cancel); }
        }

        public RelayCommand SaveButtonCommand
        {
            get { return new RelayCommand(Save); }
        }

        public RelayCommand PublicTransportShowHideCommand
        {
            get { return new RelayCommand(PublicTransportShowHide); }
        }

        public RelayCommand PrivateTransportShowHideCommand
        {
            get { return new RelayCommand(PrivateTransportShowHide); }
        }

        public RelayCommand ModeButtonCommand
        {
            get { return new RelayCommand(ShowModePageViaButton); }
        }

        public RelayCommand BusCommand
        {
            get { return new RelayCommand(Bus); }
        }

        public RelayCommand RailCommand  
        {
            get { return new RelayCommand(Rail); }
        }

        public RelayCommand BoatCommand
        {
            get { return new RelayCommand(Boat); }
        }

        public RelayCommand TaxiCommand
        {
            get { return new RelayCommand(Taxi); }
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
