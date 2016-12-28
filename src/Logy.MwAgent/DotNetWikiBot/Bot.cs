// Logy.MwAgent 
// based on DotNetWikiBot Framework 3.15 - designed to make robots for MediaWiki-powered wiki sites
// Requires Microsoft .NET Framework 3.5+ or Mono 1.9+.
// Distributed under the terms of the GNU GPL 2.0 license: http://www.gnu.org/licenses/gpl-2.0.html
// Copyright (c) it4history (2016-2017) it4history@gmail.com
// Copyright (c) Iaroslav Vassiliev (2006-2016) codedriller@gmail.com
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Logy.MwAgent.DotNetWikiBot.Exceptions;

namespace Logy.MwAgent.DotNetWikiBot
{
    /// <summary>Class defines a Bot object, it contains most general configuration settings
    /// and some auxiliary functions.</summary>
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class Bot
    {
        /// <summary>Title and description of this bot as a web agent.</summary>
        private static readonly string _BotVer = "DotNetWikiBot";

        private static readonly WebClient _webClient = new WebClient();

        private static readonly Version _Version = new Version("3.15");

        /// <summary>Dictionary containing localized DotNetWikiBot interface messages.</summary>
        private static readonly SortedDictionary<string, string> _Messages = new SortedDictionary<string, string>();

        /// <summary>Current bot's console messages language (ISO 639-1 language code). Use
        /// <see cref="LoadLocalizedMessages(string)"/> function to change language.
        /// </summary>
        /// <exclude/>
        private static readonly string _BotMessagesLang;

        /// <summary>If true the bot reports errors and warnings only.
        /// Call <see cref="Bot.EnableSilenceMode()"/>
        /// function to enable that mode, don't change this variable's value directly.</summary>
        /// <exclude/>
        private static bool _silenceMode;

        /// <summary>If set to some file name (e.g. "DotNetWikiBot.Report.txt"), the bot
        /// writes all output to that file instead of a console. If no path was specified,
        /// the bot creates that file in it's current directory. File is encoded in UTF-8.
        /// Call <see cref="Bot.EnableLogging(string)"/> function to enable log writing,
        /// don't change this variable's value directly.</summary>
        /// <exclude/>
        private static string _logFile;

        /// <summary>Initial state of boolean HttpWebRequestElement.UseUnsafeHeaderParsing
        /// configuration setting: 0 means true, 1 means false, 2 means unchanged. This is
        /// internal variable.</summary>
        /// <exclude/>
        private static int _unsafeHttpHeaderParsingUsed = 2;
       
        /// <summary>This constructor is used to initialize Bot object.</summary>
        /// <returns>Returns Bot object.</returns>
        static Bot()
        {
            Console.Write(
                _BotVer + " " + _Version + "\n" + "Copyright (c) it4history and Iaroslav Vassiliev, GNU General Public License 2.0\n\n");

            // Format full version string
            _BotVer += "/" + _Version + " (" + Environment.OSVersion.VersionString + "; ";
            if (IsRunningOnMono) 
            {
                _BotVer += "Mono";
                try 
                {
                    Type type = Type.GetType("Mono.Runtime");
                    string v = string.Empty;
                    if (type != null) 
                    {
                        MethodInfo displayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
                        if (displayName != null)
                            v = displayName.Invoke(null, null).ToString();
                            _BotVer += " " + v.Substring(0, v.IndexOf(' '));
                    }
                }
                catch (Exception)
                {
                    // ignore failure silently
                }    

                _BotVer += "; ";
            }

            _BotVer += ".NET CLR " + Environment.Version + ")";

            // Find suitable directory for cache where all required permissions are present
            char dirSep = Path.DirectorySeparatorChar;
            CacheDir = Path.GetFullPath("Cache");
            try 
            {
                if (!Directory.Exists(CacheDir))
                    Directory.CreateDirectory(CacheDir);

                // Try if write and delete permissions are set for the folder
                File.WriteAllText(CacheDir + dirSep + "test.dat", "test");
                File.Delete(CacheDir + dirSep + "test.dat");
            }
            catch (Exception) 
            {
                // occurs if permissions are missing
                // Try one more location
                // on Mono framework ApplicationData location is "/home/ibboard/.config"
                CacheDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                    dirSep + "DotNetWikiBot" + dirSep + "Cache";
                try 
                {
                    if (!Directory.Exists(CacheDir))
                        Directory.CreateDirectory(CacheDir);

                    // Try if write and delete permissions are set for the folder
                    File.WriteAllText(CacheDir + dirSep + "test.dat", "test");
                    File.Delete(CacheDir + dirSep + "test.dat");
                }
                catch (Exception)
                {
                    // occurs if permissions are missing
                    var msg = string.Format(
                        Msg("Read/write permissions are " + "required for \"{0}\" directory."),
                        Path.GetFullPath("Cache"));
                    Console.WriteLine(msg);
                    throw new WikiBotException(msg);
                }

                Console.WriteLine(string.Format(Msg("Now using \"{0}\" directory for cache."), CacheDir));
            }

            // Load localized messages if available
            if (_BotMessagesLang == null)
                _BotMessagesLang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            if (_BotMessagesLang != "en" && _BotMessagesLang != "iv")    // iv = invariant culture
                if (!LoadLocalizedMessages(_BotMessagesLang))
                    _BotMessagesLang = "en";

            // Disable SSL/TLS server certificate validation on Mono
            if (IsRunningOnMono)
                ServicePointManager.ServerCertificateValidationCallback = Validator;

            // Don't strip trailing dots in URIs, see function description for details
            DisableCanonicalizingUriAsFilePath();

            // Disable 100-continue behaviour, it's not supported on WMF servers (as of 2012)
            ServicePointManager.Expect100Continue = false;

            CheckUpdates();

            Console.WriteLine("Downloading cache files from web if missing");
            string[] cacheFiles =
                {
                    "CommonData.xml", "xhtml1-transitional.dtd", "xhtml-lat1.ent", "xhtml-special.ent", "xhtml-symbol.ent"
                };
            foreach (string cacheFile in cacheFiles)
            {
                if (!File.Exists(CacheDir + dirSep + cacheFile))
                {
                    try
                    {
                        string src = GetWebResource(
                            new Uri("http://sourceforge.net/p/dotnetwikibot/" + "svn/HEAD/tree/cache/" + cacheFile + "?format=raw"),
                            string.Empty);
                        File.WriteAllText(CacheDir + dirSep + cacheFile, src);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message); 
                    }
                }
            }

            Console.WriteLine("Loading general info cache");
            using (StreamReader reader = File.OpenText(CacheDir + dirSep + "CommonData.xml"))
                CommonDataXml = XElement.Load(reader);
        }

