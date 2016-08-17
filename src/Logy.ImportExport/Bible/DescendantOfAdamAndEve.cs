using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Logy.Entities.Documents.Bible;
using Logy.MwAgent.DotNetWikiBot;

using Skills.Xpo;

namespace Logy.ImportExport.Bible
{
    public class DescendantOfAdamAndEve : ImportBlock
    {
        private readonly List<DescendantOfAdamAndEve> _kids = new List<DescendantOfAdamAndEve>();

        private string _titleUnique;

        private DescendantOfAdamAndEve _parent;
        
        public bool GenerationNumberUnknown { get; set; }

        public int GenerationNumber { get; set; }

        public int DotsCount { get; set; }

        public DescendantOfAdamAndEve Husband { get; set; }

        /// <summary>
        /// father as a rule
        /// </summary>
        public DescendantOfAdamAndEve Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
                if (!_parent._kids.Contains(this))
                    _parent._kids.Add(this);
            }
        }

        public string LinkCaption { get; set; }

        public string OtherCaption { get; set; }

        public override string TitleUnique
        {
            get
            {
                if (_titleUnique != null)
                    return _titleUnique;

                return !string.IsNullOrEmpty(Title)
                                  ? Title
                                  : !string.IsNullOrEmpty(LinkCaption)
                                        ? LinkCaption
                                        : OtherCaption;
            }
        }

        [Required]
        public string RefName { get; set; }

        public string RefCaption { get; set; }

        public string Ref2Name { get; set; }

        public string Ref2Caption { get; set; }

        public static List<ImportBlock> ParseDescendants(Page page)
        {
            if (string.IsNullOrEmpty(page.Text))
                page.LoadTextOnly();
            return (from p in Parse(page.GetSection("Descendants")) select (ImportBlock)p).ToList();
        }

        public static List<DescendantOfAdamAndEve> Parse(string lines)
        {
            var result = new List<DescendantOfAdamAndEve>();
            DescendantOfAdamAndEve prevDescendant = null;

            // Key - dotsCount
            var prevFathers = new Dictionary<int, DescendantOfAdamAndEve>();
            var regex = new Regex(
                @"(\.*)(\d+|\+\sm\d?|\?)\.?\s*(\[\[([^\|.]+)\|?(.*)\]\])?([^{^<.]*)({{.*}})?"
                + @"(<ref(\sname=""([^/.]+)"")?\s*/?>(([^/.]*)</ref>)?)*\s?<");
            foreach (var line in Regex.Split(lines, "\r\n|\r|\n"))
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    var value1 = match.Groups[2].Value;
                    int generationNumber = 0;
                    var dotsCount = match.Groups[1].Value.Length;
                    DescendantOfAdamAndEve husband = null;
                    if (value1 == "?")
                    {
                        generationNumber = 1000 + dotsCount;
                        if (prevDescendant != null && !prevDescendant.GenerationNumberUnknown)
                        {
                            // prevFathers.Clear();
                        }
                    }
                    else if (value1.StartsWith("+ m"))
                    {
                        if (prevDescendant != null)
                        {
                            if (prevDescendant.DotsCount < dotsCount)
                                husband = prevDescendant;
                            else
                                prevFathers.TryGetValue(dotsCount, out husband);

                            if (husband == null && prevDescendant.Husband != null)
                                husband = prevDescendant.Husband;
                        }

                        if (husband != null)
                            generationNumber = husband.GenerationNumber;
                    }
                    else
                    {
                        if (!int.TryParse(value1, out generationNumber) && prevDescendant != null)
                        {
                            generationNumber = prevDescendant.GenerationNumber;
                        }
                    }

                    var ref2Name = match.Groups[10].Captures.Count > 1 ? match.Groups[10].Captures[0].Value : null;
                    var ref2Caption = match.Groups[12].Captures.Count > 1 ? match.Groups[12].Captures[0].Value : null;
                    var descendant = new DescendantOfAdamAndEve
                                         {
                                             GenerationNumber = generationNumber,
                                             DotsCount = dotsCount,
                                             GenerationNumberUnknown = value1 == "?",
                                             Husband = husband,
                                             Title = match.Groups[4].Value,
                                             LinkCaption = match.Groups[5].Value,
                                             OtherCaption = match.Groups[6].Value,
                                             RefName = match.Groups[10].Value,
                                             RefCaption = match.Groups[12].Value,
                                             Ref2Name = ref2Name,
                                             Ref2Caption = ref2Caption,
                                         }.Check();
                    result.Add(descendant);
                    if (descendant.Husband == null)
                    {
                        DescendantOfAdamAndEve father;
                        if (prevFathers.TryGetValue(dotsCount - 1, out father))
                            if (descendant.GenerationNumberUnknown == father.GenerationNumberUnknown)
                                descendant.Parent = father;

                        prevFathers[dotsCount > 0 ? dotsCount : descendant.GenerationNumber] = descendant;
                        foreach (var gene in prevFathers.Keys.ToList())
                        {
                            if (gene > descendant.GenerationNumber)
                                prevFathers.Remove(gene);
                        }
                    }

                    prevDescendant = descendant;
                }
            }

            return result;
        }

        public void SetTitleUnique()
        {
            if (TitleUnique == "Unknown")
                _titleUnique = "parent of " + _kids[0];
            else
                _titleUnique = TitleUnique + (Husband != null
                                                  ? " (wife of " + Husband
                                                  : Parent != null
                                                        ? " (son of " + Parent
                                                        : " (from " + RefName) + ")";
        }

        public override string ToString()
        {
            return TitleUnique;
        }

        internal DescendantOfAdamAndEve Check()
        {
            RefName = CheckRefCaption(RefCaption, RefName);
            Ref2Name = CheckRefCaption(Ref2Caption, Ref2Name);

            return this;
        }

        private static string CheckRefCaption(string refCaption, string refNameOriginal)
        {
            string refName = refNameOriginal;
            if (!string.IsNullOrEmpty(refCaption))
            {
                refName = BibleManager.GetRefName(refCaption);
                if (!string.IsNullOrEmpty(refNameOriginal) && refNameOriginal != refName)
                    throw new ArgumentException(refCaption + refName);
            }

            return refName;
        }
    }
}
