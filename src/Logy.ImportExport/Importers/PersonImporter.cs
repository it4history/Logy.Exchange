using System;
using System.Collections.Generic;

using Logy.Entities.Documents;
using Logy.Entities.Persons;
using Logy.MwAgent.DotNetWikiBot;

using Skills.Xpo;

using Site = Logy.Entities.Model.Site;

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

        protected Link SavePersonName(
            ImportBlock page, 
            PersonType type, 
            bool mayBeUser = true, 
            Link link = null, 
            string number = null,
            int? wikiDataItemId = null)
        {
            Person person;
            var pname = new PersonManager(XpoSession)
                .FindByNameWithoutPName(page.TitleUnique, Language, wikiDataItemId, out person, page.TitleShort, mayBeUser, type);
            if (pname == null)
            {
                if (person == null)
                {
                    person = new Person(XpoSession) { Type = type };
                    ObjectsCreated.Add(person);
                }

                pname = new PName(person)
                               {
                                   Name = page.TitleUnique,
                                   ShortName = page.TitleShort,
                                   Language = Language,
                                   WikiDataItemId = wikiDataItemId,
                                   AbsoluteUrl = Site.Url(Doc.Site.BaseUrl, page.Title)
                               };
                ObjectsCreated.Add(pname);
            }
            else
            {
                if (wikiDataItemId != null && pname.WikiDataItemId == null)
                    pname.WikiDataItemId = wikiDataItemId;
            }

            if (link == null)
                link = Doc.NewLink(number);
            link.PName = pname;
            ObjectsCreated.Add(link.Save());

            return link;
        }
    }
}
