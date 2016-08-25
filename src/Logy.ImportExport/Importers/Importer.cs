using System;

using DevExpress.Xpo;

using Logy.Entities.Documents;
using Logy.MwAgent.DotNetWikiBot;

using Skills.Xpo;

namespace Logy.ImportExport.Importers
{
    public abstract class Importer : ObjectsManager
    {
        private readonly string _language;

        private readonly Doc _doc;

        protected Importer(Doc doc)
        {
            _doc = doc;
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

        public override Session XpoSession
        {
            get { return _doc.XpoSession; }
        }

        /// <summary>
        /// called InTransaction
        /// </summary>
        public abstract void Import(ImportBlock page);
    }
}
