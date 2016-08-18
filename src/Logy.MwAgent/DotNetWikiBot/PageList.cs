using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.XPath;

using Logy.MwAgent.DotNetWikiBot.Exceptions;

namespace Logy.MwAgent.DotNetWikiBot
{
    /// <summary>Class defines a set of wiki pages. List&lt;Page&gt; object is used internally
    /// for pages storing.</summary>
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [Serializable]
    public class PageList
    {
        /// <summary>This constructor creates PageList object with specified Site object and fills
        /// it with <see cref="Page"/> objects with specified titles. When constructed, new
        /// <see cref="Page"/> in PageList doesn't contain text. Use <see cref="PageList.Load()"/>
        /// method to get texts and metadata from live wiki.</summary>
        /// <param name="site">Site object, it must be constructed beforehand.</param>
        /// <param name="pageNames">Page titles as array of strings.</param>
        /// <returns>Returns the PageList object.</returns>
        public PageList(Site site, string[] pageNames) : this(site)
        {
            foreach (string pageName in pageNames)
                Pages.Add(new Page(site, pageName));
            CorrectNsPrefixes();
        }

        /// <summary>This constructor creates PageList object with specified Site object and fills
        /// it with <see cref="Page"/> objects with specified titles. When constructed, new 
        /// <see cref="Page"/> in PageList don't contain text. Use <see cref="PageList.Load()"/>
        /// method to get texts and metadata from live wiki.</summary>
        /// <param name="site">Site object, it must be constructed beforehand.</param>
        /// <param name="pageNames">Page titles in List object.</param>
        /// <returns>Returns the PageList object.</returns>
        public PageList(Site site, List<string> pageNames) : this(site, pageNames.ToArray())
        {
        }

        /// <summary>This constructor creates empty PageList object with specified
        /// Site object.</summary>
        /// <param name="site">Site object, it must be constructed beforehand.</param>
        /// <returns>Returns the PageList object.</returns>
        public PageList(Site site)
        {
            Site = site;
            Pages = new List<ImportBlock>();
        }

        /// <summary>This constructor creates empty PageList object using most recently
        /// created <see cref="DotNetWikiBot.Site"/> object.</summary>
        /// <returns>Returns the PageList object.</returns>
        public PageList() : this(Bot.GetMostRecentSiteObject())
        {
        }

        /// <summary>Internal generic List that contains collection of pages.</summary>
        public List<ImportBlock> Pages { get; set; }

        /// <summary>Site, on which the pages are located.</summary>
        public Site Site { get; set; }

        /// <summary>This index allows to call pageList[i] instead of pageList.pages[i].</summary>
        /// <param name="index">Zero-based index.</param>
        /// <returns>Returns the Page object.</returns>
        public Page this[int index]
        {
            get { return (Page)Pages[index]; }
            set { Pages[index] = value; }
        }

        /// <summary>This index allows to use pageList["title"] syntax. Don't forget to use correct
        /// local namespace prefixes. Call <see cref="CorrectNsPrefixes()"/> function to correct
        /// namespace prefixes in a whole PageList at once.</summary>
        /// <param name="index">Title of page to get.</param>
        /// <returns>Returns the Page object, or null if there is no page with the specified
        /// title in this PageList.</returns>
        public Page this[string index]
        {
            get
            {
                foreach (Page p in Pages)
                    if (p.Title == index)
                        return p;
                return null;
            }
            set
            {
                for (int i = 0; i < Pages.Count; i++)
                    if (Pages[i].Title == index)
                        Pages[i] = value;
            }
        }

        /// <summary>This function allows to access individual pages in this PageList.
        /// But it's better to use simple pageList[i] syntax.</summary>
        /// <param name="index">Zero-based index.</param>
        /// <returns>Returns the Page object.</returns>
        public Page GetPage(int index)
        {
            return (Page)Pages[index];
        }

        /// <summary>This function allows to set individual pages in this PageList.
        /// But it's better to use simple pageList[i] syntax.</summary>
        /// <param name="page">Page object to set in this PageList.</param>
        /// <param name="index">Zero-based index.</param>
        /// <returns>Returns the Page object.</returns>
        public void SetPageAtIndex(Page page, int index)
        {
            Pages[index] = page;
        }

        /// <summary>This function allows to iterate over <see cref="Page"/> objects in this
        /// PageList using "foreach" loop.</summary>
        /// <returns>Returns IEnumerator object.</returns>
        public IEnumerator GetEnumerator()
        {
            return Pages.GetEnumerator();
        }

        /// <summary>This function adds specified page to the end of this PageList.</summary>
        /// <param name="page">Page object to add.</param>
        public void Add(Page page)
        {
            Pages.Add(page);
        }

        /// <summary>Inserts an element into this PageList at the specified index.</summary>
        /// <param name="page">Page object to insert.</param>
        /// <param name="index">Zero-based index.</param>
        public void Insert(Page page, int index)
        {
            Pages.Insert(index, page);
        }

        /// <summary>This function returns true, if this PageList contains page with the same title
        /// and same revision ID with page passed as a parameter. Before comparison this function 
        /// corrects all namespace prefixes in this PageList and in title of Page passed
        /// as a parameter.</summary>
        /// <param name="page">Page object to search for in this PageList.</param>
        /// <returns>Returns bool value.</returns>
        public bool Contains(Page page)
        {
            page.CorrectNsPrefix();
            CorrectNsPrefixes();
            foreach (Page p in Pages) {
                if (p.Title == page.Title
                    && (p.Revision == null || page.Revision == null || p.Revision == page.Revision))
                    return true;
            }

            return false;
        }

        /// <summary>This function returns true, if a page with specified title exists
        /// in this PageList. This function corrects all namespace prefixes in this PageList
        /// before comparison.</summary>
        /// <param name="title">Title of page to check.</param>
        /// <returns>Returns bool value.</returns>
        public bool Contains(string title)
        {
            title = Site.CorrectNsPrefix(title);
            CorrectNsPrefixes();
            foreach (Page p in Pages)
                if (p.Title == title)
                    return true;
            return false;
        }

        /// <summary>This function returns the number of pages in PageList.</summary>
        /// <returns>Number of pages as positive integer value.</returns>
        public int Count()
        {
            return Pages.Count;
        }

        /// <summary>Removes page at specified index from PageList.</summary>
        /// <param name="index">Zero-based index.</param>
        public void RemovePage(int index)
        {
            Pages.RemoveAt(index);
        }

        /// <summary>Removes a page with specified title from this PageList.</summary>
        /// <param name="title">Title of page to remove.</param>
        public void Remove(string title)
        {
            for (int i = 0; i < Count(); i++)
                if (Pages[i].Title == title)
                    Pages.RemoveAt(i);
        }

        /// <summary>Gets page titles for this PageList from "Special:Allpages" MediaWiki page.
        /// That means a list of pages in alphabetical order.</summary>
        /// <param name="firstPageTitle">Title of page to start enumerating from. The title
        /// must have no namespace prefix (like "Talk:"). Alternatively just a few first letters
        /// can be specified instead of full real title. Pass the empty string or null
        /// to start from the very beginning.</param>
        /// <param name="neededNSpace">The key of namespace to get pages
        /// from. Zero is a key of default namespace.</param>
        /// <param name="acceptRedirects">Set this to "false" to exclude redirects.</param>
        /// <param name="limit">Maximum allowed limit of pages to get.</param>
        public void FillFromAllPages(string firstPageTitle, int neededNSpace, bool acceptRedirects, int limit)
        {
            FillFromAllPages(firstPageTitle, neededNSpace, acceptRedirects, limit, string.Empty);
        }

