using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssFeed
{
    class Utilities
    {
    }

    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string val)
        {
            return (val is null || val.Length < 1);
        }
    }
}
