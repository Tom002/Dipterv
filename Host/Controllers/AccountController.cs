using Dipterv.Shared.Dto.SignIn;
using Dipterv.Shared.Interfaces.ComputeServices;
using Microsoft.AspNetCore.Mvc;
using Stl.Fusion.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace Templates.TodoApp.Host.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _authService;

        public AccountController(IAccountService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task SignIn([FromQuery] Session session, [FromBody] EmailPasswordDto signInDto, CancellationToken cancellationToken)
        {
            await _authService.SignIn(session, signInDto, cancellationToken);
        }

        [HttpPost]
        public async Task SignOut([FromBody] Session session)
        {
            await _authService.SignOut(session);
        }

        [HttpPost]
        public async Task RegisterUser(EmailPasswordDto signInDto, CancellationToken cancellationToken)
        {
            await _authService.RegisterUser(signInDto, cancellationToken);
        }

        [HttpPost]
        public async Task RegisterAdmin(EmailPasswordDto signInDto, CancellationToken cancellationToken)
        {
            var cookies = HttpContext.Request.Cookies;

            ///await _authService.RegisterAdmin(signInDto, cancellationToken);
        }
    }
}
