using System;

using Logy.Entities.Documents;
using Logy.Entities.Model;
using Logy.Entities.Persons;
using Logy.MwAgent.DotNetWikiBot;

using Site = Logy.Entities.Model.Site;

namespace Logy.ImportExport.Importers
{
    public class PersonImporter : Importer
    {
        public PersonImporter(Doc doc) : base(doc)
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

            Link link;
            SavePersonName(page, type, out link);
            if (link != null)
                link.Save();
        }

        /// <returns>null, if not modified</returns>
        protected PName SavePersonName(
            ImportBlock page, 
            PersonType type,
            out Link link,
            bool mayBeUser = true)
        {
            link = null;
            var modified = false;
            Person person;
            var pname = new PersonManager(XpoSession)
                .FindByNameWithoutPName(page.TitleUnique, Language, out person, page.TitleShort, mayBeUser, type);
            if (pname == null)
            {
                if (person == null)
                {
                    person = new Person(XpoSession) { Type = type };
                    ObjectAdded(person);
                }

                pname = new PName(person)
                               {
                                   Name = page.TitleUnique,
                                   ShortName = page.TitleShort,
                                   Language = Language,
                                   WikiDataItemId = page is IWikiData ? ((IWikiData)page).WikiDataItemId : null,
                                   AbsoluteUrl = Site.Url(Doc.Site.BaseUrl, page.Title)
                               };
                ObjectAdded(pname.Save()); // Save() for finding in transaction
                modified = true;
            }
            else
            {
                if (pname.WikiDataItemId == null)
                {
                    pname.WikiDataItemId = page is IWikiData ? ((IWikiData)page).WikiDataItemId : null;
                    ObjectUpdated(pname);
                    modified = true;
                }
            }

            if (modified)
            {
                link = Doc.NewLink();
                link.PName = pname;
                ObjectAdded(link); // not saved because may be cancelled
            }

            return pname;
        }
    }
}
