namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    public class Result
    {
        public string Pageid { get; set; }
        public string Title { get; set; }
        public ResultClaims Claims { get; set; }
        public int? ItemId
        {
            get
            {
                int id;
                if (int.TryParse(Title.TrimStart('Q'), out id))
                    return id;
                return null;
            }
        }
    }
}