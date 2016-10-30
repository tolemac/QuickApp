using Microsoft.AspNetCore.Mvc;

namespace QuickApp.Web.Mvc
{
    public class QuickAppController
    {
        private readonly QuickAppConfiguration _app;

        public QuickAppController(QuickAppConfiguration app)
        {
            _app = app;
        }

        [HttpPost]
        public object CallServiceMethod(string serviceName, string methodName, [FromBody] dynamic payload)
        {
            return _app.CallServiceMethod(serviceName, methodName, payload);
        }
    }
}
