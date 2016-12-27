using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Logy.Entities.Documents;
using Logy.Entities.Documents.Bible;
using Logy.Entities.Events;
using Logy.Entities.Persons;
using Logy.ImportExport.Importers;
using Logy.MwAgent;
using Logy.MwAgent.DotNetWikiBot;

namespace Logy.ImportExport.Bible
{
    public class DescendantOfAdamAndEveImporter : PersonImporter
    {
        public DescendantOfAdamAndEveImporter(Doc doc) : base(doc)
        {
        }

        public override IList GetPages()
        {
            return DescendantOfAdamAndEve.ParseDescendants(new Page(Doc.ImportTemplate.Url), true);
        }

        public override void Import(ImportBlock page)
        {
            var person = (DescendantOfAdamAndEve)page;

            // every wife in DescendantOfAdamAndEve has family with husband and managed through husband
            // her kids are in husband's family 
            if (!person.IsWife)
            {
                var fullFamilies = new Dictionary<Family, DescendantOfAdamAndEve>();
                Link link;
                var pname = SaveDescendant(person, out link);
                var fatherOrKid = pname.Person;
                Family family;
                foreach (var wife in person.Wives)
                {
                    var wifeName = SaveDescendant(wife, out link);
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

                        link.Family = family;
                        ObjectUpdated(link.Save());
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
                    var kidName = SaveDescendant(kid, out kidLink);
                    if (kidLink != null)
                        modified = true;

                    if (modified)
                    {
                        if (kidLink == null)
                            kidLink = LinkManager.FindLink(
                                kidName,
                                BibleManager.GetDocumentUrl(kid.RefName),
                                BibleManager.GetDocumentNumber(kid.RefName));
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
        private PName SaveDescendant(DescendantOfAdamAndEve block, out Link personLink)
        {
            personLink = null;
            SaveDescendant(block, block.Ref2Name, ref personLink);
            return SaveDescendant(block, block.RefName, ref personLink);
        }

        private PName SaveDescendant(DescendantOfAdamAndEve block, string refName, ref Link docLink)
        {
            Link personLink;
            var pname = SavePersonName(block, PersonType.Human, out personLink, false);
            docLink = SaveRef(block, BibleManager.GetDocumentUrl(refName), personLink ?? docLink) ?? personLink;
            return pname;
        }

        private Link SaveRef(DescendantOfAdamAndEve block, string url, Link personLink)
        {
            Link docLink = null;
            if (!string.IsNullOrEmpty(url) && personLink != null)
            {
                var foundDoc = DocManager.FindByUrl(Doc, url);
                if (foundDoc == null)
                {
                    foundDoc = new Doc(Doc) { Url = url };
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
