namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    public class ResultClaims
    {
        /// <summary>
        /// date of birth
        /// </summary>
        public SingleArray<Claim> P569 { get; set; }

        /// <summary>
        /// date of death
        /// </summary>
        public SingleArray<Claim> P570 { get; set; }

        /// <summary>
        /// image
        /// </summary>
        public SingleArray<Claim> P18 { get; set; }

        /// <summary>
        /// father
        /// </summary>
        public SingleArray<Claim> P22 { get; set; }

        /// <summary>
        /// mother
        /// </summary>
        public SingleArray<Claim> P25 { get; set; }

        /// <summary>
        /// sex or gender
        /// </summary>
        public SingleArray<Claim> P21 { get; set; }
    }
}