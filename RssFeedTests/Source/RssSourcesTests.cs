using Microsoft.VisualStudio.TestTools.UnitTesting;
using RssFeed.Source;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RssFeed.Source.Tests
{
    [TestClass()]
    public class RssSourcesTests
    {
        const string fileName = "RssFeed.json";
        private int rssSourcesCount;

        [TestInitialize]
        public void MakeRssFile()
        {
            RssSource[] items = new RssSource[]
            {
                new RssSource() {Company = "DateLine NBC", Address = "https://podcastfeeds.nbcnews.com/dateline-nbc" },
                new RssSource() {Company = "Facebook", Address = "https://www.facebook.com/xml/x.xml" },
                new RssSource() {Company = "Apple", Address = "https://www.apple.com/xml/the-cool-feed.xml" },
                new RssSource() {Company = "Netflix", Address = "https://www.netflix.com/xml/netflix.xml" },
                new RssSource() {Company = "Google", Address = "https://www.google.com/xml/the-rssfeed.xml" },
                
            };
            rssSourcesCount = items.Length;

            try
            {
                using StreamWriter writer = new StreamWriter(fileName);
                var json = JsonSerializer.Serialize<RssSource[]>(items);
                writer.Write(json);
                writer.Close();

                Console.WriteLine("Created RSS file: \"{fileName}\"");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR Creating RSS file: \"{fileName}\"\n{ex.Message}");
            }
        }

        [TestCleanup]
        public void RemoveUpRssFile()
        {
            string msg = $"RSS file \"{fileName}\" is missing";

            if (File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                    msg = $"Deleted {fileName}";
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Test Cleanup:\n{ex.Message}");
                }
            }

            Console.WriteLine(msg);
        }

        [TestMethod()]
        public void RssListSourceTest()
        {
            var sources = new RssSources(fileName);
            Assert.AreEqual(sources.Sources.Count, rssSourcesCount, $"Sources file count {sources.Sources.Count} != {rssSourcesCount}");
            foreach (var source in sources.Sources)
            {
                Console.WriteLine(source);
                Assert.IsTrue(source.Key is Object && source.Key.Length > 0, "Empty Company entry!");
                Assert.IsTrue(source.Value is Object && source.Value.Length > 0, "Empty Address entry!");
            }
        }

        [TestMethod]
        public void RssFeedTest()
        {
            var sources = new RssSources(fileName);
            Assert.AreEqual(sources.Sources.Count, rssSourcesCount, $"Sources file count {sources.Sources.Count} != {rssSourcesCount}");
            var key = sources.Sources.Keys.First();
            var uri = sources.Sources[key];
            var feed = new RssFeed(uri);
            Assert.IsTrue(feed.Load());
            Assert.IsTrue(feed.PubDate > DateTimeOffset.MinValue);
        }
    }
}