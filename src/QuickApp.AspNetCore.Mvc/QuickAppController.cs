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
        public object CallServiceMethod(string serviceName, string methodName, [FromBody] dynamic payload)
        {
            return _app.CallServiceMethod(serviceName, methodName, payload);
        }
    }
}
