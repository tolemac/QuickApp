using System;
using System.Security.Claims;
using System.Security.Principal;

namespace QuickApp.AspNetCore.Auth
{
    public class BasicAuthConfiguration<TUser>
    {
        public Func<IServiceProvider, string, string, TUser> LocateUserByNamePasswordFunc { get; private set; }
        //public Func<IServiceProvider, string, TUser> LocateUserByNameFunc { get; private set; }
        public Func<TUser, string> GetNameFunc { get; private set; }
        public Func<IServiceProvider, ClaimsPrincipal, TUser> LocateUserByPrincipal { get; private set; }

        public BasicAuthConfiguration<TUser> SetLocateUserByNamePasswordFunc(
            Func<IServiceProvider, string, string, TUser> func)
        {
            LocateUserByNamePasswordFunc = func;
            return this;
        }

        public BasicAuthConfiguration<TUser> SetLocateUserByPrincipal(
            Func<IServiceProvider, ClaimsPrincipal, TUser> func)
        {
            LocateUserByPrincipal = func;
            return this;
        }

        //public BasicAuthConfiguration<TUser> SetLocateUserByNameFunc(Func<IServiceProvider, string, TUser> func)
        //{
        //    LocateUserByNameFunc = func;
        //    return this;
        //}

        public BasicAuthConfiguration<TUser> SetGetNameFunc(Func<TUser, string> func)
        {
            GetNameFunc = func;
            return this;
        }


    }
}
