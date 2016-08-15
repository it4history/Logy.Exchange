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
        public bool GenerationNumberUnknown { get; set; }

        public int GenerationNumber { get; set; }

        public DescendantOfAdamAndEve Husband { get; set; }

        public string LinkCaption { get; set; }

        public string OtherCaption { get; set; }

        public override string TitleUnique
        {
            get { return Title ?? LinkCaption ?? OtherCaption; }
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
            var regex = new Regex(
                @"[^\d^<]*(\d+|\+\sm|\?)\.?\s*(\[\[([^\|.]+)\|?(.*)\]\])?([^{^<.]*)({{.*}})?"
                + @"(<ref(\sname=""([^/.]+)"")?\s*/?>(([^/.]*)</ref>)?)*\s?<");
            foreach (var line in Regex.Split(lines, "\r\n|\r|\n"))
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    var value1 = match.Groups[1].Value;
                    int generationNumber;
                    if (!int.TryParse(value1, out generationNumber) && prevDescendant != null)
                    {
                        generationNumber = prevDescendant.GenerationNumber;
                    }

                    var ref2Name = match.Groups[9].Captures.Count > 1 ? match.Groups[9].Captures[0].Value : null;
                    var ref2Caption = match.Groups[11].Captures.Count > 1 ? match.Groups[11].Captures[0].Value : null;
                    result.Add(prevDescendant = new DescendantOfAdamAndEve
                                                    {
                                                        GenerationNumber = generationNumber,
                                                        GenerationNumberUnknown = value1 == "?",
                                                        Husband = value1 == "+ m" ? prevDescendant : null,
                                                        Title = match.Groups[3].Value,
                                                        LinkCaption = match.Groups[4].Value,
                                                        OtherCaption = match.Groups[5].Value,
                                                        RefName = match.Groups[9].Value,
                                                        RefCaption = match.Groups[11].Value,
                                                        Ref2Name = ref2Name,
                                                        Ref2Caption = ref2Caption,
                                                    }.Check());
                }
            }

            return result;
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