        /// The destructor is used to unset Bot object.
        /// <exclude/>
        ~Bot()
        {
            // if (unsafeHttpHeaderParsingUsed != 2)
            // SwitchUnsafeHttpHeaderParsing(unsafeHttpHeaderParsingUsed == 1 ? true : false);
        }

        public static string BotVer
        {
            get { return _BotVer; }
        }

        /// <summary>Local cache directory. Adjust it if required.</summary>
        public static string CacheDir { get; set; }

        /// <summary>Last Site object constructed by the framework.</summary>
        public static Site LastSite { get; set; }

        /// <summary>Some unparsed supplementary data. You can see it
        /// <see href="https://sourceforge.net/p/dotnetwikibot/svn/HEAD/tree/cache/CommonData.xml">
        /// here.</see></summary>
        public static XElement CommonDataXml { get; set; }

        /// <summary>If true the bot asks user to confirm next Save(), RenameTo() or Delete()
        /// operation. False by default.</summary>
        /// <example><code>Bot.askConfirm = true { get; set; }</code></example>
        public static bool AskConfirm { get; set; }

        /// <summary>Auxillary internal web client that is used to access the web.</summary>
        /// <exclude/>
        public static WebClient WebClient
        {
            get { return _webClient; }
        }

        /// <summary>If true, assembly is running on Mono framework. If false,
        /// it is running on Microsoft .NET Framework. This variable is set
        /// automatically, don't change it's value.</summary>
        public static bool IsRunningOnMono
        {
            get { return Type.GetType("Mono.Runtime") != null; }
        }

