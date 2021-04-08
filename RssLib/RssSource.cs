using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssLib
{
    public class RssSource
    {
        public string Company { get; set; }
        public string Address { get; set; }

        public override string ToString()
        {
            return $"Company: {Company}, Address: {Address}";
        }
    }
}
