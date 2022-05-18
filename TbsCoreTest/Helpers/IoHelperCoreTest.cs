using System;
using TbsCore.Helpers;
using Xunit;

namespace TbsCoreTest.Helpers
{
    public class IoHelperCoreTest
    {
        [Fact]
        public void TbsPathTest()
        {
            Assert.Equal(@$"{AppContext.BaseDirectory}Data", IoHelperCore.TbsPath);

            Assert.Equal(@$"{IoHelperCore.TbsPath}\db.sqlite", IoHelperCore.SqlitePath);
            Assert.Equal(@$"{IoHelperCore.TbsPath}\useragent.json", IoHelperCore.UseragentPath);
        }

        [Fact]
        public void UserDataPathTest()
        {
            string username = "TravianBot";
            string server = "travian.com";

            Assert.Equal(@$"{IoHelperCore.TbsPath}\{server}\{username}", IoHelperCore.UserDataPath(username, server));
            Assert.Equal(@$"{IoHelperCore.TbsPath}\{server}\{username}\tasks.json", IoHelperCore.UserTaskPath(username, server));
            Assert.Equal(@$"{IoHelperCore.TbsPath}\{server}\{username}\Cache", IoHelperCore.UserCachePath(username, server));

            string host = "1.2.23.24";
            Assert.Equal(@$"{IoHelperCore.TbsPath}\{server}\{username}\Cache\{host}", IoHelperCore.UserCachePath(username, server, host));
            Assert.Equal(@$"{IoHelperCore.TbsPath}\{server}\{username}\Cache\default", IoHelperCore.UserCachePath(username, server, ""));
        }
    }
}
