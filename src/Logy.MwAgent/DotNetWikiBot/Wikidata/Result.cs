namespace Logy.MwAgent.DotNetWikiBot.Wikidata
{
    public class Result
    {
        /// <summary>starts with Q</summary>
        public string Title { get; set; }

        public string Pageid { get; set; }

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

        public static string TitleFromId(int? itemId)
        {
            return string.Format("Q{0}", itemId);
        }
    }
}