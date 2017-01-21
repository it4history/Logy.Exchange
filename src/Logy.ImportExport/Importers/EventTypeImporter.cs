using DevExpress.Xpo;
using Logy.Entities.Engine;
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

        public override void Import(object page, Job doc)
        {
            var p = (ImportBlock)page;
            var eventManager = new EventManager(this);
            EventType parent = null;
            foreach (var label in p.Categories)
            {
                var newParent = eventManager.FindType(label);
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

            eventManager.SaveEventType(p.TitleUnique, parent);
        }
    }
}
