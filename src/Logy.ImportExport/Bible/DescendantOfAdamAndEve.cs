using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using AppConfiguration;
using Logy.Entities.Documents.Bible;
using Logy.MwAgent.DotNetWikiBot;
using Skills.Xpo;

namespace Logy.ImportExport.Bible
{
    public class DescendantOfAdamAndEve : ImportBlock
    {
        private readonly List<DescendantOfAdamAndEve> _kids = new List<DescendantOfAdamAndEve>();
        
        private readonly List<DescendantOfAdamAndEve> _wives = new List<DescendantOfAdamAndEve>();

        private readonly List<DescendantOfAdamAndEve> _husbands = new List<DescendantOfAdamAndEve>();

        private DescendantOfAdamAndEve _parent;

        private string _titleUnique;
        
        public bool GenerationNumberUnknown { get; set; }

        public int GenerationNumber { get; set; }

        public int DotsCount { get; set; }

        public DescendantOfAdamAndEve Husband
        {
            get
            {
                return _husbands.Count > 0 ? _husbands[0] : null;
            }
            set
            {
                if (value != null)
                {
                    if (!_husbands.Contains(value))
                        _husbands.Add(value);
                    if (!value._wives.Contains(this))
                        value._wives.Add(this);
                }
            }
        }

        public List<DescendantOfAdamAndEve> Wives
        {
            get { return _wives; }
        }

        public List<DescendantOfAdamAndEve> Husbands
        {
            get { return _husbands; }
        }

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
                if (value != null && !value._kids.Contains(this))
                    value._kids.Add(this);
            }
        }

        public List<DescendantOfAdamAndEve> Kids
        {
            get { return _kids; }
        }

        public override string TitleUnique
        {
            get
            {
                if (_titleUnique != null)
                    return _titleUnique;

                return !string.IsNullOrEmpty(Title)
                                  ? Title
                                  : !string.IsNullOrEmpty(TitleShort)
                                        ? TitleShort
                                        : OtherCaption;
            }
        }

        /// <summary>
        /// Visible link text. Real wiki link is Title
        /// </summary>
        public override string TitleShort { get; set; }

        public string OtherCaption { get; set; }
        
        [Required]
        public string RefName { get; set; }

        public string RefCaption { get; set; }

        public string Ref2Name { get; set; }

        public string Ref2Caption { get; set; }

        public int? WikiDataitemId
        {
            get
            {
                if (!string.IsNullOrEmpty(Title))
                {
                    var p = new Page(Title);
                    var wikidataItem = p.GetWikidataItem();
                    int id;
                    if (int.TryParse(wikidataItem.Name.LocalName.TrimStart('Q'), out id))
                        return id;
                }

                return null;
            }
        }

        public static List<DescendantOfAdamAndEve> ParseDescendants(Page page, bool removeDuplicates = false)
        {
            if (string.IsNullOrEmpty(page.Text))
                page.LoadTextOnly();
            var descendants = (from p in Parse(page.GetSection("Descendants")) select p).ToList();
            return removeDuplicates ? RemoveDuplicates(descendants) : descendants;
        }

        public static List<DescendantOfAdamAndEve> RemoveDuplicates(List<DescendantOfAdamAndEve> descendants) 
        {
            var all = new List<DescendantOfAdamAndEve>();
            foreach (var p in descendants)
            {
                var descendant = p;
                var index = all.BinarySearch(descendant);
                if (index >= 0)
                {
                    if (descendant.WifeWithManyHusbands(all[index]))
                        descendant.MergeHusbands(all[index]);
                    else
                        descendant.SetTitleUnique();
                }
                else
                {
                    all.Add(descendant);
                    all.Sort();
                }
            }

            all = new List<DescendantOfAdamAndEve>();
            var unique = new List<DescendantOfAdamAndEve>();
            var dupli = new Dictionary<string, IList>();
            foreach (var descendant in descendants)
            {
                var index = all.BinarySearch(descendant);
                if (index >= 0)
                {
                    if (!descendant.WifeWithManyHusbands(all[index]))
                    {
                        unique.Remove(descendant);
                        dupli.Merge(descendant.TitleUnique, new List<DescendantOfAdamAndEve> { all[index], descendant });
                    }
                }
                else
                {
                    unique.Add(descendant);
                    all.Add(descendant);
                    all.Sort();
                }
            }

            return unique;
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
                            if (prevDescendant.DotsCount < dotsCount 
                                || (prevDescendant.DotsCount == dotsCount && prevDescendant.Husband == null))
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
                                             TitleShort = match.Groups[5].Value,
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
                            if (descendant.Husband == null && descendant.GenerationNumber > 0 // no wife
                                && gene > descendant.GenerationNumber)
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
                _titleUnique = TitleUnique + (/// Husband != null ? " (wife of " + Husband : 
                                             Parent != null
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

        /// <summary>
        /// to store different husband of the same wife duplication was designed
        /// </summary>
        private bool WifeWithManyHusbands(DescendantOfAdamAndEve descendant)
        {
            return Husband != null && descendant.Husband != null
                   && Husband.GenerationNumber == descendant.Husband.GenerationNumber;
        }

        private void MergeHusbands(DescendantOfAdamAndEve descendant)
        {
            foreach (var husband in descendant._husbands)
            {
                if (!_husbands.Contains(husband))
                    _husbands.Add(husband);
            }

            descendant._husbands.Clear();
            descendant._husbands.AddRange(_husbands);
        }
    }
}
