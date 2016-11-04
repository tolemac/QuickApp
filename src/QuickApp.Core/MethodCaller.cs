using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using QuickApp.Core.Exceptions;

namespace QuickApp
{
    internal class MethodCaller
    {
        internal class CallMethodResult
        {
            public object Result { get; }
            public bool IsVoid { get; }

            public CallMethodResult(object result, bool isVoid)
            {
                Result = result;
                IsVoid = isVoid;
            }
        }

        private static bool IsAsyncMethod(MethodInfo method)
        {
            var attType = typeof(AsyncStateMachineAttribute);

            // Obtain the custom attribute for the method. 
            // The value returned contains the StateMachineType property. 
            // Null is returned if the attribute isn't present for the method. 
            var attrib = (AsyncStateMachineAttribute)method.GetCustomAttribute(attType);

            return attrib != null;
        }

        /// <summary>
        /// Llama a un metodo especifico de un servicio pasando los parametros en un objeto JObject
        /// </summary>
        /// <param name="srv">Servicio donde localizar el método</param>
        /// <param name="methodName">Nombre del metodo a llamar</param>
        /// <param name="parameters">Parametros del método en un JObject</param>
        /// <returns>El resultado de la llamada al método</returns>
        public static async Task<CallMethodResult> Call(object srv, string methodName, JObject parameters)
        {
            MethodInfo method;
            object[] methodParameters;

            try
            {
                method = LocateMethod(srv, methodName, parameters);
            }
            catch (Exception ex)
            {
                throw new MethodLocatorExcepcion(srv.GetType(), methodName, parameters.ToString(), ex);
            }

            try
            {
                methodParameters = CreateParameters(method.GetParameters(), parameters);
            }
            catch (Exception ex)
            {

                throw new ParameterCreationExcepcion(srv.GetType(), methodName, parameters.ToString(), ex);
            }

            var isAsync = IsAsyncMethod(method);
            var isVoidMethod = method.ReturnType == typeof(void) ||
                (method.ReturnType == typeof(Task));

            object result = null;
            try
            {
                if (isVoidMethod)
                {
                    if (isAsync)
                        await (dynamic)method.Invoke(srv, methodParameters);
                    else
                        method.Invoke(srv, methodParameters);
                }
                else
                {
                    if (isAsync)
                        result = await (dynamic)method.Invoke(srv, methodParameters);
                    else
                        result = method.Invoke(srv, methodParameters);
                }
                
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }

            return new CallMethodResult(result, isVoidMethod);
        }

        private static object[] CreateParameters(ParameterInfo[] methodParameters, JObject callParameters)
        {
            return callParameters == null
                ? null
                : methodParameters.Select(par =>
                    callParameters[par.Name] == null
                        ? par.DefaultValue
                        : callParameters[par.Name].ToObject(par.ParameterType)).ToArray();
        }

        private static MethodInfo LocateMethod(object srv, string nombreMetodo, JObject parametros)
        {
            var nombreParametros = parametros != null ? parametros.Properties().Select(prop => prop.Name).ToList() : new List<string>();

            var metodo = nombreParametros.Count == 0
                ? srv.GetType()
                    .GetMethods()
                    .First(
                        m =>
                            m.Name == nombreMetodo && !m.IsGenericMethod &&
                            m.GetParameters().Length == 0)
                : srv.GetType()
                    .GetMethods()
                    .First(
                        m =>
                            m.Name == nombreMetodo && !m.IsGenericMethod &&
                            m.GetParameters().Where(p => !p.HasDefaultValue).Select(p => p.Name).All(pName => nombreParametros.Contains(pName)));

            return metodo;
        }
    }
}
