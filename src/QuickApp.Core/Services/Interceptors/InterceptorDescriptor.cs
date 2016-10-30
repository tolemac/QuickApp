using System;

namespace QuickApp.Services.Interceptors
{
    public class InterceptorDescriptor
    {
        public InterceptorDescriptor(string methodName, Moment moment, Action<CallContext> action)
        {
            Moment = moment;
            Action = action;
            MethodName = methodName;
        }

        public string MethodName { get; }
        public Moment Moment { get; }
        public Action<CallContext> Action { get; }
    }
}
