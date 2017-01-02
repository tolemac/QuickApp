using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using QuickApp.Exceptions;

namespace QuickApp.Tests
{
    public class MethodCallerTests
    {
        [Fact]
        public async Task CallVoidMethodWithoutParams()
        {
            var srv = new TestService();
            var parameters = JObject.FromObject(new {});
            var result = await MethodCaller.Call(typeof(ITestService), srv, "VoidMethodWithoutParams", parameters);
            Assert.True(result.IsVoid);
            Assert.Null(result.Result);
        }

        [Fact]
        public async Task CallVoidMethodWithParams()
        {
            var srv = new TestService();
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = new TestObject(),
                param3 = (string)null
            });
            var result = await MethodCaller.Call(typeof(ITestService), srv, "VoidMethodWithParams", parameters);
            Assert.True(result.IsVoid);
            Assert.Null(result.Result);
        }

        [Fact]
        public async Task CallVoidMethodWithDefaultParams()
        {
            var srv = new TestService();
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = new TestObject()
            });
            var result = await MethodCaller.Call(typeof(ITestService), srv, "VoidMethodWithParams", parameters);
            Assert.True(result.IsVoid);
            Assert.Null(result.Result);
        }

        [Fact]
        public async Task CallMethodCrashIfMethodNotExistsOrNoParameterMatchs()
        {
            var srv = new TestService();
            var parameters = JObject.FromObject(new
            {
                param1 = ""
            });
            await Assert.ThrowsAsync<MethodLocatorExcepcion>(async () =>
            {
                await MethodCaller.Call(typeof(ITestService), srv, "VoidMethodWithParams", parameters);
            });
            await Assert.ThrowsAsync<MethodLocatorExcepcion>(async () =>
            {
                await MethodCaller.Call(typeof(ITestService), srv, "NOTEXISTS-METHOD", parameters);
            });
        }

        [Fact]
        public async Task CallMethodCrashIfCantBuildMethodParameters()
        {
            var srv = new TestService();
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = "Type Mistmatch"
            });
            await Assert.ThrowsAsync<ParameterCreationExcepcion>(async () =>
            {
                await MethodCaller.Call(typeof(ITestService), srv, "VoidMethodWithParams", parameters);
            });
        }

        [Fact]
        public async Task CallMethodWithoutParams()
        {
            var srv = new TestService();
            var parameters = JObject.FromObject(new { });
            var result = await MethodCaller.Call(typeof(ITestService), srv, "MethodWithoutParams", parameters);
            Assert.False(result.IsVoid);
            Assert.Equal(TestService.NoParamsResult, result.Result);
        }

        [Fact]
        public async Task CallMethodWithParams()
        {
            var srv = new TestService();
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = TestService.NoParamsResult
            });
            var result = await MethodCaller.Call(typeof(ITestService), srv, "MethodWithParams", parameters);
            Assert.False(result.IsVoid);
            Assert.Equal(JsonConvert.SerializeObject(TestService.NoParamsResult), 
                JsonConvert.SerializeObject(result.Result));
        }

        [Fact]
        public async Task CallAsyncVoidMethodWithParams()
        {
            var srv = new TestService();
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = new TestObject(),
                param3 = (string)null
            });
            var result = await MethodCaller.Call(typeof(ITestService), srv, "AsyncVoidMethodWithParams", parameters);
            Assert.True(result.IsVoid);
            Assert.Null(result.Result);
        }
        
        [Fact]
        public async Task CallAsyncMethodWithParams()
        {
            var srv = new TestService();
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = TestService.NoParamsResult
            });
            var result = await MethodCaller.Call(typeof(ITestService), srv, "AsyncMethodWithParams", parameters);
            Assert.False(result.IsVoid);
            Assert.Equal(JsonConvert.SerializeObject(TestService.NoParamsResult),
                JsonConvert.SerializeObject(result.Result));
        }

        [Fact]
        public async Task CallMethodNotPublishOnInterfaceCrash()
        {
            var srv = new TestService();
            var parameters = JObject.FromObject(new { });

            await Assert.ThrowsAsync<MethodLocatorExcepcion>(async () =>
            {
                await MethodCaller.Call(typeof(ITestService), srv, "MethodNotPublishOnInterface", parameters);
            });

        }
    }
}
