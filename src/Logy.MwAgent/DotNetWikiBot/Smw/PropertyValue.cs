using System;

namespace Logy.MwAgent.DotNetWikiBot.Smw
{
    public class PropertyValue
    {
        public string Name { get; set; }

        public string Timestamp { get; set; }
        public string Raw { get; set; }

        public DateTime DateTime
        {
            get { return DateTime.Parse(Raw.Substring(2)); }
        }
    }
}