using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickApp.Exceptions;
using QuickApp.Services.Interceptors;

namespace QuickApp.Services
{
    internal class ServiceMethodCaller
    {
        public async Task<dynamic> Call(CallContext callContext)
        {

            if (callContext.Service.MethodRequireAuth(callContext.MethodName) 
                && !callContext.HttpContext.User.Identity.IsAuthenticated)
            {
                throw new AccessDeniedException($"Access denied calling '{callContext.MethodName}' method of '{callContext.Service.Name}' service.");
                //throw new AccessDeniedException($"Access denied calling {callContext.Service.Name} service.");
            }

            var interceptors = callContext.Service.GetInterceptors().ToArray();

            if (CallInterceptorObjects(interceptors, callContext, Moment.Before))
                return callContext.Result;
            if (DistpatchBeforeInterceptors(callContext))
                return callContext.Result;

            MethodCaller.CallMethodResult result;
            try
            {
                result = await MethodCaller.Call(callContext.Service.Type, callContext.Service.Instance, callContext.MethodName, callContext.Arguments);
            }
            catch (Exception ex)
            {
                callContext.Exception = ex;
                CallInterceptorObjects(interceptors, callContext, Moment.OnException);
                DistpatchOnExceptionInterceptors(callContext.Service, callContext);
                throw callContext.Exception;
            }

            callContext.Result = result.Result;
            callContext.IsVoidMethod = result.IsVoid;


            if (!CallInterceptorObjects(interceptors, callContext, Moment.After))
                DistpatchAfterInterceptors(callContext);

            return callContext.Result;
        }

        private static bool CallInterceptorObjects(IEnumerable<IServiceMethodCallInterceptor> interceptorObjects, CallContext callContext, Moment moment)
        {
            var preResult = callContext.Result;
            foreach (var interceptor in interceptorObjects)
            {
                interceptor.Intercept(moment, callContext);
                if (callContext.Result != preResult)
                    return true;
            }
            return false;
        }

        private static void DistpatchOnExceptionInterceptors(ServiceDescriptor serviceDescriptor, CallContext callContext)
        {
            var interceptors = serviceDescriptor.GetOnExceptionInterceptors(callContext.MethodName);
            foreach (var interceptor in interceptors)
            {
                interceptor.Action(callContext);
            }
        }

        private static bool DistpatchBeforeInterceptors(CallContext callContext)
        {
            var interceptors = callContext.Service.GetBeforeInterceptors(callContext.MethodName);
            foreach (var interceptor in interceptors)
            {
                interceptor.Action(callContext);
                if (callContext.Result != null)
                    return true;
            }
            return false;
        }

        private static bool DistpatchAfterInterceptors(CallContext callContext)
        {
            var interceptors = callContext.Service.GetAfterInterceptors(callContext.MethodName);
            var preResult = callContext.Result;
            foreach (var interceptor in interceptors)
            {
                interceptor.Action(callContext);
                if (callContext.Result != preResult)
                    return true;
            }
            return false;
        }

    }
}