        /// <summary>Gets page titles for this PageList from "Special:Allpages" MediaWiki page.
        /// That means a list of pages in alphabetical order.</summary>
        /// <param name="firstPageTitle">Title of page to start listing from. The title
        /// must have no namespace prefix (like "Talk:"). Just a few first letters
        /// can be specified instead of full real title. Pass the empty string or null
        /// to start from the very beginning.</param>
        /// <param name="neededNSpace">The key of namespace to get pages
        /// from. Zero is a key of default namespace.</param>
        /// <param name="acceptRedirects">Set this to "false" to exclude redirects.</param>
        /// <param name="limit">Maximum allowed limit of pages to get.</param>
        /// <param name="lastPageTitle">Title of page to stop listing at.
        /// To get all pages with some prefix use the following method: 
        /// <c>FillFromAllPages("Prefix",0,false,100,"Prefix~")</c></param>
        public void FillFromAllPages(
            string firstPageTitle, 
            int neededNSpace, 
            bool acceptRedirects,
            int limit, 
            string lastPageTitle)
        {
            if (limit <= 0)
                throw new ArgumentOutOfRangeException("limit");

            if (Site.UseApi) {
                var s = (string.IsNullOrEmpty(firstPageTitle) ? string.Empty : "&apfrom=" + Bot.UrlEncode(firstPageTitle)) +
                        (string.IsNullOrEmpty(lastPageTitle) ? string.Empty : "&apto=" + Bot.UrlEncode(lastPageTitle));
                FillFromCustomApiQuery(
                    "list=allpages",
                    "apnamespace=" + neededNSpace + (acceptRedirects ? string.Empty : "&apfilterredir=nonredirects") + s,
                    limit);
            }
            else {
                Console.WriteLine(
                    Bot.Msg("Getting {0} page titles from \"Special:Allpages\" MediaWiki page..."),
                    limit);
                int count = Pages.Count;
                limit += Pages.Count;
                Regex linkToPageRegex;
                if (acceptRedirects)
                    linkToPageRegex = new Regex("<td[^>]*>(?:<div class=\"allpagesredirect\">)?" +
                                                "<a href=\"[^\"]*?\" (?:class=\"mw-redirect\" )?title=\"([^\"]*?)\">");
                else
                    linkToPageRegex =
                        new Regex("<td[^>]*><a href=\"[^\"]*?\" title=\"([^\"]*?)\">");
                MatchCollection matches;
                do {
                    string res = Site.IndexPath + "?title=Special:Allpages" +
                                 "&from=" + Bot.UrlEncode(
                                     string.IsNullOrEmpty(firstPageTitle) ? "!" : firstPageTitle) +
                                 Bot.UrlEncode(
                                     string.IsNullOrEmpty(lastPageTitle) ? string.Empty : ("&to=" + lastPageTitle)) +
                                 "&namespace=" + neededNSpace.ToString();
                    matches = linkToPageRegex.Matches(Site.GetWebPage(res));
                    if (matches.Count < 2)
                        break;
                    for (int i = 1; i < matches.Count; i++)
                        Pages.Add(new Page(Site, HttpUtility.HtmlDecode(matches[i].Groups[1].Value)));
                    firstPageTitle = Site.RemoveNsPrefix(Pages[Pages.Count - 1].Title, neededNSpace) + "!";
                }
                while (Pages.Count < limit);
                if (Pages.Count > limit)
                    Pages.RemoveRange(limit, Pages.Count - limit);
                Console.WriteLine(
                    Bot.Msg("PageList has been filled with {0} page titles from " + "\"Special:Allpages\" MediaWiki page."), 
                    Pages.Count - count);
            }
        }

        /// <summary>Gets page titles for this PageList from specified special page.
        /// The following special pages are supported (other were not tested):<b><i>
        /// Ancientpages, BrokenRedirects, Deadendpages, Disambiguations, DoubleRedirects,
        /// Listredirects, Lonelypages, Longpages, Mostcategories, Mostimages, Mostlinkedcategories,
        /// Mostlinkedtemplates, Mostlinked, Mostrevisions, Fewestrevisions, Shortpages,
        /// Uncategorizedcategories, Uncategorizedpages, Uncategorizedimages,
        /// Uncategorizedtemplates, Unusedcategories, Unusedimages, Wantedcategories, Wantedfiles,
        /// Wantedpages, Wantedtemplates, Unwatchedpages, Unusedtemplates, Withoutinterwiki.</i></b>
        /// The function doesn't filter namespaces and does not clear PageList,
        /// so new pages will be added to existing pages.</summary>
        /// <param name="pageTitle">Title of special page, e.g. "Deadendpages".</param>
        /// <param name="limit">Maximum number of page titles to get.</param>
        public void FillFromCustomSpecialPage(string pageTitle, int limit)
        {
            if (string.IsNullOrEmpty(pageTitle))
                throw new ArgumentNullException("pageTitle");
            if (limit <= 0)
                throw new ArgumentOutOfRangeException("limit");
            Console.WriteLine(Bot.Msg("Getting {0} page titles from \"Special:{1}\" page..."), limit, pageTitle);

            int preexistingPages = Count();

            if (Site.UseApi) {
                FillFromCustomApiQuery("list=querypage", "qppage=" + pageTitle, limit);
            }
            else {
                bool fallback = false;

                // TO DO: paging
                string res = Site.IndexPath + "?title=Special:" +
                             Bot.UrlEncode(pageTitle) + "&limit=" + limit.ToString();
                string src = Site.GetWebPage(res);
                MatchCollection matches;
                if (pageTitle == "Unusedimages" || pageTitle == "Uncategorizedimages" ||
                    pageTitle == "UnusedFiles" || pageTitle == "UncategorizedFiles")
                    matches = Site.Regexes["linkToImage2"].Matches(src);
                else
                    matches = Site.Regexes["titleLinkShown"].Matches(src);
                if (matches.Count == 0) {
                    fallback = true;
                }
                else {
                    foreach (Match match in matches)
                        Pages.Add(new Page(Site, HttpUtility.HtmlDecode(match.Groups[1].Value)));
                }

                if (fallback) {    // FALLBACK, use alternative parsing way, XPath
                    src = Bot.GetXmlSubstring(src, "<!-- bodytext -->", "<!-- /bodytext -->", true);
                    XmlNamespaceManager xmlNs = new XmlNamespaceManager(new NameTable());
                    xmlNs.AddNamespace("ns", "http://www.w3.org/1999/xhtml");
                    XPathNodeIterator ni = Bot.GetXmlIterator(src, "//ns:ol/ns:li/ns:a[@title != '']", xmlNs);
                    if (ni.Count == 0)
                        throw new WikiBotException(string.Format(
                            Bot.Msg("Nothing was found on \"Special:{0}\" page."), 
                            pageTitle));
                    while (ni.MoveNext())
                        Pages.Add(new Page(Site, HttpUtility.HtmlDecode(ni.Current.GetAttribute("title", string.Empty))));
                }
            }

            Console.WriteLine(
                Bot.Msg("PageList has been filled with {0} page titles from " + "\"Special:{1}\" page."), 
                Count() - preexistingPages, 
                pageTitle);
        }

        /// <summary>Gets page titles for this PageList from specified MediaWiki events log.
        /// The following logs are supported:<b><i>
        /// block, protect, rights, delete, upload, move, import, patrol, merge, suppress,
        /// review, stable, spamblacklist, gblblock, renameuser, globalauth, gblrights,
        /// abusefilter, newusers.</i></b>
        /// The function does not filter namespaces and does not clear the
        /// existing PageList, so new pages will be added to existing pages.</summary>
        /// <param name="logType">Type of log, it could be: "block" for blocked users log;
        /// "protect" for protected pages log; "rights" for users rights log; "delete" for
        /// deleted pages log; "upload" for uploaded files log; "move" for renamed pages log;
        /// "import" for transwiki import log; "renameuser" for renamed accounts log;
        /// "newusers" for new users log; "makebot" for bot status assignment log.</param>
        /// <param name="userName">Select log entries only for specified account. Pass empty
        /// string, if that restriction is not needed.</param>
        /// <param name="pageTitle">Select log entries only for specified page. Pass empty
        /// string, if that restriction is not needed.</param>
        /// <param name="limit">Maximum number of page titles to get.</param>
        public void FillFromCustomLog(string logType, string userName, string pageTitle, int limit)
        {
            if (string.IsNullOrEmpty(logType))
                throw new ArgumentNullException("logType");
            if (limit <= 0)
                throw new ArgumentOutOfRangeException("limit");
            Console.WriteLine(Bot.Msg("Getting {0} page titles from \"{1}\" log..."), limit, logType);

            int preexistingPages = Count();

            if (Site.UseApi) {
                var queryXml =
                    from el in Bot.CommonDataXml.Element("ApiOptions").Descendants("query")
                    where el.Value == logType
                    select el;
                if (queryXml == null)
                    throw new WikiBotException(
                        string.Format(Bot.Msg("The log \"{0}\" is not supported."), logType));
                string parameters = "letype=" + logType;
                if (!string.IsNullOrEmpty(userName))
                    parameters += "&leuser=" + Bot.UrlEncode(userName);
                if (!string.IsNullOrEmpty(pageTitle))
                    parameters += "&letitle=" + Bot.UrlEncode(pageTitle);
                FillFromCustomApiQuery("list=logevents", parameters, limit);
            }
            else {
                // TO DO: paging
                string res = Site.IndexPath + "?title=Special:Log&type=" +
                             logType + "&user=" + Bot.UrlEncode(userName) + "&page=" +
                             Bot.UrlEncode(pageTitle) + "&limit=" + limit.ToString();
                string src = Site.GetWebPage(res);
                MatchCollection matches = Site.Regexes["titleLinkShown"].Matches(src);
                if (matches.Count == 0)
                    throw new WikiBotException(string.Format(
                        Bot.Msg("Log \"{0}\" does not contain page titles."), logType));
                foreach (Match match in matches)
                    Pages.Add(new Page(Site, HttpUtility.HtmlDecode(match.Groups[1].Value)));
            }

            Console.WriteLine(
                Bot.Msg("PageList has been filled with {0} page titles from \"{1}\" log."),
                Count() - preexistingPages, 
                logType);
        }

