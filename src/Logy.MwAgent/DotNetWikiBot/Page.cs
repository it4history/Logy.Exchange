using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using Logy.MwAgent.DotNetWikiBot.Exceptions;
using Logy.MwAgent.DotNetWikiBot.Wikidata;
using Newtonsoft.Json.Linq;

namespace Logy.MwAgent.DotNetWikiBot
{
    /// <summary>Class defines wiki page object.</summary>
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [Serializable]
    public class Page : ImportBlock
    {
        private const string ErrorOccurredWhenSavingPageBotOperationIsNotAllowedForThisAccountAtSite
            = "Error occurred when saving page \"{0}\": " + "Bot operation is not allowed for this account at \"{1}\" site.";

        /// <summary>This constructor creates Page object with specified title and specified
        /// Site object. This is preferable constructor. Basic title normalization occurs during
        /// construction.
        /// When constructed, new Page object doesn't contain text, use <see cref="Page.Load()"/>
        /// method to get text and metadata from live wiki.</summary>
        /// <param name="site">Site object, it must be constructed beforehand.</param>
        /// <param name="title">Page title as string.</param>
        /// <returns>Returns Page object.</returns>
        public Page(Site site, string title)
        {
            Categories = new List<string>();
            if (string.IsNullOrEmpty(title))
                throw new ArgumentNullException("title");
            if (title[0] == ':')
                title = title.TrimStart(':');
            if (title.Contains('_'))
                title = title.Replace('_', ' ');

            Site = site;
            Title = title;

            /* // RESERVED, may interfere user intentions
            int ns = GetNamespace();
            RemoveNsPrefix();
            if (site.capitalization == "first-letter")
                title = Bot.Capitalize(title);
            title = site.namespaces[ns] + title;
            */
        }

        /// <summary>This constructor creates empty Page object with specified Site object,
        /// but without title. Avoid using this constructor needlessly.</summary>
        /// <param name="site">Site object, it must be constructed beforehand.</param>
        /// <returns>Returns Page object.</returns>
        public Page(Site site)
        {
            Site = site;
        }

        /// <summary>This constructor creates Page object with specified title using most recently
        /// created Site object.</summary>
        /// <param name="title">Page title as string.</param>
        /// <returns>Returns Page object.</returns>
        public Page(string title)
            : this(Bot.GetMostRecentSiteObject(), title)
        {
        }

        /// <summary>This constructor creates Page object with specified page's numeric revision ID
        /// (also called "oldid"). Page title is retrieved automatically
        /// in this constructor.</summary>
        /// <param name="site">Site object, it must be constructed beforehand.</param>
        /// <param name="revisionId">Page's numeric revision ID (also called "oldid").</param>
        /// <returns>Returns Page object.</returns>
        public Page(Site site, long revisionId)
        {
            if (revisionId <= 0)
                throw new ArgumentOutOfRangeException("revisionID", Bot.Msg("Revision ID must be positive."));
            Site = site;
            Revision = revisionId.ToString();
            GetTitle();
        }

        /// <summary>This constructor creates Page object with specified page's numeric revision ID
        /// (also called "oldid") using most recently created Site object.</summary>
        /// <param name="revisionId">Page's numeric revision ID (also called "oldid").</param>
        /// <returns>Returns Page object.</returns>
        public Page(long revisionId)
        {
            if (revisionId <= 0)
                throw new ArgumentOutOfRangeException("revisionID", Bot.Msg("Revision ID must be positive."));
            Site = Bot.GetMostRecentSiteObject();
            Revision = revisionId.ToString();
            GetTitle();
        }

        /// <summary>This constructor creates Page object without title using most recently
        /// created Site object.</summary>
        /// <returns>Returns Page object.</returns>
        public Page()
        {
            Site = Bot.GetMostRecentSiteObject();
        }

        /// <summary>Page's text.</summary>
        public string Text { get; set; }

        /// <summary>Site, on which this page is located.</summary>
        public Site Site { get; set; }

        /// <summary>Page's ID in MediaWiki database.</summary>
        public string PageId { get; set; }

        /// <summary>Username or IP-address of last page contributor.</summary>
        public string LastUser { get; set; }

        /// <summary>Last contributor's ID in MediaWiki database.</summary>
        public string LastUserId { get; set; }

        /// <summary>Page revision ID in the MediaWiki database.</summary>
        public string Revision { get; set; }

        /// <summary>True, if last edit was minor edit.</summary>
        public bool LastMinorEdit { get; set; }

        /// <summary>Number of bytes modified during last edit.</summary>
        public int LastBytesModified { get; set; }

        /// <summary>Last edit comment.</summary>
        public string Comment { get; set; }

        /// <summary>Date and time of last edit expressed in UTC (Coordinated Universal Time).
        /// Call "timestamp.ToLocalTime()" to convert to local time if it is necessary.</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>Time of last page load (UTC). Used to detect edit conflicts.</summary>
        public DateTime LastLoadTime { get; set; }

        /// <summary>True, if this page is in bot account's watchlist.</summary>
        public bool Watched { get; set; }

        /// <summary>Formats a template with the specified title and parameters. Default formatting
        /// options are used.</summary>
        /// <param name="templateTitle">Template's title.</param>
        /// <param name="templateParams">Template's parameters in Dictionary &lt;string, string&gt;
        /// object, where keys are parameters titles and values are parameters values.</param>
        /// <returns>Returns the complete template in double braces.</returns>
        public static string FormatTemplate(string templateTitle, Dictionary<string, string> templateParams)
        {
            return FormatTemplate(templateTitle, templateParams, false, false, 0);
        }

        /// <summary>Formats a template with the specified title and parameters. Formatting
        /// options are got from provided reference template. That function is usually used to
        /// format modified template as it was in it's initial state, though absolute format
        /// consistency can not be guaranteed.</summary>
        /// <param name="templateTitle">Template's title.</param>
        /// <param name="templateParams">Template's parameters in Dictionary &lt;string, string&gt;
        /// object, where keys are parameters titles and values are parameters values.</param>
        /// <param name="referenceTemplate">Full template body to detect formatting options in.
        /// With or without double braces.</param>
        /// <returns>Returns the complete template in double braces.</returns>
        public static string FormatTemplate(string templateTitle, Dictionary<string, string> templateParams, string referenceTemplate)
        {
            if (string.IsNullOrEmpty(referenceTemplate))
                throw new ArgumentNullException("referenceTemplate");

            bool inline = false;
            bool withoutSpaces = false;
            int padding = 0;

            if (!referenceTemplate.Contains("\n|") && !referenceTemplate.Contains("\n |"))
                inline = true;
            if (!referenceTemplate.Contains("| ") && !referenceTemplate.Contains("= "))
                withoutSpaces = true;
            if (!inline && referenceTemplate.Contains("  ="))
                padding = -1;

            return FormatTemplate(templateTitle, templateParams, inline, withoutSpaces, padding);
        }

        /// <summary>Formats a template with the specified title and parameters, allows extended
        /// format options to be specified.</summary>
        /// <param name="templateTitle">Template's title.</param>
        /// <param name="templateParams">Template's parameters in Dictionary &lt;string, string&gt;
        /// object, where keys are parameters titles and values are parameters values.</param>
        /// <param name="inline">When set to true, template is formatted in one line, without any
        /// line breaks. Default value is false.</param>
        /// <param name="withoutSpaces">When set to true, template is formatted without spaces.
        /// Default value is false.</param>
        /// <param name="padding">When set to positive value, template parameters titles are padded
        /// on the right with specified number of spaces, so "=" characters could form a nice
        /// straight column. When set to -1, the number of spaces is calculated automatically.
        /// Default value is 0 (no padding). The padding will occur only when "inline" option
        /// is set to false and "withoutSpaces" option is also set to false.</param>
        /// <returns>Returns the complete template in double braces.</returns>
        public static string FormatTemplate(
            string templateTitle,
            Dictionary<string, string> templateParams,
            bool inline,
            bool withoutSpaces,
            int padding)
        {
            if (string.IsNullOrEmpty(templateTitle))
                throw new ArgumentNullException("templateTitle");
            if (templateParams == null || templateParams.Count == 0)
                throw new ArgumentNullException("templateParams");

            if (inline || withoutSpaces)
                padding = 0;
            if (padding == -1)
                foreach (KeyValuePair<string, string> kvp in templateParams)
                    if (kvp.Key.Length > padding)
                        padding = kvp.Key.Length;

            string paramBreak = "|";
            string equalsSign = "=";
            if (!inline)
                paramBreak = "\n|";
            if (!withoutSpaces)
            {
                equalsSign = " = ";
                paramBreak += " ";
            }

            int i = 1;
            string template = "{{" + templateTitle;
            foreach (KeyValuePair<string, string> kvp in templateParams)
            {
                template += paramBreak;
                if (padding <= 0)
                {
                    if (kvp.Key == i.ToString())
                        template += kvp.Value;
                    else
                        template += kvp.Key + equalsSign + kvp.Value;
                }
                else
                {
                    if (kvp.Key == i.ToString())
                        template += kvp.Value.PadRight(padding + 3);
                    else
                        template += kvp.Key.PadRight(padding) + equalsSign + kvp.Value;
                }

                i++;
            }

            if (!inline)
                template += "\n";
            template += "}}";

            return template;
        }

        /// <summary>Parses the provided template body and returns the key/value pairs of it's
        /// parameters titles and values. Everything inside the double braces must be passed to
        /// this function, so first goes the template's title, then '|' character, and then go the
        /// parameters. Please, see the usage example.</summary>
        /// <param name="template">Complete template's body including it's title, but not
        /// including double braces.</param>
        /// <returns>Returns the Dictionary &lt;string, string&gt; object, where keys are parameters
        /// titles and values are parameters values. If parameter is untitled, it's number is
        /// returned as the (string) dictionary key. If parameter value is set several times in the
        /// template (normally that shouldn't occur), only the last value is returned. Template's
        /// title is not returned as a parameter.</returns>
        /// <example><code>
        /// Dictionary &lt;string, string&gt; parameters1 =
        ///     site.ParseTemplate("TemplateTitle|param1=val1|param2=val2");
        /// string[] templates = page.GetTemplates(true, false);
        /// Dictionary &lt;string, string&gt; parameters2 = site.ParseTemplate(templates[0]);
        /// parameters1["param2"] = "newValue";
        /// </code></example>
        public static Dictionary<string, string> ParseTemplate(string template)
        {
            if (string.IsNullOrEmpty(template))
                throw new ArgumentNullException("template");
            if (template.StartsWith("{{"))
                template = template.Substring(2, template.Length - 4);

            int startPos, endPos, len;
            string str = template;

            while ((startPos = str.LastIndexOf("{{")) != -1)
            {
                endPos = str.IndexOf("}}", startPos);
                len = (endPos != -1) ? endPos - startPos + 2 : 2;
                str = str.Remove(startPos, len);
                str = str.Insert(startPos, new string('_', len));
            }

            while ((startPos = str.LastIndexOf("[[")) != -1)
            {
                endPos = str.IndexOf("]]", startPos);
                len = (endPos != -1) ? endPos - startPos + 2 : 2;
                str = str.Remove(startPos, len);
                str = str.Insert(startPos, new string('_', len));
            }

            List<int> separators = Bot.GetMatchesPositions(str, "|", false);
            if (separators == null || separators.Count == 0)
                return new Dictionary<string, string>();
            var parameters = new List<string>();
            endPos = template.Length;
            for (int i = separators.Count - 1; i >= 0; i--)
            {
                parameters.Add(template.Substring(separators[i] + 1, endPos - separators[i] - 1));
                endPos = separators[i];
            }

            parameters.Reverse();

            var templateParams = new Dictionary<string, string>();
            for (int i = 0; i < parameters.Count; i++)
            {
                int pos = parameters[i].IndexOf('=');
                if (pos == -1)
                    templateParams[i.ToString()] = parameters[i].Trim();
                else
                    templateParams[parameters[i].Substring(0, pos).Trim()] =
                        parameters[i].Substring(pos + 1).Trim();
            }

            return templateParams;
        }

        /// <summary>Loads page text from live wiki site via raw web interface.
        /// If Page.revision is specified, the function gets that specified
        /// revision. If the page doesn't exist it's text will be empty (""), no exception
        /// is thrown. This function is very fast, but it should be used only when
        /// metadata is not needed and no page modification is required.
        /// In other cases <see cref="Page.Load()"/> function should be used.</summary>
        public void LoadTextOnly()
        {
            if (string.IsNullOrEmpty(Title) && string.IsNullOrEmpty(Revision))
                throw new WikiBotException(Bot.Msg("No title is specified for page to load."));

            string res = Site.IndexPath + "?title=" + Bot.UrlEncode(Title) +
                         (string.IsNullOrEmpty(Revision) ? string.Empty : "&oldid=" + Revision) +
                         "&redirect=no&action=raw&ctype=text/plain&dontcountme=s";
            try
            {
                Text = Site.GetWebPage(res);
            }
            catch (WebException e)
            {
                string message = e.Message;
                if (message.Contains(": (404) "))
                {
                    // Not Found
                    Console.Error.WriteLine(Bot.Msg("Page \"{0}\" doesn't exist."), Title);
                    Text = string.Empty;
                    return;
                }

                throw;
            }

            LastLoadTime = DateTime.UtcNow;
            Console.WriteLine(Bot.Msg("Page \"{0}\" loaded successfully."), Title);
        }

