using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace QuickApp.AspNetCore.Auth
{
    public class BasicCookieAuthentication
    {
        public const string AuthScheme = "QuickApp.BasicCookieAuthentication";
    }

    public class BasicCookieAuthentication<TUser>
    {
        private readonly BasicAuthConfiguration<TUser> _configuration;
        private readonly IServiceProvider _serviceProvider;

        
        public BasicCookieAuthentication(BasicAuthConfiguration<TUser> configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public async Task<bool> Login(string name, string password, bool persistCookie = false)
        {
            var myUser = _configuration.LocateUserByNamePasswordFunc(_serviceProvider, name, password);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _configuration.GetNameFunc(myUser))
            };
            var props = new AuthenticationProperties
            {
                IsPersistent = persistCookie,
                ExpiresUtc = DateTime.UtcNow.AddYears(1)
            };

            var httpContext = _serviceProvider.GetService<IHttpContextAccessor>().HttpContext;

            var identity = new ClaimsIdentity(claims, BasicCookieAuthentication.AuthScheme);
            await httpContext.Authentication.SignInAsync(BasicCookieAuthentication.AuthScheme, new ClaimsPrincipal(identity), props);
            return true;
        }

        public async void Logoff()
        {
            var httpContext = _serviceProvider.GetService<IHttpContextAccessor>().HttpContext;
            await httpContext.Authentication.SignOutAsync(BasicCookieAuthentication.AuthScheme);
        }
    }
}
