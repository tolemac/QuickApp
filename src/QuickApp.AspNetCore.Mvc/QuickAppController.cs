using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace QuickApp.AspNetCore.Mvc
{
    public class QuickAppController
    {
        private readonly QuickApplication _app;

        public QuickAppController(QuickApplication app)
        {
            _app = app;
        }

        [HttpPost]
        public async Task<object> CallServiceMethod(string serviceName, string methodName, [FromBody] dynamic payload)
        {
            return await _app.CallServiceMethod(serviceName, methodName, payload).Result;
        }
    }
}