        /// <summary>Gets page titles for this PageList from results of specified custom API query.
        /// Not all queries are supported and can be parsed automatically. The function does not
        /// clear the existing PageList, so new titles will be added to existing.</summary>
        /// <param name="query">Type of query, e.g. "list=allusers" or "list=allpages".</param>
        /// <param name="queryParams">Additional query parameters, specific to the
        /// query, e.g. "cmtitle=Category:Physical%20sciences&amp;cmnamespace=0|2".
        /// Parameter values must be URL-encoded with Bot.UrlEncode() function
        /// before calling this function.</param>
        /// <param name="limit">Maximum number of resultant strings to fetch.</param>
        /// <example><code>
        /// pageList.FillFromCustomApiQuery("list=categorymembers",
        ///     "cmcategory=Physical%20sciences&amp;cmnamespace=0|14",
        ///     int.MaxValue);
        /// </code></example>
        public List<Page> FillFromCustomApiQuery(string query, string queryParams, int limit)
        {
            var titles = Site.GetApiQueryResult(query, queryParams, limit);
            var result = (from title in titles 
                          where title.ContainsKey("_Target") 
                          select new Page(Site, title["_Target"])).ToList();
            Pages.AddRange(result);

            // Show message only if the function was called by user, not by other bot function);
            if (!string.IsNullOrEmpty(Environment.StackTrace)
                && !Environment.StackTrace.Contains("FillFrom")
                && !Environment.StackTrace.Contains("FillAllFrom"))
                Console.WriteLine(string.Format(
                    Bot.Msg("PageList has been filled with {0} page " + "titles from \"{1}\" bot interface list."), 
                    titles.Count, 
                    query));

            return result;
        }

        /// <summary>Gets page titles for this PageList from recent changes page,
        /// "Special:Recentchanges". File uploads, page deletions and page renamings are
        /// not included, use
        /// <see cref="PageList.FillFromCustomLog(string,string,string,int)"/>
        /// function instead to fill from respective logs.
        /// The function does not clear the existing PageList, so new titles will be added.
        /// Use <see cref="PageList.FilterNamespaces(int[])"/> or
        /// <see cref="PageList.RemoveNamespaces(int[])"/> functions to remove
        /// pages from unwanted namespaces.</summary>
        /// <param name="hideMinor">Ignore minor edits.</param>
        /// <param name="hideBots">Ignore bot edits.</param>
        /// <param name="hideAnons">Ignore anonymous users edits.</param>
        /// <param name="hideLogged">Ignore logged-in users edits.</param>
        /// <param name="hideSelf">Ignore edits of this bot account.</param>
        /// <param name="hideReviewed">Ignore checked edits.</param>
        /// <param name="limit">Maximum number of changes to get.</param>
        /// <param name="days">Get changes for this number of recent days.</param>
        public void FillFromRecentChanges(
            bool hideMinor, 
            bool hideBots, 
            bool hideAnons,
            bool hideLogged, 
            bool hideSelf, 
            bool hideReviewed, 
            int limit, 
            int days)
        {
            if (limit <= 0)
                throw new ArgumentOutOfRangeException("limit", Bot.Msg("Limit must be positive."));
            if (days <= 0)
                throw new ArgumentOutOfRangeException("days", Bot.Msg("Number of days must be positive."));
            Console.WriteLine(Bot.Msg("Getting {0} page titles from " + "\"Special:Recentchanges\" page..."), limit);
            string uri = string.Format(
                "{0}?title=Special:Recentchanges" +
                "&hideminor={1}&hidebots={2}&hideanons={3}&hideliu={4}&hidemyself={5}" +
                "&hideReviewed={6}&limit={7}&days={8}",
                Site.IndexPath,
                hideMinor ? "1" : "0",
                hideBots ? "1" : "0",
                hideAnons ? "1" : "0",
                hideLogged ? "1" : "0",
                hideSelf ? "1" : "0",
                hideReviewed ? "1" : "0",
                limit,
                days);
            string respStr = Site.GetWebPage(uri);
            MatchCollection matches = Site.Regexes["titleLinkShown"].Matches(respStr);
            foreach (Match match in matches)
                Pages.Add(new Page(Site, HttpUtility.HtmlDecode(match.Groups[1].Value)));
            Console.WriteLine(
                Bot.Msg("PageList has been filled with {0} page titles from " + "\"Special:Recentchanges\" page."), 
                matches.Count);
        }

        /// <summary>Fills this PageList with pages from specified category page. Subcategories are
        /// not included, call <see cref="PageList.FillAllFromCategory(string)"/> function instead
        /// to get category contents with subcategories.</summary>
        /// <param name="categoryName">Category name, with or without namespace prefix.</param>
        public void FillFromCategory(string categoryName)
        {
            int count = Pages.Count;
            var pl = new PageList(Site);
            pl.FillAllFromCategory(categoryName);
            pl.RemoveNamespaces(new[] { Site.CategoryNS });
            Pages.AddRange(pl.Pages);
            if (Pages.Count != count)
                Console.WriteLine(
                    Bot.Msg("PageList has been filled with {0} page titles found in \"{1}\"" + " category."), 
                    Pages.Count - count, 
                    categoryName);
            else
                Console.Error.WriteLine(Bot.Msg("Nothing was found in \"{0}\" category."), categoryName);
        }

        /// <summary>This function fills this PageList with pages from specified
        /// category page, subcategories are also included.</summary>
        /// <param name="categoryName">Category name, with or without prefix.</param>
        public List<Page> FillAllFromCategory(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
                throw new ArgumentNullException("categoryName");
            categoryName = categoryName.Trim("[]\f\n\r\t\v ".ToCharArray());
            categoryName = Site.RemoveNsPrefix(categoryName, Site.CategoryNS);
            categoryName = Site.GetNsPrefix(Site.CategoryNS) + categoryName;
            Console.WriteLine(Bot.Msg("Getting category \"{0}\" contents..."), categoryName);

            // RemoveAll();
            if (Site.UseApi) {
                return FillFromCustomApiQuery("list=categorymembers", "cmtitle=" + Bot.UrlEncode(categoryName), int.MaxValue);
            }
            else {    // TO DO: paging
                string src = string.Empty;
                var nextPortionRegex = new Regex("&(?:amp;)?from=([^\"=]+)\" title=\"");
                var result = new List<Page>();
                do {
                    string res = Site.IndexPath + "?title=" +
                                 Bot.UrlEncode(categoryName) +
                                 "&from=" + nextPortionRegex.Match(src).Groups[1].Value;
                    src = Site.GetWebPage(res);
                    src = Bot.GetSubstring(src, " id=\"mw-subcategories\"", " id=\"mw-normal-catlinks\"");
                    string relativeIndexPath =
                        Site.IndexPath.Substring(Site.IndexPath.IndexOf('/', 10));
                    var linkRegex = new Regex(" href=\"(?:" +
                                              (!string.IsNullOrEmpty(Site.ShortPath)
                                                   ? Regex.Escape(Site.ShortPath) + "|"
                                                   : string.Empty) +
                                              Regex.Escape(relativeIndexPath) + "\\?title=)" +
                                              "(?<title>[^\"]+)");
                    MatchCollection matches = linkRegex.Matches(src);
                    result.AddRange(from Match match in matches
                                    select new Page(Site, HttpUtility.UrlDecode(match.Groups["title"].Value)));
                }
                while (nextPortionRegex.IsMatch(src));
                Pages.AddRange(result);
                return result;
            }
        }

