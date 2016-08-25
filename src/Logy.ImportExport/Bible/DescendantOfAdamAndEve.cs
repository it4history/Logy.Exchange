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
        //// IWikiData commented because this import process wants to be fast
    {
        private readonly List<DescendantOfAdamAndEve> _wives = new List<DescendantOfAdamAndEve>();

        private List<DescendantOfAdamAndEve> _husbands = new List<DescendantOfAdamAndEve>();

        private List<DescendantOfAdamAndEve> _kids = new List<DescendantOfAdamAndEve>();
        
        private DescendantOfAdamAndEve _father;

        private DescendantOfAdamAndEve _mother;

        private string _titleUnique;
        
        public bool GenerationNumberUnknown { get; set; }

        public int GenerationNumber { get; set; }

        public int DotsCount { get; set; }

        public bool IsWife
        {
            get { return Husband != null; }
        }

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

        public DescendantOfAdamAndEve Father
        {
            get
            {
                return _father;
            }
            set
            {
                _father = value;
                SetParent(value);
            }
        }

        public DescendantOfAdamAndEve Mother
        {
            get
            {
                return _mother;
            }
            set
            {
                _mother = value;
                SetParent(value);
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

        public int? WikiDataItemId
        {
            get
            {
                if (!string.IsNullOrEmpty(Title))
                {
                    var p = new Page(Title);
                    try
                    {
                        var wikidataItem = p.GetWikidataItem();
                        int id;
                        if (int.TryParse(wikidataItem.Name.LocalName.TrimStart('Q'), out id))
                            return id;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                return null;
            }
        }

        public int GenerationCalculated
        {
            get { return Father == null ? 1 : Father.GenerationCalculated + 1; }
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
                    if (descendant.Title == "Jochebed" || descendant.Title == "Sarah" // Jochebed, Sarah are women and duplicated twice
                        || descendant.Title == "Zerubbabel" // Zerubbabel was mentioned twice
                        || descendant.WifeWithManyHusbands(all[index]))
                        descendant.MergeDuplicates(all[index]);
                    else
                    {
                        descendant.SetTitleUnique();
                        if (descendant.IsWife && !descendant.Husband.Wives.Contains(descendant))
                        {
                            descendant.Husband.Wives.Add(descendant);
                        }
                    }
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
            var prevMothers = new Dictionary<int, DescendantOfAdamAndEve>();

            var regex = new Regex(
                @"(\.*\s?)(\d+|\+\sm\d?|\?)\.?\s*(\[\[([^\|.]+)\|?(.*)\]\])?([^{^<.]*)({{.*}})?"
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
                        generationNumber = dotsCount;
                    }
                    else if (value1.StartsWith("+ m"))
                    {
                        if (prevDescendant != null)
                        {
                            if (prevDescendant.DotsCount < dotsCount 
                                || (prevDescendant.DotsCount == dotsCount && !prevDescendant.IsWife))
                                husband = prevDescendant;
                            else
                                prevFathers.TryGetValue(dotsCount, out husband);

                            if (husband == null && prevDescendant.IsWife)
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
                                             Title = match.Groups[4].Value,
                                             TitleShort = match.Groups[5].Value,
                                             OtherCaption = match.Groups[6].Value,
                                             RefName = match.Groups[10].Value,
                                             RefCaption = match.Groups[12].Value,
                                             Ref2Name = ref2Name,
                                             Ref2Caption = ref2Caption,
                                         }.Check();
                    
                    // must de added after Title set in order to correctly add husband.Wives
                    descendant.Husband = husband;
                    result.Add(descendant);

                    if (descendant.IsWife) 
                    {
                        SetPrevParent(prevMothers, descendant);
                    }
                    else
                    {
                        descendant.Father = SetPrevParent(prevFathers, descendant, prevMothers);
                        descendant.Mother = SetPrevParent(prevMothers, descendant);
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
                _titleUnique = TitleUnique + (// Husband != null ? " (wife of " + Husband : 
                                             Father != null
                                                 ? " (son of " + Father
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

        private static DescendantOfAdamAndEve SetPrevParent(
            Dictionary<int, DescendantOfAdamAndEve> prevFathersOrMothers,
            DescendantOfAdamAndEve descendant,
            Dictionary<int, DescendantOfAdamAndEve> prevMothers = null)
        {
            var useDotsForWife = prevMothers == null;
            DescendantOfAdamAndEve parent;
            if (useDotsForWife)
                prevFathersOrMothers.TryGetValue(descendant.DotsCount - 1, out parent);
            else
                prevFathersOrMothers.TryGetValue(descendant.GenerationNumber - 1, out parent);
            if (parent != null)

                // parent can not be determined for wives  || descendant.GenerationNumberUnknown != parent.GenerationNumberUnknown
                if (descendant.IsWife) 
                    parent = null;

            foreach (var gene in prevFathersOrMothers.Keys.ToList())
            {
                /*if (useDots)
                    if (gene > descendant.DotsCount && descendant.DotsCount > 0)
                        prevFathersOrMothers.Remove(gene);
                    else */
                if (gene > descendant.GenerationNumber)
                    prevFathersOrMothers.Remove(gene);
            }

            if (prevMothers != null)
            {
                foreach (var gene in prevMothers.Keys.ToList())
                {
                    if (gene >= descendant.DotsCount)
                        prevMothers.Remove(gene);
                }
            }

            if (descendant.IsWife || prevMothers != null)
            {
                if (useDotsForWife && descendant.DotsCount > 0)
                    prevFathersOrMothers[descendant.DotsCount] = descendant;
                else
                    prevFathersOrMothers[descendant.GenerationNumber] = descendant;
            }

            return parent;
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

        private void SetParent(DescendantOfAdamAndEve value)
        {
            if (value != null && !value._kids.Contains(this))
                value._kids.Add(this);
        }

        /// <summary>
        /// to store different husband of the same wife the duplication was designed
        /// </summary>
        private bool WifeWithManyHusbands(DescendantOfAdamAndEve descendant)
        {
            if (Title == "Basemath" && GenerationNumber == 22 && Father == null)
                return false;
            if (IsWife && descendant.IsWife)
            {
                var geneDiff = Math.Abs(Husband.GenerationNumber - descendant.Husband.GenerationNumber);
                return geneDiff == 0

                        // Tamar with Judah
                       || geneDiff == 1;
            }

            return false;
        }

        private void MergeDuplicates(DescendantOfAdamAndEve descendant)
        {
            foreach (var husband in descendant._husbands)
                if (!_husbands.Contains(husband))
                    _husbands.Add(husband);
            descendant._husbands = _husbands;

            foreach (var kid in descendant._kids)
                if (!_kids.Contains(kid))
                    _kids.Add(kid);
            descendant._kids = _kids;

            if (Father == null)
                Father = descendant.Father;
            else
                descendant.Father = Father;
            if (Mother == null)
                Mother = descendant.Mother;
            else
                descendant.Mother = Mother;

            if (descendant.RefName != RefName)
            {
                descendant.Ref2Name = RefName;
                descendant.Ref2Caption = RefCaption;
                Ref2Name = descendant.RefName;
                Ref2Caption = descendant.RefCaption;
            }
        }
    }
}
