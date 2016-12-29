using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using Logy.Entities.Documents;
using Logy.Entities.Documents.Bible;
using Logy.Entities.Engine;
using Logy.Entities.Events;
using Logy.Entities.Persons;
using Logy.Entities.Products;
using Logy.ImportExport.Importers;
using Logy.MwAgent;
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
            return DescendantOfAdamAndEve.ParseDescendants(new Page(new Site(Site.WikipediaBaseUrl), Url), true);
        }

        public override void Import(ImportBlock page, Job job)
        {
            var person = (DescendantOfAdamAndEve)page;

            // every wife in DescendantOfAdamAndEve has family with husband and managed through husband
            // her kids are in husband's family 
            if (!person.IsWife)
            {
                var fullFamilies = new Dictionary<Family, DescendantOfAdamAndEve>();
                Link link;
                var pname = SaveDescendant(person, job, out link);
                var fatherOrKid = pname.Person;
                Family family;
                foreach (var wife in person.Wives)
                {
                    var wifeName = SaveDescendant(wife, job, out link);
                    var mother = wifeName.Person;
                    family = FamilyManager.Find(fatherOrKid, mother);
                    if (family == null)
                    {
                        family = new Family(fatherOrKid) { Mother = mother };
                        ObjectAdded(family.Save());
                        if (link == null)
                            link = LinkManager.FindLink(
                                wifeName,
                                BibleManager.GetDocumentUrl(wife.RefName),
                                BibleManager.GetDocumentNumber(wife.RefName))
                                   ?? LinkManager.FindLink(
                                       wifeName,
                                       BibleManager.GetDocumentUrl(wife.Ref2Name),
                                       BibleManager.GetDocumentNumber(wife.Ref2Name));

                        // is bug, link should not be null
                        if (link != null) 
                        {
                            link.Family = family;
                            ObjectUpdated(link.Save());
                        }
                    }

                    fullFamilies.Add(family, wife);
                }

                foreach (var kid in person.Kids)
                {
                    if (kid.Father == null)
                        throw new NullReferenceException(kid + "'s father is null");

                    var modified = false;
                    if (kid.Mother != null)
                    {
                        // Family was already created
                        family = (from pair in fullFamilies
                                  where pair.Value == kid.Mother
                                  select pair.Key).Single();
                    }
                    else
                    {
                        family = FamilyManager.Find(fatherOrKid);
                        if (family == null)
                        {
                            family = new Family(fatherOrKid);
                            ObjectAdded(family.Save());
                            modified = true;
                        }
                    }

                    Link kidLink;
                    var kidName = SaveDescendant(kid, job, out kidLink);
                    if (kidLink != null)
                        modified = true;

                    if (modified)
                    {
                        if (kidLink == null)
                            kidLink = LinkManager.FindLink(
                                kidName,
                                BibleManager.GetDocumentUrl(kid.RefName),
                                BibleManager.GetDocumentNumber(kid.RefName));

                        if (link == null) // is bug, link should not be null
                            break;

                        kidLink.Family = family;

                        var eventManager = new EventManager(XpoSession);
                        kidLink.Event = new Event(
                            eventManager.SaveEventType(kid.GenerationNumberUnknown
                                ? EventType.BirthInUnknownGeneration
                                : EventType.Birth),
                            family,
                            kidName.Person);
                        kidLink.Save();
                        ObjectAdded(kidLink.Event);
                    }
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
                var parentDoc = job.ImportTemplate.ParentDoc;
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
                personLink.PName = null; //// DEBUG, because personLink should not be saved to db

                ObjectAdded(docLink.Save());
            }

            return docLink;
        }
    }
}