        /// <summary>Gets all levels of subcategories of some wiki category (that means
        /// subcategories, sub-subcategories, and so on) and fills this PageList with titles
        /// of all pages, found in all levels of subcategories. The multiplicates of recurring pages
        /// are removed. Subcategory pages are excluded from resultant list, call
        /// <see cref="PageList.FillAllFromCategoryTree(string)"/> function instead to get PageList
        /// with subcategories on board.
        /// This operation may be very time-consuming and traffic-consuming.
        /// The function clears the PageList before filling begins.</summary>
        /// <param name="categoryName">Category name, with or without prefix.</param>
        public void FillFromCategoryTree(string categoryName, bool leaveNamespaces = true)
        {
            Clear();
            FillAllFromCategoryTree(new List<string> { categoryName });
            RemoveRecurring();
            if (!leaveNamespaces)
                RemoveNamespaces(new[] { Site.CategoryNS });
            if (Pages.Count != 0)
                Console.WriteLine(
                    Bot.Msg("PageList has been filled with {0} page titles found in \"{1}\"" + " category."), 
                    Count(), 
                    categoryName);
            else
                Console.Error.WriteLine(Bot.Msg("Nothing was found in \"{0}\" category."), categoryName);
        }

        /// <summary>Gets page history and fills this PageList with specified number of recent page
        /// revisions. Pre-existing pages will be removed from this PageList.
        /// Only revision identifiers, user names, timestamps and comments are
        /// loaded, not the texts. Call <see cref="PageList.Load()"/> to load the texts of page
        /// revisions. PageList[0] will be the most recent revision.</summary>
        /// <param name="pageTitle">Page to get history of.</param>
        /// <param name="limit">Number of last page revisions to get.</param>
        public void FillFromPageHistory(string pageTitle, int limit)
        {
            if (string.IsNullOrEmpty(pageTitle))
                throw new ArgumentNullException("pageTitle");
            if (limit <= 0)
                throw new ArgumentOutOfRangeException("limit");
            Console.WriteLine(
                Bot.Msg("Getting {0} last revisons of \"{1}\" page..."), limit, pageTitle);

            Clear();    // remove pre-existing pages

            if (Site.UseApi) {
                string queryUri = Site.ApiPath + "?action=query&prop=revisions&titles=" +
                                  Bot.UrlEncode(pageTitle) + "&rvprop=ids|user|comment|timestamp" +
                                  "&format=xml&rvlimit=" + limit.ToString();
                string src = Site.GetWebPage(queryUri);
                Page p;
                using (XmlReader reader = XmlReader.Create(new StringReader(src))) {
                    reader.ReadToFollowing("api");
                    reader.Read();
                    if (reader.Name == "error")
                        Console.Error.WriteLine(Bot.Msg("Error: {0}"), reader.GetAttribute("info"));
                    while (reader.ReadToFollowing("rev")) {
                        p = new Page(Site, pageTitle);
                        p.Revision = reader.GetAttribute("revid");
                        p.LastUser = reader.GetAttribute("user");
                        p.Comment = reader.GetAttribute("comment");
                        p.Timestamp =
                            DateTime.Parse(reader.GetAttribute("timestamp")).ToUniversalTime();
                        Pages.Add(p);
                    }
                }
            }
            else {
                // TO DO: paging
                string res = Site.IndexPath + "?title=" +
                             Bot.UrlEncode(pageTitle) + "&limit=" + limit.ToString() +
                             "&action=history";
                string src = Site.GetWebPage(res);
                src = src.Substring(src.IndexOf("<ul id=\"pagehistory\">"));
                src = src.Substring(0, src.IndexOf("</ul>") + 5);
                Page p = null;
                using (XmlReader reader = Bot.GetXmlReader(src)) {
                    while (reader.Read()) {
                        if (reader.Name == "li" && reader.NodeType == XmlNodeType.Element) {
                            p = new Page(Site, pageTitle) { LastMinorEdit = false, Comment = string.Empty };
                        }
                        else if (reader.Name == "span"
                                 && reader["class"] == "mw-history-histlinks") {
                                     reader.ReadToFollowing("a");
                                     p.Revision = reader["href"].Substring(
                                         reader["href"].IndexOf("oldid=") + 6);
                                     DateTime i;
                                     DateTime.TryParse(
                                         reader.ReadString(),
                                         Site.RegCulture,
                                         DateTimeStyles.AssumeLocal,
                                         out i);
                                     p.Timestamp = i;
                                 }
                        else if (reader.Name == "span" && reader["class"] == "history-user") {
                            reader.ReadToFollowing("a");
                            p.LastUser = reader.ReadString();
                        }
                        else if (reader.Name == "abbr")
                            p.LastMinorEdit = true;
                        else if (reader.Name == "span" && reader["class"] == "history-size")
                        {
                            int i;
                            int.TryParse(Regex.Replace(reader.ReadString(), @"[^-+\d]", string.Empty), out i);
                            p.LastBytesModified = i;
                        }
                        else if (reader.Name == "span" && reader["class"] == "comment")
                        {
                            p.Comment = Regex.Replace(reader.ReadInnerXml().Trim(), "<.+?>", string.Empty);
                            p.Comment = p.Comment.Substring(1, p.Comment.Length - 2);    // brackets
                        }

                        if (reader.Name == "li" && reader.NodeType == XmlNodeType.EndElement)
                            Pages.Add(p);
                    }
                }
            }

            Console.WriteLine(
                Bot.Msg("PageList has been filled with {0} last revisons of \"{1}\" page..."),
                Pages.Count, 
                pageTitle);
        }

        /// <summary>Gets page titles for this PageList from links in some wiki page. All links
        /// will be retrieved, from all namespaces, except interwiki links to other
        /// sites. Use <see cref="PageList.FilterNamespaces(int[])"/> or
        /// <see cref="PageList.RemoveNamespaces(int[])"/> function to remove pages from
        /// unwanted namespaces (categories, images, etc.)</summary>
        /// <param name="pageTitle">Page title as string.</param>
        /// <example><code>pageList.FillFromAllPageLinks("Art");</code></example>
        public void FillFromPageLinks(string pageTitle)
        {
            if (string.IsNullOrEmpty(pageTitle))
                throw new ArgumentNullException("pageTitle");
            Regex wikiLinkRegex = new Regex(@"\[\[ *:*(.+?)(]]|\|)");
            Page page = new Page(Site, pageTitle);
            page.Load();
            MatchCollection matches = wikiLinkRegex.Matches(page.Text);
            Regex outWikiLink = new Regex("^(" + Site.GeneralData["interwiki"] + "):");
            foreach (Match match in matches) {
                string link = match.Groups[1].Value;
                if (outWikiLink.IsMatch(link))
                    continue;
                if (link[0] == '/')    // relative link
                    link = pageTitle + link;
                if (link.Contains('_'))
                    link = link.Replace(' ', '_');
                if (!Contains(link))
                    Pages.Add(new Page(Site, link));
            }

            Console.WriteLine(Bot.Msg("PageList has been filled with links found on \"{0}\" page."), pageTitle);
        }

        /// <summary>Gets titles of pages which link to specified page. Results include redirects,
        /// call <see cref="PageList.RemoveRedirects()"/> to get rid of them. The
        /// function does not clear the existing PageList, so new titles will be added.</summary>
        /// <param name="pageTitle">Page title as string.</param>
        public void FillFromLinksToPage(string pageTitle)
        {
            if (string.IsNullOrEmpty(pageTitle))
                throw new ArgumentNullException("pageTitle");
            FillFromCustomApiQuery(
                "list=backlinks",
                "bltitle=" + Bot.UrlEncode(pageTitle),
                int.MaxValue);
            Console.WriteLine(
                Bot.Msg("PageList has been filled with titles of pages referring to \"{0}\" page."),
                pageTitle);
        }

