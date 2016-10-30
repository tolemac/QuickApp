using System;
using Newtonsoft.Json.Linq;

namespace QuickApp
{
    public class QuickAppConfiguration
    {
        private readonly ServiceContainer _serviceContainer;
        private readonly ServiceMethodCaller _serviceMethodCaller;

        public QuickAppConfiguration()
        {
            _serviceContainer = new ServiceContainer();
            _serviceMethodCaller = new ServiceMethodCaller(_serviceContainer);
        }

        public QuickAppConfiguration AddService(Type serviceType, Func<object> creatorFunc)
        {
            _serviceContainer.AddService(serviceType, creatorFunc);
            return this;
        }

        public QuickAppConfiguration AddService(Type serviceType, string serviceName, Func<object> creatorFunc)
        {
            _serviceContainer.AddService(serviceType, serviceName, creatorFunc);
            return this;
        }

        public QuickAppConfiguration AddService(Type serviceType, object instance)
        {
            _serviceContainer.AddService(serviceType, instance);
            return this;
        }

        public object CallServiceMethod(string serviceName, string methodName, object parameters)
        {
            return CallServiceMethod(serviceName, methodName, JObject.FromObject(parameters));
        }

        public object CallServiceMethod(string serviceName, string methodName, JObject parameters)
        {
            return _serviceMethodCaller.Call(serviceName, methodName, parameters);
        }


    }
}
