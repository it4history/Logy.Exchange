using System.Collections.Generic;

using Logy.Entities.Documents;
using Logy.Entities.Documents.Bible;
using Logy.Entities.Persons;
using Logy.ImportExport.Importers;
using Logy.MwAgent.DotNetWikiBot;

using Skills.Xpo;

namespace Logy.ImportExport.Bible
{
    public class DescendantOfAdamAndEveImporter : Importer
    {
        public DescendantOfAdamAndEveImporter(Doc doc, List<DbObject> objCreated)
            : base(doc, objCreated)
        {
        }

        public override void Import(ImportBlock page)
        {
            var block = (DescendantOfAdamAndEve)page;

            var url = BibleManager.GetDocumentUrl(block.RefName);
            var docManager = new DocManager(XpoSession);
            var foundDoc = docManager.FindByUrl(Doc, url);
            if (foundDoc == null)
            {
                foundDoc = new Doc(Doc) { Url = url, };
                ObjectsCreated.Add(foundDoc.Save());
            }

            var number = BibleManager.GetDocumentNumber(block.RefName);
            if (!string.IsNullOrEmpty(number))
            {
                var foundDocPart = docManager.FindPart(foundDoc, number);
                if (foundDocPart == null)
                {
                    var link = foundDoc.NewLink(number);
                    foundDocPart = link.DocPart;
                    ObjectsCreated.Add(link.Save());
                }
            }

            if (!string.IsNullOrEmpty(block.Title))
            {
                Person person;
                if (new PersonManager(XpoSession).FindByNameWithoutPName(page.TitleShort, Language, out person))
                {
                }

                ///Doc.Wiki.Url(block.Link) block.Link
            }
        }
    }
}
