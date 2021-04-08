using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RssFeed.Source
{
    public class RssFeed
    {
        readonly Uri uri;
        private XmlDocument doc;
        public DateTimeOffset PubDate { get; private set; }

        public RssFeed(string uri)
        {
            if (uri.IsNullOrEmpty())
            {
                Console.Error.WriteLine($"RSS Feed URI is null or empty!");
                return;
            }

            if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri actualUri))
            {
                Console.Error.WriteLine($"RSS Feed URI \"{uri}\" is unusable!");
                return;
            }

            this.uri = actualUri;

        }

        public bool Load()
        {
            using HttpClient client = new();
            var rss = client.GetStringAsync(uri).Result;
            var rssDoc = new XmlDocument();
            rssDoc.LoadXml(rss);
            var pubDateNode = rssDoc.SelectSingleNode(@"rss/channel/pubDate");
            var formatter = CultureInfo.GetCultureInfo("en-US").DateTimeFormat;
            DateTimeOffset pubDate = DateTimeOffset.MinValue;

            if (!DateTimeOffset.TryParse(pubDateNode.InnerText, formatter, DateTimeStyles.AdjustToUniversal, out pubDate))
            {
                Console.Error.WriteLine($"ERROR Unable to parse RSS feed's pubDate: {pubDateNode.InnerText}");
                return false;
            }

            PubDate = pubDate;

            return true;
        }
    }
}
