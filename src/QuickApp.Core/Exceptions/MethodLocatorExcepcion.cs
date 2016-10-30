using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickApp.Core.Exceptions
{
    public class MethodLocatorExcepcion : Exception
    {
        public MethodLocatorExcepcion(Type serviceType, string methodName, string parameters, Exception inner)
            : base($"Error al buscar el método {methodName} en el servicio {serviceType.FullName}. Parametros: {parameters}", inner)
        {
        }
    }
}
