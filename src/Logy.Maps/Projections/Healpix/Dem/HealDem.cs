using System.Collections.Generic;
using System.Runtime.Serialization;
using AppConfiguration;

namespace Logy.Maps.Projections.Healpix.Dem
{
    [DataContract(Namespace = UrlsManager.Namespace)]
    public class HealDem
    {
        /// <summary>
        /// key - parent P or base cell P
        /// </summary>
        [DataMember]
        public Dictionary<int, BasinDem[]> Squares { get; set; } = new Dictionary<int, BasinDem[]>();
    }
}