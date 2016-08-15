using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace Logy.MwAgent.DotNetWikiBot
{
    /// <summary>Class defines custom XML URL resolver, that has a caching capability. See
    /// <see href="http://www.w3.org/blog/systeam/2008/02/08/w3c_s_excessive_dtd_traffic">this page</see>
    /// for details.</summary>
    /// <exclude/>
    /// [PermissionSetAttribute(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class XmlUrlResolverWithCache : XmlUrlResolver
    {
        /// <summary>List of cached files absolute URIs.</summary>
        private static readonly string[] _CachedFilesUrIs = {
                                                               "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd",
                                                               "http://www.w3.org/TR/xhtml1/DTD/xhtml-lat1.ent",
                                                               "http://www.w3.org/TR/xhtml1/DTD/xhtml-symbol.ent",
                                                               "http://www.w3.org/TR/xhtml1/DTD/xhtml-special.ent"
                                                           };

        /// <summary>List of cached files names.</summary>
        private static readonly string[] _CachedFiles = {
                                                           "xhtml1-transitional.dtd",
                                                           "xhtml-lat1.ent",
                                                           "xhtml-symbol.ent",
                                                           "xhtml-special.ent"
                                                       };

        /// <summary>Local cache directory.</summary>
        private static readonly string _CacheDir = Bot.CacheDir + Path.DirectorySeparatorChar;

        /// <summary>Overriding GetEntity() function to implement local cache.</summary>
        /// <param name="absoluteUri">Absolute URI of requested entity.</param>
        /// <param name="role">User's role for accessing specified URI.</param>
        /// <param name="typeToReturn">Type of object to return.</param>
        /// <returns>Returns object or requested type.</returns>
        public override object GetEntity(Uri absoluteUri, string role, Type typeToReturn)
        {
            if (absoluteUri.ToString().EndsWith("-/W3C/DTD XHTML 1.0 Transitional/EN"))
                return new FileStream(
                    _CacheDir + "xhtml1-transitional.dtd",
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);
            if (absoluteUri.ToString().EndsWith("-//W3C//ENTITIES Latin 1 for XHTML//EN"))
                return new FileStream(
                    _CacheDir + "xhtml-lat1.ent",
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);
            if (absoluteUri.ToString().EndsWith("-//W3C//ENTITIES Symbols for XHTML//EN"))
                return new FileStream(
                    _CacheDir + "xhtml-symbol.ent",
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);
            if (absoluteUri.ToString().EndsWith("-//W3C//ENTITIES Special for XHTML//EN"))
                return new FileStream(
                    _CacheDir + "xhtml-special.ent",
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);
            for (int i = 0; i < _CachedFilesUrIs.Length; i++)
                if (absoluteUri.ToString().EndsWith(_CachedFiles[i]))
                    return new FileStream(
                        _CacheDir + _CachedFiles[i],
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read);
            return base.GetEntity(absoluteUri, role, typeToReturn);
        }
    }
}
