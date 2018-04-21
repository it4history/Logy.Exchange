using System;
using System.Runtime.Serialization;
using AppConfiguration;

namespace Logy.MwAgent.DotNetWikiBot.Smw
{
    [DataContract(Namespace = UrlsManager.Namespace)]
    public class PropertyValue
    {
        [DataMember]
        public string Name { get; set; }

        public string Timestamp { get; set; }

        /// <summary>
        /// DataMemberAttribute is used by Newtonsoft.Json
        /// </summary>
        [DataMember]
        public string Raw { get; set; }

        [DataMember]
        public string Fulltext { get; set; }

        [DataMember]
        public DateTime? DateTime
        {
            get
            {
                var raw = Raw.Substring(2);
                DateTime d;
                if (System.DateTime.TryParse(raw, out d))
                    return d;
                int i;
                if (int.TryParse(raw, out i))
                    return new DateTime(i, 1, 1);
                return null;
            }
            set
            {
            }
        }
    }
}