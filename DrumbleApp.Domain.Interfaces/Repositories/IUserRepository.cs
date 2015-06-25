
using DrumbleApp.Domain.Models.Entities;
namespace DrumbleApp.Domain.Interfaces
{
    public interface IUserRepository
    {
        void Insert(User user);

        User User { get; }

        void Update(User user);
    }
}
