using System.Collections.Generic;

using Logy.Entities.Documents;
using Logy.Entities.Documents.Bible;
using Logy.Entities.Persons;
using Logy.ImportExport.Importers;
using Logy.MwAgent.DotNetWikiBot;

using Skills.Xpo;

namespace Logy.ImportExport.Bible
{
    public class DescendantOfAdamAndEveImporter : PersonImporter
    {
        public DescendantOfAdamAndEveImporter(Doc doc, List<DbObject> objCreated)
            : base(doc, objCreated)
        {
        }

        public override void Import(ImportBlock page)
        {
            var block = (DescendantOfAdamAndEve)page;

            var link = SaveDescendant(block);

            var father = link.PName.Person;
            foreach (var wife in block.Wives)
            {
                var wifeLink = SaveDescendant(wife);
                new Family(father) { Mother = wifeLink.PName.Person };
            }

            // births
        }

        private Link SaveDescendant(DescendantOfAdamAndEve block)
        {
            SaveDescendant(block, block.Ref2Name);
            return SaveDescendant(block, block.RefName);
        }

        private Link SaveDescendant(DescendantOfAdamAndEve block, string refName)
        {
            var link = SaveRef(block, BibleManager.GetDocumentUrl(refName));
            if (link != null)
                SavePersonName(block, PersonType.Human, false, link, null, block.WikiDataitemId);
            return link;
        }

        private Link SaveRef(DescendantOfAdamAndEve block, string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;
            var foundDoc = DocManager.FindByUrl(Doc, url);
            if (foundDoc == null)
            {
                foundDoc = new Doc(Doc) { Url = url, };
                ObjectsCreated.Add(foundDoc.Save());
            }

            var number = BibleManager.GetDocumentNumber(block.RefName);
            var link = foundDoc.NewLink(number);
            ObjectsCreated.Add(link.Save());

            return link;
        }
    }
}
