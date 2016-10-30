using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickApp.Services;
using QuickApp.Services.Interceptors;

namespace QuickAppWebTest
{
    public class MongoInterceptorImplementingInterface : IServiceMethodCallInterceptor
    {
        public void Intercept(Moment moment, CallContext context)
        {
            if (context.MethodName == "Find" && moment == Moment.After)
            {
                context.Result.Add(new { name = "From", surname = "Class Interceptor" });
            }
        }
    }
}