        internal static int UnsafeHttpHeaderParsingUsed
        {
            get { return _unsafeHttpHeaderParsingUsed; }
        }

        /// <summary>Call this function to make bot write all output to the specified file
        /// instead of a console. If only error logging is desirable, first call this
        /// function and after that call <see cref="Bot.EnableSilenceMode()"/> function.</summary>
        /// <param name="logFileName">Path and name of a file to write output to.
        /// If no path was specified, the bot creates that file in it's current directory.
        /// File is encoded in UTF-8.</param>
        public static void EnableLogging(string logFileName)
        {
            _logFile = logFileName;
            StreamWriter log = File.AppendText(_logFile);
            log.AutoFlush = true;
            Console.SetError(log);
            if (!_silenceMode)
                Console.SetOut(log);
        }

        /// <summary>Call this function to make bot report only errors and warnings,
        /// no other messages will be displayed or logged.</summary>
        /// <seealso cref="Bot.DisableSilenceMode()"/>
        public static void EnableSilenceMode()
        {
            _silenceMode = true;
            Console.SetOut(new StringWriter());
        }

        /// <summary>Call this function to disable silent mode previously enabled by
        /// <see cref="Bot.EnableSilenceMode()"/> function.</summary>
        public static void DisableSilenceMode()
        {
            _silenceMode = false;
            var standardOutput = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
            Console.SetOut(standardOutput);
        }

        /// <summary>Function loads localized bot's interface messages from 
        /// <see href="http://sourceforge.net/p/dotnetwikibot/svn/HEAD/tree/DotNetWikiBot.i18n.xml">
        /// "DotNetWikiBot.i18n.xml"</see> file. Function is called in Bot class constructor, 
        /// but it can also be called manually to change interface language at runtime.</summary>
        /// <param name="language">Desired language's ISO 639-1 code (e.g., "fr").</param>
        /// <returns>Returns false if messages for specified language were not found.
        /// Returns true on success.</returns>
        public static bool LoadLocalizedMessages(string language)
        {
            if (!File.Exists("DotNetWikiBot.i18n.xml")) 
            {
                Console.Error.WriteLine("Localization file \"DotNetWikiBot.i18n.xml\" is missing.");
                Console.Error.Write("\n");
                return false;
            }

            using (XmlReader reader = XmlReader.Create("DotNetWikiBot.i18n.xml"))
            {
                if (!reader.ReadToFollowing(language))
                {
                    var s = "\nLocalized messages not found for language \"{0}\"." +
                            "\nYou can help DotNetWikiBot project by translating the messages in\n" +
                            "\"DotNetWikiBot.i18n.xml\" file and sending it to developers for " +
                            "distribution.\n";
                    Console.Error.WriteLine(s, language);
                    return false;
                }

                if (!reader.ReadToDescendant("msg"))
                    return false;
                if (_Messages.Count > 0)
                    _Messages.Clear();
                _Messages[reader["id"]] = reader.ReadString();
                while (reader.ReadToNextSibling("msg"))
                    _Messages[reader["id"]] = reader.ReadString();
            }

            return true;
        }

        /// <summary>Gets localized (translated) version of specified bot's
        /// interface message.</summary>
        /// <param name="message">Message in English. Placeholders for substituted parameters must
        /// be denoted in curly brackets: {0}, {1}, {2}, etc.</param>
        /// <returns>Returns localized version of the specified message,
        /// or English version if localized edition was not found.</returns>
        public static string Msg(string message)
        {
            if (_BotMessagesLang == "en")
                return message;
            try 
            {
                return _Messages[message];
            }
            catch (KeyNotFoundException) 
            {
                return message;
            }
        }

