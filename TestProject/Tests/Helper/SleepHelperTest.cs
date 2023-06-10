using MainCore.Helper.Implementations.Base;
using MainCore.Helper.Interface;
using MainCore.Models.Database;
using MainCore.Models.Runtime;
using MainCore.Services.Implementations;
using MainCore.Services.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.Tests.Helper
{
    [TestClass]
    public class SleepHelperTest
    {
        private IRestClientManager restClientManager;
        private IAccessHelper accessHelper;

        [TestInitialize]
        public void Setup()
        {
            restClientManager = new RestClientManager();
            accessHelper = new AccessHelper();
        }

        [TestMethod]
        [Ignore("This test is for manual testing only")]
        public void TestProxy()
        {
            var client = restClientManager.Get(new ProxyInfo(new Access()
            {
                ProxyHost = "proxy ip",
                ProxyPort = 1,
                ProxyUsername = "proxy username",
                ProxyPassword = "proxy password"
            }));
            var result = accessHelper.IsValid(client);
            Assert.IsTrue(result);
        }
    }
}