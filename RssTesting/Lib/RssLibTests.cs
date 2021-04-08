using Microsoft.VisualStudio.TestTools.UnitTesting;
using RssLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RssTesting.Lib.Tests
{
    [TestClass()]
    public class RssLibTests
    {
        const string fileName = "RssFeed.json";
        private int rssSourcesCount;

        [TestInitialize]
        public void MakeRssFile()
        {
            RssSource[] items = new RssSource[]
            {
                new RssSource() {Company = "DateLine NBC", Address = "https://podcastfeeds.nbcnews.com/dateline-nbc" },
                new RssSource() {Company = "The Daily by the New Your Times", Address = "http://rss.art19.com/the-daily" },
                new RssSource() {Company = "The Experiment", Address = "http://feeds.wnyc.org/experiment_podcast" },
                new RssSource() {Company = "Netflix SEC Filings", Address = "https://ir.netflix.net/rss/SECFiling.aspx?Exchange=CIK&Symbol=0001065280" },
                new RssSource() {Company = "Google", Address = "https://feeds.feedburner.com/google/think" },
                
            };
            rssSourcesCount = items.Length;

            try
            {
                using StreamWriter writer = new StreamWriter(fileName);
                var json = JsonSerializer.Serialize<RssSource[]>(items);
                writer.Write(json);
                writer.Close();

                Console.WriteLine($"Created RSS file: \"{fileName}\"");
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
                    msg = $"Deleted RSS file: {fileName}";
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
            foreach (var source in sources.Sources)
            {
                var uri = source.Value;
                using var feed = new RssFeed(uri);
                Assert.IsTrue(feed.Load());
                Assert.IsTrue(feed.PubDate > DateTimeOffset.MinValue);

                Console.WriteLine(feed);
            }
        }

        [TestMethod]
        public void CalcAgeTest()
        {
            var sources = new RssSources(fileName);
            Assert.AreEqual(sources.Sources.Count, rssSourcesCount, $"Sources file count {sources.Sources.Count} != {rssSourcesCount}");

            foreach (var source in sources.Sources)
            {
                var uri = source.Value;
                using var feed = new RssFeed(uri);
                Assert.IsTrue(feed.Load());
                Assert.IsTrue(feed.PubDate > DateTimeOffset.MinValue);

                Console.WriteLine(feed);
            }

            using var aging = new CheckAge()
            {
                CurrentDateTime = DateTimeOffset.Now,
                MaxAge = TimeSpan.FromDays(2),
                Feeds = sources.Sources
            };

            aging.Check();

            var companyEnum = sources.Sources.Keys.GetEnumerator();
            foreach (var isOverAge in aging.OverAge)
            {
                string companyName = "<unknown>";
                if (companyEnum.MoveNext())
                {
                    companyName = companyEnum.Current;
                }
                Console.WriteLine($"\"{companyName}\" RSS Feed is{(isOverAge?" ":" not ")}over {aging.MaxAge.Days} days");
            }
        }
    }
}