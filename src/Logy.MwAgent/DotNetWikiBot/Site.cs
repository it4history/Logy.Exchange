using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;

using Logy.MwAgent.DotNetWikiBot.Exceptions;

namespace Logy.MwAgent.DotNetWikiBot
{
    /// <summary>Class defines wiki site object.</summary>
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [Serializable]
    public class Site
    {
        /// <summary>Default edit comment. You can set it to whatever you would like.</summary>
        /// <example><code>mySite.defaultEditComment = "My default edit comment";</code></example>
        public const string DefaultEditComment = "Automatic page editing by robot";

        /// <summary>If set to true, all bot's edits are marked as minor by default.</summary>
        public const bool MinorEditByDefault = true;

        public const int CategoryNS = 14;
        public const int TemplateNS = 10;

        /// <summary>This is a maximum degree of server load when bot is
        /// still allowed to edit pages. Higher values mean more aggressive behaviour.
        /// See <see href="https://www.mediawiki.org/wiki/Manual:Maxlag_parameter">this page</see>
        /// for details.</summary>
        public const int MaxLag = 5;

        /// <summary>Number of times to retry bot web action in case of temporary connection
        /// failure or some server problems.</summary>
        public const int RetryTimes = 3;

        /// <summary>Number of list items to fetch at a time. This settings concerns special pages
        /// output and API lists output. Default is 500. Bot accounts are allowed to fetch
        /// up to 5000 items at a time. Adjust this number if required.</summary>
        public const int FetchRate = 500;

        public const string WikipediaBaseUrl = "https://en.wikipedia.org/wiki";

        /// <summary>Shortcut for Environment.NewLine property.
        /// It's "\r\n" on Windows and "\n" on Unix-like systems.</summary>
        private static readonly string Nl = Environment.NewLine;

        private static Site _wikipedia;

        /// <summary>Default domain for LDAP authentication, if such authentication is allowed on
        /// this site. Additional information can be found
        /// <see href="http://www.mediawiki.org/wiki/Extension:LDAP_Authentication">here</see>.
        /// </summary>
        private readonly string _userDomain = string.Empty;

        private bool _useApi = true;

        /// <summary>This constructor uses default userName and password for site, if
        /// userName and password are found in "Defaults.dat" file in bot's "Cache"
        /// subdirectory. File must be UTF8-encoded and must contain user names and passwords in
        /// the following format:
        /// <code>
        /// https://en.wikipedia.org|MyUserName|MyPassword
        /// https://de.wikipedia.org|MyUserName|MyPassword|MyDomain
        /// </code>
        /// Each site's accouint must be on the new line.
        /// This function allows distributing compiled bots without revealing passwords.</summary>
        /// <param name="address">Wiki site's URI. It must point to the main page of the wiki, e.g.
        /// "https://en.wikipedia.org" or "http://127.0.0.1:80/w/index.php?title=Main_page".</param>
        /// <returns>Returns Site object.</returns>
        public Site(string address)
        {
            string defaultsFile = Bot.CacheDir + Path.DirectorySeparatorChar + "Defaults.dat";
            if (File.Exists(defaultsFile))
            {
                string[] lines = File.ReadAllLines(defaultsFile, Encoding.UTF8);
                foreach (string line in lines)
                {
                    if (line.StartsWith(address + '|'))
                    {
                        string[] tokens = line.Split('|');
                        if (tokens.GetUpperBound(0) < 2)
                        {
                            throw new WikiBotException(Bot.Msg("\"\\Cache\\Defaults.dat\" file is invalid."));
                        }

                        Address = tokens[0];
                        UserName = tokens[1];
                        UserPass = tokens[2];
                        if (tokens.GetUpperBound(0) >= 3)
                            _userDomain = tokens[3];
                    }
                }

                if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserPass))
                    throw new WikiBotException(string.Format(
                        Bot.Msg("Site \"{0}\" was not found in \"\\Cache\\Defaults.dat\" file."),
                        address));
            }
            else
            {
                Address = address;
                Console.WriteLine(Bot.Msg("\"\\Cache\\Defaults.dat\" file not found."));
            }