        /// <summary>Gets most recent <see cref="Site"/> object constructed by framework.</summary>
        /// <returns>Returns <see cref="Site"/> object.</returns>
        public static Site GetMostRecentSiteObject()
        {
            return LastSite;
            /// throw new WikiBotException(Msg("No default Site object is available."));
        }

        /// <summary>This function asks user to confirm next action. The message
        /// "Would you like to proceed (y/n/a)? " is displayed and user response is
        /// evaluated. Make sure to set <see cref="AskConfirm"/> variable to "true" before
        /// calling this function.</summary>
        /// <returns>Returns true, if user has confirmed the action.</returns>
        /// <example><code>
        /// if (Bot.askConfirm) {
        ///     Console.Write("Some action on live wiki is going to occur.\n\n");
        ///     if(!Bot.UserConfirms())
        ///         return;
        /// }
        /// </code></example>
        public static bool UserConfirms()
        {
            if (!AskConfirm)
                return true;
            ConsoleKeyInfo k;
            Console.Write(Msg("Would you like to proceed (y/n/a)?") + " ");
            k = Console.ReadKey();
            Console.Write("\n");
            if (k.KeyChar == 'y')
                return true;
            if (k.KeyChar == 'a')
            {
                AskConfirm = false;
                return true;
            }

            return false;
        }

        /// <summary>This auxiliary function counts the occurrences of specified string
        /// in specified text. This count is often required, but strangely there is no
        /// such function in .NET Framework's String class.</summary>
        /// <param name="text">String to look in.</param>
        /// <param name="str">String to look for.</param>
        /// <param name="ignoreCase">Pass "true" if you require case-insensitive search.
        /// Case-sensitive search is faster.</param>
        /// <returns>Returns the number of found occurrences.</returns>
        /// <example>
        /// <code>int m = Bot.CountMatches("Bot Bot bot", "Bot", false); // m=2</code>
        /// </example>
        public static int CountMatches(string text, string str, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (string.IsNullOrEmpty(str))
                throw new ArgumentNullException("str");
            int matches = 0;
            int position = 0;
            StringComparison rule = ignoreCase
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
            while ((position = text.IndexOf(str, position, rule)) != -1) 
            {
                matches++;
                position++;
            }

            return matches;
        }

        /// <summary>This auxiliary function returns the zero-based indexes of all occurrences
        /// of specified string in specified text.</summary>
        /// <param name="text">String to look in.</param>
        /// <param name="str">String to look for.</param>
        /// <param name="ignoreCase">Pass "true" if you require case-insensitive search.
        /// Case-sensitive search is faster.</param>
        /// <returns>Returns the List&lt;int&gt; object containing zero-based indexes of all found 
        /// occurrences or empty List&lt;int&gt; if nothing was found.</returns>
        public static List<int> GetMatchesPositions(string text, string str, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (string.IsNullOrEmpty(str))
                throw new ArgumentNullException("str");
            List<int> positions = new List<int>();
            StringComparison rule = ignoreCase
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
            int position = 0;
            while ((position = text.IndexOf(str, position, rule)) != -1) {
                positions.Add(position);
                position++;
            }

            return positions;
        }

        /// <summary>This auxiliary function returns part of the string which begins
        /// with some specified substring and ends with some specified substring.</summary>
        /// <param name="src">Source string.</param>
        /// <param name="startTag">Substring with which the resultant string
        /// must begin. Can be null or empty, in this case the source string is returned
        /// from the very beginning.</param>
        /// <param name="endTag">Substring that the resultant string
        /// must end with. Can be null or empty, in this case the source string is returned
        /// to the very end.</param>
        /// <returns>Portion of the source string.</returns>
        public static string GetSubstring(string src, string startTag, string endTag)
        {
            return GetSubstring(src, startTag, endTag, false, false, true);
        }

