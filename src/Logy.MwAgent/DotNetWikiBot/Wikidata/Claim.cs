namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    public class Claim
    {
        public Snak Mainsnak { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public string Rank { get; set; }
        public SingleArray<Reference> References { get; set; }

        public Qualifiers Qualifiers { get; set; }
    }
}