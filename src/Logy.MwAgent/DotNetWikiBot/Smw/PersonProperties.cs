using System;
using System.Runtime.Serialization;
using AppConfiguration;

namespace Logy.MwAgent.DotNetWikiBot.Smw
{
    [DataContract(Namespace = UrlsManager.Namespace)]
    public class PersonProperties
    {
        [DataMember]
        public DateTime? Birthday { get; set; }

        [DataMember]
        public DateTime? Deathday { get; set; }
    }
}