        /// <summary>This auxiliary function returns part of the string which begins
        /// with some specified substring and ends with some specified substring.</summary>
        /// <param name="src">Source string.</param>
        /// <param name="startTag">Substring that the resultant string
        /// must begin with. Can be null or empty, in this case the source string is returned
        /// from the very beginning.</param>
        /// <param name="endTag">Substring that the resultant string
        /// must end with. Can be null or empty, in this case the source string is returned
        /// to the very end.</param>
        /// <param name="removeStartTag">If true, startTag is not included into returned substring.
        /// Default is false.</param>
        /// <param name="removeEndTag">If true, endTag is not included into returned substring.
        /// Default is false.</param>
        /// <param name="raiseExceptionIfTagNotFound">When set to true, raises
        /// ArgumentOutOfRangeException if specified startTag or endTag was not found.
        /// Default is true.</param>
        /// <returns>Part of the source string.</returns>
        public static string GetSubstring(
            string src, 
            string startTag, 
            string endTag,
            bool removeStartTag, 
            bool removeEndTag, 
            bool raiseExceptionIfTagNotFound)
        {
            if (string.IsNullOrEmpty(src))
                throw new ArgumentNullException("src");
            int startPos = 0;
            int endPos = src.Length;

            if (!string.IsNullOrEmpty(startTag)) 
            {
                startPos = src.IndexOf(startTag);
                if (startPos == -1) 
                {
                    if (raiseExceptionIfTagNotFound)
                        throw new ArgumentOutOfRangeException("startPos");
                    startPos = 0;
                }
                else if (removeStartTag)
                    startPos += startTag.Length;
            }

            if (!string.IsNullOrEmpty(endTag)) 
            {
                endPos = src.IndexOf(endTag, startPos);
                if (endPos == -1) 
                {
                    if (raiseExceptionIfTagNotFound)
                        throw new ArgumentOutOfRangeException("endPos");
                    endPos = src.Length;
                }
                else if (!removeEndTag)
                    endPos += endTag.Length;
            }

            return src.Substring(startPos, endPos - startPos);
        }

        /// <summary>This helper function deletes everything before <paramref name="startTag"/>
        /// and everything after <paramref name="endTag"/> in the provided XML/XHTML source code
        /// and then inserts back the deleted DOCTYPE definition and root element of XML/XHTML
        /// document.</summary>
        /// <remarks>This function is very basic, it's not a true parser and thus it must not
        /// be used to parse documents generated outside MediaWiki software.</remarks>
        /// <param name="text">Source text.</param>
        /// <param name="startTag">A tag which identifies the beginning of target content.</param>
        /// <param name="endTag">A tag which identifies the end of target content.</param>
        /// <param name="removeTags">If true, specified startTag and endTag will be
        /// removed from resultant string.</param>
        /// <returns>Returns part of the source XML markup.</returns>
        public static string GetXmlSubstring(string text, string startTag, string endTag, bool removeTags)
        {
            if (string.IsNullOrEmpty(startTag))
                throw new ArgumentNullException("startTag");
            if (string.IsNullOrEmpty(endTag))
                throw new ArgumentNullException("endTag");

            int cursor = 0;

            string headerText = string.Empty;
            string footerText = string.Empty;

            while (cursor < text.Length && (text[cursor] == ' ' || text[cursor] == '\n'
                || text[cursor] == '\r' || text[cursor] == '\t'))
                    cursor++;    // skip whitespaces
            if (text.StartsWith("<?xml ")) 
            {
                cursor += text.IndexOf("?>", cursor) + 2;
                while (cursor < text.Length && (text[cursor] == ' ' || text[cursor] == '\n'
                    || text[cursor] == '\r' || text[cursor] == '\t'))
                        cursor++;    // skip whitespaces
            }

            if (text.StartsWith("<!DOCTYPE ")) 
            {
                cursor += text.IndexOf('>', cursor) + 1;
                while (cursor < text.Length && (text[cursor] == ' ' || text[cursor] == '\n'
                    || text[cursor] == '\r' || text[cursor] == '\t'))
                        cursor++;    // skip whitespaces
            }

            if (text.StartsWith("<!--"))    // comment
                cursor += text.IndexOf("-->", cursor) + 3;
            
            int headerEndPos = text.IndexOf('>', cursor) + 1;
            if (headerEndPos > 0)
                headerText = text.Substring(0, headerEndPos);

            int footerPos = text.LastIndexOf('<', cursor);
            if (footerPos > 0 && footerPos > headerEndPos)
                footerText = text.Substring(footerPos);

            return headerText +
                GetSubstring(text, startTag, endTag, removeTags, removeTags, true) + footerText;
        }