        /// <summary>Gets titles of pages which transclude (embed) the specified page. The function
        /// does not clear the existing PageList, so new titles will be added.</summary>
        /// <param name="pageTitle">Page title as string.</param>
        public void FillFromTransclusionsOfPage(string pageTitle)
        {
            if (string.IsNullOrEmpty(pageTitle))
                throw new ArgumentNullException("pageTitle");
            if (Site.UseApi) {
                FillFromCustomApiQuery(
                    "list=embeddedin",
                    "eititle=" + Bot.UrlEncode(pageTitle),
                    int.MaxValue);
            }
            else {    
                // TO DO: paging
                string res = Site.IndexPath + "?title=Special:Whatlinkshere/" +
                             Bot.UrlEncode(pageTitle) + "&limit=5000&hidelinks=1&hideredirs=1";
                string src = Site.GetWebPage(res);
                src = Bot.GetSubstring(src, " id=\"mw-whatlinkshere-list\"", " class=\"printfooter\"");
                MatchCollection matches = Site.Regexes["titleLinkInList"].Matches(src);
                foreach (Match match in matches)
                    Pages.Add(new Page(Site, HttpUtility.HtmlDecode(match.Groups["title"].Value)));
                Console.WriteLine(
                    Bot.Msg("PageList has been filled with titles of pages, which" + " transclude \"{0}\" page."), 
                    pageTitle);
            }
        }

        /// <summary>Gets titles of pages, in which the specified image file is included.
        /// Function also works with non-image files.</summary>
        /// <param name="imageFileTitle">File title. With or without "Image:" or
        /// "File:" prefix.</param>
        public void FillFromPagesUsingImage(string imageFileTitle)
        {
            if (string.IsNullOrEmpty(imageFileTitle))
                throw new ArgumentNullException("imageFileTitle");
            int pagesCount = Count();
            imageFileTitle = Site.RemoveNsPrefix(imageFileTitle, 6);

            if (Site.UseApi) {
                FillFromCustomApiQuery(
                    "list=imageusage", 
                    "iutitle=" + Bot.UrlEncode(Site.GetNsPrefix(6)) + Bot.UrlEncode(imageFileTitle), 
                    int.MaxValue);
            }
            else {    
                // TO DO: paging
                string res = Site.IndexPath + "?title=" +
                             Bot.UrlEncode(Site.GetNsPrefix(6)) +
                             Bot.UrlEncode(imageFileTitle);
                string src = Site.GetWebPage(res);
                try {
                    src = Bot.GetSubstring(src, "<h2 id=\"filelinks\"", "<h2 id=\"globalusage\"");
                }
                catch (ArgumentOutOfRangeException) {
                    Console.Error.WriteLine(Bot.Msg("No page contains the image \"{0}\"."), imageFileTitle);
                    return;
                }

                MatchCollection matches = Site.Regexes["titleLink"].Matches(src);
                foreach (Match match in matches)
                    Pages.Add(new Page(Site, HttpUtility.HtmlDecode(match.Groups["title"].Value)));
            }

            if (pagesCount == Count())
                Console.Error.WriteLine(Bot.Msg("No page contains the image \"{0}\"."), imageFileTitle);
            else
                Console.WriteLine(
                    Bot.Msg("PageList has been filled with titles of pages containing \"{0}\"" + " image."), 
                    imageFileTitle);
        }

        /// <summary>Gets page titles for this PageList from user contributions
        /// of specified user. The function does not remove redirecting
        /// pages from the results. Call <see cref="PageList.RemoveRedirects()"/> manually,
        /// if you require it. And the function does not clears the existing PageList,
        /// so new titles will be added.</summary>
        /// <param name="userName">User's name.</param>
        /// <param name="limit">Maximum number of page titles to get.</param>
        public void FillFromUserContributions(string userName, int limit)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");
            if (limit <= 0)
                throw new ArgumentOutOfRangeException("limit", Bot.Msg("Limit must be positive."));
            string res = Site.IndexPath +
                         "?title=Special:Contributions&target=" + Bot.UrlEncode(userName) +
                         "&limit=" + limit.ToString();
            string src = Site.GetWebPage(res);
            MatchCollection matches = Site.Regexes["titleLinkShown"].Matches(src);
            foreach (Match match in matches)
                Pages.Add(new Page(Site, HttpUtility.HtmlDecode(match.Groups[1].Value)));
            Console.WriteLine(
                Bot.Msg("PageList has been filled with user \"{0}\"'s contributions."), userName);
        }

        /// <summary>Gets page titles for this PageList from watchlist
        /// of bot account. The function does not remove redirecting
        /// pages from the results. Call <see cref="PageList.RemoveRedirects()"/> manually,
        /// if you need that. And the function neither filters namespaces, nor clears the
        /// existing PageList, so new titles will be added to the existing in PageList.</summary>
        public void FillFromWatchList()
        {
            string src = Site.GetWebPage(Site.IndexPath + "?title=Special:Watchlist/edit");
            MatchCollection matches = Site.Regexes["titleLinkShown"].Matches(src);
            if (Site.WatchList == null)
                Site.WatchList = new PageList(Site);
            else
                Site.WatchList.Clear();
            foreach (Match match in matches) {
                string title = HttpUtility.HtmlDecode(match.Groups[1].Value);
                Pages.Add(new Page(Site, title));
                Site.WatchList.Add(new Page(Site, title));
            }

            Console.WriteLine(Bot.Msg("PageList has been filled with bot account's watchlist."));
        }

        /// <summary>Gets page titles for this PageList from list of recently changed
        /// watched articles (watched by bot account). The function does not internally
        /// remove redirecting pages from the results. Call <see cref="PageList.RemoveRedirects()"/>
        /// manually, if you need it. And the function neither filters namespaces, nor clears
        /// the existing PageList, so new titles will be added to the existing
        /// in PageList.</summary>
        public void FillFromChangedWatchedPages()
        {
            string src = Site.GetWebPage(Site.IndexPath + "?title=Special:Watchlist/edit");
            MatchCollection matches = Site.Regexes["titleLinkShown"].Matches(src);
            foreach (Match match in matches)
                Pages.Add(new Page(Site, HttpUtility.HtmlDecode(match.Groups[1].Value)));
            Console.WriteLine(
                Bot.Msg("PageList has been filled with changed pages from bot account's" +
                        " watchlist."));
        }

        /// <summary>Gets page titles for this PageList from site's internal search results.
        /// The function does not filter namespaces. And the function does not clear
        /// the existing PageList, so new titles will be added.</summary>
        /// <param name="searchStr">String to search.</param>
        /// <param name="limit">Maximum number of page titles to get.</param>
        public void FillFromSearchResults(string searchStr, int limit)
        {
            if (string.IsNullOrEmpty(searchStr))
                throw new ArgumentNullException("searchStr");
            if (limit <= 0)
                throw new ArgumentOutOfRangeException("limit", Bot.Msg("Limit must be positive."));
            if (Site.UseApi) {
                FillFromCustomApiQuery(
                    "list=search",
                    "srsearch=" + Bot.UrlEncode(searchStr),
                    limit);
            }
            else {    // TO DO: paging
                string res = Site.IndexPath + "?title=Special:Search&fulltext=Search&search=" +
                             Bot.UrlEncode(searchStr) + "&limit=" + limit;
                string src = Site.GetWebPage(res);
                src = Bot.GetSubstring(src, "<ul class='mw-search-results'>", "</ul>");
                MatchCollection matches = Site.Regexes["titleLink"].Matches(src);
                foreach (Match match in matches)
                    Pages.Add(new Page(Site, HttpUtility.HtmlDecode(match.Groups["title"].Value)));
            }

            Console.WriteLine(Bot.Msg("PageList has been filled with search results."));
        }

