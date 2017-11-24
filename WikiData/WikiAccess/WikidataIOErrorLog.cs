using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikiAccess
{
    public class WikidataIOErrorLog : ErrorLog
    {
        public string Module {get {return "D";}}
        public List<ErrorMessage> Errors { get; set; }

        public WikidataIOErrorLog()
        {
            Errors = new List<ErrorMessage>();
#if DEBUG
            Errors.Add(new ErrorMessage(Module, 0, "WikidataIO module"));
#endif
        }

        public void UnableToRetrieveData()
        {
            Errors.Add(new ErrorMessage(Module,1,"Unable to retrieve data"));
        }

      
    
    }
}
