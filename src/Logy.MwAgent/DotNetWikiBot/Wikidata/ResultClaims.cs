using System.Collections.Generic;
using System.Linq;

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
        public List<int> BirthPlace
        {
            get { return GetList(P19); }
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
            get { return GetItem(P22); }
        }

        /// <summary>
        /// mother
        /// </summary>
        public SingleArray<Claim> P25 { get; set; }
        public int? Mother
        {
            get { return GetItem(P25); }
        }

        /// <summary>
        /// spouse
        /// </summary>
        public SingleArray<Claim> P26 { get; set; }
        public List<int> Spouses
        {
            get { return GetList(P26); }
        }

        /// <summary>
        /// child 
        /// </summary>
        public SingleArray<Claim> P40 { get; set; }
        public List<int> Kids
        {
            get { return GetList(P40); }
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
        public bool IsMale
        {
            get
            {
                return Sex == Sex.Male;
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

        #region place
        /// <summary>
        /// coordinate location
        /// </summary>
        public SingleArray<Claim> P625 { get; set; }
        public Coor Coor
        {
            get
            {
                return (Coor)P625.A.Mainsnak.ValueTyped;
            }
        }

        public Coor CoorRiverSource
        {
            get
            {
                return (Coor)(from q in P625
                    where
                        q.ToObject<Claim>().Qualifiers != null
                        && ((ValueItem)q.ToObject<Claim>().Qualifiers.P518.A.ValueTyped).NumericId == 7376362
                    select q.ToObject<Claim>().Mainsnak.ValueTyped).SingleOrDefault();
            }
        }

        public Coor CoorRiverMouth
        {
            get
            {
                return (Coor)(from q in P625
                    where
                        q.ToObject<Claim>().Qualifiers != null
                        && ((ValueItem)q.ToObject<Claim>().Qualifiers.P518.A.ValueTyped).NumericId == 1233637
                    select q.ToObject<Claim>().Mainsnak.ValueTyped).SingleOrDefault();
            }
        }
        #endregion

        private int? GetItem(SingleArray<Claim> claim)
        {
            if (claim == null)
                return null;
            var item = (ValueItem)claim.A.Mainsnak.ValueTyped;
            return item == null ? (int?)null : item.NumericId;
        }

        private List<int> GetList(SingleArray<Claim> claim)
        {
            return claim == null ? null : (from link in claim.Children()
                                           select ((ValueItem)link.ToObject<Claim>().Mainsnak.ValueTyped).NumericId).ToList();
        }
    }
}