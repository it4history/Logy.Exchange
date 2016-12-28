using DevExpress.Xpo;
using Logy.Entities.Documents;
using Logy.Entities.Events;
using Logy.Entities.Import;
using Logy.Entities.Model;
using Logy.MwAgent;

namespace Logy.ImportExport.Importers
{
    public class EventTypeImporter : CategoryImporter
    {
        public EventTypeImporter(Session session) : base(session)
        {
        }

        public override EntityType EntityType
        {
            get { return EntityType.EventType; }
        }

        public override void Import(ImportBlock page, Doc doc)
        {
            var eventManager = new EventManager(XpoSession);
            EventType parent = null;
            foreach (var label in page.Categories)
            {
                var newParent = eventManager.FindByName(label);
                if (newParent == null)
                {
                    parent = new EventType(XpoSession) { Label = label, Parent = parent };
                    ObjectAdded(parent.Save());
                }
                else
                {
                    parent = newParent;
                }
            }

            eventManager.SaveEventType(page.TitleUnique, parent);
        }
    }
}