        /// <summary>Gets page titles for this PageList from Google search results.
        /// The function gets pages of all namespaces and it does not clear
        /// the existing PageList, so new pages will be added.</summary>
        /// <param name="searchStr">Words to search for. Use quotes to find exact phrases.</param>
        /// <param name="limit">Maximum number of page titles to get.</param>
        public void FillFromGoogleSearchResults(string searchStr, int limit)
        {
            if (string.IsNullOrEmpty(searchStr))
                throw new ArgumentNullException("searchStr");
            if (limit <= 0)
                throw new ArgumentOutOfRangeException("limit", Bot.Msg("Limit must be positive."));

            // TO DO: paging
            Uri res = new Uri("http://www.google.com/search?q=" + Bot.UrlEncode(searchStr) +
                              "+site:" + Site.Address.Substring(Site.Address.IndexOf("://") + 3) +
                              "&num=" + limit);
            string src = Bot.GetWebResource(res, string.Empty);
            string relativeIndexPath = Site.IndexPath.Substring(Site.IndexPath.IndexOf('/', 10));
            string googleLinkToPagePattern = "<h3[^>]*><a href=\"(?<double_escape>/url\\?q=)?" +
                                             Regex.Escape(Site.Address).Replace("https:", "https?:") + "(?:" +
                                             (!string.IsNullOrEmpty(Site.ShortPath)
                                                  ? Regex.Escape(Site.ShortPath) + "|"
                                                  : string.Empty) +
                                             Regex.Escape(relativeIndexPath) + "\\?title=)?" + "(?<title>[^&\"]+)";
            Regex googleLinkToPageRegex = new Regex(googleLinkToPagePattern);
            MatchCollection matches = googleLinkToPageRegex.Matches(src);
            foreach (Match match in matches) {
                string title = HttpUtility.UrlDecode(match.Groups["title"].Value);
                if (title == "/") {
                    if (Site.Messages == null)
                        Site.LoadMediawikiMessages(true);
                    string mainPageTitle = Site.Messages["mainpage"];
                    Page p = new Page(Site, mainPageTitle);
                    p.ResolveRedirect();
                    Pages.Add(p);
                }
                else {
                    if (!string.IsNullOrEmpty(match.Groups["double_escape"].Value))
                        title = HttpUtility.UrlDecode(title);
                    Pages.Add(new Page(Site, title));
                }
            }

            Console.WriteLine(Bot.Msg("PageList has been filled with www.google.com search results."));
        }

        /// <summary>Gets page titles from UTF8-encoded file. Each title must be on a new line.
        /// The function does not clear the existing PageList, new pages will be added.</summary>
        /// <param name="filePathName">Full file path and name.</param>
        public void FillFromFile(string filePathName)
        {
            // RemoveAll();
            StreamReader strmReader = new StreamReader(filePathName);
            string input;
            while ((input = strmReader.ReadLine()) != null) {
                input = input.Trim(" []".ToCharArray());
                if (string.IsNullOrEmpty(input) != true)
                    Pages.Add(new Page(Site, input));
            }

            strmReader.Close();
            Console.WriteLine(
                Bot.Msg("PageList has been filled with titles found in \"{0}\" file."),
                filePathName);
        }

        /// <summary>Protects or unprotects all pages in this PageList, so only chosen category
        /// of users can edit or rename it. Changing page protection modes requires administrator
        /// (sysop) rights on target wiki.</summary>
        /// <param name="editMode">Protection mode for editing this page (0 = everyone allowed
        /// to edit, 1 = only registered users are allowed, 2 = only administrators are allowed 
        /// to edit).</param>
        /// <param name="renameMode">Protection mode for renaming this page (0 = everyone allowed to
        /// rename, 1 = only registered users are allowed, 2 = only administrators
        /// are allowed).</param>
        /// <param name="cascadeMode">In cascading mode, all the pages, included into this page
        /// (e.g., templates or images) are also fully automatically protected.</param>
        /// <param name="expiryDate">Date ant time, expressed in UTC, when the protection expires
        /// and page becomes fully unprotected. Use DateTime.ToUniversalTime() method to convert
        /// local time to UTC, if necessary. Pass DateTime.MinValue to make protection
        /// indefinite.</param>
        /// <param name="reason">Reason for protecting this page.</param>
        public void Protect(int editMode, int renameMode, bool cascadeMode, DateTime expiryDate, string reason)
        {
            if (IsEmpty())
                throw new WikiBotException(Bot.Msg("The PageList is empty. Nothing to protect."));
            foreach (Page p in Pages)
                p.Protect(editMode, renameMode, cascadeMode, expiryDate, reason);
        }

        /// <summary>Adds all pages in this PageList to bot account's watchlist.</summary>
        public void Watch()
        {
            if (IsEmpty())
                throw new WikiBotException(Bot.Msg("The PageList is empty. Nothing to watch."));
            foreach (Page p in Pages)
                p.Watch();
        }

        /// <summary>Removes all pages in this PageList from bot account's watchlist.</summary>
        public void Unwatch()
        {
            if (IsEmpty())
                throw new WikiBotException(Bot.Msg("The PageList is empty. Nothing to unwatch."));
            foreach (Page p in Pages)
                p.Unwatch();
        }

        /// <summary>Removes pages that are not in the given namespaces.</summary>
        /// <param name="neededNSs">Array of integers, presenting keys of namespaces
        /// to retain.</param>
        /// <example><code>pageList.FilterNamespaces(new int[] {0,3});</code></example>
        public void FilterNamespaces(int[] neededNSs)
        {
            for (int i = Pages.Count - 1; i >= 0; i--) {
                if (Array.IndexOf(neededNSs, this[i].GetNamespace()) == -1)
                    Pages.RemoveAt(i); }
        }

        /// <summary>Removes the pages, that are in given namespaces.</summary>
        /// <param name="needlessNSs">Array of integers, presenting keys of namespaces
        /// to remove.</param>
        /// <example><code>pageList.RemoveNamespaces(new int[] {2,4});</code></example>
        public void RemoveNamespaces(int[] needlessNSs)
        {
            for (int i = Pages.Count - 1; i >= 0; i--) {
                if (Array.IndexOf(needlessNSs, this[i].GetNamespace()) != -1)
                    Pages.RemoveAt(i); }
        }

        /// <summary>This function sorts all pages in PageList by titles.</summary>
        public void Sort()
        {
            if (IsEmpty())
                throw new WikiBotException(Bot.Msg("The PageList is empty. Nothing to sort."));
            Pages.Sort(ComparePagesByTitles);
        }

        /// <summary>Compares pages by titles in language-specific manner. This is required to
        /// compare titles in Japanese, Chinese, etc. properly</summary>
        /// <param name="x">First page.</param>
        /// <param name="y">Second page.</param>
        /// <returns>Returns 1 if x is greater (alphabetically), -1 if y is greater, 0 if equal.
        /// </returns>
        public int ComparePagesByTitles(ImportBlock x, ImportBlock y)
        {
            int r = string.Compare(x.Title, y.Title, false, Site.LangCulture);
            return (r != 0) ? r / Math.Abs(r) : 0;
        }

        /// <summary>Removes all pages in PageList from specified category by deleting
        /// links to that category in pages texts.</summary>
        /// <param name="categoryName">Category name, with or without prefix.</param>
        public void RemoveFromCategory(string categoryName)
        {
            foreach (Page p in Pages)
                p.RemoveFromCategory(categoryName);
        }

        /// <summary>Adds all pages in PageList to the specified category by adding
        /// links to that category in pages texts.</summary>
        /// <param name="categoryName">Category name, with or without prefix.</param>
        public void AddToCategory(string categoryName)
        {
            foreach (Page p in Pages)
                p.AddToCategory(categoryName);
        }

        /// <summary>Adds a specified template to the end of all pages in PageList.</summary>
        /// <param name="templateText">Template text, like "{{template_name|...|...}}".</param>
        public void AddTemplate(string templateText)
        {
            foreach (Page p in Pages)
                p.AddTemplate(templateText);
        }

        /// <summary>Removes a specified template from all pages in PageList.</summary>
        /// <param name="templateTitle">Title of template  to remove.</param>
        public void RemoveTemplate(string templateTitle)
        {
            foreach (Page p in Pages)
                p.RemoveTemplate(templateTitle);
        }

        /// <summary>Loads text for pages in PageList from site via common web interface.
        /// Please, don't use this function when going to edit big amounts of pages on
        /// popular public wikis, as it compromises edit conflict detection. In that case,
        /// each page's text should be loaded individually right before its processing
        /// and saving.</summary>
        public void Load()
        {
            if (IsEmpty())
                throw new WikiBotException(Bot.Msg("The PageList is empty. Nothing to load."));
            foreach (Page page in Pages)
                page.Load();
        }

