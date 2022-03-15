using Dipterv.Dal.DbContext;
using Dipterv.Dal.Model;
using Dipterv.Shared.Dto.SignIn;
using Dipterv.Shared.Enum;
using Dipterv.Shared.Interfaces.ComputeServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Stl.Fusion.Authentication;
using Stl.Fusion.Authentication.Commands;
using System.Collections.Immutable;
using System.Security.Claims;

namespace Dipterv.Bll.Services
{
    public class AccountService : IAccountService
    {
        public string[] IdClaimKeys { get; set; } = { ClaimTypes.NameIdentifier };
        public string[] NameClaimKeys { get; set; } = { ClaimTypes.Name };
        public TimeSpan SessionInfoUpdatePeriod { get; set; } = TimeSpan.FromSeconds(30);

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthBackend _authBackend;
        private readonly IAuth _auth;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<long>> _roleManager;
        private readonly FusionDbContext _dbContext;

        public AccountService(
            IHttpContextAccessor httpContextAccessor,
            IAuthBackend authBackend,
            IAuth auth,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole<long>> roleManager,
            FusionDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _authBackend = authBackend;
            _auth = auth;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dbContext = dbContext; 
        }

        public async Task SignIn(Session session, EmailPasswordDto signInDto, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(signInDto.Email);
            if (user is ApplicationUser)
            {
                var sessionInfo = await _auth.GetSessionInfo(session, cancellationToken);
                if (sessionInfo.IsAuthenticated)
                    throw new Exception("You are already signed in");

                var signInResult = await _signInManager.CheckPasswordSignInAsync(user, signInDto.Password, lockoutOnFailure: false);
                if (signInResult.Succeeded)
                {
                    var claims = await _userManager.GetClaimsAsync(user);
                    var roles = await _userManager.GetRolesAsync(user);
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    foreach (var role in roles)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, role));
                    }
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                    var principal = new ClaimsPrincipal(identity);

                    var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
                    var userAgent = _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("User-Agent", out var userAgentValues)
                        ? userAgentValues.FirstOrDefault() ?? ""
                        : "";

                    var mustUpdateSessionInfo =
                        !StringComparer.Ordinal.Equals(sessionInfo.IPAddress, ipAddress)
                        || !StringComparer.Ordinal.Equals(sessionInfo.UserAgent, userAgent);
                    if (mustUpdateSessionInfo)
                    {
                        var setupSessionCommand = new SetupSessionCommand(session, ipAddress, userAgent);
                        await _authBackend.SetupSession(setupSessionCommand, cancellationToken);
                    }

                    var fusionUser = new User(session.Id);
                    var (newUser, authenticatedIdentity) = CreateFusionUser(fusionUser, principal, CookieAuthenticationDefaults.AuthenticationScheme);
                    var signInCommand = new SignInCommand(session, newUser, authenticatedIdentity);

                    await _authBackend.SignIn(signInCommand, cancellationToken);
                }
            }
        }

        protected virtual (User User, UserIdentity AuthenticatedIdentity) CreateFusionUser(User user, ClaimsPrincipal httpUser, string schema)
        {
            var httpUserIdentityName = httpUser.Identity?.Name ?? "";
            var claims = httpUser.Claims.ToImmutableDictionary(c => c.Type, c => c.Value);
            var id = FirstClaimOrDefault(claims, IdClaimKeys) ?? httpUserIdentityName;
            var name = FirstClaimOrDefault(claims, NameClaimKeys) ?? httpUserIdentityName;
            var identity = new UserIdentity(schema, id);
            var identities = ImmutableDictionary<UserIdentity, string>.Empty.Add(identity, "");

            user = new User("", name)
            {
                Claims = claims,
                Identities = identities
            };
            return (user, identity);
        }

        protected static string? FirstClaimOrDefault(IReadOnlyDictionary<string, string> claims, string[] keys)
        {
            foreach (var key in keys)
                if (claims.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value))
                    return value;
            return null;
        }

        public async Task SignOut(Session session)
        {
            var signOutCommand = new SignOutCommand(session);
            await _auth.SignOut(signOutCommand);
        }

        public async Task RegisterUser(EmailPasswordDto emailPasswordDto, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(emailPasswordDto.Email);
            if (user == null)
            {
                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var customer = new Customer
                        {
                            AccountNumber = Guid.NewGuid().ToString().Substring(0, 10),
                        };
                        _dbContext.Customers.Add(customer);

                        await _dbContext.SaveChangesAsync();

                        var newUser = new ApplicationUser 
                        {
                            UserName = emailPasswordDto.Email,
                            Email = emailPasswordDto.Email,
                            EmailConfirmed = true,
                            UserType = UserType.Customer,
                            Customer = customer,
                        };

                        await _userManager.CreateAsync(newUser, emailPasswordDto.Password);

                        if(!await _roleManager.RoleExistsAsync("Customer"))
                        {
                            var result = await _roleManager.CreateAsync(new IdentityRole<long> { Name = "Customer" });
                        }
                        await _userManager.AddToRoleAsync(newUser, "Customer");

                        var claims = new List<Claim>();
                        claims.Add(new Claim("order_product", "true"));
                        claims.Add(new Claim("customer_id", customer.CustomerId.ToString()));

                        await _userManager.AddClaimsAsync(newUser, claims);

                        await transaction.CommitAsync();
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                    }
                }
            }
        }

        public async Task RegisterAdmin(EmailPasswordDto emailPasswordDto, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(emailPasswordDto.Email);
            if (user == null)
            {
                var newUser = new ApplicationUser 
                { 
                    UserName = emailPasswordDto.Email,
                    Email = emailPasswordDto.Email,
                    EmailConfirmed = true,
                    UserType = UserType.Admin
                };

                var registerResult = await _userManager.CreateAsync(newUser, emailPasswordDto.Password);

                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole<long> { Name = "Admin" });
                }
                await _userManager.AddToRoleAsync(newUser, "Admin");

                var claims = new List<Claim>();
                claims.Add(new Claim("comment_delete", "true"));

                await _userManager.AddClaimsAsync(newUser, claims);
            }
        }
    }
}