            Initialize();
        }

        /// <summary>This constructor is used to generate most Site objects.</summary>
        /// <param name="address">Wiki site's URI. It must point to the main page of the wiki, e.g.
        /// "https://en.wikipedia.org" or "http://127.0.0.1:80/w/index.php?title=Main_page".</param>
        /// <param name="userName">User name to log in.</param>
        /// <param name="userPass">Password.</param>
        /// <returns>Returns Site object.</returns>
        public Site(string address, string userName, string userPass)
            : this(address, userName, userPass, string.Empty)
        {
        }

        /// <summary>This constructor is used for LDAP authentication. Additional information can
        /// be found <see href="http://www.mediawiki.org/wiki/Extension:LDAP_Authentication">here
        /// </see>.</summary>
        /// <param name="address">Wiki site's URI. It must point to the main page of the wiki, e.g.
        /// "https://en.wikipedia.org" or "http://127.0.0.1:80/w/index.php?title=Main_page".</param>
        /// <param name="userName">User name to log in.</param>
        /// <param name="userPass">Password.</param>
        /// <param name="userDomain">Domain name for LDAP authentication.</param>
        /// <returns>Returns Site object.</returns>
        public Site(string address, string userName, string userPass, string userDomain)
        {
            Address = address;
            UserName = userName;
            UserPass = userPass;
            _userDomain = userDomain;

            Initialize();
        }

        /// <summary>If true, assembly is running on Mono framework. If false,
        /// it is running on Microsoft .NET Framework. This variable is set
        /// automatically, don't change it's value.</summary>
        public static bool IsRunningOnMono
        {
            get { return Type.GetType("Mono.Runtime") != null; }
        }

        public static Site Wikipedia
        {
            get { return _wikipedia ?? (_wikipedia = new Site(WikipediaBaseUrl)); }
        }

        /// <summary>Number of seconds to pause for between edits on this site.
        /// Adjust this variable if required, but it may be overriden by the site policy.</summary>
        public static int ForceSaveDelay { get; set; }

        /// <summary>If set to false, bot will use MediaWiki's common user interface where
        /// possible, instead of using special API interface for robots (api.php). Default is true.
        /// Set it to false manually if some problem with API interface arises on site.</summary>
        public bool UseApi
        {
            get { return _useApi; }
            set { _useApi = value; }
        }

        /// <summary>Site's URI.</summary>
        public string Address { get; set; }

        /// <summary>User's account to login with.</summary>
        public string UserName { get; set; }

        /// <summary>User's password to login with.</summary>
        public string UserPass { get; set; }

        /// <summary>Site title, e.g. "Wikipedia".</summary>
        public string Name { get; set; }

        /// <summary>Site's software identificator, e.g. "MediaWiki 1.21".</summary>
        public string Software { get; set; }

        /// <summary>MediaWiki version as Version object.</summary>
        public Version Version { get; set; }

        /// <summary>Page title capitalization rule on this site.
        /// On most sites capitalization rule is "first-letter".</summary>
        public string Capitalization { get; set; }

        /// <summary>Site's time offset from UTC.</summary>
        public string TimeOffset { get; set; }

        /// <summary>Absolute path to MediaWiki's "index.php" file on the server.</summary>
        public string IndexPath { get; set; }

        /// <summary>Absolute path to MediaWiki's "api.php" file on the server.</summary>
        public string ApiPath { get; set; }

        /// <summary>Short relative path to wiki pages (if such alias is set on the server), e.g.
        /// "/wiki/". See <see href="http://www.mediawiki.org/wiki/Manual:Short URL">this page</see>
        /// for details.</summary>
        public string ShortPath { get; set; }

        /// <summary>User's watchlist. This <see cref="PageList"/> is not filled automatically when
        /// Site object is constructed, you need to call <see cref="PageList.FillFromWatchList()"/>
        /// function to fill it.</summary>
        public PageList WatchList { get; set; }

        /// <summary>MediaWiki system messages (those listed on "Special:Allmessages" page),
        /// user-modified versions. This dictionary is not filled automatically when Site object
        /// is constructed, you need to call <see cref="LoadMediawikiMessages(bool)"/> 
        /// function with "true" parameter to load messages into this dictionary.</summary>
        public Dictionary<string, string> Messages { get; set; }

        /// <summary>Templates, which are used to distinguish disambiguation pages. Set this
        /// variable manually if required. Multiple templates can be specified, use '|'
        /// character as the delimeter. Letters case doesn't matter.</summary>
        /// <example><code>site.disambig = "disambiguation|disambig|disam";</code></example>
        public string Disambig { get; set; }

        /// <summary>A set of regular expressions for parsing pages. Usually there is no need
        /// to edit these regular expressions manually.</summary>
        public Dictionary<string, Regex> Regexes { get; set; }

        /// <summary>Site's cookies.</summary>
        public CookieContainer Cookies { get; set; }

        /// <summary>Local namespaces, default namespaces and local namespace aliases, joined into
        /// strings, enclosed in and delimited by '|' character.</summary>
        public Dictionary<int, string> Namespaces { get; set; }

        /// <summary>Parsed supplementary data, mostly localized strings.</summary>
        public Dictionary<string, string> GeneralData { get; set; }

        /// <summary>Parsed API session-wide security tokens for editing.</summary>
        public Dictionary<string, string> Tokens { get; set; }

        /// <summary>Site's language.</summary>
        public string Language { get; set; }

        /// <summary>Site's language culture. Required for string comparison.</summary>
        public CultureInfo LangCulture { get; set; }

        /// <summary>Randomly chosen regional culture for this site's language.
        /// Required to parse dates.</summary>
        public CultureInfo RegCulture { get; set; }

        /// <summary>Supplementary data, mostly localized strings.</summary>
        /// <exclude/>
        public XElement GeneralDataXml { get; set; }

        /// <summary>Wiki server's time offset from local computer's time in seconds.
        /// Timezones difference is omitted, UTC time is compared with UTC time.</summary>
        /// <exclude/>
        public int TimeOffsetSeconds { get; set; }

        /// <summary>Time of last page saving operation on this site expressed in UTC.
        /// This internal parameter is used to prevent server overloading.</summary>
        /// <exclude/>
        public DateTime LastWriteTime { get; set; }

        public static string PostDataAndGetResult(string pageURL, string postData, string address, CookieContainer cookies = null, bool allowRedirect = true)
        {
            if (string.IsNullOrEmpty(pageURL))
                throw new ArgumentNullException("pageURL", Bot.Msg("No URL specified."));

            if (pageURL.StartsWith("/") && !pageURL.StartsWith("//"))
                pageURL = address + pageURL;

            int retryDelaySeconds = 60;
            HttpWebResponse webResp = null;
            for (int errorCounter = 0;; errorCounter++)
            {
                var webReq = CreateRequest(pageURL, allowRedirect, cookies); /// == null || cookies.Count == 0 ? new CookieContainer() : cookies);

                if (!string.IsNullOrEmpty(postData))
                {
                    if (IsRunningOnMono)    // Mono bug 636219 evasion
                        webReq.AllowAutoRedirect = false;

                    // https://bugzilla.novell.com/show_bug.cgi?id=636219
                    webReq.Method = "POST";
                    /// webReq.Timeout = 180000;
                    postData += "&maxlag=" + MaxLag;
                    var postBytes = Encoding.UTF8.GetBytes(postData);
                    webReq.ContentLength = postBytes.Length;
                    using (var reqStrm = webReq.GetRequestStream())
                    {
                        reqStrm.Write(postBytes, 0, postBytes.Length);
                        reqStrm.Close();
                    }
                }

                try
                {
                    webResp = (HttpWebResponse)webReq.GetResponse();
                    if (webResp.Headers["Retry-After"] != null)
                        throw new WebException("Service is unavailable due to high load.");

                    // API can return HTTP code 200 (OK) along with "Retry-After"
                    break;
                }
                catch (WebException e)
                {
                    if (webResp == null)
                        throw;

                    if (webReq.AllowAutoRedirect == false &&
                        webResp.StatusCode == HttpStatusCode.Redirect)    // Mono bug 636219 evasion
                        return string.Empty;

                    if (e.Message.Contains("Section=ResponseStatusLine"))
                    {   // Known Squid problem
                        Bot.SwitchUnsafeHttpHeaderParsing(true);
                        return PostDataAndGetResult(pageURL, postData, address, cookies, allowRedirect);
                    }

                    if (webResp.Headers["Retry-After"] != null)
                    {    // Server is very busy
                        if (errorCounter > RetryTimes)
                            throw;

                        // See https://www.mediawiki.org/wiki/Manual:Maxlag_parameter
                        int seconds;
                        int.TryParse(webResp.Headers["Retry-After"], out seconds);
                        if (seconds > 0)
                            retryDelaySeconds = seconds;
                        Console.Error.WriteLine(e.Message);
                        Console.Error.WriteLine(Bot.Msg("Retrying in {0} seconds..."), retryDelaySeconds);
                        Bot.Wait(retryDelaySeconds);
                    }
                    else if (e.Status == WebExceptionStatus.ProtocolError)
                    {
                        var code = (int)webResp.StatusCode;
                        if (code == 500 || code == 502 || code == 503 || code == 504)
                        {
                            // Remote server problem, retry
                            if (errorCounter > RetryTimes)
                                throw;
                            Console.Error.WriteLine(e.Message);
                            Console.Error.WriteLine(Bot.Msg("Retrying in {0} seconds..."), retryDelaySeconds);
                            Bot.Wait(retryDelaySeconds);
                        }
                        else
                            throw;
                    }
                    else
                        throw;
                }
            }

            string respStr;
            using (var respStream = webResp.GetResponseStream())
            {
                var r = respStream;
                if (webResp.ContentEncoding.ToLower().Contains("gzip"))
                {
                    r = new GZipStream(respStream, CompressionMode.Decompress);
                }
                else if (webResp.ContentEncoding.ToLower().Contains("deflate"))
                {
                    r = new DeflateStream(respStream, CompressionMode.Decompress);
                }

                if (cookies != null)
                {
                    var siteUri = new Uri(address);
                    foreach (Cookie cookie in webResp.Cookies)
                    {
                        if (cookie.Domain[0] == '.' &&
                            cookie.Domain.Substring(1) == siteUri.Host)
                            cookie.Domain = cookie.Domain.TrimStart(new char[] { '.' });
                        cookies.Add(cookie);
                    }
                }

                using (var strmReader = new StreamReader(r, Encoding.UTF8))
                {
                    respStr = strmReader.ReadToEnd();
                    strmReader.Close();
                }

                r.Close();
                respStream.Close();
            }

            webResp.Close();
            return respStr;
        }

        /// <summary>Gets MediaWiki system messages
        /// (those listed on "Special:Allmessages" page).</summary>
        /// <param name="modified">If true, the user-customized messages are returned.
        /// If false, original unmodified messages from MediaWiki package are returned.
        /// The latter is required very rarely.</param>
        /// <returns>Returns dictionary, where keys are message identifiers (all in lower case)
        /// and values are message texts.</returns>
        public Dictionary<string, string> LoadMediawikiMessages(bool modified)
        {
            Console.WriteLine(Bot.Msg("Updating MediaWiki messages. Please, wait..."));
            Dictionary<string, string> mediaWikiMessages;

            if (UseApi && modified) {    // there is no way to get unmodified versions via API
                // no paging is required, all messages are returned in one chunk
                string src = GetWebPage(ApiPath + "?action=query" + "&meta=allmessages&format=xml&amenableparser=1&amcustomised=all");
                /// "&amcustomised=all" query actually brings users-modified messages

                XElement messagesXml = XElement.Parse(src);

                mediaWikiMessages = (from el in messagesXml.Descendants("message")
                                     select new
                                                {
                                                    id = el.Attribute("name").Value,
                                                    body = el.Value
                                                }).ToDictionary(s => s.id, s => s.body);
            }
            else {
                // paging may be broken
                string res = IndexPath + "?title=Special:AllMessages&limit=50000&filter=all";
                string src = string.Empty, messageId = string.Empty;
                mediaWikiMessages = new Dictionary<string, string>();
                Regex nextPortionRegex = new Regex("offset=([^\"&]+)[^\"]*?\" title=\"[^\"]+\" rel=\"next\"");
                do {
                    var offset = !string.IsNullOrEmpty(src) 
                        ? "&offset=" + HttpUtility.HtmlDecode(nextPortionRegex.Match(src).Groups[1].Value) 
                        : string.Empty;
                    src = GetWebPage(res + offset);
                    src = Bot.GetSubstring(src, "<tbody>", "</tbody>");
                    using (XmlReader reader = Bot.GetXmlReader(src)) {
                        reader.ReadToFollowing("tbody");
                        while (reader.Read()) {
                            if (reader.Name == "tr"
                                && reader.NodeType == XmlNodeType.Element
                                && reader["id"] != null)
                                messageId = reader["id"].Replace("msg_", string.Empty);
                            else if (reader.Name == "td"
                                     && reader.NodeType == XmlNodeType.Element
                                     && reader["class"] == "am_default")
                                mediaWikiMessages[messageId] = reader.ReadString();
                            else if (modified
                                     && reader.Name == "td"
                                     && reader.NodeType == XmlNodeType.Element
                                     && reader["class"] == "am_actual")
                                mediaWikiMessages[messageId] = reader.ReadString();
                            else if (reader.Name == "tbody"
                                     && reader.NodeType == XmlNodeType.EndElement)
                                break;
                        }
                    }
                } while (nextPortionRegex.IsMatch(src));
            }

            if (modified)
                Messages = mediaWikiMessages;
            Console.WriteLine(Bot.Msg("MediaWiki system messages have been loaded successfully."));
            return mediaWikiMessages;
        }

        /// <summary>Gets the list of all WikiMedia Foundation wiki sites as listed
        /// <see href="http://meta.wikimedia.org/wiki/Special:SiteMatrix">here</see>.</summary>
        /// <param name="officialOnly">If set to false, function also returns special and private
        /// WikiMedia projects.</param>
        /// <returns>Returns list of strings.</returns>
        public List<string> GetWikimediaProjects(bool officialOnly)
        {
            string src = GetWebPage("http://meta.wikimedia.org/wiki/Special:SiteMatrix");
            if (officialOnly)
                src = Bot.GetSubstring(src, "<a id=\"aa\" name=\"aa\">", "<a id=\"total\" name=\"total\">");
            else
                src = Bot.GetSubstring(src, "<a id=\"aa\" name=\"aa\">", "class=\"printfooter\"");
            Regex siteRegex = new Regex("<a href=\"(?://)?([^\"/]+)");
            return siteRegex.Matches(src).Cast<Match>().Select(m => m.Groups[1].Value).ToList();
        }

        /// <summary>Gets the text of page from web.</summary>
        /// <param name="pageURL">Absolute or relative URI of page to get.</param>
        /// <returns>Returns source code.</returns>
        public string GetWebPage(string pageURL)
        {
            return PostDataAndGetResult(pageURL, string.Empty, true, true);
        }

        /// <summary>Posts specified string to requested resource
        /// and gets the result text.</summary>
        /// <param name="pageURL">Absolute or relative URI of page to get.</param>
        /// <param name="postData">String to post to site with web request.</param>
        /// <returns>Returns text.</returns>
        public string PostDataAndGetResult(string pageURL, string postData)
        {
            return PostDataAndGetResult(pageURL, postData, true, true);
        }

        /// <summary>Posts specified string to requested resource
        /// and gets the result text.</summary>
        /// <param name="pageURL">Absolute or relative URI of page to get.</param>
        /// <param name="postData">String to post to site with web request.</param>
        /// <param name="getCookies">If set to true, gets cookies from web response and
        /// saves it in Site.cookies container.</param>
        /// <param name="allowRedirect">Allow auto-redirection of web request by server.</param>
        /// <returns>Returns text.</returns>
        public string PostDataAndGetResult(string pageURL, string postData, bool getCookies, bool allowRedirect)
        {
            return PostDataAndGetResult(pageURL, postData, Address, getCookies ? Cookies : null, allowRedirect);
        }

        /// <summary>Gets and parses results of specified custom API query.
        /// Only some basic queries are supported and can be parsed automatically.</summary>
        /// <param name="query">Type of query, e.g. "list=logevents" or "prop=links".</param>
        /// <param name="queryParams">Additional query parameters, specific to the
        /// query. Options and their descriptions can be obtained by calling api.php on target site
        /// without parameters, e.g. http://en.wikipedia.org/w/api.php,
        /// <see href="http://en.wikipedia.org/wiki/Special:ApiSandbox">API Sandbox</see>
        /// is also very useful for experiments.
        /// Parameters' values must be URL-encoded with <see cref="Bot.UrlEncode(string)"/> function
        /// before calling this function.</param>
        /// <param name="limit">Maximum number of resultant strings to fetch.</param>
        /// <example><code>
        /// GetApiQueryResult("list=categorymembers",
        ///     "cmnamespace=0|14&amp;cmcategory=" + Bot.UrlEncode("Physical sciences"),
        ///     int.MaxValue);
        /// </code></example>
        /// <example><code>
        /// GetApiQueryResult("list=logevents",
        ///     "letype=patrol&amp;titles=" + Bot.UrlEncode("Physics"),
        ///     200);
        /// </code></example>
        /// <example><code>
        /// GetApiQueryResult("prop=links",
        ///     "titles=" + Bot.UrlEncode("Physics"),
        ///     int.MaxValue);
        /// </code></example>
        /// <returns>List of dictionary objects is returned. Dictionary keys will contain the names
        /// of attributes of each found target element, and dictionary values will contain values
        /// of those attributes. If target element is not empty element, it's value will be
        /// included into dictionary under special "_Value" key.</returns>
        public List<Dictionary<string, string>> GetApiQueryResult(string query, string queryParams, int limit)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentNullException("query");
            if (limit <= 0)
                throw new ArgumentOutOfRangeException("limit");

            var queryXml =
                from el in Bot.CommonDataXml.Element("ApiOptions").Descendants("query")
                where el.Value == query
                select el;
            if (queryXml == null)
                throw new WikiBotException(
                    string.Format(Bot.Msg("The list \"{0}\" is not supported."), query));

            string prefix = queryXml.FirstOrDefault().Attribute("prefix").Value;
            string targetTag = queryXml.FirstOrDefault().Attribute("tag").Value;
            string targetAttribute = queryXml.FirstOrDefault().Attribute("attribute").Value;
            if (string.IsNullOrEmpty(prefix) || string.IsNullOrEmpty(targetTag))
                throw new WikiBotException(
                    string.Format(Bot.Msg("The list \"{0}\" is not supported."), query));

            var results = new List<Dictionary<string, string>>();
            string continueFromAttr = prefix + "from";
            string continueAttr = prefix + "continue";
            string queryUri = ApiPath + "?format=xml&action=query&" + query +
                              '&' + prefix + "limit=" + (limit > FetchRate ? FetchRate : limit).ToString();
            string src = string.Empty, next = string.Empty, queryFullUri = string.Empty;
            do
            {
                queryFullUri = queryUri;
                if (next != string.Empty)
                    queryFullUri += '&' + prefix + "continue=" + Bot.UrlEncode(next);
                src = PostDataAndGetResult(queryFullUri, queryParams);
                using (XmlTextReader reader = new XmlTextReader(new StringReader(src)))
                {
                    next = string.Empty;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == targetTag) {
                            var dict = new Dictionary<string, string>();
                            if (!reader.IsEmptyElement) {
                                dict["_Value"] = HttpUtility.HtmlDecode(reader.Value);
                                if (targetAttribute == null)
                                    dict["_Target"] = dict["_Value"];
                            }

                            for (int i = 0; i < reader.AttributeCount; i++) {
                                reader.MoveToAttribute(i);
                                dict[reader.Name] = HttpUtility.HtmlDecode(reader.Value);
                                if (targetAttribute != null && reader.Name == targetAttribute)
                                    dict["_Target"] = dict[reader.Name];
                            }

                            results.Add(dict);
                        }
                        else if (reader.IsEmptyElement && reader[continueFromAttr] != null)
                            next = reader[continueFromAttr];
                        else if (reader.IsEmptyElement && reader[continueAttr] != null)
                            next = reader[continueAttr];
                    }
                }
            }
            while (next != string.Empty && results.Count < limit);

            if (results.Count > limit) {
                results.RemoveRange(limit, results.Count - limit);
            }

            return results;
        }

        /// <summary>Gets main local prefix for specified namespace and colon.</summary>
        /// <param name="nsIndex">Index of namespace to get prefix for.</param>
        /// <returns>Returns the prefix with colon, e.g., "Kategorie:".</returns>
        public string GetNsPrefix(int nsIndex)
        {
            if (nsIndex == 0)
                return string.Empty;
            if (!Namespaces.Keys.Contains(nsIndex))
                throw new ArgumentOutOfRangeException("nsIndex");
            return Namespaces[nsIndex].Substring(1, Namespaces[nsIndex].IndexOf('|', 1) - 1) + ':';
        }

        /// <summary>Gets canonical default English prefix for specified namespace and colon.
        /// If default prefix is not found the main local prefix is returned.</summary>
        /// <param name="nsIndex">Index of namespace to get prefix for.</param>
        /// <returns>Returns the prefix with colon, e.g., "Category:".</returns>
        public string GetEnglishNsPrefix(int nsIndex)
        {
            if (nsIndex == 0)
                return string.Empty;
            if (!Namespaces.Keys.Contains(nsIndex))
                throw new ArgumentOutOfRangeException("nsIndex");
            int secondDelimPos = Namespaces[nsIndex].IndexOf('|', 1);
            int thirdDelimPos = Namespaces[nsIndex].IndexOf('|', secondDelimPos + 1);
            if (thirdDelimPos == -1)
                return Namespaces[nsIndex].Substring(1, secondDelimPos - 1) + ':';
                return Namespaces[nsIndex].Substring(secondDelimPos + 1, thirdDelimPos - secondDelimPos - 1) + ':';
        }

        /// <summary>Gets all names and aliases for specified namespace delimited by '|' character
        /// and escaped for use within Regex patterns.</summary>
        /// <param name="nsIndex">Index of namespace to get prefixes for.</param>
        /// <returns>Returns prefixes string, e.g. "Category|Kategorie".</returns>
        public string GetNsPrefixes(int nsIndex)
        {
            if (!Namespaces.Keys.Contains(nsIndex))
                throw new ArgumentOutOfRangeException("nsIndex");
            string str = Namespaces[nsIndex].Substring(1, Namespaces[nsIndex].Length - 2);
            str = str.Replace('|', '%');    // '%' is not escaped
            str = Regex.Escape(str);    // escapes only \*+?|{[()^$.# and whitespaces
            str = str.Replace('%', '|');    // return back '|' delimeter
            return str;
        }

        /// <summary>Identifies the namespace of the page.</summary>
        /// <param name="pageTitle">Page title to identify the namespace of.</param>
        /// <returns>Returns the integer key of the namespace.</returns>
        public int GetNamespace(string pageTitle)
        {
            if (string.IsNullOrEmpty(pageTitle))
                throw new ArgumentNullException("pageTitle");
            int colonPos = pageTitle.IndexOf(':');
            if (colonPos == -1 || colonPos == 0)
                return 0;
            string pageNS = '|' + pageTitle.Substring(0, colonPos) + '|';
            return (from ns in Namespaces where ns.Value.Contains(pageNS) select ns.Key).FirstOrDefault();
        }

        /// <summary>Removes the namespace prefix from page title.</summary>
        /// <param name="pageTitle">Page title to remove prefix from.</param>
        /// <param name="nsIndex">Integer key of namespace to remove. If this parameter is 0
        /// any found namespace prefix is removed.</param>
        /// <returns>Page title without prefix.</returns>
        public string RemoveNsPrefix(string pageTitle, int nsIndex)
        {
            if (string.IsNullOrEmpty(pageTitle))
                throw new ArgumentNullException("pageTitle");
            if (!Namespaces.Keys.Contains(nsIndex))
                throw new ArgumentOutOfRangeException("nsIndex");
            if (pageTitle[0] == ':')
                pageTitle = pageTitle.TrimStart(':');
            int colonPos = pageTitle.IndexOf(':');
            if (colonPos == -1)
                return pageTitle;
            string pagePrefixPattern = '|' + pageTitle.Substring(0, colonPos) + '|';
            if (nsIndex != 0) {
                if (Namespaces[nsIndex].Contains(pagePrefixPattern))
                    return pageTitle.Substring(colonPos + 1);
            }
            else {
                foreach (KeyValuePair<int, string> ns in Namespaces) {
                    if (ns.Value.Contains(pagePrefixPattern))
                        return pageTitle.Substring(colonPos + 1);
                }
            }

            return pageTitle;
        }

        /// <summary>Function changes default English namespace prefixes and local namespace aliases
        /// to canonical local prefixes (e.g. for German wiki-sites it changes "Category:..."
        /// to "Kategorie:...").</summary>
        /// <param name="pageTitle">Page title to correct prefix in.</param>
        /// <returns>Page title with corrected prefix.</returns>
        public string CorrectNsPrefix(string pageTitle)
        {
            if (string.IsNullOrEmpty(pageTitle))
                throw new ArgumentNullException("pageTitle");
            if (pageTitle[0] == ':')
                pageTitle = pageTitle.TrimStart(':');
            int ns = GetNamespace(pageTitle);
            if (ns == 0)
                return pageTitle;
            return GetNsPrefix(ns) + RemoveNsPrefix(pageTitle, ns);
        }

        /// <summary>Shows names and integer keys of local and default namespaces and namespace
        /// aliases.</summary>
        public void ShowNamespaces()
        {
            foreach (KeyValuePair<int, string> ns in Namespaces) {
                Console.WriteLine(ns.Key.ToString() + '\t' + ns.Value.Replace("|", Nl + '\t'));
            }
        }

        private static HttpWebRequest CreateRequest(string pageURL, bool allowRedirect, CookieContainer cookies)
        {
            var webReq = (HttpWebRequest)WebRequest.Create(pageURL);
            webReq.Proxy.Credentials = CredentialCache.DefaultCredentials;
            webReq.UseDefaultCredentials = true;
            webReq.ContentType = "application/x-www-form-urlencoded";
            webReq.Headers.Add("Cache-Control", "no-cache, must-revalidate");
            webReq.UserAgent = Bot.BotVer;
            webReq.AllowAutoRedirect = allowRedirect;
            webReq.CookieContainer = cookies;
            if (Bot.UnsafeHttpHeaderParsingUsed == 0)
            {
                webReq.ProtocolVersion = HttpVersion.Version10;
                webReq.KeepAlive = false;
            }

            if (!IsRunningOnMono)
            {    // Mono bug evasion
                // last checked in January 2015 on Mono 3.12 for Windows
                // http://mono.1490590.n4.nabble.com/...
                // ...EntryPointNotFoundException-CreateZStream-td4661364.html
                webReq.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            }

            return webReq;
        }

        /// <summary>This internal function initializes Site object.</summary>
        /// <exclude/>
        private void Initialize()
        {
            LastWriteTime = DateTime.MinValue;
            Cookies = new CookieContainer();

            // Correct the address if required
            if (!Address.StartsWith("http"))
                Address = "http://" + Address;
            if (Bot.CountMatches(Address, "/", false) == 3 && Address.EndsWith("/"))
                Address = Address.Remove(Address.Length - 1);

            InitRegexes();

            // Find path to index.php
            string cacheFile = Bot.CacheDir + Path.DirectorySeparatorChar +
                               Bot.UrlEncode(Address.Replace("://", ".").Replace("/", ".")) + ".xml";
            if (!Directory.Exists(Bot.CacheDir))
                Directory.CreateDirectory(Bot.CacheDir);
            XElement cache;
            if (File.Exists(cacheFile))
            {
                cache = XElement.Load(cacheFile);
                IndexPath = cache.Descendants("indexPath").FirstOrDefault().Value;
            }
            else
            {
                string src = GetWebPage(Address);
                var addressUri = new Uri(Address);
                var hrefRegex = new Regex("(?i) href=\"(([^\"]*)(index|api)\\.php)");
                try
                {
                    foreach (Match m in hrefRegex.Matches(src))
                    {
                        if (m.Groups[1].Value.StartsWith(Address))
                        {
                            IndexPath = m.Groups[2].Value + "index.php";
                            break;
                        }

                        if (m.Groups[1].Value.StartsWith("//" + addressUri.Authority))
                        {
                            if (Address.StartsWith("https:"))
                                IndexPath = "https:" + m.Groups[2].Value + "index.php";
                            else
                                IndexPath = "http:" + m.Groups[2].Value + "index.php";
                            break;
                        }

                        if (m.Groups[1].Value[0] == '/' && m.Groups[1].Value[1] != '/')
                        {
                            if (Address.StartsWith("https:"))
                                IndexPath = "https://" + addressUri.Authority + m.Groups[2].Value + "index.php";
                            else
                                IndexPath = "http://" + addressUri.Authority + m.Groups[2].Value + "index.php";

                            // was indexPath = address + m.Groups[2].Value + "index.php";
                            break;
                        }

                        if (string.IsNullOrEmpty(m.Groups[2].Value))
                        {
                            IndexPath = Address + "/index.php";
                            break;
                        }
                    }
                }
                catch
                {
                    throw new WikiBotException(Bot.Msg("Can't find path to index.php."));
                }

                if (IndexPath == null)
                    throw new WikiBotException(Bot.Msg("Can't find path to index.php."));
                if (IndexPath.Contains("api.php"))
                    IndexPath = IndexPath.Replace("api.php", "index.php");

                cache = new XElement("siteInfo", new XElement("indexPath", IndexPath));
                cache.Save(cacheFile);
            }

            ApiPath = IndexPath.Replace("index.php", "api.php");

            LogIn();

            LoadGeneralInfo();

            // Load API security tokens if available
            string tokensXmlSrc = GetWebPage(ApiPath + "?action=query&format=xml&meta=tokens" +
                                             "&type=csrf|deleteglobalaccount|patrol|rollback|setglobalaccountstatus" +
                                             "|userrights|watch&curtimestamp");
            var tokensXml = XElement.Parse(tokensXmlSrc);
            if (tokensXml.Element("query") != null)
            {
                Tokens = (from attr in tokensXml.Element("query").Element("tokens").Attributes()
                          select new
                                     {
                                         attrName = attr.Name.ToString(),
                                         attrValue = attr.Value
                                     }).ToDictionary(s => s.attrName, s => s.attrValue);
            }

            if (!IsRunningOnMono)
                Bot.DisableCanonicalizingUriAsFilePath();    // .NET bug evasion

            Bot.LastSite = this;

            Console.WriteLine(Bot.Msg("Site: {0} ({1})"), Name, Software);
        }

        private void InitRegexes()
        {
            Regexes = new Dictionary<string, Regex>();
            Regexes["titleLink"] =
                new Regex("<a [^>]*title=\"(?<title>.+?)\"");
            Regexes["titleLinkInList"] =
                new Regex("<li(?: [^>]*)?>\\s*<a [^>]*title=\"(?<title>.+?)\"");
            Regexes["titleLinkInTable"] =
                new Regex("<td(?: [^>]*)?>\\s*<a [^>]*title=\"(?<title>.+?)\"");
            Regexes["titleLinkShown"] =
                new Regex("<a [^>]*title=\"([^\"]+)\"[^>]*>\\s*\\1\\s*</a>");
            Regexes["linkToSubCategory"] =
                new Regex(">([^<]+)</a></div>\\s*<div class=\"CategoryTreeChildren\"");
            Regexes["linkToImage"] =
                new Regex("<div class=\"gallerytext\">\n<a href=\"[^\"]*?\" title=\"([^\"]+?)\">");
            Regexes["wikiLink"] =
                new Regex(@"\[\[(?<link>(?<title>.+?)(?<params>\|.+?)?)]]");
            Regexes["wikiTemplate"] =
                new Regex(@"(?s)\{\{(.+?)((\|.*?)*?)}}");
            Regexes["webLink"] =
                new Regex("(https?|t?ftp|news|nntp|telnet|irc|gopher)://([^\\s'\"<>]+)");
            Regexes["noWikiMarkup"] =
                new Regex("(?is)<nowiki>(.*?)</nowiki>");
            Regexes["editToken"] =
                new Regex("(?i)value=\"([^\"]+)\"[^>]+name=\"wpEditToken\"" +
                          "|name=\"wpEditToken\"[^>]+value=\"([^\"]+)\"");
            Regexes["editTime"] =
                new Regex("(?i)value=\"([^\"]+)\"[^>]+name=\"wpEdittime\"" +
                          "|name=\"wpEdittime\"[^>]+value=\"([^\"]+)\"");
            Regexes["startTime"] =
                new Regex("(?i)value=\"([^\"]+)\"[^>]+name=\"wpStarttime\"" +
                          "|name=\"wpStarttime\"[^>]+value=\"([^\"]+)\"");
            Regexes["baseRevId"] =
                new Regex("(?i)value=\"([^\"]+)\"[^>]+name=\"baseRevId\"" +
                          "|name=\"baseRevId\"[^>]+value=\"([^\"]+)\"");
        }

        /// <summary>This internal function gets general information about the site.</summary>
        /// <exclude/>
        private void LoadGeneralInfo()
        {
            var pageUrl = ApiPath + "?action=query&format=xml" +
                          "&meta=siteinfo&siprop=general|namespaces|namespacealiases|magicwords|" +
                          "interwikimap|fileextensions|variables";
            string src = GetWebPage(pageUrl);
            GeneralDataXml = XElement.Parse(src).Element("query");

            // Load namespaces
            Namespaces = (from el in GeneralDataXml.Element("namespaces").Descendants("ns") // where el.Attribute("id").Value != "0"
                          select new
                          {
                              code = int.Parse(el.Attribute("id").Value),
                              name = '|' + (el.IsEmpty ? string.Empty : el.Value) +
                                      '|' + (!el.IsEmpty && el.Value != el.Attribute("canonical").Value
                                                 ? el.Attribute("canonical").Value + '|'
                                                 : string.Empty)
                          }).ToDictionary(s => s.code, s => s.name);

            // Load and add namespace aliases
            var aliases = from el in GeneralDataXml.Element("namespacealiases").Descendants("ns")
                          select new
                          {
                              code = int.Parse(el.Attribute("id").Value),
                              name = el.Value
                          };
            foreach (var alias in aliases)
                if (Namespaces.ContainsKey(alias.code)) // there may be aliases 104-Type, 105-Type talk without namespaces
                            Namespaces[alias.code] += alias.name + '|'; // namespace 0 may have an alias (!)

            // Load general site properties
            GeneralData = (from attr in GeneralDataXml.Element("general").Attributes()
                           select new
                           {
                               attrName = attr.Name.ToString(),
                               attrValue = attr.Value
                           }).ToDictionary(s => s.attrName, s => s.attrValue);

            // Load interwiki which are recognized locally, interlanguage links are included
            // Prefixes are combined into string delimited by '|'
            GeneralData["interwiki"] = string.Join(
                "|",
                (from el in GeneralDataXml.Descendants("iw") select el.Attribute("prefix").Value).ToArray());

            // Load MediaWiki variables (https://www.mediawiki.org/wiki/Help:Magic_words)
            // These are used in double curly brackets, like {{CURRENTVERSION}} and must
            // be distinguished from templates.
            // Variables are combined into string delimited by '|'.
            GeneralData["variables"] = string.Join(
                "|",
                (from el in GeneralDataXml.Descendants("v") select el.Value).ToArray());

            // Load MediaWiki magic words (https://www.mediawiki.org/wiki/Help:Magic_words)
            // These include MediaWiki variables and parser functions which are used in
            // double curly brackets, like {{padleft:xyz|stringlength}} or
            // {{#formatdate:date}} and must be distinguished from templates.
            // Magic words are combined into string delimited by '|'.
            GeneralData["magicWords"] = string.Join(
                "|", (from el in GeneralDataXml.Element("magicwords").Descendants("alias") select el.Value).ToArray());

            // Set Site object's properties
            if (GeneralData.ContainsKey("articlepath"))
                ShortPath = GeneralData["articlepath"].Replace("$1", string.Empty);
            if (GeneralData.ContainsKey("generator"))
            {
                Version = new Version(Regex.Replace(GeneralData["generator"], @"[^\d\.]", string.Empty));
                if (Version < new Version("1.20"))
                {
                    var message = "WARNING: This MediaWiki " +
                                  "version is outdated, some bot functions may not work properly. Please " +
                                  "consider downgrading to DotNetWikiBot {0} to work with this site.";
                    Console.WriteLine(Nl + Nl + Bot.Msg(message) + Nl + Nl, "2.x");
                }
            }

            Language = GeneralData["lang"];
            Capitalization = GeneralData["case"];
            TimeOffset = GeneralData["timeoffset"];
            Name = GeneralData["sitename"];
            Software = GeneralData["generator"];

            DateTime wikiServerTime = DateTime.ParseExact(
                GeneralData["time"],
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'",
                CultureInfo.InvariantCulture);
            
            // 2 seconds are substracted so we never get time in the future on the server
            TimeOffsetSeconds = (int)(wikiServerTime - DateTime.UtcNow).TotalSeconds - 2;

            LoadRegenals();

            // Load local redirection tags
            GeneralData["redirectTags"] = (from el in Bot.CommonDataXml.Element("RedirectionTags").Descendants("rd")
                                           where el.Attribute("lang").Value == Language
                                           select el.Value).SingleOrDefault();

            ConstructRegexes();

            // that's right, localized namespace prefix only

            // Parser functions are now loaded from site (among magicWords)
            // generalData.Add("parserFunctions",
            // Bot.commonDataXml.Descendants("pf").Select(el => el.Value).ToArray());
        }

        /// <summary>
        /// Construct regular expressions
        /// RegexOptions.Compiled option seems to be too slow
        /// </summary>
        private void ConstructRegexes()
        {
            Regexes["redirect"] = new Regex(@"(?i)^ *#(?:" + GeneralData["redirectTags"] + @")\s*:?\s*\[\[(.+?)(\|.+)?]]");

            Regexes["magicWordsAndVars"] = new Regex(
                @"^(?:" + GeneralData["magicWords"].ToLower() + '|' + GeneralData["variables"] + ')');

            string allNsPrefixes = string.Join(
                "|",
                Namespaces.Select(x => x.Value.Substring(1, x.Value.Length - 2)).ToArray());
            allNsPrefixes = allNsPrefixes.Replace("||", "|");    // remove empty string from ns 0

            // (?i)^(?:Media|Special|Talk|User|U|User talk|UT|...|Module talk):
            Regexes["allNsPrefixes"] = new Regex(@"(?i)^(?:" + allNsPrefixes + "):");

            Regexes["interwikiLink"] = new Regex(@"(?i)\[\[((" + GeneralData["interwiki"] + "):(.+?))]]");

            Regexes["wikiCategory"] = new Regex(@"(?i)\[\[\s*(((" + GetNsPrefixes(CategoryNS) + @"):(.+?))(\|.+?)?)]]");

            Regexes["wikiImage"] = new Regex(@"\[\[(?i)((" + GetNsPrefixes(6) + @"):(.+?))(\|(.+?))*?]]");

            Regexes["linkToImage2"] = new Regex("<a href=\"[^\"]*?\" title=\"(" + Regex.Escape(GetNsPrefix(6)) + "[^\"]+?)\">");
        }

        /// <summary>
        /// Select general and regional CultureInfo, mainly for datetime parsing
        /// </summary>
        private void LoadRegenals()
        {
            try
            {
                LangCulture = new CultureInfo(Language, false);
            }
            catch (Exception)
            {
                LangCulture = new CultureInfo(string.Empty);
            }

            if (LangCulture.Equals(CultureInfo.CurrentUICulture.Parent))
                RegCulture = CultureInfo.CurrentUICulture;
            else
            {
                try
                {
                    RegCulture = CultureInfo.CreateSpecificCulture(Language);
                }
                catch (Exception)
                {
                    foreach (CultureInfo ci in
                        CultureInfo.GetCultures(CultureTypes.SpecificCultures))
                    {
                        if (LangCulture.Equals(ci.Parent))
                        {
                            RegCulture = ci;
                            break;
                        }
                    }

                    if (RegCulture == null)
                        RegCulture = CultureInfo.InvariantCulture;
                }
            }
        }

        /// <summary>Logs in and retrieves cookies.</summary>
        private void LogIn()
        {
            if (string.IsNullOrEmpty(UserName))
                return;

            Console.WriteLine(Bot.Msg("Logging in..."));
            if (!UseApi)
            {
                string loginPageSrc = PostDataAndGetResult(IndexPath + "?title=Special:Userlogin", string.Empty, true, true);
                string loginToken = string.Empty;
                int loginTokenPos = loginPageSrc.IndexOf(
                    "<input type=\"hidden\" name=\"wpLoginToken\" value=\"");
                if (loginTokenPos != -1)
                    loginToken = loginPageSrc.Substring(loginTokenPos + 48, 32);

                string postData = string.Format(
                    "wpName={0}&wpPassword={1}&wpDomain={2}" + "&wpLoginToken={3}&wpRemember=1&wpLoginattempt=Log+in",
                    Bot.UrlEncode(UserName),
                    Bot.UrlEncode(UserPass),
                    Bot.UrlEncode(_userDomain),
                    Bot.UrlEncode(loginToken));
                string respStr = PostDataAndGetResult(
                    IndexPath + "?title=Special:Userlogin&action=submitlogin&type=login",
                    postData,
                    true,
                    false);
                if (respStr.Contains("<div class=\"errorbox\">"))
                    throw new WikiBotException("\n\n" + Bot.Msg("Login failed. Check your username and password.") + "\n");
                Console.WriteLine(Bot.Msg("Logged in as {0}."), UserName);
            }
            else
            {
                string postData = string.Format(
                    "lgname={0}&lgpassword={1}&lgdomain={2}",
                    Bot.UrlEncode(UserName),
                    Bot.UrlEncode(UserPass),
                    Bot.UrlEncode(_userDomain));

                // At first load login security token
                string tokenXmlSrc = PostDataAndGetResult(
                    ApiPath + "?action=query&meta=tokens&type=login&format=xml", 
                    string.Empty, 
                    true, 
                    false);
                XElement tokenXml = XElement.Parse(tokenXmlSrc);
                string respStr = string.Empty, loginToken = string.Empty;
                try
                {
                    loginToken = tokenXml.Element("query").Element("tokens")
                        .Attribute("logintoken").Value;
                }
                catch
                {
                    // old fallback method
                    respStr = PostDataAndGetResult(ApiPath + "?action=login&format=xml", postData, true, false);
                    if (respStr.Contains("result=\"Success\""))
                    {
                        Console.WriteLine(Bot.Msg("Logged in as {0}."), UserName);
                        return;
                    }

                    int tokenPos = respStr.IndexOf("token=\"");
                    if (tokenPos < 1)
                        throw new WikiBotException("\n\n" + Bot.Msg("Login failed. Check your username and password.") + "\n");
                    loginToken = respStr.Substring(tokenPos + 7, 32);
                }

                postData += "&lgtoken=" + Bot.UrlEncode(loginToken);
                respStr = PostDataAndGetResult(
                    ApiPath + "?action=login&format=xml", 
                    postData, 
                    true, 
                    false);
                if (!respStr.Contains("result=\"Success\""))
                    throw new WikiBotException("\n\n" + Bot.Msg("Login failed. Check your username and password.") + "\n");
                Console.WriteLine(Bot.Msg("Logged in as {0}."), UserName);
            }
        }
    }
}
