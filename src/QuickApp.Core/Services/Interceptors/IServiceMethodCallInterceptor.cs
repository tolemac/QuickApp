using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickApp.Services.Interceptors
{
    public interface IServiceMethodCallInterceptor
    {
        void Intercept(Moment moment, CallContext context);
    }
}
