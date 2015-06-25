using DrumbleApp.Shared.Data.Repositories;

namespace DrumbleApp.Shared.Interfaces
{
    public interface IUnitOfWork
    {
        void Save();

        bool DropAndCreateDatabase();

        bool CreateDatabase();

        bool DatabaseExists();

        void Dispose();

        void ClearCachedItems();

        AppSettingRepository AppSettingRepository { get; }

        UserRepository UserRepository { get; }

        PublicTransportOperatorRepository PublicTransportOperatorRepository { get; }

        CacheSettingRepository CacheSettingRepository { get; }

        TransportModeRepository TransportModeRepository { get; }

        OperatorSettingRepository OperatorSettingRepository { get; }

        SearchItemCategoryRepository SearchItemCategoryRepository { get; }

        RecentTripRepository RecentTripRepository { get; }

        FavouriteRepository FavouriteRepository { get; }

        PlaceOfInterestCategoryRepository PlaceOfInterestCategoryRepository { get; }

        WeatherRepository WeatherRepository { get; }

        PathRepository PathRepository { get; }

        UberTripRepository UberTripRepository { get; }
    }
}
