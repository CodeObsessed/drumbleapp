using DrumbleApp.Shared.ValueObjects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DrumbleApp.Shared.Interfaces
{
    public interface IUberService
    {
        Task<UberOption> GetUberOption(CancellationToken ct, Coordinate startLocation, Coordinate endLocation);

        Task<UberAuthenticationDetails> Authenticate(CancellationToken ct, UberOAuthCredentials credentials);

        Task<ValueObjects.UberRequest> PostUberRequest(CancellationToken ct, string accessToken, Guid productId, Coordinate startLocation, Coordinate endLocation, string surgeConfirmationId);

        Task<ValueObjects.UberRequest> GetUberRequest(CancellationToken ct, string accessToken, Guid requestId);

        Task<string> PutUberRequest(CancellationToken ct, string accessToken, Guid requestId, string status);

        Task<string> PutUberProduct(CancellationToken ct, string accessToken, Guid productId, double surgeMultilpier);

        Task<bool> DeleteUberRequest(CancellationToken ct, string accessToken, Guid requestId);

        Task<ValueObjects.UberMap> GetUberMap(CancellationToken ct, string accessToken, Guid requestId);
    }
}