        /// <summary>This wrapper function encodes string for use in URI.
        /// The function is necessary because Mono framework doesn't support HttpUtility.UrlEncode()
        /// method and Uri.EscapeDataString() method doesn't support long strings, so a loop is
        /// required. By the way HttpUtility.UrlDecode() is supported by Mono, and a functions
        /// pair Uri.EscapeDataString()/HttpUtility.UrlDecode() is commonly recommended for
        /// encoding/decoding. Although there is another trouble with Uri.EscapeDataString():
        /// prior to .NET 4.5 it doesn't support RFC 3986, only RFC 2396.
        /// </summary>
        /// <param name="str">String to encode.</param>
        /// <returns>Encoded string.</returns>
        public static string UrlEncode(string str)
        {
            int limit = 32766; // 32766 is the longest string allowed in Uri.EscapeDataString()
            if (str.Length <= limit)
            {
                return Uri.EscapeDataString(str);
            }

            var sb = new StringBuilder(str.Length);
            int portions = str.Length / limit;
            for (int i = 0; i <= portions; i++)
            {
                sb.Append(i < portions
                              ? Uri.EscapeDataString(str.Substring(limit * i, limit))
                              : Uri.EscapeDataString(str.Substring(limit * i)));
            }

            return sb.ToString();
        }