        /// <summary>Loads texts and metadata (revision ID, timestamp, last comment,
        /// last contributor, minor edit mark) for pages in this PageList.
        /// Non-existent pages will be automatically removed from the PageList.
        /// Please, don't use this function when going to edit big amount of pages on
        /// popular public wikis, as it compromises edit conflict detection. In that case,
        /// each page's text should be loaded individually right before its processing
        /// and saving.</summary>
        /// <exclude/>
        public void LoadWithMetadata()
        {
            if (IsEmpty())
                throw new WikiBotException(Bot.Msg("The PageList is empty. Nothing to load."));
            Console.WriteLine(Bot.Msg("Loading {0} pages..."), Pages.Count);

            string res = Site.IndexPath + "?title=Special:Export&action=submit";
            string postData = "curonly=True&pages=";
            foreach (Page page in Pages)
                postData += Bot.UrlEncode(page.Title) + "\r\n";
            string src = Site.PostDataAndGetResult(res, postData);
            XmlReader reader = XmlReader.Create(new StringReader(src));
            PageList pl = new PageList(Site);
            while (reader.ReadToFollowing("page"))
            {
                Page p = new Page(Site);
                p.ParsePageXml(reader.ReadOuterXml());
                pl.Add(p);
            }

            reader.Close();
            if (Pages.Count > 0)
            {
                Clear();
                Pages = pl.Pages;
                return;
            }

            // FALLBACK, use alternative parsing way, XPath
            Console.WriteLine(
                Bot.Msg("XML parsing failed, switching to alternative parser..."), Pages.Count);
            src = Bot.RemoveXmlRootAttributes(src);
            StringReader strReader = new StringReader(src);
            XPathDocument doc = new XPathDocument(strReader);
            strReader.Close();
            XPathNavigator nav = doc.CreateNavigator();
            foreach (Page page in Pages)
            {
                if (page.Title.Contains("'"))
                {
                    // There's no good way to escape "'" in XPath
                    page.LoadWithMetadata();
                    continue;
                }

                string query = "//page[title='" + page.Title + "']/";
                try
                {
                    page.Text =
                        nav.SelectSingleNode(query + "revision/text").InnerXml;
                }
                catch (NullReferenceException)
                {
                    continue;
                }

                page.Text = HttpUtility.HtmlDecode(page.Text);
                page.PageId = nav.SelectSingleNode(query + "id").InnerXml;
                try
                {
                    page.LastUser = nav.SelectSingleNode(query + "revision/contributor/username").InnerXml;
                    page.LastUserId = nav.SelectSingleNode(query + "revision/contributor/id").InnerXml;
                }
                catch (NullReferenceException)
                {
                    page.LastUser = nav.SelectSingleNode(query + "revision/contributor/ip").InnerXml;
                }

                page.LastUser = HttpUtility.HtmlDecode(page.LastUser);
                page.Revision = nav.SelectSingleNode(query + "revision/id").InnerXml;
                page.LastMinorEdit = (nav.SelectSingleNode(query + "revision/minor") == null) ? false : true;
                try
                {
                    page.Comment = nav.SelectSingleNode(query + "revision/comment").InnerXml;
                    page.Comment = HttpUtility.HtmlDecode(page.Comment);
                }
                catch (NullReferenceException)
                {
                }

                page.Timestamp = nav.SelectSingleNode(query + "revision/timestamp").ValueAsDateTime;
            }

            if (string.IsNullOrEmpty(this[0].Text))
            {
                // FALLBACK 2, load pages one-by-one
                foreach (Page page in Pages)
                    page.LoadWithMetadata();
            }
        }

        /// <summary>Gets page titles and page text from local XML dump.
        /// This function consumes much resources.</summary>
        /// <param name="filePathName">The path to and name of the XML dump file as string.</param>
        public void FillAndLoadFromXmlDump(string filePathName)
        {
            Console.WriteLine(Bot.Msg("Loading pages from XML dump..."));
            XmlReader reader = XmlReader.Create(filePathName);
            while (reader.ReadToFollowing("page")) {
                Page p = new Page(Site);
                p.ParsePageXml(reader.ReadOuterXml());
                Pages.Add(p);
            }

            reader.Close();
            Console.WriteLine(Bot.Msg("XML dump has been loaded successfully."));
        }

        /// <summary>Gets page titles and page texts from all ".txt" files in the specified
        /// directory (folder). Each file becomes a page. Page titles are constructed from
        /// file names. Page text is read from file contents. If any Unicode numeric codes
        /// (also known as numeric character references or NCRs) of the forbidden characters
        /// (forbidden in filenames) are recognized in filenames, those codes are converted
        /// to characters (e.g. "&#x7c;" is converted to "|").</summary>
        /// <param name="dirPath">The path and name of a directory (folder)
        /// to load files from.</param>
        public void FillAndLoadFromFiles(string dirPath)
        {
            foreach (string fileName in Directory.GetFiles(dirPath, "*.txt")) {
                Page p = new Page(Site, Path.GetFileNameWithoutExtension(fileName));
                p.Title = p.Title.Replace("&#x22;", "\"");
                p.Title = p.Title.Replace("&#x3c;", "<");
                p.Title = p.Title.Replace("&#x3e;", ">");
                p.Title = p.Title.Replace("&#x3f;", "?");
                p.Title = p.Title.Replace("&#x3a;", ":");
                p.Title = p.Title.Replace("&#x5c;", "\\");
                p.Title = p.Title.Replace("&#x2f;", "/");
                p.Title = p.Title.Replace("&#x2a;", "*");
                p.Title = p.Title.Replace("&#x7c;", "|");
                p.LoadFromFile(fileName);
                Pages.Add(p);
            }
        }

        /// <summary>Saves all pages in PageList to live wiki site. Uses 
        /// <see cref="DotNetWikiBot.Site.DefaultEditComment"/> and <see cref="DotNetWikiBot.Site.MinorEditByDefault"/>
        /// settings. This function doesn't limit the saving speed, so in case of working on
        /// public wiki, it's better to use <see cref="PageList.SaveSmoothly()"/> function in order
        /// to decrease server load.</summary>
        public void Save()
        {
            Save(Site.DefaultEditComment, Site.MinorEditByDefault);
        }

        /// <summary>Saves all pages in PageList to live wiki site. This function
        /// doesn't limit the saving speed, so in case of working on public wiki it's better
        /// to use <see cref="PageList.SaveSmoothly(int,string,bool)"/>
        /// function in order to decrease server load.</summary>
        /// <param name="comment">Your edit comment.</param>
        /// <param name="isMinorEdit">Minor edit mark (true = minor edit).</param>
        public void Save(string comment, bool isMinorEdit)
        {
            foreach (Page page in Pages)
                page.Save(page.Text, comment, isMinorEdit);
        }

        /// <summary>Saves all pages in PageList to live wiki site. The function waits for
        /// <see cref="DotNetWikiBot.Site.ForceSaveDelay"/> seconds (or for 5 seconds if
        /// <see cref="DotNetWikiBot.Site.ForceSaveDelay"/> equals zero, the default)
        /// between each page save operation in order to decrease server load. It uses
        /// <see cref="DotNetWikiBot.Site.DefaultEditComment"/> and <see cref="DotNetWikiBot.Site.MinorEditByDefault"/>
        /// settings.</summary>
        public void SaveSmoothly()
        {
            SaveSmoothly(
                Site.ForceSaveDelay > 0 ? Site.ForceSaveDelay : 5,
                Site.DefaultEditComment,
                Site.MinorEditByDefault);
        }

        /// <summary>Saves all pages in PageList to live wiki site. The function waits for specified
        /// number of seconds between each page save operation in order to decrease server load.
        /// It uses <see cref="DotNetWikiBot.Site.DefaultEditComment"/> and <see cref="DotNetWikiBot.Site.MinorEditByDefault"/>
        /// settings.</summary>
        /// <param name="intervalSeconds">Number of seconds to wait between each
        /// save operation.</param>
        public void SaveSmoothly(int intervalSeconds)
        {
            SaveSmoothly(intervalSeconds, Site.DefaultEditComment, Site.MinorEditByDefault);
        }

