using System;
using System.Collections;
using DevExpress.Xpo;
using Logy.Entities.Documents;
using Logy.Entities.Documents.Bible;
using Logy.Entities.Engine;
using Logy.Entities.Import;
using Logy.Entities.Links;
using Logy.Entities.Model;
using Logy.Entities.Persons;
using Logy.Entities.Products;
using Logy.MwAgent.DotNetWikiBot;

namespace Logy.ImportExport.Bible
{
    public class DescendantOfAdamAndEveImporter : DescendantImporter
    {
        public DescendantOfAdamAndEveImporter(Session session) : base(session, LogyEventsDb.LogyWikiId)
        {
        }

        public override string Url
        {
            get { return LogyEventsDb.DescendantsOfAdamAndEve; }
        }
    }
}
