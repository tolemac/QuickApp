using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickApp
{
    public class ServiceContainer
    {
        private readonly List<Tuple<Type, string, Func<object>>> _serviceList = new List<Tuple<Type, string, Func<object>>>();

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
            _serviceList.Add(new Tuple<Type, string, Func<object>>(serviceType, serviceName, creatorFunc));
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
            _serviceList.Add(new Tuple<Type, string, Func<object>>(serviceType, serviceName, () => instance));
            return this;
        }

        private bool MatchServiceName(string serviceToMatch, string currentServiceName)
        {
            return currentServiceName.Equals(serviceToMatch, StringComparison.CurrentCultureIgnoreCase)
                   || (currentServiceName.StartsWith("I")
                       && currentServiceName.Substring(1).Equals(serviceToMatch, StringComparison.CurrentCultureIgnoreCase));
        }

        private Tuple<Type, string, Func<object>> GetServiceTupleByName(string name)
        {
            return _serviceList.FirstOrDefault(t => MatchServiceName(name, t.Item2));
        }

        public object ResolveServiceByName(string name)
        {
            var tuple = GetServiceTupleByName(name);

            if (tuple.Item3 != null)
            {
                return tuple.Item3();
            }

            return ServiceActivator.Activate(tuple.Item1);

        }
    }
}
