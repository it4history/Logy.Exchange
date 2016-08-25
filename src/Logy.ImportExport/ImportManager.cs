using System;
using System.Collections;
using System.Linq;

using DevExpress.Xpo;

using Logy.Entities;
using Logy.Entities.Documents;
using Logy.Entities.Engine;
using Logy.Entities.Model;
using Logy.Entities.Persons;
using Logy.ImportExport.Bible;
using Logy.ImportExport.Importers;
using Logy.MwAgent.DotNetWikiBot;

using Site = Logy.Entities.Model.Site;

namespace Logy.ImportExport
{
    public class ImportManager : SecureManager
    {
        public ImportManager(Parameter input) : base(input)
        {
        }

        public static void ImportDescendantsOfAdamAndEve()
        {
            var man = new ImportManager(new Parameter { Admin = true });
            var template = (from o in new XPQuery<ImportTemplate>(man.XpoSession)
                            where o.Url == ImportTemplate.DescendantsOfAdamAndEve
                            select o).Single();

            man.Import(man.XpoSession.GetObjectByKey<Site>(Wikipedia.Id), template);
        }

        public string Import(Site site, ImportTemplate importTemplate)
        {
            Person mover;
            var error = CheckUser(out mover);
            if (error != null)
                return error;

            var doc = new Doc(XpoSession)
                          {
                              Site = site,
                              ImportTemplate = importTemplate,
                              Parent = importTemplate.ParentDoc,
                              Url = Site.Url(site.BaseUrl, importTemplate.Url),
                              Type = DocType.Import,
                              AddedBy = UserCurrent,
                              Mover = mover,
                          };
            switch (doc.Type)
            {
                case DocType.Import:
                    switch (doc.ImportTemplate.SyncType)
                    {
                        case SyncType.ImportFromMW:
                            IList pages;
                            Importer importer = null;
                            var mediawikiSite = new MwAgent.DotNetWikiBot.Site(site.BaseUrl);
                            if (mediawikiSite.GetNamespace(importTemplate.Url) == MwAgent.DotNetWikiBot.Site.CategoryNS)
                            {
                                var pl = new PageList(mediawikiSite);
                                pl.FillFromCategoryTree(importTemplate.Url);
                                pages = pl.Pages;
                                switch (importTemplate.EntityType)
                                {
                                    case EntityType.Person:
                                        importer = new PersonImporter(doc);
                                        break;
                                    case EntityType.EventType:
                                        importer = new EventTypeImporter(doc);
                                        break;
                                }
                            }
                            else
                            {
                                pages = DescendantOfAdamAndEve.ParseDescendants(new Page(importTemplate.Url), true);
                                importer = new DescendantOfAdamAndEveImporter(doc);
                            }
                            
                            if (importer == null)
                                return "no importer";

                            XpoSession.BeginTransaction();
                            try
                            {
                                doc.Save();

                                foreach (var page in pages)
                                {
                                    importer.Import((ImportBlock)page);
                                    if (importer.ObjectsCreated.Count > 20000)
                                        break;
                                }

                                if (importTemplate.ParentDoc != null &&
                                    importTemplate.ParentDoc.Type == DocType.Common)
                                    CalcManager.StartCalculation(doc);

                                doc.ImportStatus = string.Format(
                                    "Objects Created: {0}",
                                    importer.ObjectsCreated.Count);
                                XpoSession.CommitTransaction();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                XpoSession.RollbackTransaction();
                            }

                            break;
                    }

                    break;
            }

            return null;
        }
    }
}
