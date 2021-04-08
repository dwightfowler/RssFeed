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
Instantiate an instance of the `CheckAge` class. The library expects the given `Dictionary<string, string>` to be formatted as Key=CompanyName, Value=AbsoluteURL.
