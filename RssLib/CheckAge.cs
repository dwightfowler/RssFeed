using RssLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssLib
{
    /// <summary>
    /// This could be done a dozen different ways. I'm just going to let the 
    /// programmer fill in the correct values. It makes each instance re-usable and puts
    /// no constraints on the constructor. You can instatiate a CheckAge class
    /// and get default results from it. Meaning, a new instance will not throw unexpected exceptions.
    /// </summary>
    public class CheckAge: IDisposable
    {

        /// <summary>
        /// Set the CurrentDateTime. This allows you to avoid an ever increasing system clock.
        /// It also allows mocking for tests.
        /// </summary>
        public DateTimeOffset CurrentDateTime { get; set; }

        /// <summary>
        /// The maximum age and feed can be to be considered Over Aged.
        /// <seealso cref="OverAge"/>
        /// </summary>
        public TimeSpan MaxAge { get; set; }
        
        /// <summary>
        /// The results of the age calculation.
        /// <seealso cref="MaxAge"/>
        /// </summary>
        public IList<bool> OverAge { get; private set; }
        
        /// <summary>
        /// The dictionary of company names and their RSS URI addresses 
        /// </summary>
        public IDictionary<string, string> Feeds { get; set; }

        /// <summary>
        ///  Default constructor populates all the member fields with default instances
        /// </summary>
        public CheckAge()
        {
            CurrentDateTime = DateTimeOffset.MinValue;
            MaxAge = TimeSpan.MaxValue;
            OverAge = new List<bool>();
            Feeds = new Dictionary<string, string>();
        }

        /// <summary>
        /// Using the fields of the class, calculate if the RSS feeds are over the 
        /// given maximum age.
        /// </summary>
        /// <param name="useNow">true - use the computer's current time</param>
        public void Check(bool useNow = false)
        {
            if (Feeds is null || Feeds.Count < 1)
            {
                OverAge = new List<bool>();
                return;
            }

            OverAge = new List<bool>(Feeds.Count);
            foreach(var feed in Feeds)
            {
                bool result; // = false
                var rssFeed = new RssFeed(feed.Value);
                if (result = rssFeed.Load())
                {
                    DateTimeOffset now = useNow ? DateTimeOffset.Now : CurrentDateTime;
                    result = now.Subtract(rssFeed.PubDate) > MaxAge;
                }
                OverAge.Add(result);
            }
        }

        /// <summary>
        /// Release the potentially large OverAge list and Feeds dictionary
        /// </summary>
        public void Dispose()
        {
            OverAge = null;
            Feeds = null;

            GC.SuppressFinalize(this);
        }
    }
}
