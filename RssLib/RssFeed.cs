using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RssLib
{
    /// <summary>
    /// <p>An single instance of an RSS feed indicated by the URI string passed into the constructor.</p>
    /// The URI passed in must be an absolute path.
    /// </summary>
    public class RssFeed : IDisposable
    {
        readonly Uri uri;
        private XmlDocument doc;

        /// <summary>
        /// The RSS feed channel's publication date
        /// </summary>
        public DateTimeOffset PubDate { get; private set; }

        /// <summary>
        /// The RSS feed's title</br>
        /// Will be null if loading RSS Feed fails
        /// </summary>
        public String Title { get; private set; }

        /// <summary>
        /// The RSS feed's Description</br>
        /// Will be null if loading RSS Feed fails
        /// </summary>
        public String Description { get; private set; }

        /// <summary>
        /// <p>Construct an instance of an RSS Feed using the URI endpoint given as a string.</p>
        /// The URI must be an absolute path.
        /// </summary>
        /// <param name="uri">An absolute URI path to the RSS feed. An invalid value will return an instance with a PubDate = DateTimeOffset.MinValue</param>
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
            bool result = true;
            DateTimeOffset pubDate = DateTimeOffset.MinValue;

            try
            {
                using HttpClient client = new();
                var rss = client.GetStringAsync(uri).Result;
                doc = new XmlDocument();
                doc.LoadXml(rss);
                var nodeChannel = doc.SelectSingleNode(@"rss/channel");
                var nodeDates = nodeChannel.SelectNodes(@"pubDate | lastBuildDate");
                var nodePubDate = nodeDates.Item(0);
                var nodeTitle = nodeChannel.SelectSingleNode(@"title");
                var nodeDescription = nodeChannel.SelectSingleNode(@"description");

                var formatter = CultureInfo.GetCultureInfo("en-US").DateTimeFormat;
                if (!DateTimeOffset.TryParse(nodePubDate.InnerText, formatter, DateTimeStyles.AdjustToUniversal, out pubDate))
                {
                    Console.Error.WriteLine($"ERROR Unable to parse RSS feed's pubDate: {nodePubDate.InnerText}");
                    result = false;
                }

                Title = nodeTitle.InnerText.Trim();
                Description = nodeDescription.InnerText.Trim();

                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"ERROR:\n{ex.Message}");
                return false;
            }
            finally
            {
                PubDate = pubDate;
            }
        }

        public override string ToString()
        {
            return $"Title: {Title}\nDescription: {Description}\nPublication Date: {PubDate}";
        }

        public void Dispose()
        {
            // release the potentially large XmlDocument object
            doc = null;

            GC.SuppressFinalize(this);
        }
    }
}
