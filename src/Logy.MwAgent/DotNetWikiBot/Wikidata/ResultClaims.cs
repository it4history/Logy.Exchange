using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    public class ResultClaims
    {
        #region dates
        /// <summary>
        /// date of birth
        /// </summary>
        public SingleArray<Claim> P569 { get; set; }
        public ValueTime Born
        {
            get
            {
                return (ValueTime)P569.A.Mainsnak.ValueTyped;
            }
        }
        public SingleArray<Claim> P19 { get; set; }
        public int? BirthPlace
        {
            get
            {
                var item = (ValueItem)P19.A.Mainsnak.ValueTyped;
                return item == null ? (int?)null : item.NumericId;
            }
        }

        /// <summary>
        /// date of death
        /// </summary>
        public SingleArray<Claim> P570 { get; set; }
        public ValueTime Died
        {
            get
            {
                return (ValueTime)P570.A.Mainsnak.ValueTyped;
            }
        }
        #endregion

        #region family
        /// <summary>
        /// father
        /// </summary>
        public SingleArray<Claim> P22 { get; set; }
        public int? Father
        {
            get
            {
                var item = (ValueItem)P22.A.Mainsnak.ValueTyped;
                return item == null ? (int?)null : item.NumericId;
            }
        }

        /// <summary>
        /// mother
        /// </summary>
        public SingleArray<Claim> P25 { get; set; }
        public int? Mother
        {
            get
            {
                var item = (ValueItem)P25.A.Mainsnak.ValueTyped;
                return item == null ? (int?)null : item.NumericId;
            }
        }

        /// <summary>
        /// spouse
        /// </summary>
        public SingleArray<Claim> P26 { get; set; }
        public List<int> Spouses
        {
            get
            {
                return (from link in P26.Children()
                    select ((ValueItem)link.ToObject<Claim>().Mainsnak.ValueTyped).NumericId).ToList();
            }
        }

        /// <summary>
        /// child 
        /// </summary>
        public SingleArray<Claim> P40 { get; set; }
        public List<int> Kids
        {
            get
            {
                return (from link in P40.Children()
                    select ((ValueItem)link.ToObject<Claim>().Mainsnak.ValueTyped).NumericId).ToList();
            }
        }
        #endregion

        /// <summary>
        /// sex or gender
        /// </summary>
        public SingleArray<Claim> P21 { get; set; }
        public Sex Sex
        {
            get
            {
                var item1 = (ValueItem)P21.A.Mainsnak.ValueTyped;
                return (Sex)item1.NumericId;
            }
        }

        /// <summary>
        /// image
        /// </summary>
        public SingleArray<Claim> P18 { get; set; }
        public string Image
        {
            get
            {
                return string.Format("https://commons.wikimedia.org/wiki/File:{0}", P18.A.Mainsnak.ValueTyped);
            }
        }
    }
}