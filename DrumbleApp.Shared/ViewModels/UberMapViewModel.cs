using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Linq;
using System.Threading;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class UberMapViewModel : AnalyticsBase, IDisposable
    {
        private IUberService uberService;

        public UberMapViewModel(IAggregateService aggregateService, IUberService uberService)
            : base(ApplicationPage.UberMap, aggregateService)
        {
            this.uberService = uberService;
        }

        #region Overrides

        protected override void PageLoaded()
        {
            base.PageLoaded();

            Action fetchMap = async () =>
            {
                if (user != null)
                {
                    UberTrip uberTrip = UnitOfWork.UberTripRepository.GetAll().FirstOrDefault();

                    if (uberTrip != null)
                    {

                        UberMap uberMap = await uberService.GetUberMap(CancellationToken.None, user.UberInfo.AccessToken, uberTrip.RequestId);

                        if (uberMap != null)
                        {
                            UberMapHref = uberMap.Href.ToString();
                        }
                        else
                        {
                            
                        }
                    }
                }

                HideHeaderLoader();
            };

            DispatcherHelper.CheckBeginInvokeOnUI(fetchMap);
        }

        protected override void PageUnloaded()
        {
            base.PageUnloaded();
        }

        #endregion

        #region Properties

        private string uberMapHref = String.Empty;
        public string UberMapHref
        {
            get { return uberMapHref; }
            set
            {
                uberMapHref = value;
                RaisePropertyChanged("UberMapHref");
            }
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
