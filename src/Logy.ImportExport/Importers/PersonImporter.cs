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
            var ns = WikiSite.GetNamespace(p.Title);
            if (ns != MwAgent.DotNetWikiBot.Site.CategoryNS && ns != MwAgent.DotNetWikiBot.Site.TemplateNS
                && !p.TitleUnique.Contains("/"))
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

        protected override void UpdatePerson(Person person, PersonType type)
        {
            // careful updating: it will be strange to convert Organization to Animal
            if ((person.Type == PersonType.Human && type == PersonType.Dynasty)
                || (person.Type == PersonType.Dynasty && type == PersonType.Human))
            {
                person.Type = type;
                ObjectUpdated(person.Save());
            }
        }
    }
}
