using DrumbleApp.Shared.Data.Factories;
using DrumbleApp.Shared.Data.Models;
using DrumbleApp.Shared.Data.Schema;
using DrumbleApp.Shared.Interfaces;
using System.Linq;

namespace DrumbleApp.Shared.Data.Repositories
{
    public sealed class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(CacheContext context)
            : base(context)
        {

        }

        public void Insert(Entities.User user)
        {
            base.DbSet.InsertOnSubmit(DataModelFactory.Create(user));
        }

        public Entities.User GetUser()
        {
            User user = base.DbSet.SingleOrDefault();
            return (user == null) ? null : EntityModelFactory.Create(user);
        }

        public void DeleteAll()
        {
            base.DbSet.DeleteOnSubmit(base.DbSet.FirstOrDefault());
        }

        public void Update(Entities.User user)
        {
            User userToUpdate = base.DbSet.FirstOrDefault();
            if (user.Country != null)
            {
                userToUpdate.CountryCode = user.Country.Code;
                userToUpdate.CountryName = user.Country.Name;
            }
            else
            {
                userToUpdate.CountryCode = null;
                userToUpdate.CountryName = null;
            }
            userToUpdate.DismissedLocationPopup = user.DismissedLocationPopup;
            if (user.LastKnownGeneralLocation != null)
            {
                userToUpdate.Latitude = user.LastKnownGeneralLocation.Latitude;
                userToUpdate.Longitude = user.LastKnownGeneralLocation.Longitude;
            }
            userToUpdate.LastLocationUpdate = user.LastLocationUpdate;
            userToUpdate.DismissedRateAppPopup = user.DismissedRateAppPopup;
            userToUpdate.DismissedSignUpPopup = user.DismissedSignUpPopup;
            userToUpdate.DismissedLoginUberPopup = user.DismissedLoginUberPopup;

            userToUpdate.IsBumbleRegistered = user.IsBumbleRegistered;
            userToUpdate.IsFacebookRegistered = user.IsFacebookRegistered;
            userToUpdate.IsTwitterRegistered = user.IsTwitterRegistered;
            userToUpdate.IsUberAuthenticated = user.IsUberAuthenticated;

            if (user.FacebookInfo != null)
            {
                userToUpdate.FacebookId = user.FacebookInfo.FacebookId;
                userToUpdate.FacebookAccessToken = user.FacebookInfo.AccessToken;
            }
            else
            {
                userToUpdate.FacebookId = null;
                userToUpdate.FacebookAccessToken = null;
            }

            if (user.UberInfo != null)
            {
                userToUpdate.UberAccessToken = user.UberInfo.AccessToken;
                userToUpdate.UberRefreshToken = user.UberInfo.RefreshToken;
            }
            else
            {
                userToUpdate.UberAccessToken = null;
                userToUpdate.UberRefreshToken = null;
            }

            userToUpdate.AppUsageCount = user.AppUsageCount;
        }
    }
}
