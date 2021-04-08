# RssFeed Libary
Check RSS feeds for inactivity

This is a .NET 5.0 library. It allows you to check the age of a Dictionary of RSS feeds.

## How to use the Command-Line Tool:

~~~~dos
C:\> RssApp.exe "The Daily by The New York Times" http://rss.art19.com/the-daily 2 
RSS Feed: "The Daily by The New York Times"
URL: http://rss.art19.com/the-daily
Is not over 2 days old.
C:\> 
~~~~

# How it Works
Instantiate the `CheckAge` class. The library expects the `Feeds` property to be a `Dictionary<string, string>`. It must be formatted as Key=CompanyName, Value=AbsoluteURL.

An example of how to fill your instance using the `args[]` in a console app:

~~~~C#
using RssLib;
using System;
using System.Collections.Generic;
  .
  .
  .
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

~~~~

Using the `CheckAge` class is a three step process:
  1. Instantiate the class `new CheckAge();`
  2. Set your input fields:
      1. `CurrentDateTime` - the date and time of the moment you want to compare against your RSS feed.
      2. `MaxAge` - a `TimeSpan` denoting the maximum age you are considering.
      3. `Feeds` - a `Dictionary<string, string>` holding your company name and its' RSS feed URL.
  3. Call the `Check()` method on your instance.

The `Check()` method is expensive. It makes a network call to each or URLs. It receives the XML string, creates an `XmlDocument`, and traverses the XML using XPath.
