using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class TripOptionViewModel : AnalyticsBase, IDisposable
    {
        public TripOptionViewModel(IAggregateService aggregateService)
            : base(ApplicationPage.TripOptions, aggregateService)
        {

        }

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
