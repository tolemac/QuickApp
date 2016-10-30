using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickApp.Services
{
    public class ServiceContainer
    {
        private readonly Dictionary<string, ServiceDescriptor> _services = new Dictionary<string, ServiceDescriptor>();

        public ServiceContainer AddService<TService>(Func<TService> creatorFunc = null)
        {
            return AddService(typeof (TService), creatorFunc);
        }

        public ServiceContainer AddService(Type serviceType, Func<object> creatorFunc = null)
        {
            return AddService(serviceType, serviceType.Name, creatorFunc);
        }

        public ServiceContainer AddService(Type serviceType, string serviceName, Func<object> creatorFunc = null)
        {
            _services.Add(serviceName, new ServiceDescriptor(serviceName, serviceType, creatorFunc));
            return this;
        }

        public ServiceContainer AddService(Type serviceType, object instance)
        {
            return AddService(serviceType, serviceType.Name, instance);
        }

        public ServiceContainer AddService(object instance)
        {
            return AddService(instance.GetType(), instance);
        }

        public ServiceContainer AddService(Type serviceType, string serviceName, object instance)
        {
            _services.Add(serviceName, new ServiceDescriptor(serviceName, serviceType, () => instance));
            return this;
        }

        private bool MatchServiceName(string serviceToMatch, string currentServiceName)
        {
            return currentServiceName.Equals(serviceToMatch, StringComparison.CurrentCultureIgnoreCase)
                   || (currentServiceName.StartsWith("I")
                       && currentServiceName.Substring(1).Equals(serviceToMatch, StringComparison.CurrentCultureIgnoreCase));
        }

        public ServiceDescriptor GetServiceDescriptorByName(string name)
        {
            return _services.Where(p => MatchServiceName(name, p.Key)).Select(p => p.Value).FirstOrDefault();
        }

    }
}
