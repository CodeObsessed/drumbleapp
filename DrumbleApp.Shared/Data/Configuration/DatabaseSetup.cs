using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Interfaces;
using System;
using System.Globalization;
using System.Linq;

namespace DrumbleApp.Shared.Data.Configuration
{
    public static class DatabaseSetup
    {
        private static DateTime lastDatabaseUpdate = new DateTime(2015, 05, 13, 15, 35, 0);

#if DEBUG
        // Test server and test key.

        
        public static string DrumbleGatewayUrl = "https://Drumblegatewaytest.Bumble.co.za/v1/";
        public static string BumbleGatewayUrl = "https://Bumblegatewaytest.Bumble.co.za/";
#else
        

        

        // Test server and test key.

        
        public static string DrumbleGatewayUrl = "https://Drumblegateway.Bumble.co.za/v1/";
        public static string BumbleGatewayUrl = "https://Bumblegateway.Bumble.co.za/";

#endif

        /// <summary>
        /// Creates and seeds the local database with default settings.
        /// (DEBUG) : In debug mode the Db is always re-created.
        /// (RELEASE) : In reelase, the Db is not dropped.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public static bool Seed(IUnitOfWork unitOfWork, bool repopulate = false)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            bool isCreated = false;
            User user = null;

            //#if DEBUG
            if (repopulate)
            {
                unitOfWork.ClearCachedItems();
                unitOfWork.Save();
            }
            //#else
                isCreated = unitOfWork.CreateDatabase();
            //#endif

            if (!isCreated)
            {
                try
                {
                    CacheSetting database = unitOfWork.CacheSettingRepository.GetByType(ResourceType.Database);

                    if (database == null || database.LastRefreshedDateUtc < lastDatabaseUpdate)
                    {
                        if (database != null)
                        {
                            // Save the user's details to be re-added to the database.
                            user = unitOfWork.UserRepository.GetUser();
                        }

                        isCreated = unitOfWork.DropAndCreateDatabase();

                        unitOfWork.Save();
                    }
                }
                catch (Exception)
                {
                    isCreated = unitOfWork.DropAndCreateDatabase();

                    unitOfWork.Save();
                }
            }

            if (isCreated || repopulate)
            {
                // Prepopulate the cache settings.
                unitOfWork.CacheSettingRepository.Insert(new CacheSetting(null, ResourceType.Operators));
                unitOfWork.CacheSettingRepository.Insert(new CacheSetting(lastDatabaseUpdate, ResourceType.Database));

                // Prepopulate the application settings.
                unitOfWork.AppSettingRepository.Insert(new AppSetting(ApplicationSetting.AllowLocation, false));
                unitOfWork.AppSettingRepository.Insert(new AppSetting(ApplicationSetting.UseMetric, new RegionInfo(CultureInfo.CurrentCulture.Name).IsMetric));
                unitOfWork.AppSettingRepository.Insert(new AppSetting(ApplicationSetting.ShowWeather, true));
                unitOfWork.AppSettingRepository.Insert(new AppSetting(ApplicationSetting.StoreLocation, true));
                unitOfWork.AppSettingRepository.Insert(new AppSetting(ApplicationSetting.UseUber, true));
                unitOfWork.AppSettingRepository.Insert(new AppSetting(ApplicationSetting.AutoPopulateLocation, true));
                unitOfWork.AppSettingRepository.Insert(new AppSetting(ApplicationSetting.AutoPopulateMostFrequent, false));
                unitOfWork.AppSettingRepository.Insert(new AppSetting(ApplicationSetting.AutoPopulateMostRecent, false));
                unitOfWork.AppSettingRepository.Insert(new AppSetting(ApplicationSetting.StoreRecent, true));
                unitOfWork.AppSettingRepository.Insert(new AppSetting(ApplicationSetting.SkipTripSelection, false));
                unitOfWork.AppSettingRepository.Insert(new AppSetting(ApplicationSetting.ShowAnnouncementsApplicationBar, true));
                unitOfWork.AppSettingRepository.Insert(new AppSetting(ApplicationSetting.ShowTripApplicationBar, true));
                unitOfWork.AppSettingRepository.Insert(new AppSetting(ApplicationSetting.LoginUber, false));

                // Prepopulate the modes.
                unitOfWork.TransportModeRepository.Insert(new TransportMode(ApplicationTransportMode.Bus, true));
                unitOfWork.TransportModeRepository.Insert(new TransportMode(ApplicationTransportMode.Rail, true));
                unitOfWork.TransportModeRepository.Insert(new TransportMode(ApplicationTransportMode.Taxi, true));
                unitOfWork.TransportModeRepository.Insert(new TransportMode(ApplicationTransportMode.Boat, true));

                // Save cache setting to the database.
                unitOfWork.Save();

                if (user != null)
                {
                    // Re-populate the user
                    user = new User(Guid.NewGuid(), user.Token, user.Country, user.DismissedLocationPopup, user.LastKnownGeneralLocation, user.LastLocationUpdate, user.DismissedRateAppPopup, user.DismissedSignUpPopup, user.IsBumbleRegistered, user.Email, user.FirstName, user.LastName, user.IsFacebookRegistered, user.IsTwitterRegistered, user.FacebookInfo, user.TwitterInfo, user.TwitterHandle, user.AppUsageCount, user.DismissedLoginUberPopup, user.UberInfo, user.IsUberAuthenticated);
                    unitOfWork.UserRepository.Insert(user);
                    unitOfWork.Save();
                }
            }

            return isCreated;
        }
    }
}
