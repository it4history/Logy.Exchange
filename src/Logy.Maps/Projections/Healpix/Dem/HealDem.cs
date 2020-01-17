using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AppConfiguration;

namespace Logy.Maps.Projections.Healpix.Dem
{
    [DataContract(Namespace = UrlsManager.Namespace)]
    public class HealDem
    {
        [DataMember]
        public int[] SquareKeys
        {
            get { return Squares.Keys.ToArray(); } set { }
        }

        [DataMember]
        public BasinDem[][] SquareValues
        {
            get { return Squares.Values.ToArray(); } set { }
        }

        /// <summary>
        /// key - parent P or base cell P
        /// </summary>
        public Dictionary<int, BasinDem[]> Squares { get; set; } = new Dictionary<int, BasinDem[]>();
    }
}