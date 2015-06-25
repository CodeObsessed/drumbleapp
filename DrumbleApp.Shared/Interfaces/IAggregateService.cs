
namespace DrumbleApp.Shared.Interfaces
{
    public interface IAggregateService
    {
        IInMemoryApplicationSettingModel InMemoryApplicationSettingModel { get; set; }
        INavigationService NavigationService { get; set; }
        IUnitOfWork UnitOfWork { get; set; }
    }
}
