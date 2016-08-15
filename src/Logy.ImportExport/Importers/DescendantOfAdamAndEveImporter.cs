using System;
using System.Collections.Generic;

using Logy.Entities.Documents;
using Logy.Entities.Documents.Bible;
using Logy.Entities.Persons;
using Logy.ImportExport.Bible;
using Logy.MwAgent.DotNetWikiBot;

using Skills.Xpo;

namespace Logy.ImportExport.Importers
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
            var foundDoc = new DocManager(XpoSession).FindByUrl(Doc, url);
            if (foundDoc == null)
            {
                foundDoc = new Doc(Doc) { Url = url, };
                ObjectsCreated.Add(foundDoc.Save());
            }

            var number = BibleManager.GetDocumentNumber(block.RefName);
            var foundDocPart = new DocManager(XpoSession).FindPart(Doc, number);
            if (foundDocPart == null)
            {
                var link = foundDoc.NewLink(number);
                foundDocPart = link.DocPart;
                ObjectsCreated.Add(link.Save());
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
