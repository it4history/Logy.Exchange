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

            foreach (var link in new[]
                                     {
                                         SaveRef(block, BibleManager.GetDocumentUrl(block.RefName)),
                                         SaveRef(block, BibleManager.GetDocumentUrl(block.Ref2Name))
                                     })
            {
                if (link != null)
                    SavePersonName(page, PersonType.Human, false, link, block.WikiDataitemId);
            }

            // families
            // births
        }

        private Link SaveRef(DescendantOfAdamAndEve block, string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;
            var docManager = new DocManager(XpoSession);
            var foundDoc = docManager.FindByUrl(Doc, url);
            if (foundDoc == null)
            {
                foundDoc = new Doc(Doc) { Url = url, };
                ObjectsCreated.Add(foundDoc.Save());
            }

            var number = BibleManager.GetDocumentNumber(block.RefName);
            Link link;
            var foundDocPart = docManager.FindPart(foundDoc, number);
            if (foundDocPart == null)
            {
                link = foundDoc.NewLink(number);
            }
            else
                link = new Link(XpoSession)
                           {
                               Doc = foundDoc,
                               DocPart = foundDocPart
                           };

            ObjectsCreated.Add(link.Save());

            return link;
        }
    }
}
