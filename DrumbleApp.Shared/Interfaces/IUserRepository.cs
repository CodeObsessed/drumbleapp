
namespace DrumbleApp.Shared.Interfaces
{
    public interface IUserRepository
    {
        void Insert(Entities.User user);

        Entities.User GetUser();

        void DeleteAll();

        void Update(Entities.User user);
    }
}
