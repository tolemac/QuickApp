using System;

namespace QuickApp.Services
{
    internal class ServiceActivator
    {
        public static object Activate(Type serviceType)
        {
            return Activator.CreateInstance(serviceType);
        }
    }
}
