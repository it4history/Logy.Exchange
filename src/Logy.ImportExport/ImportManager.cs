using System.Collections;
using System.Collections.Generic;

using Logy.Entities;
using Logy.Entities.Documents;
using Logy.Entities.Engine;
using Logy.Entities.Model;
using Logy.Entities.Persons;
using Logy.ImportExport.Bible;
using Logy.ImportExport.Importers;
using Logy.MwAgent.DotNetWikiBot;

using Skills.Xpo;

namespace Logy.ImportExport
{
    public class ImportManager : SecureManager
    {
        public ImportManager(Parameter input) : base(input)
        {
        }

        public string Import(Wiki wiki, ImportTemplate importTemplate)
        {
            Person mover;
            var error = CheckUser(out mover);
            if (error != null)
                return error;

            var doc = new Doc(XpoSession)
                          {
                              Wiki = wiki,
                              ImportTemplate = importTemplate,
                              Parent = importTemplate.ParentDoc,
                              Url = Wiki.Url(wiki.BaseUrl, importTemplate.Url),
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
                            var objCreated = new List<DbObject>();
                            Importer importer = null;
                            var site = new Site(wiki.BaseUrl);
                            if (site.GetNamespace(importTemplate.Url) == Site.CategoryNS)
                            {
                                var pl = new PageList(site);
                                pl.FillFromCategoryTree(importTemplate.Url);
                                pages = pl.Pages;
                                switch (importTemplate.EntityType)
                                {
                                    case EntityType.Person:
                                        importer = new PersonImporter(doc, objCreated);
                                        break;
                                    case EntityType.EventType:
                                        importer = new EventTypeImporter(doc, objCreated);
                                        break;
                                }
                            }
                            else
                            {
                                pages = DescendantOfAdamAndEve.ParseDescendants(new Page(importTemplate.Url));
                                importer = new DescendantOfAdamAndEveImporter(doc, objCreated);
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
                                }

                                if (importTemplate.ParentDoc != null &&
                                    importTemplate.ParentDoc.Type == DocType.Common)
                                {
                                    CalcManager.StartCalculation(doc);
                                }

                                doc.ImportStatus = string.Format(
                                    "Objects Created: {0}",
                                    objCreated.Count);
                                XpoSession.CommitTransaction();
                            }
                            catch
                            {
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
