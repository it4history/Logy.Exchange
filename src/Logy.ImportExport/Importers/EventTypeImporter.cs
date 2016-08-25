using System;

using Logy.Entities.Documents;
using Logy.Entities.Events;
using Logy.MwAgent.DotNetWikiBot;

namespace Logy.ImportExport.Importers
{
    public class EventTypeImporter : Importer
    {
        public EventTypeImporter(Doc doc) : base(doc)
        {
        }

        public override void Import(ImportBlock page)
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
