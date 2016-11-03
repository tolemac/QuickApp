using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using QuickApp.Services;
using QuickApp.Services.Interceptors;

namespace QuickApp
{
    public class QuickApplication
    {
        public IServiceProvider ServiceProvider { get; set; }
        private readonly ServiceContainer _serviceContainer;
        private readonly ServiceMethodCaller _serviceMethodCaller;

        //public QuickApplication() : this(null)
        //{

        //}

        public QuickApplication(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            _serviceContainer = new ServiceContainer();
            _serviceMethodCaller = new ServiceMethodCaller();
        }

        public QuickApplication AddService(ServiceDescriptor serviceDescriptor, Action<ServiceDescriptor> configureService)
        {
            configureService?.Invoke(serviceDescriptor);
            _serviceContainer.AddService(serviceDescriptor);
            return this;
        }

        public object CallServiceMethod(string serviceName, string methodName, object arguments)
        {
            return CallServiceMethod(serviceName, methodName, JObject.FromObject(arguments));
        }

        public object CallServiceMethod(string serviceName, string methodName, JObject arguments)
        {
            var serviceDescriptor = _serviceContainer.GetServiceDescriptorByName(serviceName);

            var httpContext = ((IHttpContextAccessor) ServiceProvider
                .GetService(typeof(IHttpContextAccessor))).HttpContext;

            var callContext = new CallContext(httpContext, serviceDescriptor, methodName, arguments);

            return _serviceMethodCaller.Call(callContext);
        }

        public QuickApplication AddInterceptor(string serviceName, string methodName, 
            Moment when, Action<CallContext> action)
        {
            _serviceContainer.GetServiceDescriptorByName(serviceName).AddInterceptor(methodName, when, action);
            return this;
        }

        public QuickApplication AddInterceptor(string serviceName, IServiceMethodCallInterceptor interceptor)
        {
            _serviceContainer.GetServiceDescriptorByName(serviceName).AddInterceptor(interceptor);
            return this;
        }
    }
}