        /// <summary>Loads page text and metadata (last revision's ID, timestamp, comment, author,
        /// minor edit mark) from wiki site. If the page doesn't exist
        /// it's text will be empty (""), no exception is thrown.</summary>
        public void Load()
        {
            LoadWithMetadata();
        }

        /// <summary>Loads page text and metadata (last revision's ID, timestamp, comment, author,
        /// minor edit mark). Now this function is synonym for <see cref="Page.Load()"/>.
        /// If the page doesn't exist it's text will be empty (""), no exception is thrown.
        /// </summary>
        /// <exclude/>
        public void LoadWithMetadata()
        {
            if (Site.UseApi) {
                if (string.IsNullOrEmpty(Title) && string.IsNullOrEmpty(Revision))
                    throw new WikiBotException(Bot.Msg("No title is specified for page to load."));

                string res = Site.ApiPath + "?action=query&prop=revisions&format=xml" +
                             "&rvprop=content|user|userid|comment|ids|flags|timestamp";
                if (!string.IsNullOrEmpty(Revision))
                    res += "&revids=" + Revision;
                else if (!string.IsNullOrEmpty(Title))
                    res += "&titles=" + Bot.UrlEncode(Title);
                string src = Site.GetWebPage(res);
                LastLoadTime = DateTime.UtcNow;

                XElement pageXml = XElement.Parse(src).Element("query").Element("pages").Element("page");
                if (pageXml.Attribute("missing") != null) {
                    Console.Error.WriteLine(Bot.Msg("Page \"{0}\" doesn't exist."), Title);
                    Text = string.Empty;
                    return;
                }

                PageId = pageXml.Attribute("pageid").Value;
                Title = pageXml.Attribute("title").Value;

                XElement revXml = pageXml.Element("revisions").Element("rev");
                Revision = revXml.Attribute("revid").Value;
                Timestamp = DateTime.Parse(revXml.Attribute("timestamp").Value);
                Timestamp = Timestamp.ToUniversalTime();
                LastUser = revXml.Attribute("user").Value;
                LastUserId = revXml.Attribute("userid").Value;
                LastMinorEdit = revXml.Attribute("minor") != null;
                Comment = revXml.Attribute("comment").Value;
                Text = revXml.Value;
            }
            else {
                if (string.IsNullOrEmpty(Title))
                    throw new WikiBotException(Bot.Msg("No title is specified for page to load."));
                string res = Site.IndexPath + "?title=Special:Export/" +
                             Bot.UrlEncode(Title) + "&action=submit";
                string src = Site.GetWebPage(res);
                LastLoadTime = DateTime.UtcNow;
                ParsePageXml(src);
            }

            Console.WriteLine(Bot.Msg("Page \"{0}\" loaded successfully."), Title);
        }

        /// <summary>This internal function parses MediaWiki XML export data using XmlDocument
        /// to get page text and metadata.</summary>
        /// <param name="xmlSrc">XML export source code.</param>
        /// <exclude/>
        public void ParsePageXml(string xmlSrc)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlSrc);
            if (doc.GetElementsByTagName("page").Count == 0) {
                Console.Error.WriteLine(Bot.Msg("Page \"{0}\" doesn't exist."), Title);
                Text = string.Empty;
                return;
            }

