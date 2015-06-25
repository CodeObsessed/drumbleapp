
namespace DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model
{
    public sealed class UberError
    {
        public int status { get; set; }
        public string code { get; set; }
        public string title { get; set; }
        public string message { get; set; }
    }
}
