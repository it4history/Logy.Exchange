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

            Person person;
            if (new PersonManager(XpoSession).FindByNameWithoutPName(page.TitleShort, Language, out person, true, type))
            {
                if (person == null)
                {
                    person = new Person(XpoSession) { Type = type };
                    ObjectsCreated.Add(person);
                }

                var name = new PName(person) { Name = page.TitleShort, Language = Language };
                ObjectsCreated.Add(name);
                var link = Doc.NewLink(page.Title);
                link.PName = name;
                ObjectsCreated.Add(link.Save());
            }
        }
    }
}
