using System.Threading.Tasks;

namespace QuickApp.Tests
{
    public class TestObject
    {
        public string Property1 { get; set; }
        public bool Property2 { get; set; }
        public object Property3 { get; set; }
        
    }

    public interface ITestService
    {
        void VoidMethodWithoutParams();
        void VoidMethodWithParams(string param1, TestObject param2, string param3 = null);
        TestObject MethodWithoutParams();
        TestObject MethodWithParams(string param1, TestObject param2, string param3 = null);
        Task AsyncVoidMethodWithParams(string param1, TestObject param2, string param3 = null);
        Task<TestObject> AsyncMethodWithParams(string param1, TestObject param2, string param3 = null);
    }

    public class TestService : ITestService
    {
        public static TestObject NoParamsResult = new TestObject
        {
            Property1 = "dummy",
            Property2 = true,
            Property3 = new { name = "test" }
        };

        public void MethodNotPublishOnInterface()
        {

        }

        public void VoidMethodWithoutParams()
        {

        }

        public void VoidMethodWithParams(string param1, TestObject param2, string param3 = null)
        {
        }

        public TestObject MethodWithoutParams()
        {
            return NoParamsResult;
        }

        public TestObject MethodWithParams(string param1, TestObject param2, string param3 = null)
        {
            return param2;
        }

        public async Task AsyncVoidMethodWithParams(string param1, TestObject param2, string param3 = null)
        {
            await Task.Run(() => { });
        }

        public async Task<TestObject> AsyncMethodWithParams(string param1, TestObject param2, string param3 = null)
        {
            return await Task.Run(() => param2);
        }
    }
}
