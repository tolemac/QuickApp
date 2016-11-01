using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using QuickApp.Services.Interceptors;

namespace QuickApp.Services
{
    internal class ServiceMethodCaller
    {
        private readonly ServiceContainer _container;

        public ServiceMethodCaller(ServiceContainer container)
        {
            _container = container;
        }

        public object Call(string serviceName, string methodName, object arguments)
        {
            return Call(serviceName, methodName, JObject.FromObject(arguments));
        }

        public object Call(string serviceName, string methodName, JObject arguments)
        {
            var serviceDescriptor = _container.GetServiceDescriptorByName(serviceName);
            
            var callContext = new CallContext(serviceDescriptor, methodName, arguments);

            var interceptors = serviceDescriptor.GetInterceptors().ToArray();

            if (CallInterceptorObjects(interceptors, callContext, Moment.Before))
                return callContext.Result;
            if (DistpatchBeforeInterceptors(serviceDescriptor, callContext))
                return callContext.Result;

            try
            {
                callContext.Result = MethodCaller.Call(serviceDescriptor.Instance, methodName, arguments);
            }
            catch (Exception ex)
            {
                callContext.Exception = ex;
                CallInterceptorObjects(interceptors, callContext, Moment.OnException);
                DistpatchOnExceptionInterceptors(serviceDescriptor, callContext);
                throw callContext.Exception;
            }

            if (!CallInterceptorObjects(interceptors, callContext, Moment.After))
                DistpatchAfterInterceptors(serviceDescriptor, callContext);

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

        private static bool DistpatchBeforeInterceptors(ServiceDescriptor serviceDescriptor, CallContext callContext)
        {
            var interceptors = serviceDescriptor.GetBeforeInterceptors(callContext.MethodName);
            foreach (var interceptor in interceptors)
            {
                interceptor.Action(callContext);
                if (callContext.Result != null)
                    return true;
            }
            return false;
        }

        private static bool DistpatchAfterInterceptors(ServiceDescriptor serviceDescriptor, CallContext callContext)
        {
            var interceptors = serviceDescriptor.GetAfterInterceptors(callContext.MethodName);
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
