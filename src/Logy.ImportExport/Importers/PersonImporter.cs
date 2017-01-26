using System;
using DevExpress.Xpo;
using Logy.Entities.Engine;
using Logy.Entities.Import;
using Logy.Entities.Links;
using Logy.Entities.Model;
using Logy.Entities.Persons;
using Logy.MwAgent;
using Site = Logy.Entities.Model.Site;

namespace Logy.ImportExport.Importers
{
    public class PersonImporter : CategoryImporter
    {
        public PersonImporter(Session session) : base(session)
        {
        }

        public override EntityType EntityType
        {
            get { return EntityType.Person; }
        }

        public override void Import(object page, Job job)
        {
            var p = (ImportBlock)page;
            var type = PersonType.Human;
            for (var i = p.Categories.Count - 1; i >= 0; i--)
            {
                var cat = p.Categories[i];
                if (Enum.TryParse(ImportBlock.GetShortTitle(cat), out type))
                {
                    break;
                }
            }

            Link link;
            SavePersonName(p, job, type, out link);
            link.Save();
        }

        /// <returns>null, if not modified</returns>
        protected PName SavePersonName(
            ImportBlock page,
            Job job,
            PersonType type, 
            out Link link,
            bool mayBeUser = true)
        {
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
                    /* commented because this import process wants to be fast. Set WikidataItemId later for page.Title
                    WikidataItemId = page is IWikidata ? ((IWikidata)page).WikidataItemId : null,*/
                    AbsoluteUrl = Site.Url(Wiki.BaseUrl, page.Title)
                };
                ObjectAdded(pname.Save()); // Save() for finding in transaction, next link.Save() does not help with link.PName

                link = job.NewLink();
                link.PName = pname;
                ObjectAdded(link); // not saved here, please save by yourself
            }
            else
            {
                link = LinkManager.FindLink(pname);
            }

            if (link == null)
                throw new ArgumentNullException("every PName should be linked");

            return pname;
        }
    }
}
