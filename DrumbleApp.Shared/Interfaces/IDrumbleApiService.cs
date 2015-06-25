
using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Infrastructure.Services.Api.Drumble.ResultModel.Results;
using DrumbleApp.Shared.ValueObjects;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DrumbleApp.Shared.Interfaces
{
    public interface IDrumbleApiService
    {
        Task<Token> RegisterAnonymous(CancellationToken ct);

        Task<ContactResult> Contact(CancellationToken ct, User user, Email from, string subject, string message);

        Task<bool> RegisterEmail(CancellationToken ct, User user, Email email);

        Task<User> RegisterFacebook(CancellationToken ct, User user);

        Task<User> RegisterTwitter(CancellationToken ct, User user);

        Task<User> LoginFacebook(CancellationToken ct, User user);

        Task<User> LoginTwitter(CancellationToken ct, User user);

        Task<bool> LoginEmail(CancellationToken ct, Email email);

        Task<User> Identify(CancellationToken ct, Email email, Pin onetimePin);

        Task<User> Authorise(CancellationToken ct, User user, Email email, Pin onetimePin);
    }
}
