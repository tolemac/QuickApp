using System;
using Microsoft.AspNetCore.Http;

namespace QuickApp.Services
{
    public class CallContext
    {
        public CallContext(HttpContext httpContext, ServiceDescriptor service, string methodName, dynamic arguments)
        {
            HttpContext = httpContext;
            Arguments = arguments;
            MethodName = methodName;
            Service = service;
        }

        public HttpContext HttpContext { get; }
        public dynamic Arguments { get; }
        public string MethodName { get; }
        public ServiceDescriptor Service { get; }
        public dynamic Result { get; set; }
        public bool IsVoidMethod { get; set; }
        public Exception Exception { get; set; }
    }
}
