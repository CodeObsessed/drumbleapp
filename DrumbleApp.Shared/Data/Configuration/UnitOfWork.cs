using DrumbleApp.Shared.Data.Repositories;
using DrumbleApp.Shared.Data.Schema;
using DrumbleApp.Shared.Interfaces;
using System;

namespace DrumbleApp.Shared.Data.Configuration
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private CacheContext context;

        private AppSettingRepository appSettingRepository;
        private UserRepository userRepository;
        private PublicTransportOperatorRepository publicTransportOperatorRepository;
        private CacheSettingRepository cacheSettingRepository;
        private TransportModeRepository transportModeRepository;
        private OperatorSettingRepository operatorSettingRepository;
        private SearchItemCategoryRepository searchItemCategoryRepository;
        private RecentTripRepository recentTripRepository;
        private FavouriteRepository favouriteRepository;
        private PlaceOfInterestCategoryRepository placeOfInterestCategoryRepository;
        private WeatherRepository weatherRepository;
        private PathRepository pathRepository;
        private UberTripRepository uberTripRepository;

        public UnitOfWork(CacheContext context)
        {
            this.context = context;
        }

        public UberTripRepository UberTripRepository
        {
            get
            {
                if (this.uberTripRepository == null)
                {
                    this.uberTripRepository = new UberTripRepository(context);
                }
                return uberTripRepository;
            }
        }

        public PathRepository PathRepository
        {
            get
            {
                if (this.pathRepository == null)
                {
                    this.pathRepository = new PathRepository(context);
                }
                return pathRepository;
            }
        }

        public WeatherRepository WeatherRepository
        {
            get
            {
                if (this.weatherRepository == null)
                {
                    this.weatherRepository = new WeatherRepository(context);
                }
                return weatherRepository;
            }
        }

        public PlaceOfInterestCategoryRepository PlaceOfInterestCategoryRepository
        {
            get
            {
                if (this.placeOfInterestCategoryRepository == null)
                {
                    this.placeOfInterestCategoryRepository = new PlaceOfInterestCategoryRepository(context);
                }
                return placeOfInterestCategoryRepository;
            }
        }

        public FavouriteRepository FavouriteRepository
        {
            get
            {
                if (this.favouriteRepository == null)
                {
                    this.favouriteRepository = new FavouriteRepository(context);
                }
                return favouriteRepository;
            }
        }

        public RecentTripRepository RecentTripRepository
        {
            get
            {
                if (this.recentTripRepository == null)
                {
                    this.recentTripRepository = new RecentTripRepository(context);
                }
                return recentTripRepository;
            }
        }

        public SearchItemCategoryRepository SearchItemCategoryRepository
        {
            get
            {
                if (this.searchItemCategoryRepository == null)
                {
                    this.searchItemCategoryRepository = new SearchItemCategoryRepository(context);
                }
                return searchItemCategoryRepository;
            }
        }

        public OperatorSettingRepository OperatorSettingRepository
        {
            get
            {
                if (this.operatorSettingRepository == null)
                {
                    this.operatorSettingRepository = new OperatorSettingRepository(context);
                }
                return operatorSettingRepository;
            }
        }

        public TransportModeRepository TransportModeRepository
        {
            get
            {
                if (this.transportModeRepository == null)
                {
                    this.transportModeRepository = new TransportModeRepository(context);
                }
                return transportModeRepository;
            }
        }

        public CacheSettingRepository CacheSettingRepository
        {
            get
            {
                if (this.cacheSettingRepository == null)
                {
                    this.cacheSettingRepository = new CacheSettingRepository(context);
                }
                return cacheSettingRepository;
            }
        }

        public PublicTransportOperatorRepository PublicTransportOperatorRepository
        {
            get
            {
                if (this.publicTransportOperatorRepository == null)
                {
                    this.publicTransportOperatorRepository = new PublicTransportOperatorRepository(context);
                }
                return publicTransportOperatorRepository;
            }
        }

        public UserRepository UserRepository
        {
            get
            {
                if (this.userRepository == null)
                {
                    this.userRepository = new UserRepository(context);
                }
                return userRepository;
            }
        }

        public AppSettingRepository AppSettingRepository
        {
            get
            {
                if (this.appSettingRepository == null)
                {
                    this.appSettingRepository = new AppSettingRepository(context);
                }
                return appSettingRepository;
            }
        }

        public void ClearCachedItems()
        {
            this.PublicTransportOperatorRepository.RemoveAll();
            this.AppSettingRepository.RemoveAll();
            this.CacheSettingRepository.RemoveAll();
            this.FavouriteRepository.RemoveAll();
            this.OperatorSettingRepository.RemoveAll();
            this.TransportModeRepository.RemoveAll();
            this.PathRepository.RemoveAll();
            this.UserRepository.RemoveAll();
        }

        public bool DropAndCreateDatabase()
        {
            // Drop the database.
            if (this.context.DatabaseExists())
                this.context.DeleteDatabase();

            // Create the local database.
            this.context.CreateDatabase();

            return true;
        }

        public bool CreateDatabase()
        {
            // Only create if database does not exist
            if (!this.context.DatabaseExists())
            {
                this.context.CreateDatabase();

                return true;
            }
            else
                return false;
        }

        public bool DatabaseExists()
        {
            return this.context.DatabaseExists();
        }

        public void Save()
        {
            context.SubmitChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
