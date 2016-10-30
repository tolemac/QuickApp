using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickApp.Core.Exceptions
{
    public class ParameterCreationExcepcion : Exception
    {
        public ParameterCreationExcepcion(Type serviceType, string methodName, string parameters, Exception inner)
            : base($"Error al crear parametros para el método {methodName} en el servicio {serviceType.FullName}. Parametros: {parameters}", inner)
        {
        }
    }
}
