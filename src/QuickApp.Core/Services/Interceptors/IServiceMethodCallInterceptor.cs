namespace QuickApp.Services.Interceptors
{
    public interface IServiceMethodCallInterceptor
    {
        void Intercept(Moment moment, CallContext context);
    }
}
