using System.Reflection;

namespace QuickApp.Services.Interceptors
{
    public class InterceptorByMethodName : IServiceMethodCallInterceptor
    {
        public void Intercept(Moment moment, CallContext context)
        {
            var method = GetType().GetMethod(moment + context.MethodName);
            method?.Invoke(this, new object[] {context});
        }
    }
}
