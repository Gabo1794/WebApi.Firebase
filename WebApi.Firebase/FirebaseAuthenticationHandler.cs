using FirebaseAdmin.Auth;
using FirebaseAdmin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebApi.Firebase
{
    public class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly FirebaseApp _firebaseApp;

        public FirebaseAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, FirebaseApp firebaseApp) : base(options, logger, encoder, clock)
        {
            _firebaseApp = firebaseApp;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                if (!Context.Request.Headers.ContainsKey("Authorization"))
                {
                    return AuthenticateResult.NoResult();
                }

                string bearerToken = Context.Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(bearerToken) || !bearerToken.StartsWith("Bearer "))
                {
                    return AuthenticateResult.Fail("Invalid scheme.");
                }

                string token = bearerToken.Substring("Bearer ".Length);

                FirebaseToken firebaseToken = await FirebaseAuth.GetAuth(_firebaseApp).VerifyIdTokenAsync(token);

                return AuthenticateResult.Success(CreateAuthenticationTicket(firebaseToken));
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
        }

        private AuthenticationTicket CreateAuthenticationTicket(FirebaseToken firebaseToken)
        {
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>()
            {
                new ClaimsIdentity(ToClaims(firebaseToken.Claims), nameof(ClaimsIdentity))
            });

            return new AuthenticationTicket(claimsPrincipal, JwtBearerDefaults.AuthenticationScheme);
        }

        private IEnumerable<Claim> ToClaims(IReadOnlyDictionary<string, object> claims)
        {
            return new List<Claim>
            {
                new Claim("id", claims["user_id"].ToString() ?? String.Empty),
                new Claim("email", claims["email"].ToString() ?? String.Empty),
                new Claim("name", claims["name"].ToString() ?? String.Empty),
            };
        }
    }
}
