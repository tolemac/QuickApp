using System;

namespace QuickApp
{
    internal class ServiceActivator
    {
        public static object Activate(Type serviceType)
        {
            return Activator.CreateInstance(serviceType);
        }
    }
}
