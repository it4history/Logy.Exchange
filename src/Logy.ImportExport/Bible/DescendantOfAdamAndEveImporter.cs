using System;
using System.Collections;
using DevExpress.Xpo;
using Logy.Entities.Documents;
using Logy.Entities.Documents.Bible;
using Logy.Entities.Engine;
using Logy.Entities.Links;
using Logy.Entities.Persons;
using Logy.Entities.Products;
using Logy.ImportExport.Importers;
using Logy.MwAgent.DotNetWikiBot;
using Site = Logy.MwAgent.DotNetWikiBot.Site;

namespace Logy.ImportExport.Bible
{
    public class DescendantOfAdamAndEveImporter : PersonImporter
    {
        public DescendantOfAdamAndEveImporter(Session session) : base(session)
        {
        }

        public override string Url
        {
            get { return EventsDb.DescendantsOfAdamAndEve; }
        }

        public override IList GetPages()
        {
            return DescendantOfAdamAndEve.ParseDescendants(new Page(Site.Wikipedia, Url), true);
        }

        /// <summary>
        /// DescendantOfAdamAndEve sent here are sorted by position in text
        /// </summary>
        public override void Import(object page, Job job)
        {
            var person = (DescendantOfAdamAndEve)page;
            var fm = new FamilyManager(this);

            // every wife in DescendantOfAdamAndEve has family with husband and managed through husband
            // her kids are in husband's family 
            if (!person.IsWife)
            {
                Link link;
                var pname = SaveDescendant(person, job, out link);
                var fatherOrKid = pname.Person;
                foreach (var wife in person.Wives)
                {
                    var wifeName = SaveDescendant(wife, job, out link);

                    fm.AddWife(job, fatherOrKid, wifeName, link, wife.RefName, wife.Ref2Name);
                }
                
                foreach (var kid in person.Kids)
                {
                    if (kid.Father == null)
                        throw new NullReferenceException(kid + "'s father is null");

                    var wifeName = SaveDescendant(kid.Mother, job, out link);
                    var familyLink = fm.AddFamily(job, fatherOrKid, wifeName.Person);

                    Link kidLink;
                    var kidName = SaveDescendant(kid, job, out kidLink);

                    if (kidLink == null)
                        kidLink = LinkManager.FindLink(
                            kidName,
                            BibleManager.GetDocumentUrl(kid.RefName),
                            BibleManager.GetDocumentNumber(kid.RefName));

                    fm.AddKid(job, kidName, familyLink.Family, kidLink, kid.GenerationNumberUnknown);
                }
            }
        }

        /// <returns>not null, because block.RefName is not null </returns>
        private PName SaveDescendant(DescendantOfAdamAndEve block, Job job, out Link personLink)
        {
            personLink = null;
            SaveDescendant(block, job, block.Ref2Name, ref personLink);
            return SaveDescendant(block, job, block.RefName, ref personLink);
        }

        private PName SaveDescendant(DescendantOfAdamAndEve block, Job job, string refName, ref Link docLink)
        {
            Link personLink;
            var pname = SavePersonName(block, job, PersonType.Human, out personLink, false);
            docLink = SaveRef(block, job, BibleManager.GetDocumentUrl(refName), personLink ?? docLink) ?? personLink;
            return pname;
        }

        private Link SaveRef(DescendantOfAdamAndEve block, Job job, string url, Link personLink)
        {
            Link docLink = null;
            if (!string.IsNullOrEmpty(url) && personLink != null)
            {
                Doc foundDoc = null;
                var parentDoc = job.Template.ParentDoc;
                if (parentDoc != null)
                    foundDoc = DocManager.FindByUrl(parentDoc, url);

                if (foundDoc == null)
                {
                    foundDoc = parentDoc != null ? new Doc(parentDoc) : new Doc(job.XpoSession);
                    foundDoc.Url = url;
                    foundDoc.Save();
                    ObjectAdded(foundDoc);
                }

                var number = BibleManager.GetDocumentNumber(block.RefName);
                docLink = foundDoc.NewLink(number);
                docLink.PName = personLink.PName;
                docLink.Job = job;
                if (docLink.DocPart != null)
                {
                    // to eliminate duplication
                    docLink.DocPart.Save();
                }
                personLink.PName = null; //// DEBUG, because personLink should not be saved to db

                ObjectAdded(docLink.Save());
            }

            return docLink;
        }
    }
}
