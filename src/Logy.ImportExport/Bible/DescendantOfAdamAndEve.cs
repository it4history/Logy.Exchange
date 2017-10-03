using Logy.Entities.Import;

namespace Logy.ImportExport.Bible
{
    public class DescendantOfAdamAndEve : Descendant
    {
        public override bool IsNotDuplicate
        {
            get
            {
                return Title == "wikipedia:Jochebed" ||
                       Title == "wikipedia:Sarah" // Jochebed, Sarah are women and duplicated twice
                       || Title == "wikipedia:Zerubbabel"; // Zerubbabel was mentioned twice }
            }
        }
    }
}
