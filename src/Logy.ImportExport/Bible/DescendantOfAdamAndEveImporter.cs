﻿using System;
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
                var fatherOrKid = SaveDescendant(person, job).PName.Person;
                foreach (var wife in person.Wives)
                {
                    var link = SaveDescendant(wife, job);
                    fm.AddFamily(job, fatherOrKid, link.PName.Person);
                }
                
                foreach (var kid in person.Kids)
                {
                    if (kid.Father == null)
                        throw new NullReferenceException(kid + "'s father is null");

                    var link = SaveDescendant(kid.Mother, job);
                    var familyLink = fm.AddFamily(job, fatherOrKid, link.PName.Person);

                    link = SaveDescendant(kid, job);
                    fm.AddKid(job, link.PName, familyLink.Family, link, kid.GenerationNumberUnknown);
                }
            }
        }

        /// <returns>Link.PName - not null, and because block.RefName is not null as well</returns>
        private Link SaveDescendant(DescendantOfAdamAndEve block, Job job)
        {
            if (!string.IsNullOrEmpty(block.Ref2Name))
                SaveDescendant(block, job, block.Ref2Name);
            return SaveDescendant(block, job, block.RefName);
        }

        private Link SaveDescendant(DescendantOfAdamAndEve block, Job job, string refName)
        {
            var url = BibleManager.GetDocumentUrl(refName);
            Link nameLink;
            var name = SavePersonName(block, job, PersonType.Human, out nameLink, false);
            if (string.IsNullOrEmpty(url))
                return (Link)nameLink.Save();
            return SaveRef(block, job, url, name);
        }

        private Link SaveRef(DescendantOfAdamAndEve block, Job job, string url, PName name)
        {
            Doc foundDoc = null;
            var parentDoc = job.Template.ParentDoc;
            if (parentDoc != null)
                foundDoc = DocManager.FindByUrl(parentDoc, url);

            if (foundDoc == null)
            {
                foundDoc = parentDoc != null ? new Doc(parentDoc) : new Doc(job.XpoSession);
                foundDoc.Url = url;
                ObjectAdded(foundDoc.Save());
            }

            var number = BibleManager.GetDocumentNumber(block.RefName);
            var docLink = LinkManager.FindLink(name, job.Url, url, number);
            if (docLink == null)
            {
                docLink = foundDoc.NewLink(number);
                docLink.PName = name;
                docLink.Job = job;
                ObjectAdded(docLink.Save());
            }

            return docLink;
        }
    }
}
