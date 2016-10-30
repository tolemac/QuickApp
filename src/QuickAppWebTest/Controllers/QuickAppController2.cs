using Microsoft.AspNetCore.Mvc;
using QuickApp;

namespace QuickAppWebTest.Controllers
{
    public class QuickAppController2
    {
        private readonly QuickAppConfiguration _app;

        public QuickAppController2(QuickAppConfiguration app)
        {
            _app = app;
        }

        [HttpPost]
        public dynamic CallServiceMethod(string serviceName, string methodName, [FromBody] dynamic payload)
        {
            var result = _app.CallServiceMethod(serviceName, methodName, payload);
            return result;
        }
    }
}
