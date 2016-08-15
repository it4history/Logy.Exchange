using System.Collections.Generic;

using Logy.Entities.Documents;
using Logy.Entities.Events;
using Logy.MwAgent.DotNetWikiBot;

using Skills.Xpo;

namespace Logy.ImportExport.Importers
{
    public class EventTypeImporter : Importer
    {
        public EventTypeImporter(Doc doc, List<DbObject> objCreated)
            : base(doc, objCreated)
        {
        }

        public override void Import(ImportBlock page)
        {
            var eventManager = new EventManager(XpoSession);
            if (eventManager.FindByName(page.TitleShort) == null)
            {
                EventType parent = null;
                foreach (var category in page.Categories)
                {
                    var label = ImportBlock.GetShortTitle(category);
                    var newParent = eventManager.FindByName(label);
                    if (newParent == null)
                    {
                        parent = new EventType(XpoSession) { Label = label, Parent = parent };
                        ObjectsCreated.Add(parent.Save());
                    }
                    else
                    {
                        parent = newParent;
                    }
                }

                ObjectsCreated.Add(
                    new EventType(XpoSession) { Label = page.TitleShort, Parent = parent }.Save());
            }
        }
    }
}
