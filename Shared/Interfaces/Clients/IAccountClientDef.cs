using Dipterv.Shared.Dto.SignIn;
using RestEase;
using Stl.Fusion.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace Dipterv.Shared.Interfaces.Clients
{
    [BasePath("account")]
    public interface IAccountClientDef
    {
        [Post("signIn")]
        public Task SignIn([Query] Session session, [Body] EmailPasswordDto signInDto, CancellationToken cancellationToken);

        [Post("registerUser")]
        public Task RegisterUser([Body] EmailPasswordDto emailPasswordDto, CancellationToken cancellationToken);

        [Post("registerAdmin")]
        public Task RegisterAdmin([Body] EmailPasswordDto emailPasswordDto, CancellationToken cancellationToken);

        [Post("signOut")]
        public Task SignOut([Body] Session session);
    }
}
