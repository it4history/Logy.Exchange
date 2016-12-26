namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    public class ValueTime
    {
        /// <example>http://www.wikidata.org/entity/Q1985727 - Gregorian</example>
        public string Calendarmodel { get; set; }
        public string Time { get; set; }
        public int Timezone { get; set; }
        public int Before { get; set; }
        public int After { get; set; }
        public int Precision { get; set; }
    }
}