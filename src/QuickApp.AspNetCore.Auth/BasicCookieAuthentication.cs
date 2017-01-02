using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace QuickApp.AspNetCore.Auth
{
    public interface IBasicCookieAuthentication
    {
        Task<bool> Login(string name, string password, bool persistCookie = false);
        Task Logoff();
        Task<dynamic> UserInfo();
    }

    public class BasicCookieAuthentication<TUser> : IBasicCookieAuthentication
    {
        private readonly BasicAuthConfiguration<TUser> _configuration;
        private readonly IServiceProvider _serviceProvider;

        
        public BasicCookieAuthentication(BasicAuthConfiguration<TUser> configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public async Task SignIn(TUser user, bool persistCookie)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _configuration.GetNameFunc(user))
            };
            var props = new AuthenticationProperties
            {
                IsPersistent = persistCookie,
                ExpiresUtc = DateTime.UtcNow.AddYears(1)
            };

            var httpContext = _serviceProvider.GetService<IHttpContextAccessor>().HttpContext;

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await httpContext.Authentication.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), props);
        }

        public async Task<bool> Login(string name, string password, bool persistCookie = false)
        {
            var myUser = _configuration.LocateUserByNamePasswordFunc(_serviceProvider, name, password);
            await SignIn(myUser, persistCookie);
            return true;
        }

        public async Task Logoff()
        {
            var httpContext = _serviceProvider.GetService<IHttpContextAccessor>().HttpContext;
            await httpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<dynamic> UserInfo()
        {
            var httpContext = _serviceProvider.GetService<IHttpContextAccessor>().HttpContext;
            
            return await Task.FromResult(_configuration.LocateUserByPrincipal(_serviceProvider, httpContext.User));
        }
    }
}
