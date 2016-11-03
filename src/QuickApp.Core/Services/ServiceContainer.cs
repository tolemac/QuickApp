using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickApp.Services
{
    public class ServiceContainer
    {
        private readonly Dictionary<string, ServiceDescriptor> _services = new Dictionary<string, ServiceDescriptor>();

        public ServiceContainer AddService(ServiceDescriptor serviceDescriptor)
        {
            _services.Add(serviceDescriptor.Name, serviceDescriptor);
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
