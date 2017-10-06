using DevExpress.Xpo;
using Logy.Entities.Import;
using Logy.Entities.Products;

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

        /// <inheritdoc />
        public override bool IsGroup { get { return false; } }
    }
}
