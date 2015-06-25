using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System.Linq;
using System;
using System.Collections.Generic;
/*using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Data.Configuration;
using DrumbleApp.Shared.Data.Schema;
using DrumbleApp.Shared.Infrastructure.Services.Api.Bumble;*/
using System.Threading;

namespace DrumbleApp.ScheduledTaskAgent71
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected async override void OnInvoke(ScheduledTask task)
        {
            /*CacheContext cache = new CacheContext();
            IUnitOfWork unitOfWork = new UnitOfWork(cache);
            IBumbleApiService BumbleApi = new BumbleApiService(new Uri(DatabaseSetup.BumbleGatewayUrl, UriKind.Absolute), DatabaseSetup.AppKey, unitOfWork);
            IEnumerable<TransportMode> modes = unitOfWork.TransportModeRepository.GetAll();
            IEnumerable<OperatorSetting> operatorSettings = unitOfWork.OperatorSettingRepository.GetAll();

            try
            {
                List<string> excludedModes = modes.Where(x => x.IsEnabled == false).Select(x => x.ApplicationTransportMode.ToString()).ToList();
                List<string> excludedOperators = operatorSettings.Where(x => x.IsEnabled == false).Select(x => x.OperatorName).ToList();

                IEnumerable<Announcement> announcements = await BumbleApi.Announcements(CancellationToken.None, unitOfWork.UserRepository.GetUser(), excludedModes, excludedOperators, DateTime.UtcNow.AddHours(-1));

                // get tile
                ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("/Views/Announcements.xaml"));

                if (null != tile)
                {
                    var data = new StandardTileData()
                    {
                        Title = "Announcements",
                        BackgroundImage = new Uri("/Images/Tiles/TileAnnouncement7.png", UriKind.Relative),
                        Count = announcements.Count()
                    };
                    // update tile
                    tile.Update(data);
                }
            }
            catch (Exception)
            {

            }*/

#if DEBUG
	ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(30));
	System.Diagnostics.Debug.WriteLine("Periodic task started again: " + task.Name);
#endif

            NotifyComplete();
        }
    }
}