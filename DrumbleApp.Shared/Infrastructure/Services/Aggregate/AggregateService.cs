using DrumbleApp.Shared.Interfaces;

namespace DrumbleApp.Shared.Infrastructure.Services.Aggregate
{
    public sealed class AggregateService : IAggregateService
    {
        public IInMemoryApplicationSettingModel InMemoryApplicationSettingModel { get; set; }
        public INavigationService NavigationService { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }

        public AggregateService(IUnitOfWork unitOfWork, IInMemoryApplicationSettingModel inMemoryApplicationSettingModel, INavigationService navigationService)
        {
            this.UnitOfWork = unitOfWork;
            this.NavigationService = navigationService;
            this.InMemoryApplicationSettingModel = inMemoryApplicationSettingModel;
        }
    }
}
