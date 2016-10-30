using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickApp.Services;
using QuickApp.Services.Interceptors;

namespace QuickAppWebTest
{
    public class MongoInterceptorUsingMethodsNames : InterceptorByMethodName
    {
        public void AfterFind(CallContext context)
        {
            context.Result.Add(new { name = "From", surname = "Class by method Interceptor" });
        }
    }
}
