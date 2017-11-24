using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikiAccess
{
    public class WikidataExtractErrorLog : ErrorLog
    {
        public string Module {get {return "E";}}
        public List<ErrorMessage> Errors { get; set; }

        public WikidataExtractErrorLog()
        {
            Errors = new List<ErrorMessage>();
#if DEBUG
            Errors.Add(new ErrorMessage(Module, 0, "WikidataExtract module"));
#endif
        }

        public void NotWikidata()
        {
            Errors.Add(new ErrorMessage(Module,1,"Download not in expected format"));
        }

        public void QcodeNotExist(string qcode)
        {
            Errors.Add(new ErrorMessage(Module, 2, qcode + " not found on Wikidata"));
        }

    }
}