        /// <summary>This auxiliary function makes the first letter in specified string upper-case.
        /// This is often required, but strangely there is no such function in .NET Framework's
        /// String class.</summary>
        /// <param name="str">String to capitalize.</param>
        /// <returns>Capitalized string.</returns>
        public static string Capitalize(string str)
        {
            if (char.IsUpper(str[0]))
                return str;
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        /// <summary>This auxiliary function makes the first letter in specified string lower-case.
        /// This is often required, but strangely there is no such function in .NET Framework's
        /// String class.</summary>
        /// <param name="str">String to uncapitalize.</param>
        /// <returns>Returns uncapitalized string.</returns>
        public static string Uncapitalize(string str)
        {
            if (char.IsLower(str[0]))
                return str;
            return char.ToLower(str[0]) + str.Substring(1);
        }

        /// <summary>Suspends execution for specified number of seconds.</summary>
        /// <param name="seconds">Number of seconds to wait.</param>
        public static void Wait(int seconds)
        {
            Thread.Sleep(seconds * 1000);
        }

        /// <summary>This internal function switches unsafe HTTP headers parsing on or off.
        /// Because there are many misconfigured servers on the web it is often required
        /// to ignore minor HTTP protocol violations.</summary>
        /// <exclude />
        public static void SwitchUnsafeHttpHeaderParsing(bool enabled)
        {
            var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(
                System.Configuration.ConfigurationUserLevel.None);
            var section = (System.Net.Configuration.SettingsSection)config.GetSection("system.net/settings");
            if (UnsafeHttpHeaderParsingUsed == 2)
                _unsafeHttpHeaderParsingUsed = section.HttpWebRequest.UseUnsafeHeaderParsing ? 1 : 0;
            section.HttpWebRequest.UseUnsafeHeaderParsing = enabled;
            config.Save();
            System.Configuration.ConfigurationManager.RefreshSection("system.net/settings");
        }

        /// <summary>This internal function clears the CanonicalizeAsFilePath attribute in
        /// .NET UriParser to fix a major .NET bug which makes System.Uri incorrectly strip trailing 
        /// dots in URIs.</summary>
        /// <exclude />
        public static void DisableCanonicalizingUriAsFilePath()
        {
            // See https://connect.microsoft.com/VisualStudio/feedback/details/386695/system-uri-in
            var getSyntax = typeof(UriParser).GetMethod("GetSyntax", BindingFlags.Static | BindingFlags.NonPublic);
            var flagsField = typeof(UriParser).GetField("m_Flags", BindingFlags.Instance | BindingFlags.NonPublic);
            if (getSyntax != null && flagsField != null)
            {
                foreach (string scheme in new[] { "http", "https" })
                {
                    var parser = (UriParser)getSyntax.Invoke(null, new object[] { scheme });
                    if (parser != null)
                    {
                        var flagsValue = (int)flagsField.GetValue(parser);

                        // Clear the CanonicalizeAsFilePath attribute
                        if ((flagsValue & 0x1000000) != 0)
                            flagsField.SetValue(parser, flagsValue & ~0x1000000);
                    }
                }
            }
        }

        /// <summary>
        /// This internal function is used to disable SSL/TLS server certificate validation on Mono.
        /// See <see href="http://www.mono-project.com/docs/faq/security/">this page</see> and
        /// <see href="http://www.mono-project.com/archived/usingtrustedrootsrespectfully/">
        /// this page</see>.
        /// </summary>
        /// <exclude />
        public static bool Validator(
            object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;    // validate every certificate
        }

        /// <summary>This helper function removes all attributes of root XML/XHTML element
        /// (XML namespace declarations, schema links, etc.) to ease processing.</summary>
        /// <param name="xmlSource">XML source code.</param>
        /// <returns>Corrected XML source code.</returns>
        public static string RemoveXmlRootAttributes(string xmlSource)
        {
            int startPos = ((xmlSource.StartsWith("<!") || xmlSource.StartsWith("<?"))
                && xmlSource.IndexOf('>') != -1) ? xmlSource.IndexOf('>') + 1 : 0;
            int firstSpacePos = xmlSource.IndexOf(' ', startPos);
            int firstCloseTagPos = xmlSource.IndexOf('>', startPos);
            if (firstSpacePos != -1 && firstCloseTagPos != -1 && firstSpacePos < firstCloseTagPos)
                return xmlSource.Remove(firstSpacePos, firstCloseTagPos - firstSpacePos);
            return xmlSource;
        }

        /// <summary>This helper function constructs XPathDocument object, makes XPath query and
        /// returns XPathNodeIterator object for selected nodes.</summary>
        /// <param name="xmlSource">Source XML data.</param>
        /// <param name="xpathQuery">XPath query to select specific nodes in XML data.</param>
        /// <param name="xmlNs">XML namespace manager.</param>
        /// <returns>XPathNodeIterator object.</returns>
        public static XPathNodeIterator GetXmlIterator(string xmlSource, string xpathQuery, XmlNamespaceManager xmlNs)
        {
            XmlReader reader = GetXmlReader(xmlSource);
            XPathDocument doc = new XPathDocument(reader);
            XPathNavigator nav = doc.CreateNavigator();
            return nav.Select(xpathQuery, xmlNs);
        }

        /// <summary>This helper function constructs XmlReader object
        /// using provided XML source code.</summary>
        /// <param name="xmlSource">Source XML data.</param>
        /// <returns>XmlReader object.</returns>
        public static XmlReader GetXmlReader(string xmlSource)
        {
            if (xmlSource.Contains("<!DOCTYPE html>"))
            {
                const string NewValue = "<!DOCTYPE html PUBLIC " +
                                        "\"-//W3C//DTD XHTML 1.0 Transitional//EN\" " +
                                        "\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">";
                xmlSource = xmlSource.Replace("<!DOCTYPE html>", NewValue);
            }

            if (!xmlSource.Contains("<html xmlns=")) 
            {
                xmlSource = xmlSource.Replace("<html", "<html xmlns=\"http://www.w3.org/1999/xhtml\"");
            }

            var strReader = new StringReader(xmlSource);
            var settings = new XmlReaderSettings
                               {
                                   XmlResolver = new XmlUrlResolverWithCache(),
                                   CheckCharacters = false,
                                   IgnoreComments = true,
                                   IgnoreProcessingInstructions = true,
                                   IgnoreWhitespace = true
                               };

            // For .NET 4.0 and higher DtdProcessing property should be used instead of ProhibitDtd
            if (settings.GetType().GetProperty("DtdProcessing") != null) 
            {
                Type t = typeof(XmlReaderSettings).GetProperty("DtdProcessing").PropertyType;
                settings.GetType().InvokeMember(
                    "DtdProcessing",
                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty, 
                    null, 
                    settings,
                    new[] { Enum.Parse(t, "2") });    // 2 is a value of DtdProcessing.Parse
            }
            else if (settings.GetType().GetProperty("ProhibitDtd") != null) 
            {
                settings.GetType().InvokeMember(
                    "ProhibitDtd",
                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty,
                    null, 
                    settings, 
                    new object[] { false });
            }

            return XmlReader.Create(strReader, settings);
        }

        /// <summary>This internal function initializes web client before it accesses
        /// web resources.</summary>
        /// <exclude />
        public static void InitWebClient()
        {
            if (!IsRunningOnMono)
                WebClient.UseDefaultCredentials = true;
            WebClient.Encoding = Encoding.UTF8;
            WebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            WebClient.Headers.Add("Accept-Encoding", "identity");    // disallow traffic compression
            WebClient.Headers.Add("User-agent", _BotVer);
        }

        /// <summary>This wrapper function gets web resource in a fault-tolerant manner.
        /// It should be used only in simple cases, because it sends no cookies, it doesn't support
        /// traffic compression and it lacks other useful features.</summary>
        /// <param name="address">Web resource's URI.</param>
        /// <param name="postData">Data to post with web request,
        /// it can be empty string or null.</param>
        /// <returns>Returns web resource as text.</returns>
        public static string GetWebResource(Uri address, string postData)
        {
            string webResourceText;
            for (int errorCounter = 0;; errorCounter++) {
                try {
                    InitWebClient();
                    webResourceText = string.IsNullOrEmpty(postData) 
                        ? WebClient.DownloadString(address) 
                        : WebClient.UploadString(address, postData);
                    break;
                }
                catch (WebException e) {
                    if (errorCounter > 3)    // retry 3 times by default
                        throw;
                    if (Regex.IsMatch(e.Message, ": \\(50[0234]\\) ")) {
                        // Remote server problem, retry
                        Console.Error.WriteLine(e.Message);
                        Console.Error.WriteLine(string.Format(Msg("Retrying in {0} seconds..."), 60));
                        Wait(60);
                    }
                    else if (e.Message.Contains("Section=ResponseStatusLine")) {
                        // Known Squid problem
                        SwitchUnsafeHttpHeaderParsing(true);
                        return GetWebResource(address, postData);
                    }
                    else
                        throw;
                }
            }

            return webResourceText;
        }

        private static void CheckUpdates()
        {
            // Check for updates
            /*try {
                string verInfo = GetWebResource(
                    new Uri("http://dotnetwikibot.sourceforge.net/info.php"), "");
                Match currentVer = Regex.Match(verInfo, "(?i)stable version: (([^ ]+)[^<]+)");
                if (new Version(currentVer.Groups[2].Value) > version)
                    Console.WriteLine("*** " + Msg("New version is available") + ": " +
                        currentVer.Groups[1].Value + " ***\n");
            }
            catch (Exception) {}    // ignore failure silently
            */
        }
    }
}
