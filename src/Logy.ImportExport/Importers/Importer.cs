using System.Collections.Generic;

using DevExpress.Xpo;

using Logy.Entities.Documents;
using Logy.MwAgent.DotNetWikiBot;

using Skills.Xpo;

namespace Logy.ImportExport.Importers
{
    public abstract class Importer
    {
        private readonly string _language;

        private readonly Doc _doc;

        private readonly List<DbObject> _objectsCreated;
        
        protected Importer(Doc doc, List<DbObject> objectsCreated)
        {
            _doc = doc;
            _objectsCreated = objectsCreated;
            _language = doc.Site.MainLanguage;
        }

        public string Language
        {
            get { return _language; }
        }

        public Doc Doc
        {
            get { return _doc; }
        }

        public List<DbObject> ObjectsCreated
        {
            get { return _objectsCreated; }
        }

        protected Session XpoSession
        {
            get { return _doc.XpoSession; }
        }

        /// <summary>
        /// called InTransaction
        /// </summary>
        public abstract void Import(ImportBlock page);
    }
}
