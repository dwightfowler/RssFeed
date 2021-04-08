using RssLib;
using System;
using System.Collections.Generic;

namespace RssApp
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: RssApp <Description> <URL> <Days>\n" +
                    "  <Description> \"My RSS Feed\"\n" +
                    "  <URL>          https://www.myrssfeed.com/myrss.rss \n" +
                    "  <Days>         2");

                return 1;
            }

            var kv = KeyValuePair.Create<string, string>(args[0], args[1]);
            var feeds = new Dictionary<string, string>(new[] { kv });
            int days = Int32.Parse(args[2]);

            CheckAge checker = new ()
            {
                CurrentDateTime = DateTimeOffset.Now,
                MaxAge = TimeSpan.FromDays(days),
                Feeds = feeds
            };

            checker.Check();

            Console.WriteLine($"RSS Feed: \"{kv.Key}\"\nURL: {kv.Value}\nIs{(checker.OverAge[0] ? " " : " not")} over {days} days old.");

            return 0;
        }
    }
}
