using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssFeedLib
{
    internal static class StringExtensions
    {
        internal static bool IsNullOrEmpty(this string val)
        {
            return (val is null || val.Length < 1);
        }
    }
}
