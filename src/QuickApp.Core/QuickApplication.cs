using System;
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

        public QuickApplication() : this(null)
        {

        }

        public QuickApplication(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            _serviceContainer = new ServiceContainer();
            _serviceMethodCaller = new ServiceMethodCaller(_serviceContainer);
        }

        public QuickApplication AddService(Type serviceType, Func<object> creatorFunc)
        {
            _serviceContainer.AddService(serviceType, creatorFunc);
            return this;
        }

        public QuickApplication AddService(Type serviceType, string serviceName, Func<object> creatorFunc)
        {
            _serviceContainer.AddService(serviceType, serviceName, creatorFunc);
            return this;
        }

        public QuickApplication AddService(Type serviceType, object instance)
        {
            _serviceContainer.AddService(serviceType, instance);
            return this;
        }

        public object CallServiceMethod(string serviceName, string methodName, object arguments)
        {
            return CallServiceMethod(serviceName, methodName, JObject.FromObject(arguments));
        }

        public object CallServiceMethod(string serviceName, string methodName, JObject arguments)
        {
            return _serviceMethodCaller.Call(serviceName, methodName, arguments);
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
