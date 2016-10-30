using Newtonsoft.Json.Linq;

namespace QuickApp
{
    public class ServiceMethodCaller
    {
        private readonly ServiceContainer _container;

        public ServiceMethodCaller(ServiceContainer container)
        {
            _container = container;
        }

        public object Call(string serviceName, string methodName, object parameters)
        {
            return Call(serviceName, methodName, JObject.FromObject(parameters));
        }

        public object Call(string serviceName, string methodName, JObject parameters)
        {
            var service = ResolveService(serviceName);
            
            return MethodCaller.Call(service, methodName, parameters);
        }

        private object ResolveService(string serviceName)
        {
            return _container.ResolveServiceByName(serviceName);
        }
    }
}
