using Dipterv.Shared.Dto.SignIn;
using Stl.Fusion.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace Dipterv.Shared.Interfaces.ComputeServices
{
    public interface IAccountService
    {
        public Task SignIn(Session session, EmailPasswordDto signInDto, CancellationToken cancellationToken);

        public Task RegisterUser(EmailPasswordDto emailPasswordDto, CancellationToken cancellationToken);

        public Task RegisterAdmin(EmailPasswordDto emailPasswordDto, CancellationToken cancellationToken);

        public Task SignOut(Session session);
    }
}
