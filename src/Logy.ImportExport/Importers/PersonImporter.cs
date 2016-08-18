using System;
using System.Collections.Generic;

using Logy.Entities.Documents;
using Logy.Entities.Persons;
using Logy.MwAgent.DotNetWikiBot;

using Skills.Xpo;

namespace Logy.ImportExport.Importers
{
    public class PersonImporter : Importer
    {
        public PersonImporter(Doc doc, List<DbObject> objCreated)
            : base(doc, objCreated)
        {
        }

        public override void Import(ImportBlock page)
        {
            var type = PersonType.Human;
            for (var i = page.Categories.Count - 1; i >= 0; i--)
            {
                var cat = page.Categories[i];
                if (Enum.TryParse(ImportBlock.GetShortTitle(cat), out type))
                {
                    break;
                }
            }

            SavePersonName(page, type);
        }

        protected void SavePersonName(ImportBlock page, PersonType type, bool mayBeUser = true, Link link = null, int? wikiItemId = null)
        {
            Person person;
            if (new PersonManager(XpoSession)
                .FindByNameWithoutPName(page.TitleUnique, Language, out person, page.TitleShort, mayBeUser, type))
            {
                if (person == null)
                {
                    person = new Person(XpoSession) { Type = type, WikiDataItemId = wikiItemId };
                    ObjectsCreated.Add(person);
                }
                else
                {
                    if (wikiItemId != null && person.WikiDataItemId == null)
                        person.WikiDataItemId = wikiItemId;
                }

                var name = new PName(person) { Name = page.TitleUnique, ShortName = page.TitleShort, Language = Language };
                ObjectsCreated.Add(name);
                if (link == null)
                    link = Doc.NewLink(page.Title);
                link.PName = name;
                ObjectsCreated.Add(link.Save());
            }
        }
    }
}
