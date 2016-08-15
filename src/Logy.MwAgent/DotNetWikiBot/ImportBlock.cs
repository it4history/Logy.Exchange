using System;
using System.Collections.Generic;
using System.Linq;

namespace Logy.MwAgent.DotNetWikiBot
{
    public abstract class ImportBlock : IComparable
    {
        /// <summary>title, including namespace prefix.</summary>
        public string Title { get; set; }

        public string TitleShort
        {
            get { return GetShortTitle(Title); }
        }

        /// <summary>Filled during page importing from categories tree. Sorted from parent to child.</summary>
        public List<string> Categories { get; set; }

        public virtual string TitleUnique
        {
            get { return Title; }
        }

        public static string GetShortTitle(string pageTitle)
        {
            var ss = pageTitle.Split(':');
            return ss.Last();
        }

        public override bool Equals(object obj)
        {
            return CompareTo(obj) == 0;
        }

        public override int GetHashCode()
        {
            return (TitleUnique ?? GetType().Name).GetHashCode();
        }

        public int CompareTo(object other)
        {
            var ib = other as ImportBlock;
            return ib == null ? -1 : TitleUnique.CompareTo(ib.TitleUnique);
        }
    }
}
