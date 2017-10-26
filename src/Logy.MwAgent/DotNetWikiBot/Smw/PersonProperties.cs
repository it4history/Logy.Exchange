using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using AppConfiguration;
using Logy.MwAgent.DotNetWikiBot.Wikidata;

namespace Logy.MwAgent.DotNetWikiBot.Smw
{
    [DataContract(Namespace = UrlsManager.Namespace)]
    public class PersonProperties
    {
        public static string MakeCondition
        {
            get
            {
                var props = typeof(PersonProperties)
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(p => p.Name);
                return string.Format(
                    "{0}|?{1}",
                    props.Aggregate(
                        string.Empty,
                        (c, prop) => c + (string.IsNullOrEmpty(c) ? string.Empty : " OR ") + string.Format("[[{0}::+]]", prop)),
                    props.Aggregate((c, prop) => c + "|?" + prop));
            }
        }

        [DataMember]
        public DateTime? Birthday { get; set; }
        [DataMember]
        public string BirthdayText { get; set; }

        [DataMember]
        public DateTime? Deathday { get; set; }

        [DataMember]
        public Sex? Sex { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}