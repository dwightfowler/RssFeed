﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace RssLib
{
    public class RssSources
    {
        public readonly Dictionary<String, String> Sources;

        public RssSources(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Sources = new Dictionary<string, string>();
                return;
            }

            using StreamReader reader = new(filePath);
            var items = JsonSerializer.Deserialize<RssSource[]>(reader.ReadToEnd());
            Sources = new Dictionary<string, string>(items.Length);
            foreach (var item in items)
            {
                Sources.Add(item.Company, item.Address);
            }
        }
    }
}