            Text = doc.GetElementsByTagName("text")[0].InnerText;
            PageId = doc.GetElementsByTagName("id")[0].InnerText;
            if (doc.GetElementsByTagName("username").Count != 0) {
                LastUser = doc.GetElementsByTagName("username")[0].InnerText;
                LastUserId = doc.GetElementsByTagName("id")[2].InnerText;
            }
            else if (doc.GetElementsByTagName("ip").Count != 0)
                LastUser = doc.GetElementsByTagName("ip")[0].InnerText;
            else
                LastUser = "(n/a)";
            Revision = doc.GetElementsByTagName("id")[1].InnerText;
            if (doc.GetElementsByTagName("comment").Count != 0)
                Comment = doc.GetElementsByTagName("comment")[0].InnerText;
            Timestamp = DateTime.Parse(doc.GetElementsByTagName("timestamp")[0].InnerText);
            Timestamp = Timestamp.ToUniversalTime();
            LastMinorEdit = (doc.GetElementsByTagName("minor").Count != 0) ? true : false;
            if (string.IsNullOrEmpty(Title))
                Title = doc.GetElementsByTagName("title")[0].InnerText;
            else
                Console.WriteLine(Bot.Msg("Page \"{0}\" loaded successfully."), Title);
        }

        /// <summary>Loads page text from the specified UTF8-encoded file.</summary>
        /// <param name="filePathName">Path and name of the file.</param>
        public void LoadFromFile(string filePathName)
        {
            StreamReader strmReader = new StreamReader(filePathName);
            Text = strmReader.ReadToEnd();
            strmReader.Close();
            Console.WriteLine(
                Bot.Msg("Text for page \"{0}\" has been loaded from \"{1}\" file."),
                Title, 
                filePathName);
        }

        /// <summary>Retrieves the title for this Page object using page's numeric revision ID
        /// (also called "oldid"), stored in "revision" object's property. Make sure that
        /// "revision" property is set before calling this function. Use this function
        /// when working with old revisions to detect if the page was renamed at some
        /// point.</summary>
        public void GetTitle()
        {
            if (string.IsNullOrEmpty(Revision))
                throw new WikiBotException(
                    Bot.Msg("No revision ID is specified for page to get title for."));
            if (Site.UseApi) {
                string src = Site.GetWebPage(Site.ApiPath +
                                             "?action=query&prop=info&format=xml&revids=" + Revision);
                var infoXml = XElement.Parse(src).Element("query");
                if (infoXml.Element("badrevids") != null)
                    throw new WikiBotException(string.Format(
                        "No page revision with ID \"{0}\" was found.", Revision));
                Title = infoXml.Descendants("page").FirstOrDefault().Attribute("title").Value;
            }
            else {
                string src = Site.GetWebPage(Site.IndexPath + "?oldid=" + Revision);
                Match m = Regex.Match(src, "<h1 [^>]*=\"firstHeading\"[^>]*>" + "(<span[^>]*>)?(?<title>.+?)(</span>)?</h1>");
                if (string.IsNullOrEmpty(m.Groups["title"].Value))
                    throw new WikiBotException(string.Format("No page revision with ID \"{0}\" was found.", Revision));
                Title = m.Groups["title"].Value;
            }
        }

        /// <summary>Gets security tokens which are required by MediaWiki to perform page
        /// modifications.</summary>
        /// <param name="action">Type of action, that security token is required for.</param>
        /// <returns>Returns Dictionary object.</returns>
        public Dictionary<string, string> GetSecurityTokens(string action)
        {
            if (string.IsNullOrEmpty(action))
                throw new ArgumentNullException("action");
            if (string.IsNullOrEmpty(Title))
                throw new ArgumentNullException("title");

            var url = Site.ApiPath + "?action=query&prop=info&intoken=" +
                      action + "&inprop=protection|watched|watchers|notificationtimestamp|readable" +
                      "&format=xml&titles=" + Bot.UrlEncode(Title);
            string src = Site.GetWebPage(url);
            var tokensXml = XElement.Parse(src).Element("query").Element("pages");
            var tokens = (from attr in tokensXml.Element("page").Attributes()
                          select new
                                     {
                                         attrName = attr.Name.ToString(),
                                         attrValue = attr.Value
                                     })
                .ToDictionary(s => s.attrName, s => s.attrValue);
            return tokens;
        }

        /// <summary>Saves contents of <see cref="Text"/> to live wiki site. Uses
        /// <see cref="DotNetWikiBot.Site.DefaultEditComment"/> and <see cref="DotNetWikiBot.Site.MinorEditByDefault"/>
        /// (true by default).</summary>
        /// <exception cref="InsufficientRightsException">Insufficient rights
        /// to edit this page.</exception>
        /// <exception cref="BotDisallowedException">Bot operation on this page
        /// is disallowed.</exception>
        /// <exception cref="EditConflictException">Edit conflict
        /// detected.</exception>
        /// <exception cref="WikiBotException">Any wiki-related error.</exception>
        public void Save()
        {
            Save(Text, Site.DefaultEditComment, Site.MinorEditByDefault, false);
        }

        /// <summary>Saves specified text in page on live wiki. Uses
        /// <see cref="DotNetWikiBot.Site.DefaultEditComment"/> and <see cref="DotNetWikiBot.Site.MinorEditByDefault"/>
        /// (true by default).</summary>
        /// <param name="newText">New text for this page.</param>
        /// <exception cref="InsufficientRightsException">Insufficient rights to edit
        /// this page.</exception>
        /// <exception cref="BotDisallowedException">Bot operation on this page
        /// is disallowed.</exception>
        /// <exception cref="EditConflictException">Edit conflict was detected.</exception>
        /// <exception cref="WikiBotException">Wiki-related error.</exception>
        public void Save(string newText)
        {
            Save(newText, Site.DefaultEditComment, Site.MinorEditByDefault, false);
        }

        /// <summary>Saves <see cref="Text"/> contents to live wiki site.</summary>
        /// <param name="comment">Your edit comment.</param>
        /// <param name="isMinorEdit">Minor edit mark (true = minor edit).</param>
        /// <exception cref="InsufficientRightsException">Insufficient rights to edit
        /// this page.</exception>
        /// <exception cref="BotDisallowedException">Bot operation on this page
        /// is disallowed.</exception>
        /// <exception cref="EditConflictException">Edit conflict was detected.</exception>
        /// <exception cref="WikiBotException">Wiki-related error.</exception>
        public void Save(string comment, bool isMinorEdit)
        {
            Save(Text, comment, isMinorEdit, false);
        }

        /// <summary>Saves specified text on page on live wiki.</summary>
        /// <param name="newText">New text for this page.</param>
        /// <param name="comment">Your edit comment.</param>
        /// <param name="isMinorEdit">Minor edit mark (true = minor edit).</param>
        /// <exception cref="InsufficientRightsException">Insufficient rights to edit
        /// this page.</exception>
        /// <exception cref="BotDisallowedException">Bot operation on this page
        /// is disallowed.</exception>
        /// <exception cref="EditConflictException">Edit conflict was detected.</exception>
        /// <exception cref="WikiBotException">Wiki-related error.</exception>
        public void Save(string newText, string comment, bool isMinorEdit)
        {
            Save(newText, comment, isMinorEdit, false);
        }

        /// <summary>Saves specified text on page on live wiki.</summary>
        /// <param name="newText">New text for this page.</param>
        /// <param name="comment">Your edit comment.</param>
        /// <param name="isMinorEdit">Minor edit mark (true = minor edit).</param>
        /// <param name="reviewVersion">If true, the page revision after saving is marked
        /// reviewed. Bot's account must have sufficient rights to mark revision reviewed.
        /// Not every wiki site does have this option.</param>
        /// <exception cref="InsufficientRightsException">Insufficient rights to edit
        /// this page.</exception>
        /// <exception cref="BotDisallowedException">Bot operation on this page
        /// is disallowed.</exception>
        /// <exception cref="EditConflictException">Edit conflict was detected.</exception>
        /// <exception cref="WikiBotException">Wiki-related error.</exception>
        public void Save(string newText, string comment, bool isMinorEdit, bool reviewVersion)
        {
            if (string.IsNullOrEmpty(newText) && string.IsNullOrEmpty(Text))
                throw new ArgumentNullException("newText", Bot.Msg("No text is specified for page to save."));
            if (string.IsNullOrEmpty(Title))
                throw new WikiBotException(Bot.Msg(
                    "No title is specified for page to save text to."));
            if (Text != null && Regex.IsMatch(
                Text,
@"(?is)\{\{(nobots|bots\|(allow=none|" + @"deny=(?!none)[^\}]*(" + Site.UserName + @"|all)|optout=all))\}\}"))
                throw new BotDisallowedException(string.Format(
                    Bot.Msg("Bot action on \"{0}\" page is prohibited " + "by \"nobots\" or \"bots|allow=none\" template."),
                    Title));
            
            if (Site.ForceSaveDelay > 0) {
                int secondsPassed = (int)(DateTime.Now - Site.LastWriteTime).TotalSeconds;
                if (Site.ForceSaveDelay > secondsPassed)
                    Bot.Wait(Site.ForceSaveDelay - secondsPassed);
            }

            if (Site.UseApi && !reviewVersion)
            {
                // Get security token for editing
                string editToken = string.Empty;
                if (Site.Tokens != null && Site.Tokens.ContainsKey("csrftoken"))
                    editToken = Site.Tokens["csrftoken"];
                else
                {
                    var tokens = GetSecurityTokens("edit");
                    if (!tokens.ContainsKey("edittoken") || tokens["edittoken"] == string.Empty)
                        throw new InsufficientRightsException(string.Format(
                            Bot.Msg("Insufficient rights to edit page \"{0}\"."),
                            Title));
                    editToken = tokens["edittoken"];
                }

                var timestamp = Timestamp != DateTime.MinValue ? "&basetimestamp=" + Timestamp.ToString("s") + "Z" : string.Empty;
                var lastLoadTime = LastLoadTime != DateTime.MinValue
                    ? "&starttimestamp=" + LastLoadTime.AddSeconds(Site.TimeOffsetSeconds).ToString("s") + "Z"
                    : string.Empty;
                var postData = string.Format(
                    "action=edit&title={0}&summary={1}&text={2}" + "&watchlist={3}{4}{5}{6}&bot=1&format=xml&token={7}",
                    Bot.UrlEncode(Title),
                    Bot.UrlEncode(comment),
                    Bot.UrlEncode(newText),
                    "nochange",
                    isMinorEdit ? "&minor=1" : "&notminor=1",
                    timestamp,
                    lastLoadTime,
                    Bot.UrlEncode(editToken));
                if (Bot.AskConfirm)
                {
                    Console.Write("\n\n" + Bot.Msg("The following text is going to be saved on page \"{0}\":"), Title);
                    Console.Write("\n\n" + Text + "\n\n");
                    if (!Bot.UserConfirms())
                        return;
                }

                SavePostApi(postData);
            }
            else
            {
                string editToken, lastEditTime, lastEditRevId, editStartTime;
                string editPageSrc = Site.GetWebPage(Site.IndexPath + "?title=" +
                                                     Bot.UrlEncode(Title) + "&action=edit");
                Match m;
                m = Site.Regexes["editToken"].Match(editPageSrc);
                editToken = m.Success ? m.Groups[1].Value : string.Empty;
                m = Site.Regexes["editTime"].Match(editPageSrc);
                lastEditTime = m.Success ? m.Groups[1].Value : string.Empty;
                m = Site.Regexes["startTime"].Match(editPageSrc);
                if (LastLoadTime == DateTime.MinValue && m.Success)
                    editStartTime = m.Groups[1].Value;
                else
                    editStartTime = LastLoadTime.AddSeconds(Site.TimeOffsetSeconds)
                        .ToString("yyyyMMddHHmmss");
                m = Site.Regexes["baseRevId"].Match(editPageSrc);
                if (string.IsNullOrEmpty(Revision) && m.Success)
                    lastEditRevId = m.Groups[1].Value;
                else
                    lastEditRevId = Revision;

                // See if page is watched or not
                if (Site.WatchList == null)
                {
                    Site.WatchList = new PageList(Site);
                    Site.WatchList.FillFromWatchList();
                }

                Watched = Site.WatchList.Contains(this);

                string postData = string.Format(
                    "wpSection=&wpStarttime={0}&wpEdittime={1}{2}&wpScrolltop=&wpTextbox1={3}" +
                    "&wpSummary={4}&wpSave=Save%20Page&wpEditToken={5}{6}{7}{8}",
                    editStartTime,
                    lastEditTime,
                    string.IsNullOrEmpty(lastEditRevId) ? "&baseRevId=" + lastEditRevId : string.Empty,
                    Bot.UrlEncode(newText),
                    Bot.UrlEncode(comment),
                    Bot.UrlEncode(editToken),
                    Watched ? "&wpWatchthis=1" : string.Empty,
                    isMinorEdit ? "&wpMinoredit=1" : string.Empty,
                    reviewVersion ? "&wpReviewEdit=1" : string.Empty);
                if (Bot.AskConfirm)
                {
                    Console.Write("\n\n" + string.Format(Bot.Msg("The following text is going to be saved on page \"{0}\":"), Title));
                    Console.Write("\n\n" + Text + "\n\n");
                    if (!Bot.UserConfirms())
                        return;
                }

                SavePost(postData);
            }

            Console.WriteLine(Bot.Msg("Page \"{0}\" saved successfully."), Title);
            Site.LastWriteTime = DateTime.UtcNow;
            LastLoadTime = DateTime.UtcNow;
            Timestamp = DateTime.MinValue;
            Text = newText;
        }

        /// <summary>Undoes the last edit, so page text reverts to previous contents.
        /// The function doesn't affect other actions like renaming.</summary>
        /// <param name="comment">Revert comment.</param>
        /// <param name="isMinorEdit">Minor edit mark (pass true for minor edit).</param>
        public void Revert(string comment, bool isMinorEdit)
        {
            if (string.IsNullOrEmpty(Title))
                throw new WikiBotException(Bot.Msg("No title is specified for page to revert."));
            PageList pl = new PageList(Site);
            pl.FillFromPageHistory(Title, 2);
            if (pl.Count() != 2) {
                Console.Error.WriteLine(Bot.Msg("Can't revert page \"{0}\"."), Title);
                return;
            }

            pl[1].Load();
            Save(pl[1].Text, comment, isMinorEdit);
            Console.WriteLine(Bot.Msg("Page \"{0}\" has been reverted."), Title);
        }

        /// <summary>Undoes all last edits made by last contributor.
        /// The function doesn't affect other operations
        /// like renaming or protecting.</summary>
        /// <param name="comment">Comment.</param>
        /// <param name="isMinorEdit">Minor edit mark (pass true for minor edit).</param>
        /// <returns>Returns true if last edits were undone.</returns>
        public bool UndoLastEdits(string comment, bool isMinorEdit)
        {
            if (string.IsNullOrEmpty(Title))
                throw new WikiBotException(Bot.Msg("No title is specified for page to revert."));
            PageList pl = new PageList(Site);
            string lastEditor = string.Empty;
            for (int i = 50; i <= 5000; i *= 10) {
                pl.FillFromPageHistory(Title, i);
                lastEditor = pl[0].LastUser;
                foreach (Page p in pl)
                    if (p.LastUser != lastEditor) {
                        p.Load();
                        Save(p.Text, comment, isMinorEdit);
                        Console.WriteLine(
                            Bot.Msg("Last edits of page \"{0}\" by user {1} have been undone."),
                            Title, 
                            lastEditor);
                        return true;
                    }

                if (pl.Pages.Count < i)
                    break;
                pl.Clear();
            }

            Console.Error.WriteLine(
                Bot.Msg("Can't undo last edits of page \"{0}\" by user {1}."),
                Title,
                lastEditor);
            return false;
        }

        /// <summary>Protects or unprotects the page, so only authorized group of users can edit or
        /// rename it. Changing page protection mode requires administrator (sysop)
        /// rights.</summary>
        /// <param name="editMode">Protection mode for editing this page (0 = everyone allowed
        /// to edit, 1 = only registered users are allowed, 2 = only administrators are allowed
        /// to edit).</param>
        /// <param name="renameMode">Protection mode for renaming this page (0 = everyone allowed to
        /// rename, 1 = only registered users are allowed, 2 = only administrators
        /// are allowed).</param>
        /// <param name="cascadeMode">In cascading mode all the pages, included into this page
        /// (e.g., templates or images) are also automatically protected.</param>
        /// <param name="expiryDate">Date and time, expressed in UTC, when protection expires
        /// and page becomes unprotected. Use DateTime.ToUniversalTime() method to convert local
        /// time to UTC, if necessary. Pass DateTime.MinValue to make protection indefinite.</param>
        /// <param name="reason">Reason for protecting this page.</param>
        /// <example><code>
        /// page.Protect(2, 2, false, DateTime.Now.AddDays(20), "persistent vandalism");
        /// </code></example>
        public void Protect(int editMode, int renameMode, bool cascadeMode, DateTime expiryDate, string reason)
        {
            if (string.IsNullOrEmpty(Title))
                throw new WikiBotException(Bot.Msg("No title is specified for page to protect."));
            string errorMsg =
                Bot.Msg("Only values 0, 1 and 2 are accepted. Please consult documentation.");
            if (editMode > 2 || editMode < 0)
                throw new ArgumentOutOfRangeException("editMode", errorMsg);
            if (renameMode > 2 || renameMode < 0)
                throw new ArgumentOutOfRangeException("renameMode", errorMsg);
            if (expiryDate != DateTime.MinValue    && expiryDate < DateTime.Now)
                throw new ArgumentOutOfRangeException("expiryDate", Bot.Msg("Protection expiry date must be later than now."));

            if (Site.UseApi) {
                string token = string.Empty;
                if (Site.Tokens != null && Site.Tokens.ContainsKey("csrftoken"))
                    token = Site.Tokens["csrftoken"];
                else {
                    var tokens = GetSecurityTokens("protect");
                    if (tokens.ContainsKey("missing"))
                        throw new WikiBotException(
                            string.Format(Bot.Msg("Page \"{0}\" doesn't exist."), Title));
                    if (!tokens.ContainsKey("protecttoken") || tokens["protecttoken"] == string.Empty) {
                        Console.Error.WriteLine(
                            Bot.Msg("Unable to change protection mode for page \"{0}\"."), Title);
                        return;
                    }

                    token = tokens["protecttoken"];
                }

                string date = Regex.Replace(expiryDate.ToString("u"), "\\D", string.Empty);
                string postData = string.Format(
                    "token={0}&protections=edit={1}|move={2}" +
                    "&cascade={3}&expiry={4}|{5}&reason={6}&watchlist=nochange",
                    Bot.UrlEncode(token),
                    editMode == 2 ? "sysop" : editMode == 1 ? "autoconfirmed" : string.Empty,
                    renameMode == 2 ? "sysop" : renameMode == 1 ? "autoconfirmed" : string.Empty,
                    cascadeMode == true ? "1" : string.Empty,
                    expiryDate == DateTime.MinValue ? string.Empty : date,
                    expiryDate == DateTime.MinValue ? string.Empty : date,
                    Bot.UrlEncode(reason));

                string respStr = Site.PostDataAndGetResult(
                    Site.ApiPath + "?action=protect" + "&title=" + Bot.UrlEncode(Title) + "&format=xml", 
                    postData);
                if (respStr.Contains("<error"))
                    throw new WikiBotException(
                        string.Format(Bot.Msg("Failed to delete page \"{0}\"."), Title));
            }
            else {
                string respStr = Site.GetWebPage(Site.IndexPath + "?title=" +
                                                 Bot.UrlEncode(Title) + "&action=protect");
                Match m = Site.Regexes["editToken"].Match(respStr);
                string securityToken = string.IsNullOrEmpty(m.Groups[1].Value)
                                           ? m.Groups[2].Value : m.Groups[1].Value;
                if (string.IsNullOrEmpty(securityToken)) {
                    Console.Error.WriteLine(
                        Bot.Msg("Unable to change protection mode for page \"{0}\"."), Title);
                    return;
                }

                if (Site.WatchList == null) {
                    Site.WatchList = new PageList(Site);
                    Site.WatchList.FillFromWatchList();
                }

                Watched = Site.WatchList.Contains(this);

                // ToString("u") is like "2010-06-15 20:45:30Z"
                var postData = string.Format(
                  "mwProtect-level-edit={0}&mwProtect-level-move={1}" + "&mwProtect-reason={2}&wpEditToken={3}&mwProtect-expiry={4}{5}{6}",
                  editMode == 2 ? "sysop" : editMode == 1 ? "autoconfirmed" : string.Empty,
                  renameMode == 2 ? "sysop" : renameMode == 1 ? "autoconfirmed" : string.Empty,
                  Bot.UrlEncode(reason),
                  Bot.UrlEncode(securityToken),
                  expiryDate == DateTime.MinValue ? string.Empty : expiryDate.ToString("u"), 
                  cascadeMode ? "&mwProtect-cascade=1" : string.Empty,
                  Watched ? "&mwProtectWatch=1" : string.Empty);
                respStr = Site.PostDataAndGetResult(
                    Site.IndexPath + "?title=" + Bot.UrlEncode(Title) + "&action=protect", 
                    postData);

                Regex successMsg = new Regex(
                    "<h1[^>]*>(<span[^>]*>)?\\s*" + HttpUtility.HtmlEncode(Title) + "\\s*<");
                if (!successMsg.IsMatch(respStr)) {
                    throw new WikiBotException(string.Format(
                        Bot.Msg("Unable to change protection mode for page \"{0}\"."), 
                        Title));
                }
            }

            Console.WriteLine(Bot.Msg("Protection mode for page \"{0}\" has been changed successfully."), Title);
        }

        /// <summary>Adds this page to bot account's watchlist.</summary>
        public void Watch()
        {
            if (string.IsNullOrEmpty(Title))
                throw new WikiBotException(Bot.Msg("No title is specified for page to watch."));

            if (Site.UseApi) {
                string res = Site.ApiPath + "?format=xml&action=query&meta=tokens&type=watch" +
                             "&titles=" + Bot.UrlEncode(Title);
                string respStr = Site.GetWebPage(res);
                string securityToken = string.Empty;
                string titleFallback = string.Empty;
                try {
                    securityToken = XElement.Parse(respStr).Element("query")
                        .Element("tokens").Attribute("watchtoken").Value;
                }
                catch {    // FALLBACK for older version
                    res = Site.ApiPath + "?format=xml&action=query&prop=info&intoken=watch" +
                          "&titles=" + Bot.UrlEncode(Title);
                    respStr = Site.GetWebPage(res);
                    securityToken = XElement.Parse(respStr).Element("query").Element("pages")
                        .Element("page").Attribute("watchtoken").Value;
                    titleFallback = "&title=" + Bot.UrlEncode(Title);
                }

                string postData = string.Format(
                    "titles={0}{1}&action=watch&token={2}&format=xml",
                    Bot.UrlEncode(Title),
                    titleFallback,
                    Bot.UrlEncode(securityToken));
                respStr = Site.PostDataAndGetResult(Site.ApiPath, postData);
            }
            else {
                string res = Site.IndexPath + "?action=watch&title=" +
                             Bot.UrlEncode(Title);
                string respStr = Site.GetWebPage(res);
                string securityToken = string.Empty;
                Match m = Site.Regexes["editToken"].Match(respStr);
                if (m.Success)
                {
                    securityToken = string.IsNullOrEmpty(m.Groups[1].Value)
                                        ? m.Groups[2].Value : m.Groups[1].Value;
                }
                else
                {
                    Console.Error.WriteLine(Bot.Msg("Can't add page \"{0}\" to watchlist."), Title);
                    return;
                }

                string postData = string.Format(
                    "title={0}&action=watch&wpEditToken={1}",
                    Bot.UrlEncode(Title),
                    Bot.UrlEncode(securityToken));
                respStr = Site.PostDataAndGetResult(
                    Site.IndexPath + "?title=" + Bot.UrlEncode(Title), 
                    postData);
            }

            Watched = true;
            if (Site.WatchList == null)
                Site.WatchList.FillFromWatchList();
            if (!Site.WatchList.Contains(this))
                Site.WatchList.Add(this);
            Console.WriteLine(Bot.Msg("Page \"{0}\" added to watchlist."), Title);
        }

        /// <summary>Removes page from bot account's watchlist.</summary>
        public void Unwatch()
        {
            if (string.IsNullOrEmpty(Title))
                throw new WikiBotException(Bot.Msg("No title is specified for page to unwatch."));

            if (Site.UseApi) {
                string res = Site.ApiPath + "?format=xml&action=query&meta=tokens&type=watch" +
                             "&titles=" + Bot.UrlEncode(Title);
                string respStr = Site.GetWebPage(res);
                string securityToken = string.Empty;
                string titleFallback = string.Empty;
                try {
                    securityToken = XElement.Parse(respStr).Element("query")
                        .Element("tokens").Attribute("watchtoken").Value.ToString();
                }
                catch {    // FALLBACK for older version
                    res = Site.ApiPath + "?format=xml&action=query&prop=info&intoken=watch" +
                          "&titles=" + Bot.UrlEncode(Title);
                    respStr = Site.GetWebPage(res);
                    securityToken = XElement.Parse(respStr).Element("query").Element("pages")
                        .Element("page").Attribute("watchtoken").Value;
                    titleFallback = "&title=" + Bot.UrlEncode(Title);
                }

                string postData = string.Format(
                    "titles={0}{1}&token={2}" + "&format=xml&action=watch&unwatch=1",
                    Bot.UrlEncode(Title),
                    titleFallback,
                    Bot.UrlEncode(securityToken));
                respStr = Site.PostDataAndGetResult(Site.ApiPath, postData);
            }
            else {
                string res = Site.IndexPath + "?action=unwatch&title=" + Bot.UrlEncode(Title);
                string respStr = Site.GetWebPage(res);
                string securityToken = string.Empty;
                Match m = Site.Regexes["editToken"].Match(respStr);
                if (m.Success) {
                    securityToken = string.IsNullOrEmpty(m.Groups[1].Value)
                                        ? m.Groups[2].Value : m.Groups[1].Value;
                }
                else {
                    Console.Error.WriteLine(Bot.Msg("Can't remove page \"{0}\" from watchlist."), Title);
                    return;
                }

                string postData = string.Format(
                    "title={0}&action=unwatch&wpEditToken={1}",
                    Bot.UrlEncode(Title),
                    Bot.UrlEncode(securityToken));
                respStr = Site.PostDataAndGetResult(
                    Site.IndexPath + "?title=" + Bot.UrlEncode(Title), 
                    postData);
            }

            Watched = false;
            if (Site.WatchList != null && Site.WatchList.Contains(this))
                Site.WatchList.Remove(Title);
            Console.WriteLine(Bot.Msg("Page \"{0}\" has been removed from watchlist."), Title);
        }

        /// <summary>This function opens page text in Microsoft Word for editing.
        /// Just close Word after editing, and the revised text will appear back in
        /// <see cref="Text"/> variable.</summary>
        /// <remarks>Appropriate PIAs (Primary Interop Assemblies) for available MS Office
        /// version must be installed and referenced in order to use this function. Follow
        /// instructions in "Compile and Run.bat" file to reference PIAs properly in compilation
        /// command, and then recompile the framework. Redistributable PIAs can be downloaded from
        /// <see href="http://www.microsoft.com/en-us/download/search.aspx?q=Office%20PIA">
        /// Microsoft web site</see>.</remarks>
        public void ReviseInMsWord()
        {
#if MS_WORD_INTEROP
            if (string.IsNullOrEmpty(text))
                throw new WikiBotException(Bot.Msg("No text on page to revise in Microsoft Word."));
            Microsoft.Office.Interop.Word.Application app =
                new Microsoft.Office.Interop.Word.Application();
            app.Visible = true;
            object mv = System.Reflection.Missing.Value;
            object template = mv;
            object newTemplate = mv;
            object documentType = Microsoft.Office.Interop.Word.WdDocumentType.wdTypeDocument;
            object visible = true;
            Microsoft.Office.Interop.Word.Document doc =
                app.Documents.Add(ref template, ref newTemplate, ref documentType, ref visible);
            doc.Words.First.InsertBefore(text);
            text = null;
            Microsoft.Office.Interop.Word.DocumentEvents_Event docEvents =
                (Microsoft.Office.Interop.Word.DocumentEvents_Event) doc;
            docEvents.Close +=
                new Microsoft.Office.Interop.Word.DocumentEvents_CloseEventHandler(
                    delegate { text = doc.Range(ref mv, ref mv).Text; doc.Saved = true; } );
            app.Activate();
            while (text == null);
            text = Regex.Replace(text, "\r(?!\n)", "\r\n");
            app = null;
            doc = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Console.WriteLine(
                Bot.Msg("Text of \"{0}\" page has been revised in Microsoft Word."), title);
#else
            throw new WikiBotException(Bot.Msg("Page.ReviseInMSWord() function requires MS " +
                                               "Office PIAs to be installed and referenced. Please see remarks in function's " +
                                               "documentation in \"Documentation.chm\" file for additional instructions.\n"));
#endif
        }

        /// <summary>Uploads local image to wiki site. Function also works with non-image files.
        /// Note: uploaded image title (wiki page title) will be the same as title of this Page
        /// object, not the title of source file.</summary>
        /// <param name="filePathName">Path and name of local file.</param>
        /// <param name="description">File (image) description.</param>
        /// <param name="license">File license type (may be template title). Used only on
        /// some wiki sites. Pass empty string, if the wiki site doesn't require it.</param>
        /// <param name="copyStatus">File (image) copy status. Used only on some wiki sites. Pass
        /// empty string, if the wiki site doesn't require it.</param>
        /// <param name="source">File (image) source. Used only on some wiki sites. Pass
        /// empty string, if the wiki site doesn't require it.</param>
        public void UploadImage(string filePathName, string description, string license, string copyStatus, string source)
        {
            CheckFileName(filePathName);

            Console.WriteLine(Bot.Msg("Uploading image \"{0}\"..."), Title);

            var tokens = GetSecurityTokens("edit");    // there is no more specific token type
            if (!tokens.ContainsKey("edittoken") || tokens["edittoken"] == string.Empty)
                throw new WikiBotException(
                    string.Format(Bot.Msg("Error occurred when uploading image \"{0}\"."), Title));

            string targetName = Site.RemoveNsPrefix(Title, 6);
            targetName = Bot.Capitalize(targetName);

            string res = Site.IndexPath + "?title=" +
                         HttpUtility.HtmlEncode(Site.GetNsPrefix(-1)) + "Upload";

            int retryDelaySeconds = 60;
            WebResponse webResp = null;
            for (int errorCounter = 0;; errorCounter++) {
                var webReq = (HttpWebRequest)WebRequest.Create(res);
                webReq.Proxy.Credentials = CredentialCache.DefaultCredentials;
                webReq.UseDefaultCredentials = true;
                webReq.Method = "POST";
                string boundary = DateTime.Now.Ticks.ToString("x");
                webReq.ContentType = "multipart/form-data; boundary=" + boundary;
                webReq.UserAgent = Bot.BotVer;
                webReq.CookieContainer = Site.Cookies;
                if (Bot.UnsafeHttpHeaderParsingUsed == 0) {
                    webReq.ProtocolVersion = HttpVersion.Version10;
                    webReq.KeepAlive = false;
                }

                webReq.CachePolicy = new System.Net.Cache.HttpRequestCachePolicy(
                    System.Net.Cache.HttpRequestCacheLevel.Refresh);
                StringBuilder sb = new StringBuilder();
                string paramHead = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"";
                sb.Append(paramHead + "maxlag\"\r\n\r\n" + Site.MaxLag + "\r\n");
                sb.Append(paramHead + "wpIgnoreWarning\"\r\n\r\n1\r\n");
                sb.Append(paramHead + "wpDestFile\"\r\n\r\n" + targetName + "\r\n");
                sb.Append(paramHead + "wpUploadAffirm\"\r\n\r\n1\r\n");
                sb.Append(paramHead + "wpWatchthis\"\r\n\r\n0\r\n");
                sb.Append(paramHead + "wpEditToken\"\r\n\r\n" + tokens["edittoken"] + "\r\n");
                sb.Append(paramHead + "wpUploadCopyStatus\"\r\n\r\n" + copyStatus + "\r\n");
                sb.Append(paramHead + "wpUploadSource\"\r\n\r\n" + source + "\r\n");
                sb.Append(paramHead + "wpUpload\"\r\n\r\n" + "upload bestand" + "\r\n");
                sb.Append(paramHead + "wpLicense\"\r\n\r\n" + license + "\r\n");
                sb.Append(paramHead + "wpUploadDescription\"\r\n\r\n" + description + "\r\n");
                sb.Append(paramHead + "wpUploadFile\"; filename=\"" +
                          Bot.UrlEncode(Path.GetFileName(filePathName)) + "\"\r\n" +
                          "Content-Type: application/octet-stream\r\n\r\n");
                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sb.ToString());
                byte[] fileBytes = File.ReadAllBytes(filePathName);
                byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                webReq.ContentLength = postHeaderBytes.Length + fileBytes.Length +
                                       boundaryBytes.Length;
                Stream reqStream = webReq.GetRequestStream();
                reqStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                reqStream.Write(fileBytes, 0, fileBytes.Length);
                reqStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                try {
                    webResp = webReq.GetResponse();
                    break;
                }
                catch (WebException e) {
                    if (webResp == null)
                        throw;

                    if (e.Message.Contains("Section=ResponseStatusLine")) {   // Known Squid problem
                        Bot.SwitchUnsafeHttpHeaderParsing(true);
                        UploadImage(filePathName, description, license, copyStatus, source);
                        return;
                    }

                    if (webResp.Headers["Retry-After"] != null) {    // Server is very busy
                        if (errorCounter > Site.RetryTimes)
                            throw;
                        int seconds;
                        int.TryParse(webResp.Headers["Retry-After"], out seconds);
                        if (seconds > 0)
                            retryDelaySeconds = seconds;
                        Console.Error.WriteLine(e.Message);
                        Console.Error.WriteLine(string.Format(
                            Bot.Msg("Retrying in {0} seconds..."), 
                            retryDelaySeconds));
                        Bot.Wait(retryDelaySeconds);
                    }
                    else if (e.Status == WebExceptionStatus.ProtocolError) {
                        int code = (int)((HttpWebResponse)webResp).StatusCode;
                        if (code == 500 || code == 502 || code == 503 || code == 504)
                        {
                            // Remote server problem
                            if (errorCounter > Site.RetryTimes)
                                throw;
                            Console.Error.WriteLine(e.Message);
                            Console.Error.WriteLine(string.Format(Bot.Msg("Retrying in {0} seconds..."), retryDelaySeconds));
                            Bot.Wait(retryDelaySeconds);
                        }
                        else
                            throw;
                    }
                    else
                        throw;
                }
            }

            StreamReader strmReader = new StreamReader(webResp.GetResponseStream());
            string respStr = strmReader.ReadToEnd();
            strmReader.Close();
            webResp.Close();
            UploadImageCheck(targetName, respStr);

            Title = Site.GetNsPrefix(6) + targetName;
            Text = description;
            Console.WriteLine(Bot.Msg("Image \"{0}\" has been uploaded successfully."), Title);
        }

        /// <summary>Uploads web image to wiki site.</summary>
        /// <param name="imageFileUrl">Full URL of image file on the web.</param>
        /// <param name="description">Image description.</param>
        /// <param name="license">Image license type. Used only in some wiki sites. Pass
        /// empty string, if the wiki site doesn't require it.</param>
        /// <param name="copyStatus">Image copy status. Used only in some wiki sites. Pass
        /// empty string, if the wiki site doesn't require it.</param>
        public void UploadImageFromWeb(string imageFileUrl, string description, string license, string copyStatus)
        {
            if (string.IsNullOrEmpty(imageFileUrl))
                throw new ArgumentNullException("imageFileUrl", Bot.Msg("No URL specified for image to upload."));
            Uri res = new Uri(imageFileUrl);
            Bot.InitWebClient();
            string imageFileName = imageFileUrl.Substring(imageFileUrl.LastIndexOf("/") + 1);
            try {
                Bot.WebClient.DownloadFile(res, Bot.CacheDir + Path.DirectorySeparatorChar + imageFileName);
            }
            catch (WebException) {
                throw new WikiBotException(string.Format(
                    Bot.Msg("Can't access image \"{0}\"."), 
                    imageFileUrl));
            }

            if (!File.Exists(Bot.CacheDir + Path.DirectorySeparatorChar + imageFileName))
                throw new WikiBotException(string.Format(
                    Bot.Msg("Error occurred when downloading image \"{0}\"."), 
                    imageFileUrl));
            UploadImage(
                Bot.CacheDir + Path.DirectorySeparatorChar + imageFileName,
                description,
                license,
                copyStatus,
                imageFileUrl);
            File.Delete(Bot.CacheDir + Path.DirectorySeparatorChar + imageFileName);
        }

        /// <summary>Downloads image, audio or video file, pointed by this page's title,
        /// from the wiki site to local computer. Redirection is resolved automatically.</summary>
        /// <param name="filePathName">Path and name of local file to save image to.</param>
        public void DownloadImage(string filePathName)
        {
            string res = Site.IndexPath + "?title=" + Bot.UrlEncode(Title);
            string src = string.Empty;
            try {
                src = Site.GetWebPage(res);
            }
            catch (WebException e) {
                string message = e.Message;
                if (message.Contains(": (404) ")) {    // Not found
                    Console.Error.WriteLine(Bot.Msg("Page \"{0}\" doesn't exist."), Title);
                    Text = string.Empty;
                    return;
                }

                throw;
            }

            Regex fileLinkRegex1 = new Regex("<a href=\"([^\"]+?)\" class=\"internal\"");
            Regex fileLinkRegex2 =
                new Regex("<div class=\"fullImageLink\" id=\"file\"><a href=\"([^\"]+?)\"");
            string fileLink = string.Empty;
            if (fileLinkRegex1.IsMatch(src))
                fileLink = fileLinkRegex1.Match(src).Groups[1].ToString();
            else if (fileLinkRegex2.IsMatch(src))
                fileLink = fileLinkRegex2.Match(src).Groups[1].ToString();
            else
                throw new WikiBotException(string.Format(
                    Bot.Msg("Image \"{0}\" doesn't exist."), Title));
            if (!fileLink.StartsWith("http"))
                fileLink = new Uri(new Uri(Site.Address), fileLink).ToString();
            Bot.InitWebClient();
            Console.WriteLine(Bot.Msg("Downloading image \"{0}\"..."), Title);
            Bot.WebClient.DownloadFile(fileLink, filePathName);
            Console.WriteLine(Bot.Msg("Image \"{0}\" has been downloaded successfully."), Title);
        }

        /// <summary>Saves page text to the specified file. If the target file already exists
        /// it is overwritten.</summary>
        /// <param name="filePathName">Path and name of the file.</param>
        public void SaveToFile(string filePathName)
        {
            if (IsEmpty()) {
                Console.Error.WriteLine(Bot.Msg("Page \"{0}\" contains no text to save."), Title);
                return;
            }

            File.WriteAllText(filePathName, Text, Encoding.UTF8);
            Console.WriteLine(Bot.Msg("Text of \"{0}\" page has been saved to \"{1}\" file."), Title, filePathName);
        }

        /// <summary>Saves <see cref="Text"/> to the ".txt" file in current directory.
        /// Use Directory.SetCurrentDirectory() function to change the current directory (but don't
        /// forget to change it back after saving file). The name of the file is constructed
        /// from the title of the article. Forbidden characters in filenames are replaced
        /// with their Unicode numeric codes (also known as numeric character references
        /// or NCRs).</summary>
        public void SaveToFile()
        {
            string fileTitle = Title;
            ///Path.GetInvalidFileNameChars();
            fileTitle = fileTitle.Replace("\"", "&#x22;");
            fileTitle = fileTitle.Replace("<", "&#x3c;");
            fileTitle = fileTitle.Replace(">", "&#x3e;");
            fileTitle = fileTitle.Replace("?", "&#x3f;");
            fileTitle = fileTitle.Replace(":", "&#x3a;");
            fileTitle = fileTitle.Replace("\\", "&#x5c;");
            fileTitle = fileTitle.Replace("/", "&#x2f;");
            fileTitle = fileTitle.Replace("*", "&#x2a;");
            fileTitle = fileTitle.Replace("|", "&#x7c;");
            SaveToFile(fileTitle + ".txt");
        }

        /// <summary>Returns true, if <see cref="Text"/> field is empty. Don't forget
        /// to call <see cref="Page.Load()"/> before using this function.</summary>
        /// <returns>Returns bool value.</returns>
        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Text);
        }

        /// <summary>Returns true, if <see cref="Text"/> field is not empty. Don't forget
        /// to call <see cref="Page.Load()"/> before using this function.</summary>
        /// <returns>Returns bool value.</returns>
        public bool Exists()
        {
            return (string.IsNullOrEmpty(Text) == true) ? false : true;
        }

        /// <summary>Returns true, if page redirects to another page. Don't forget to load
        /// actual page contents from live wiki <see cref="Page.Load()"/> before using this
        /// function.</summary>
        /// <returns>Returns bool value.</returns>
        public bool IsRedirect()
        {
            if (!Exists())
                return false;
            return Site.Regexes["redirect"].IsMatch(Text);
        }

        /// <summary>Returns redirection target. Don't forget to load
        /// actual page contents from live wiki <see cref="Page.Load()"/> before using this
        /// function.</summary>
        /// <returns>Returns redirection target page title. Returns empty string, if this
        /// Page is not a redirection.</returns>
        public string RedirectsTo()
        {
            if (IsRedirect())
                return Site.Regexes["redirect"].Match(Text).Groups[1].ToString().Trim();
            else
                return string.Empty;
        }

        /// <summary>If this page is a redirection, this function loads the title and text
        /// of redirection target page into this Page object.</summary>
        public void ResolveRedirect()
        {
            if (IsRedirect()) {
                Revision = null;
                Title = RedirectsTo();
                Load();
            }
        }

        /// <summary>Returns true, if this page is a disambiguation page. This function
        /// automatically recognizes disambiguation templates on Wikipedia sites in
        /// different languages. But in order to be used on other sites, <see cref="DotNetWikiBot.Site.Disambig"/>
        /// variable must be manually set before this function is called.
        /// <see cref="DotNetWikiBot.Site.Disambig"/> should contain local disambiguation template's title or
        /// several titles delimited by '|' character, letters case doesn't matter, e.g.
        /// "disambiguation|disambig|disam". Page text
        /// will be loaded from wiki if it was not loaded prior to function call.</summary>
        /// <returns>Returns bool value.</returns>
        public bool IsDisambig()
        {
            if (string.IsNullOrEmpty(Text))
                Load();
            if (!string.IsNullOrEmpty(Site.Disambig))
                return Regex.IsMatch(Text, @"(?i)\{\{(" + Site.Disambig + ")}}");

            string message = "You need to " +
                             "manually set site.disambigStr variable before calling this function." +
                             "Please, refer to documentation for details.";
            if (!Site.Address.Contains(".wikipedia.org"))
                throw new ArgumentNullException("site.disambigStr", Bot.Msg(message));

            Console.WriteLine(Bot.Msg("Loading disambiguation template tags..."));
            var disambigTemplate = string.Empty;

            // Try to get template, that English Wikipedia's "Disambiguation" interwiki points to
            if (Site.Address.Contains("//en.wikipedia.org"))
                disambigTemplate = "Template:Disambiguation";
            else {
                string src = Site.GetWebPage(
                    Site.ApiPath + "?format=xml&action=query" + "&list=langbacklinks&lbllang=en&lbltitle=Template%3ADisambiguation");
                var xdoc = XDocument.Parse(src);
                try {
                    disambigTemplate = xdoc.Descendants("ll").First().Attribute("title").Value;
                }
                catch {
                    throw new ArgumentNullException("site.disambigStr", Bot.Msg(message));
                }
            }

            Site.Disambig = Site.RemoveNsPrefix(disambigTemplate, 10);

            // Get local aliases - templates that redirect to discovered disambiguation template
            string src2 = Site.GetWebPage(Site.ApiPath + "?format=xml&action=query" +
                                          "&list=backlinks&bllimit=500&blfilterredir=redirects&bltitle=" +
                                          Bot.UrlEncode(disambigTemplate));
            var xdoc2 = XDocument.Parse(src2);
            try {
                var disambigRedirects = (from link in xdoc2.Descendants("bl")
                                         select link.Attribute("title").Value).ToList();
                foreach (var disambigRedirect in disambigRedirects)
                    Site.Disambig += '|' + Site.RemoveNsPrefix(disambigRedirect, 10);
            }
            catch
            {
                // silently continue if no alias was found               
            }    

            return Regex.IsMatch(Text, @"(?i)\{\{(" + Site.Disambig + ")}}");
        }

        /// <summary>Changes default English namespace prefixes to correct local prefixes
        /// (e.g. for German wiki sites it changes "Category:..." to "Kategorie:...").</summary>
        public void CorrectNsPrefix()
        {
            Title = Site.CorrectNsPrefix(Title);
        }

        public string GetSection(string sectionTitle)
        {
            var result = string.Empty;
            var r = new Regex(@"(?s)\s*=+\s*" + sectionTitle + @"=+(.+)");
            var match = r.Match(Text);
            if (match.Success)
            {
                result = match.Groups[1].Value;
                match = new Regex(@"(?m)^\s*=+\s*").Match(result);
                if (match.Success)
                    return result.Substring(0, match.Index - Environment.NewLine.Length);
            }

            return result;
        }

        /// <summary>Returns the list of strings, containing all wikilinks ([[...]])
        /// found in page's text, excluding links in image descriptions, but including
        /// interlanguage links, links to sister wiki projects, categories, images, etc.</summary>
        /// <returns>Returns untouched links in a List.</returns>
        public List<string> GetAllLinks()
        {
            MatchCollection matches = Site.Regexes["wikiLink"].Matches(Text);
            var matchStrings = new List<string>();
            foreach (Match m in matches)
                matchStrings.Add(m.Groups["title"].Value.Trim());
            return matchStrings;
        }

        /// <summary>Finds all wikilinks in page text, excluding interwiki
        /// links, categories, embedded images and links in image descriptions.</summary>
        /// <returns>Returns the PageList object, in which page titles are the wikilinks,
        /// found in text.</returns>
        public PageList GetLinks()
        {
            MatchCollection matches = Site.Regexes["wikiLink"].Matches(Text);
            var exclLinks = GetSisterwikiLinks();
            exclLinks.AddRange(GetInterLanguageLinks());
            var pl = new PageList(Site);
            for (int i = 0; i < matches.Count; i++) {
                string str = matches[i].Groups["title"].Value;
                if (str.StartsWith(Site.GetNsPrefix(6), true, Site.LangCulture) ||    // image
                    str.StartsWith(Site.GetEnglishNsPrefix(6), true, Site.LangCulture) ||
                    str.StartsWith(Site.GetNsPrefix(Site.CategoryNS), true, Site.LangCulture) ||    // category
                    str.StartsWith(Site.GetEnglishNsPrefix(Site.CategoryNS), true, Site.LangCulture))
                    continue;
                str = str.TrimStart(':');
                if (exclLinks.Contains(str))
                    continue;
                int fragmentPosition = str.IndexOf("#");

                // in-page section link
                if (fragmentPosition == 0)    
                    continue;
                if (fragmentPosition != -1)
                    str = str.Substring(0, fragmentPosition);
                pl.Add(new Page(Site, str));
            }

            return pl;
        }

        /// <summary>Returns the list of strings which contains external links
        /// found in page's text.</summary>
        /// <returns>Returns the List object.</returns>
        public List<string> GetExternalLinks()
        {
            MatchCollection matches = Site.Regexes["webLink"].Matches(Text);
            List<string> matchStrings = new List<string>();
            foreach (Match m in matches)
                matchStrings.Add(m.Value);
            return matchStrings;
        }

        /// <summary>Gets interlanguage links for pages on WikiMedia Foundation's
        /// projects.</summary>
        /// <remarks>WARNING: Because of WikiMedia software bug, this function does not work
        /// properly on main pages of WikiMedia Foundation's projects.</remarks>
        /// <returns>Returns Listof strings. Each string contains page title 
        /// prefixed with language code and colon, e.g. "de:Stern".</returns>
        public List<string> GetInterLanguageLinks()
        {
            string src = Site.GetWebPage(Site.ApiPath +
                                         "?format=xml&action=query&prop=langlinks&lllimit=500&titles=" +
                                         Bot.UrlEncode(Title));
            var xdoc = XDocument.Parse(src);
            var langLinks = (from link in xdoc.Descendants("ll")
                             select link.Attribute("lang").Value + ':' + link.Value).ToList();
            return langLinks;
        }

        /// <summary>Returns links to sister wiki projects, found in this page's text. These may
        /// include interlanguage links but only those embedded in text, not those located 
        /// on wikidata.org</summary>
        /// <returns>Returns the List&lt;string&gt; object.</returns>
        public List<string> GetSisterwikiLinks()
        {
            string src = Site.GetWebPage(Site.ApiPath +
                                         "?action=query&prop=iwlinks&format=xml&iwlimit=5000&titles=" +
                                         Bot.UrlEncode(Title));
            var xdoc = XDocument.Parse(src);
            var links = (from el in xdoc.Descendants("ns")
                         select el.Attribute("prefix").Value + '|' + el.Value).ToList();
            return links;
        }

        /// <summary>For pages of Wikimedia foundation projects this function returns
        /// interlanguage links located on <see href="https://wikidata.org">
        /// Wikidata.org</see>.</summary>
        /// <returns>Returns the List&lt;string&gt; object.</returns>
        public List<string> GetWikidataLinks()
        {
            var src = Site.GetWebPage(Site.IndexPath + "?title=" + Bot.UrlEncode(Title));
            var list = new List<string>();
            if (!src.Contains("<li class=\"interlanguage-link "))
                return list;
            src = "<ul>" + Bot.GetSubstring(src, "<li class=\"interlanguage-link ", "</ul>");
            MatchCollection matches = Regex.Matches(src, "interlanguage-link interwiki-([^\" ]+)");
            list.AddRange(from Match m in matches select m.Groups[1].Value);
            return list;
        }

        /// <summary>For pages that have associated items on <see href="https://wikidata.org">
        /// Wikidata.org</see> this function returns
        /// If page is not associated with a Wikidata item null is returned.</summary>
        /// <param name="itemId">starts with Q</param>
        public Result GetWikidataItem(string itemId = null)
        {
            if (itemId == null)
            {
                string src = Site.GetWebPage(Site.IndexPath + "?title=" + Bot.UrlEncode(Title));
                Match m = Regex.Match(src, "href=\"//www\\.wikidata\\.org/wiki/(Q\\d+)");
                if (!m.Success) // fallback
                    m = Regex.Match(src, "\"wgWikibaseItemId\"\\:\"(Q\\d+)\"");
                if (!m.Success)
                {
                    Console.WriteLine(Bot.Msg("No Wikidata item is associated with page \"{0}\"."), Title);
                    return null;
                }

                return new Result { Title = m.Groups[1].Value };
            }
            string jsonSrc = Site.GetWebPage("http://www.wikidata.org/wiki/Special:EntityData/" +
                                             Bot.UrlEncode(itemId) + ".json"); // raises "404: Not found" if not found
            return ParseWikidataItem(jsonSrc);
        }

        /// <summary>Function converts basic HTML markup in this page's text to wiki
        /// markup, except for tables markup. Use
        /// <see cref="Page.ConvertHtmlTablesToWikiTables()"/> function to convert HTML
        /// tables markup to wiki format.</summary>
        public void ConvertHtmlMarkupToWikiMarkup()
        {
            Text = Regex.Replace(Text, "(?is)n?<(h1)( [^/>]+?)?>(.+?)</\\1>n?", "\n= $3 =\n");
            Text = Regex.Replace(Text, "(?is)n?<(h2)( [^/>]+?)?>(.+?)</\\1>n?", "\n== $3 ==\n");
            Text = Regex.Replace(Text, "(?is)n?<(h3)( [^/>]+?)?>(.+?)</\\1>n?", "\n=== $3 ===\n");
            Text = Regex.Replace(Text, "(?is)n?<(h4)( [^/>]+?)?>(.+?)</\\1>n?", "\n==== $3 ====\n");
            Text = Regex.Replace(Text, "(?is)n?<(h5)( [^/>]+?)?>(.+?)</\\1>n?", "\n===== $3 =====\n");
            Text = Regex.Replace(Text, "(?is)n?<(h6)( [^/>]+?)?>(.+?)</\\1>n?", "\n====== $3 ======\n");
            Text = Regex.Replace(Text, "(?is)\n?\n?<p( [^/>]+?)?>(.+?)</p>", "\n\n$2");
            Text = Regex.Replace(Text, "(?is)<a href ?= ?[\"'](http:[^\"']+)[\"']>(.+?)</a>", "[$1 $2]");
            Text = Regex.Replace(Text, "(?i)</?(b|strong)>", "'''");
            Text = Regex.Replace(Text, "(?i)</?(i|em)>", "''");
            Text = Regex.Replace(Text, "(?i)\n?<hr ?/?>\n?", "\n----\n");
            Text = Regex.Replace(Text, "(?i)<(hr|br)( [^/>]+?)? ?/?>", "<$1$2 />");
        }

        /// <summary>Function converts HTML table markup in this page's text to wiki
        /// table markup.</summary>
        /// <seealso cref="Page.ConvertHtmlMarkupToWikiMarkup()"/>
        public void ConvertHtmlTablesToWikiTables()
        {
            if (!Text.Contains("</table>"))
                return;
            Text = Regex.Replace(Text, ">\\s+<", "><");
            Text = Regex.Replace(Text, "<table( ?[^>]*)>", "\n{|$1\n");
            Text = Regex.Replace(Text, "</table>", "|}\n");
            Text = Regex.Replace(Text, "<caption( ?[^>]*)>", "|+$1 | ");
            Text = Regex.Replace(Text, "</caption>", "\n");
            Text = Regex.Replace(Text, "<tr( ?[^>]*)>", "|-$1\n");
            Text = Regex.Replace(Text, "</tr>", "\n");
            Text = Regex.Replace(Text, "<th([^>]*)>", "!$1 | ");
            Text = Regex.Replace(Text, "</th>", "\n");
            Text = Regex.Replace(Text, "<td([^>]*)>", "|$1 | ");
            Text = Regex.Replace(Text, "</td>", "\n");
            Text = Regex.Replace(Text, "\n(\\||\\|\\+|!) \\| ", "\n$1 ");
            Text = Text.Replace("\n\n|", "\n|");
        }

        /// <summary>Returns the list of strings which contains category names found in
        /// page's text, with namespace prefix, without sorting keys. You can use the resultant
        /// strings to call <see cref="PageList.FillFromCategory(string)"/>
        /// or <see cref="PageList.FillFromCategoryTree(string)"/>
        /// function. Categories added by templates are not returned. Use GetAllCategories()
        /// function to get such categories too.</summary>
        /// <returns>Returns the List object.</returns>
        public List<string> GetCategories()
        {
            return GetCategories(true, false);
        }

        /// <summary>Returns the list of strings which contains categories' names found in
        /// page text. Categories added by templates are not returned. Use
        /// <see cref="Page.GetAllCategories()"/>
        /// function to get categories added by templates too.</summary>
        /// <param name="withNameSpacePrefix">If true, function returns strings with
        /// namespace prefix like "Category:Stars", not just "Stars".</param>
        /// <param name="withSortKey">If true, function returns strings with sort keys,
        /// if found. Like "Stars|D3" (in [[Category:Stars|D3]]).</param>
        /// <returns>Returns the List object.</returns>
        public List<string> GetCategories(bool withNameSpacePrefix, bool withSortKey)
        {
            MatchCollection matches = Site.Regexes["wikiCategory"].Matches(
                Regex.Replace(Text, "(?is)<nowiki>.+?</nowiki>", string.Empty));
            var matchStrings = new List<string>();
            foreach (Match m in matches) {
                string str = m.Groups[4].Value.Trim();
                if (withSortKey)
                    str += m.Groups[5].Value.Trim();
                if (withNameSpacePrefix)
                    str = Site.GetNsPrefix(Site.CategoryNS) + str;
                matchStrings.Add(str);
            }

            return matchStrings;
        }

        /// <summary>Returns list of strings, containing category names found in
        /// page's text and added by page's templates.</summary>
        /// <returns>Category names with namespace prefixes (e.g. "Category:Art").</returns>
        public List<string> GetAllCategories()
        {
            string uri;
            string xpathQuery;
            if (Site.UseApi) {
                uri = Site.ApiPath + "?action=query&prop=categories" +
                      "&clprop=sortkey|hidden&cllimit=5000&format=xml&titles=" +
                      Bot.UrlEncode(Title);
                xpathQuery = "//categories/cl/@title";
            }
            else {
                uri = Site.IndexPath + "?title=" + Bot.UrlEncode(Title) + "&redirect=no";
                xpathQuery = "//ns:div[ @id='mw-normal-catlinks' or @id='mw-hidden-catlinks' ]" +
                             "/ns:ul/ns:li/ns:a";
            }

            string src = Site.GetWebPage(uri);
            if (Site.UseApi) {
                int startPos = src.IndexOf("<!-- start content -->");
                int endPos = src.IndexOf("<!-- end content -->");
                if (startPos != -1 && endPos != -1 && startPos < endPos)
                    src = src.Remove(startPos, endPos - startPos);
                else {
                    startPos = src.IndexOf("<!-- bodytext -->");
                    endPos = src.IndexOf("<!-- /bodytext -->");
                    if (startPos != -1 && endPos != -1 && startPos < endPos)
                        src = src.Remove(startPos, endPos - startPos);
                }
            }

            var xmlNs = new XmlNamespaceManager(new NameTable());
            xmlNs.AddNamespace("ns", "http://www.w3.org/1999/xhtml");
            XPathNodeIterator iterator = Bot.GetXmlIterator(src, xpathQuery, xmlNs);
            var matchStrings = new List<string>();
            iterator.MoveNext();
            for (int i = 0; i < iterator.Count; i++) {
                matchStrings.Add(Site.GetNsPrefix(14) +
                                 Site.RemoveNsPrefix(HttpUtility.HtmlDecode(iterator.Current.Value), Site.CategoryNS));
                iterator.MoveNext();
            }

            return matchStrings;
        }

        /// <summary>Adds the page to the specified category by adding a
        /// link to that category to the very end of page's text.
        /// If the link to the specified category
        /// already exists, the function silently does nothing.</summary>
        /// <param name="categoryName">Category name, with or without prefix.
        /// Sort key can also be included after "|", like "Category:Stars|D3".</param>
        public void AddToCategory(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
                throw new ArgumentNullException("categoryName");
            categoryName = Site.RemoveNsPrefix(categoryName, Site.CategoryNS);
            string cleanCategoryName = !categoryName.Contains("|") ? categoryName.Trim()
                                           : categoryName.Substring(0, categoryName.IndexOf('|')).Trim();
            List<string> categories = GetCategories(false, false);
            foreach (string category in categories)
                if (category == Bot.Capitalize(cleanCategoryName) ||
                    category == Bot.Uncapitalize(cleanCategoryName))
                    return;
            Text += (categories.Count == 0 ? "\n" : string.Empty) +
                    "\n[[" + Site.GetNsPrefix(Site.CategoryNS) + categoryName + "]]\n";
            Text = Text.TrimEnd("\r\n".ToCharArray());
        }

        /// <summary>Removes the page from category by deleting link to that category in
        /// page text.</summary>
        /// <param name="categoryName">Category name, with or without prefix.</param>
        public void RemoveFromCategory(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
                throw new ArgumentNullException("categoryName");
            categoryName = Site.RemoveNsPrefix(categoryName, Site.CategoryNS).Trim();
            categoryName = !categoryName.Contains("|") ? categoryName
                               : categoryName.Substring(0, categoryName.IndexOf('|'));
            List<string> categories = GetCategories(false, false);
            if (!categories.Contains(Bot.Capitalize(categoryName)) &&
                !categories.Contains(Bot.Uncapitalize(categoryName)))
                return;
            string regexCategoryName = Regex.Escape(categoryName);
            regexCategoryName = regexCategoryName.Replace("_", "\\ ").Replace("\\ ", "[_\\ ]");
            int firstCharIndex = (regexCategoryName[0] == '\\') ? 1 : 0;
            regexCategoryName = "[" + char.ToLower(regexCategoryName[firstCharIndex]) + 
                                char.ToUpper(regexCategoryName[firstCharIndex]) + "]" +
                                regexCategoryName.Substring(firstCharIndex + 1);
            Text = Regex.Replace(
                Text,
                @"\[\[((?i)" + Site.GetNsPrefixes(Site.CategoryNS) + "): ?" + regexCategoryName + @"(\|.*?)?]]\r?\n?",
                string.Empty);
            Text = Text.TrimEnd("\r\n".ToCharArray());
        }

        /// <summary>Returns the templates found in text of this page (those inside double
        /// curly brackets {{...}} ). MediaWiki's
        /// <see href="https://www.mediawiki.org/wiki/Help:Magic_words">"magic words"</see>
        /// are not returned as templates.</summary>
        /// <param name="withParameters">If set to true, everything inside curly brackets is
        /// returned with all parameters untouched.</param>
        /// <param name="includePages">If set to true, titles of "transcluded" pages are returned as
        /// templates and messages with "msgnw:" prefix are also returned as templates. See
        /// <see href="https://www.mediawiki.org/wiki/Transclusion">this page</see> for details.
        /// Default is false.</param>
        /// <returns>Returns the List object.</returns>
        public List<string> GetTemplates(bool withParameters, bool includePages)
        {
            // Blank unsuitable regions with '_' char for easiness
            string str = Site.Regexes["noWikiMarkup"].Replace(
                Text,
                match => new string('_', match.Value.Length));
            if (GetNamespace() == 10)    // template
                /// remove template parameters
                str = Regex.Replace(
                    str,
                    @"\{\{\{.*?}}}",
                    match => new string('_', match.Value.Length));

            var templPos = new Dictionary<int, int>();
            var templates = new List<string>();
            int startPos, endPos, len = 0;

            // Find all templates positions, blank found templates with '_' char for easiness
            while ((startPos = str.LastIndexOf("{{")) != -1) {
                endPos = str.IndexOf("}}", startPos);
                len = (endPos != -1) ? endPos - startPos + 2 : 2;
                if (len != 2)
                    templPos.Add(startPos, len);
                str = str.Remove(startPos, len);
                str = str.Insert(startPos, new string('_', len));
            }

            // Collect templates using found positions, remove non-templates
            foreach (KeyValuePair<int, int> pos in templPos) {
                str = Text.Substring(pos.Key + 2, pos.Value - 4).Trim();
                if (str == string.Empty || str[0] == '#')
                    continue;
                if (Site.Regexes["magicWordsAndVars"].IsMatch(str))
                    continue;
                if (!withParameters) {
                    endPos = str.IndexOf('|');
                    if (endPos != -1)
                        str = str.Substring(0, endPos);
                    if (str == string.Empty)
                        continue;
                }

                if (!includePages) {
                    if (str[0] == ':'
                        || Site.Regexes["allNsPrefixes"].IsMatch(str)
                        || str.StartsWith("msgnw:")
                        || str.StartsWith("MSGNW:"))
                    {
                        continue;
                    }
                } 
                else 
                {
                    if (str[0] == ':')
                        str = str.Remove(0, 1);
                    else if (str.StartsWith("msgnw:") || str.StartsWith("MSGNW:"))
                        str = str.Remove(0, 6);
                }

                templates.Add(str);
            }

            templates.Reverse();
            return templates;
        }

        /// <summary>Adds a specified template to the end of the page text,
        /// but before categories.</summary>
        /// <param name="templateText">Complete template in double brackets,
        /// e.g. "{{TemplateTitle|param1=val1|param2=val2}}".</param>
        public void AddTemplate(string templateText)
        {
            if (string.IsNullOrEmpty(templateText))
                throw new ArgumentNullException("templateText");
            Regex templateInsertion = new Regex("([^}]\n|}})\n*\\[\\[((?i)" +
                                                Site.GetNsPrefixes(Site.CategoryNS) + "):");
            if (templateInsertion.IsMatch(Text))
                Text = templateInsertion.Replace(
                    Text,
                    "$1\n" + templateText + "\n\n[[" + Site.GetNsPrefix(Site.CategoryNS), 
                    1);
            else {
                Text += "\n\n" + templateText;
                Text = Text.TrimEnd("\r\n".ToCharArray());
            }
        }

        /// <summary>Removes all instances of a specified template from page text.</summary>
        /// <param name="templateTitle">Title of template to remove.</param>
        public void RemoveTemplate(string templateTitle)
        {
            if (string.IsNullOrEmpty(templateTitle))
                throw new ArgumentNullException("templateTitle");
            templateTitle = Regex.Escape(templateTitle);
            templateTitle = "(" + char.ToUpper(templateTitle[0]) + "|" +
                            char.ToLower(templateTitle[0]) + ")" +
                            (templateTitle.Length > 1 ? templateTitle.Substring(1) : string.Empty);
            Text = Regex.Replace(Text, @"(?s)\{\{\s*" + templateTitle + @"(.*?)}}\r?\n?", string.Empty);
        }

        /// <summary>Returns specified parameter of a specified template. If several instances
        /// of specified template are found in text of this page, all parameter values
        /// are returned.</summary>
        /// <param name="templateTitle">Title of template to get parameter of.</param>
        /// <param name="templateParameter">Title of template's parameter. If parameter is
        /// untitled, specify it's number as string. If parameter is titled, but it's number is
        /// specified, the function will return empty List &lt;string&gt; object.</param>
        /// <returns>Returns the List &lt;string&gt; object with strings, containing values of
        /// specified parameters in all found template instances. Returns empty List &lt;string&gt;
        /// object if no specified template parameters were found.</returns>
        public List<string> GetTemplateParameter(string templateTitle, string templateParameter)
        {
            if (string.IsNullOrEmpty(templateTitle))
                throw new ArgumentNullException("templateTitle");
            if (string.IsNullOrEmpty(templateParameter))
                throw new ArgumentNullException("templateParameter");
            if (string.IsNullOrEmpty(Text))
                throw new ArgumentNullException("text");

            var parameterValues = new List<string>();
            Dictionary<string, string> parameters;
            templateTitle = templateTitle.Trim();
            templateParameter = templateParameter.Trim();
            var pattern = "^\\s*(" + Bot.Capitalize(Regex.Escape(templateTitle)) + "|" +
                          Bot.Uncapitalize(Regex.Escape(templateTitle)) +
                          ")\\s*\\|";
            Regex templateTitleRegex = new Regex(pattern);
            foreach (string template in GetTemplates(true, false)) {
                if (templateTitleRegex.IsMatch(template)) {
                    parameters = ParseTemplate(template);
                    if (parameters.ContainsKey(templateParameter))
                        parameterValues.Add(parameters[templateParameter]);
                }
            }

            return parameterValues;
        }

        /// <summary>This helper method returns specified parameter of a first found instance of
        /// specified template. If no such template or no such parameter was found,
        /// empty string "" is returned.</summary>
        /// <param name="templateTitle">Title of template to get parameter of.</param>
        /// <param name="templateParameter">Title of template's parameter. If parameter is
        /// untitled, specify it's number as string. If parameter is titled, but it's number is
        /// specified, the function will return empty List &lt;string&gt; object.</param>
        /// <returns>Returns parameter as string or empty string "".</returns>
        /// <remarks>Thanks to Eyal Hertzog and metacafe.com team for idea of this
        /// function.</remarks>
        public string GetFirstTemplateParameter(string templateTitle, string templateParameter)
        {
            var paramsList = GetTemplateParameter(templateTitle, templateParameter);
            if (paramsList.Count == 0)
                return string.Empty;
            return paramsList[0];
        }

        /// <summary>Sets the specified parameter of the specified template to new value.
        /// If several instances of specified template are found in text of this page, either
        /// first value can be set, or all values in all instances.</summary>
        /// <param name="templateTitle">Title of template.</param>
        /// <param name="templateParameter">Title of template's parameter.</param>
        /// <param name="newParameterValue">New value to set the parameter to.</param>
        /// <param name="firstTemplateOnly">When set to true, only first found template instance
        /// is modified. When set to false, all found template instances are modified.</param>
        /// <returns>Returns the number of modified values.</returns>
        /// <remarks>Thanks to Eyal Hertzog and metacafe.com team for idea of this
        /// function.</remarks>
        public int SetTemplateParameter(
            string templateTitle, 
            string templateParameter,
            string newParameterValue, 
            bool firstTemplateOnly)
        {
            if (string.IsNullOrEmpty(templateTitle))
                throw new ArgumentNullException("templateTitle");
            if (string.IsNullOrEmpty(templateParameter))
                throw new ArgumentNullException("templateParameter");
            if (string.IsNullOrEmpty(Text))
                throw new ArgumentNullException("text");

            int i = 0;
            Dictionary<string, string> parameters;
            templateTitle = templateTitle.Trim();
            templateParameter = templateParameter.Trim();
            var templateTitleRegex = new Regex("^\\s*(" +
                                                 Bot.Capitalize(Regex.Escape(templateTitle)) + "|" +
                                                 Bot.Uncapitalize(Regex.Escape(templateTitle)) +
                                                 ")\\s*\\|");
            foreach (string template in GetTemplates(true, false)) {
                if (templateTitleRegex.IsMatch(template)) {
                    parameters = ParseTemplate(template);
                    if (newParameterValue != null)
                        parameters[templateParameter] = newParameterValue;
                    else
                        parameters.Remove(templateParameter);
                    string newTemplate = FormatTemplate(templateTitle, parameters, template);
                    var oldTemplate = new Regex(Regex.Escape(template));
                    newTemplate = newTemplate.Substring(2, newTemplate.Length - 4);
                    newTemplate = newTemplate.TrimEnd("\n".ToCharArray());
                    Text = oldTemplate.Replace(Text, newTemplate, 1);
                    i++;
                    if (firstTemplateOnly == true)
                        break;
                }
            }

            return i;
        }

        /// <summary>Removes the specified parameter of the specified template.
        /// If several instances of specified template are found in text of this page, either
        /// first instance can be affected or all instances.</summary>
        /// <param name="templateTitle">Title of template.</param>
        /// <param name="templateParameter">Title of template's parameter.</param>
        /// <param name="firstTemplateOnly">When set to true, only first found template instance
        /// is modified. When set to false, all found template instances are modified.</param>
        /// <returns>Returns the number of removed instances.</returns>
        public int RemoveTemplateParameter(string templateTitle, string templateParameter, bool firstTemplateOnly)
        {
            return SetTemplateParameter(templateTitle, templateParameter, null, firstTemplateOnly);
        }

        /// <summary>Returns a list of files, embedded in this page.</summary>
        /// <returns>Returns the List object. Returned file names contain namespace prefixes.
        /// The list can be empty. Strings in list may recur
        /// indicating that file was embedded several times.</returns>
        public List<string> GetImages()
        {
            if (string.IsNullOrEmpty(Text))
                throw new ArgumentNullException("text");
            string nsPrefixes = Site.GetNsPrefixes(6);
            MatchCollection matches =
                Regex.Matches(Text, @"\[\[\s*((?i)" + nsPrefixes + @"):(?<filename>[^|\]]+)");
            List<string> matchStrings = new List<string>();
            foreach (Match m in matches) {
                matchStrings.Add(Site.GetNsPrefix(6) + m.Groups["filename"].Value.Trim());
            }

            if (Regex.IsMatch(Text, "(?i)<gallery>")) {
                matches = Regex.Matches(
                    Text,
                    @"^\s*((?i)" + nsPrefixes + "):(?<filename>[^|\\]\r?\n]+)");
                foreach (Match m in matches) {
                    matchStrings.Add(Site.GetNsPrefix(6) + m.Groups["filename"].Value.Trim());
                }
            }
            
            return matchStrings;
        }

        /// <summary>Identifies the namespace of the page.</summary>
        /// <returns>Returns the integer key of the namespace.</returns>
        public int GetNamespace()
        {
            return Site.GetNamespace(Title);
        }

        /// <summary>Sends page title to console.</summary>
        public void ShowTitle()
        {
            Console.Write("\n" + Bot.Msg("The title of this page is \"{0}\".") + "\n", Title);
        }

        /// <summary>Sends page text to console.</summary>
        public void ShowText()
        {
            Console.Write("\n" + Bot.Msg("The text of \"{0}\" page:"), Title);
            Console.Write("\n\n" + Text + "\n\n");
        }

        /// <summary>Renames the page. Redirection from old title to new title is
        /// automatically created.</summary>
        /// <param name="newTitle">New title of this page.</param>
        /// <param name="reason">Reason for renaming.</param>
        public void RenameTo(string newTitle, string reason)
        {
            RenameTo(newTitle, reason, false, false);
        }

        /// <summary>Renames the page. Redirection from old title to new title is
        /// automatically created.</summary>
        /// <param name="newTitle">New title of this page.</param>
        /// <param name="reason">Reason for renaming.</param>
        /// <param name="renameTalkPage">If true, corresponding talk page will
        /// also be renamed.</param>
        /// <param name="renameSubPages">If true, subpages (like User:Me/Subpage)
        /// will also be renamed.</param>
        public void RenameTo(string newTitle, string reason, bool renameTalkPage, bool renameSubPages)
        {
            if (string.IsNullOrEmpty(newTitle))
                throw new ArgumentNullException("newTitle");
            if (string.IsNullOrEmpty(Title))
                throw new WikiBotException(Bot.Msg("No title is specified for page to rename."));

            if (Bot.AskConfirm) {
                Console.Write(
                    "\n\n" + Bot.Msg("The page \"{0}\" is going to be renamed to \"{1}\".\n"),
                    Title,
                    newTitle);
                if (!Bot.UserConfirms())
                    return;
            }

            if (Site.UseApi) {
                string token = string.Empty;
                if (Site.Tokens != null && Site.Tokens.ContainsKey("csrftoken"))
                    token = Site.Tokens["csrftoken"];
                else {
                    var tokens = GetSecurityTokens("move");
                    if (tokens.ContainsKey("missing"))
                        throw new WikiBotException(
                            string.Format(Bot.Msg("Page \"{0}\" doesn't exist."), Title));
                    if (!tokens.ContainsKey("movetoken") || tokens["movetoken"] == string.Empty)
                        throw new WikiBotException(string.Format(
                            Bot.Msg("Unable to rename page \"{0}\" to \"{1}\"."), Title, newTitle));
                    token = tokens["movetoken"];
                }

                string postData = string.Format(
                    "from={0}&to={1}&reason={2}{3}{4}&token={5}",
                    Bot.UrlEncode(Title),
                    Bot.UrlEncode(newTitle),
                    Bot.UrlEncode(reason),
                    renameTalkPage ? "&movetalk=1" : string.Empty,
                    renameSubPages ? "&movesubpages=1" : string.Empty,
                    Bot.UrlEncode(token));
                string respStr = Site.PostDataAndGetResult(Site.ApiPath + "?action=move" + "&format=xml", postData);
                if (respStr.Contains("<error"))
                    throw new WikiBotException(string.Format(
                        Bot.Msg("Failed to rename page \"{0}\" to \"{1}\"."), Title, newTitle));
            }
            else {
                string respStr = Site.GetWebPage(Site.IndexPath + "?title=Special:Movepage/" +
                                                 Bot.UrlEncode(Title));
                Match m = Site.Regexes["editToken"].Match(respStr);
                string securityToken = string.IsNullOrEmpty(m.Groups[1].Value)
                                           ? m.Groups[2].Value : m.Groups[1].Value;
                if (string.IsNullOrEmpty(securityToken)) {
                    Console.Error.WriteLine(
                        Bot.Msg("Unable to rename page \"{0}\" to \"{1}\"."), Title, newTitle);
                    return;
                }

                if (Site.WatchList == null) {
                    Site.WatchList = new PageList(Site);
                    Site.WatchList.FillFromWatchList();
                }

                Watched = Site.WatchList.Contains(this);

                string postData = string.Format(
                    "wpNewTitle={0}&wpOldTitle={1}&wpEditToken={2}" + "&wpReason={3}{4}{5}{6}",
                    Bot.UrlEncode(newTitle),
                    Bot.UrlEncode(Title),
                    Bot.UrlEncode(securityToken),
                    Bot.UrlEncode(reason),
                    renameTalkPage ? "&wpMovetalk=1" : string.Empty,
                    renameSubPages ? "&wpMovesubpages=1" : string.Empty,
                    Watched ? "&wpWatch=1" : string.Empty);
                respStr = Site.PostDataAndGetResult(Site.IndexPath + "?title=Special:Movepage&action=submit", postData);

                if (Site.Messages == null)
                    Site.LoadMediawikiMessages(true);
                Regex successMsg = new Regex(
                    "<h1[^>]*>(<span[^>]*>)?\\s*" + Site.Messages["pagemovedsub"] + "\\s*<");
                if (!successMsg.IsMatch(respStr))
                    throw new WikiBotException(string.Format(
                        Bot.Msg("Failed to rename page \"{0}\" to \"{1}\"."), Title, newTitle));
            }

            Title = newTitle;
            Console.WriteLine(
                Bot.Msg("Page \"{0}\" has been successfully renamed to \"{1}\"."), Title, newTitle);
        }

        /// <summary>Deletes the page. Administrator's rights are required
        /// for this action.</summary>
        /// <param name="reason">Reason for deletion.</param>
        public void Delete(string reason)
        {
            if (string.IsNullOrEmpty(Title))
                throw new WikiBotException(Bot.Msg("No title is specified for page to delete."));

            if (Bot.AskConfirm) {
                Console.Write("\n\n" + Bot.Msg("The page \"{0}\" is going to be deleted.\n"), Title);
                if (!Bot.UserConfirms())
                    return;
            }

            if (Site.UseApi) {
                string token = string.Empty;
                if (Site.Tokens != null && Site.Tokens.ContainsKey("csrftoken"))
                    token = Site.Tokens["csrftoken"];
                else {
                    var tokens = GetSecurityTokens("delete");
                    if (tokens.ContainsKey("missing"))
                        throw new WikiBotException(
                            string.Format(Bot.Msg("Page \"{0}\" doesn't exist."), Title));
                    if (!tokens.ContainsKey("deletetoken") || tokens["deletetoken"] == string.Empty)
                        throw new WikiBotException(
                            string.Format(Bot.Msg("Unable to delete page \"{0}\"."), Title));
                    token = tokens["deletetoken"];
                }

                string postData = string.Format("reason={0}&token={1}", Bot.UrlEncode(reason), Bot.UrlEncode(token));
                string respStr = Site.PostDataAndGetResult(
                    Site.ApiPath + "?action=delete" + "&title=" + Bot.UrlEncode(Title) + "&format=xml", 
                    postData);
                if (respStr.Contains("<error"))
                    throw new WikiBotException(
                        string.Format(Bot.Msg("Failed to delete page \"{0}\"."), Title));
            }
            else {
                string respStr = Site.GetWebPage(Site.IndexPath + "?title=" +
                                                 Bot.UrlEncode(Title) + "&action=delete");
                Match m = Site.Regexes["editToken"].Match(respStr);
                string securityToken = string.IsNullOrEmpty(m.Groups[1].Value)
                                           ? m.Groups[2].Value : m.Groups[1].Value;
                if (string.IsNullOrEmpty(securityToken)) {
                    Console.Error.WriteLine(
                        Bot.Msg("Unable to delete page \"{0}\"."), Title);
                    return;
                }

                string postData = string.Format(
                    "wpReason={0}&wpEditToken={1}",
                    Bot.UrlEncode(reason),
                    Bot.UrlEncode(securityToken));
                respStr = Site.PostDataAndGetResult(
                    Site.IndexPath + "?title=" + Bot.UrlEncode(Title) + "&action=delete", 
                    postData);

                if (Site.Messages == null)
                    Site.LoadMediawikiMessages(true);
                Regex successMsg = new Regex(
                    "<h1[^>]*>(<span[^>]*>)?\\s*" + Site.Messages["actioncomplete"] + "\\s*<");
                if (!successMsg.IsMatch(respStr))
                    throw new WikiBotException(
                        string.Format(Bot.Msg("Failed to delete page \"{0}\"."), Title));
            }

            Console.WriteLine(Bot.Msg("Page \"{0}\" has been successfully deleted."), Title);
            Title = string.Empty;
        }

        internal Result ParseWikidataItem(string json)
        {
            var result = JObject.Parse(json)["entities"].Single().Single().ToObject<Result>();
            Console.WriteLine(
                Bot.Msg("Wikidata item {0} associated with page \"{1}\" was parsed successfully."),
                result.Title,
                Title);
            return result;
        }

        private void SavePostApi(string postData)
        {
            string respStr = Site.PostDataAndGetResult(Site.ApiPath, postData);

            XElement respXml = XElement.Parse(respStr);
            if (respXml.Element("error") != null)
            {
                string error = respXml.Element("error").Attribute("code").Value;
                string desc = respXml.Element("error").Attribute("info").Value;
                if (error == "editconflict")
                    throw new EditConflictException(string.Format(
                        Bot.Msg("Edit conflict occurred while trying to savе page \"{0}\"."),
                        Title));
                if (error == "noedit")
                    throw new InsufficientRightsException(string.Format(
                        Bot.Msg("Insufficient rights to edit page \"{0}\"."),
                        Title));
                throw new WikiBotException(desc);
            }

            if (respXml.Element("edit") != null
                && respXml.Element("edit").Element("captcha") != null)
            {
                throw new BotDisallowedException(string.Format(
                    Bot.Msg(ErrorOccurredWhenSavingPageBotOperationIsNotAllowedForThisAccountAtSite),
                    Title,
                    Site.Address));
            }
        }

        private void SavePost(string postData)
        {
            string respStr = Site.PostDataAndGetResult(
                Site.IndexPath + "?title=" + Bot.UrlEncode(Title) + "&action=submit",
                postData);
            if (respStr.Contains(" name=\"wpTextbox2\""))
                throw new EditConflictException(string.Format(
                    Bot.Msg("Edit conflict occurred while trying to savе page \"{0}\"."),
                    Title));
            if (respStr.Contains("<div class=\"permissions-errors\">"))
                throw new InsufficientRightsException(string.Format(
                    Bot.Msg("Insufficient rights to edit page \"{0}\"."),
                    Title));
            if (respStr.Contains("input name=\"wpCaptchaWord\" id=\"wpCaptchaWord\""))
                throw new BotDisallowedException(string.Format(
                    Bot.Msg(ErrorOccurredWhenSavingPageBotOperationIsNotAllowedForThisAccountAtSite),
                    Title,
                    Site.Address));
        }

        private void CheckFileName(string filePathName)
        {
            if (!File.Exists(filePathName))
                throw new ArgumentNullException(
                    "filePathName",
                    string.Format(Bot.Msg("Image file \"{0}\" doesn't exist."), filePathName));
            if (string.IsNullOrEmpty(Title))
                throw new WikiBotException(Bot.Msg("No title is specified for image to upload."));
            if (Path.GetFileNameWithoutExtension(filePathName).Length < 3)
                throw new WikiBotException(string.Format(
                    Bot.Msg("Name of file \"{0}\" must " + "contain at least 3 characters (excluding extension) for successful upload."),
                    filePathName));
        }

        private void UploadImageCheck(string targetName, string respStr)
        {
            if (!respStr.Contains(HttpUtility.HtmlEncode(targetName)))
                throw new WikiBotException(string.Format(
                    Bot.Msg("Error occurred when uploading image \"{0}\"."), Title));
            try
            {
                if (Site.Messages == null)
                    Site.LoadMediawikiMessages(true);
                string errorMessage = Site.Messages["uploaderror"];
                if (respStr.Contains(errorMessage))
                    throw new WikiBotException(string.Format(
                        Bot.Msg("Error occurred when uploading image \"{0}\"."), Title));
            }
            catch (WikiBotException e)
            {
                if (!e.Message.Contains("Uploadcorrupt"))    // skip if MediaWiki message not found
                    throw;
            }
        }
    }
}
