using System;
using System.Collections.Generic;
using DevExpress.Xpo;
using Logy.Entities.Engine;
using Logy.Entities.Import;
using Logy.Entities.Links;
using Logy.Entities.Model;
using Logy.Entities.Persons;
using Logy.MwAgent;

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
            if (WikiSite.GetNamespace(p.TitleUnique) != MwAgent.DotNetWikiBot.Site.CategoryNS)
            {
                Link link;
                SavePersonName(p, job, GetPersonTypeByCategories(p.Categories), out link);

                link.Save();
            }
        }

        /// <summary>
        /// looking for the biggest (int)PersonType value
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        internal static PersonType GetPersonTypeByCategories(IList<string> categories)
        {
            var biggest = PersonType.Person;
            if (categories != null)
                for (var i = categories.Count - 1; i >= 0; i--)
                {
                    var cat = categories[i];
                    PersonType type;
                    if (Enum.TryParse(ImportBlock.GetShortTitle(cat), out type)
                        && type > biggest)
                    {
                        biggest = type;
                    }
                }
            return biggest;
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
                .FindByNameWithoutPName(page.TitleUnique, Language, out person, mayBeUser, page.TitleShort, type);
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
                    Url = page.Title,
                    /* commented because this import process wants to be fast. Set WikidataItemId later for page.Title
                    WikidataItemId = page is IWikidata ? ((IWikidata)page).WikidataItemId : null,*/
                };
                ObjectAdded(pname.Save()); // Save() for finding in transaction, next link.Save() does not help with link.PName

                link = job.NewLink();
                link.PName = pname;
                ObjectAdded(link); // not saved here, please save by yourself
            }
            else
            {
                if (person.Type != type)
                {
                    // careful updating: it will be strange to convert Organization to Animal
                    person.Type = type;
                    ObjectUpdated(person.Save());
                }
                link = LinkManager.FindLink(pname);
            }

            if (link == null)
                throw new ArgumentNullException("every PName should be linked");

            return pname;
        }
    }
}
