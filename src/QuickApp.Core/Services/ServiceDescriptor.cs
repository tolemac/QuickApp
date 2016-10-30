using System;
using System.Collections.Generic;
using System.Linq;
using QuickApp.Services.Interceptors;

namespace QuickApp.Services
{
    public class ServiceDescriptor
    {
        private readonly List<InterceptorDescriptor> _interceptorDescriptors = 
            new List<InterceptorDescriptor>();
        private readonly List<IServiceMethodCallInterceptor> _interceptorObjects = 
            new List<IServiceMethodCallInterceptor>();
        private object _instance;
        public object Instance => _instance ?? (_instance = CreateInstance());

        public string Name { get; }
        public Type Type { get; }
        public Func<object> CreationFunc { get; }


        public ServiceDescriptor(string name, Type type, Func<object> creationFunc)
        {
            Name = NormaliceServiceName(name);
            Type = type;
            CreationFunc = creationFunc;
        }

        private static string NormaliceServiceName(string name)
        {
            if (name.StartsWith("I") && name.Length > 2 && char.IsUpper(name[2]))
                return name.Substring(1);
            return name;
        }

        private object CreateInstance()
        {
            return CreationFunc != null ? CreationFunc() : ServiceActivator.Activate(Type);
        }

        public void AddInterceptor(string methodName, Moment when, Action<CallContext> action)
        {
            _interceptorDescriptors.Add(new InterceptorDescriptor(methodName, when, action));
        }

        public void AddInterceptor(IServiceMethodCallInterceptor interceptor)
        {
            _interceptorObjects.Add(interceptor);
        }

        public IEnumerable<InterceptorDescriptor> GetBeforeInterceptors(string methodName)
        {
            return _interceptorDescriptors.Where(i => i.MethodName == methodName && i.Moment == Moment.Before);
        }

        public IEnumerable<IServiceMethodCallInterceptor> GetInterceptors()
        {
            return _interceptorObjects.ToArray();
        }

        public IEnumerable<InterceptorDescriptor> GetAfterInterceptors(string methodName)
        {
            return _interceptorDescriptors.Where(i => i.MethodName == methodName && i.Moment == Moment.After);
        }

        public IEnumerable<InterceptorDescriptor> GetOnExceptionInterceptors(string methodName)
        {
            return _interceptorDescriptors.Where(i => i.MethodName == methodName && i.Moment == Moment.OnException);
        }
    }
}
