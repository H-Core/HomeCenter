using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Notifiers.IntegrationTests
{
    [TestClass]
    public class RssNotifierTests
    {
        [TestMethod]
        public async Task Test()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            using var instance = new RssNotifier();

            instance.SetSetting("IntervalInMilliseconds", "1000");
            instance.SetSetting("Url", "https://www.upwork.com/ab/feed/topics/rss?securityToken=3046355554bbd7e304e77a4f04ec54ff90dcfe94eb4bb6ce88c120b2a660a42c47a42de8cfd7db2f3f4962ccb8c9a8d1bb2bff326e55b5b464816c9919c4e66c&userUid=749097038387695616&orgUid=749446993539981313");

            await Task.Delay(TimeSpan.FromSeconds(5), cancellationTokenSource.Token);

            var value = instance.GetModuleVariableValue("$rss_last_title$");
            Console.WriteLine($@"Rss Last Title: {value}");
        }
    }
}