        /// <summary>Saves all pages in PageList to live wiki site. The function waits for specified
        /// number of seconds between each page save operation in order to decrease server load.
        /// </summary>
        /// <param name="intervalSeconds">Number of seconds to wait between each
        /// save operation.</param>
        /// <param name="comment">Edit comment.</param>
        /// <param name="isMinorEdit">Minor edit mark (true = minor edit).</param>
        public void SaveSmoothly(int intervalSeconds, string comment, bool isMinorEdit)
        {
            foreach (Page page in Pages) {
                Bot.Wait(intervalSeconds);
                page.Save(page.Text, comment, isMinorEdit);
            }
        }

        /// <summary>Undoes the last edit of every page in this PageList, so every page text reverts
        /// to previous contents. The function doesn't affect other operations
        /// like renaming.</summary>
        /// <param name="comment">Your edit comment.</param>
        /// <param name="isMinorEdit">Minor edit mark (true = minor edit).</param>
        public void Revert(string comment, bool isMinorEdit)
        {
            foreach (Page page in Pages)
                page.Revert(comment, isMinorEdit);
        }

        /// <summary>Saves titles of all pages in PageList to the specified file. Each title
        /// on a separate line. If the target file already exists, it is overwritten.</summary>
        /// <param name="filePathName">The path to and name of the target file as string.</param>
        public void SaveTitlesToFile(string filePathName)
        {
            StringBuilder titles = new StringBuilder();
            foreach (Page page in Pages)
                titles.Append(page.Title + "\r\n");
            File.WriteAllText(filePathName, titles.ToString().Trim(), Encoding.UTF8);
            Console.WriteLine(Bot.Msg("Titles in PageList have been saved to \"{0}\" file."), filePathName);
        }

        /// <summary>Saves the contents of all pages in pageList to ".txt" files in specified
        /// directory. Each page is saved to separate file, the name of that file is constructed
        /// from page title. Forbidden characters in filenames are replaced with their
        /// Unicode numeric codes (also known as numeric character references or NCRs).
        /// If the target file already exists, it is overwritten.</summary>
        /// <param name="dirPath">The path and name of a directory (folder)
        /// to save files to.</param>
        public void SaveToFiles(string dirPath)
        {
            string curDirPath = Directory.GetCurrentDirectory();
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            Directory.SetCurrentDirectory(dirPath);
            foreach (Page page in Pages)
                page.SaveToFile();
            Directory.SetCurrentDirectory(curDirPath);
        }

        /// <summary>Loads the contents of all pages in this pageList from live wiki site via XML
        /// export interface and saves the retrieved XML content to the specified file.
        /// The functions just dumps data, it does not load pages in PageList itself,
        /// call <see cref="PageList.Load()"/> or
        /// <see cref="PageList.FillAndLoadFromXmlDump(string)"/> to do that.
        /// Note that on some sites MediaWiki messages
        /// from standard namespace 8 are not available for export.</summary>
        /// <param name="filePathName">The path to and name of the target file.</param>
        public void SaveXmlDumpToFile(string filePathName)
        {
            Console.WriteLine(Bot.Msg("Loading {0} pages for XML dump..."), this.Pages.Count);
            string res = Site.IndexPath + "?title=Special:Export&action=submit";
            string postData = "catname=&curonly=true&action=submit&pages=";
            foreach (Page page in Pages)
                postData += Bot.UrlEncode(page.Title + "\r\n");
            string rawXML = Site.PostDataAndGetResult(res, postData);
            rawXML = Bot.RemoveXmlRootAttributes(rawXML).Replace("\n", "\r\n");
            if (File.Exists(filePathName))
                File.Delete(filePathName);
            FileStream fs = File.Create(filePathName);
            byte[] xmlBytes = new UTF8Encoding(true).GetBytes(rawXML);
            fs.Write(xmlBytes, 0, xmlBytes.Length);
            fs.Close();
            Console.WriteLine(Bot.Msg("XML dump has been saved to \"{0}\" file."), filePathName);
        }

        /// <summary>Removes all empty pages from PageList. But firstly don't forget to load
        /// the pages from site using pageList.LoadWithMetadata().</summary>
        public void RemoveEmpty()
        {
            for (int i = Pages.Count - 1; i >= 0; i--)
                if (this[i].IsEmpty())
                    Pages.RemoveAt(i);
        }

        /// <summary>Removes all recurring pages from PageList. Only one page with some title will
        /// remain in PageList. This makes all page elements in PageList unique.</summary>
        public void RemoveRecurring(bool merge = true)
        {
            for (int i = Pages.Count - 1; i >= 0; i--)
                for (int j = i - 1; j >= 0; j--)
                    if (Pages[i].Title == Pages[j].Title)
                    {
                        if (merge)
                            Pages[j].Categories.AddRange(Pages[i].Categories);
                        Pages.RemoveAt(i);
                        break;
                    }
        }

        /// <summary>Removes all redirecting pages from PageList. But firstly don't forget to load
        /// the pages from site using <see cref="PageList.Load()"/>.</summary>
        public void RemoveRedirects()
        {
            for (int i = Pages.Count - 1; i >= 0; i--)
                if (this[i].IsRedirect())
                    Pages.RemoveAt(i);
        }

        /// <summary>For all redirecting pages in this PageList, this function loads the titles and
        /// texts of redirected-to pages.</summary>
        public void ResolveRedirects()
        {
            foreach (Page page in Pages) {
                if (page.IsRedirect() == false)
                    continue;
                page.Title = page.RedirectsTo();
                page.Load();
            }
        }

        /// <summary>Removes all disambiguation pages from PageList. But firstly don't
        /// forget to load the pages from site using <see cref="PageList.Load()"/>.</summary>
        public void RemoveDisambigs()
        {
            for (int i = Pages.Count - 1; i >= 0; i--)
                if (this[i].IsDisambig())
                    Pages.RemoveAt(i);
        }

        /// <summary>Removes all pages from PageList.</summary>
        public void RemoveAll()
        {
            Pages.Clear();
        }

        /// <summary>Removes all pages from PageList.</summary>
        public void Clear()
        {
            Pages.Clear();
        }

        /// <summary>Function changes default English namespace prefixes to correct local prefixes
        /// (e.g. for German wiki-sites it changes "Category:..." to "Kategorie:...").</summary>
        public void CorrectNsPrefixes()
        {
            foreach (Page p in Pages)
                p.CorrectNsPrefix();
        }

        /// <summary>Shows if there are any Page objects in this PageList.</summary>
        /// <returns>Returns bool value.</returns>
        public bool IsEmpty()
        {
            return (Pages.Count == 0) ? true : false;
        }

        /// <summary>Sends titles of all contained pages to console.</summary>
        public void ShowTitles()
        {
            Console.WriteLine("\n" + Bot.Msg("Pages in this PageList:"));
            foreach (Page p in Pages)
                Console.WriteLine(p.Title);
            Console.WriteLine("\n");
        }

        /// <summary>Sends texts of all contained pages to console.</summary>
        public void ShowTexts()
        {
            Console.WriteLine("\n" + Bot.Msg("Texts of all pages in this PageList:"));
            Console.WriteLine("--------------------------------------------------");
            foreach (Page p in Pages) {
                p.ShowText();
                Console.WriteLine("--------------------------------------------------");
            }

            Console.WriteLine("\n");
        }

        /// <summary>Gets all levels of subcategories of some wiki category (that means
        /// subcategories, sub-subcategories, and so on) and fills this PageList with titles
        /// of all pages, found in all levels of subcategories. The multiplicates of recurring pages
        /// are removed. Subcategory pages are included.
        /// This operation may be very time-consuming and traffic-consuming.
        /// The function clears the PageList before filling begins.</summary>
        /// <param name="categories">Category names, with or without prefix. Only last is fetched</param>
        private void FillAllFromCategoryTree(List<string> categories)
        {
            var last = categories.Last();
            last = Site.CorrectNsPrefix(last);
            foreach (var page in FillAllFromCategory(last))
            {
                page.Categories.AddRange(categories);
                if (page.GetNamespace() == Site.CategoryNS)
                {
                    FillAllFromCategoryTree(new List<string>(categories) { page.Title });
                }
            }
        }
    }
}
