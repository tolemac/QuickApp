using System;

namespace QuickApp.Services
{
    public class CallContext
    {
        public CallContext(ServiceDescriptor service, string methodName, dynamic arguments)
        {
            Arguments = arguments;
            MethodName = methodName;
            Service = service;
        }

        public dynamic Arguments { get; }
        public string MethodName { get; }
        public ServiceDescriptor Service { get; }
        public dynamic Result { get; set; }
        public Exception Exception { get; set; }
    }
}
