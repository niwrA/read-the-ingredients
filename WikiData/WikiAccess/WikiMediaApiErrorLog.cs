using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikiAccess
{
    public class WikiMediaApiErrorLog : ErrorLog
    {
        public string Module {get {return "A";}}
        public List<ErrorMessage> Errors { get; set; }

        public WikiMediaApiErrorLog()
        {
            Errors = new List<ErrorMessage>();
#if DEBUG
            Errors.Add(new ErrorMessage(Module, 0, "WikimediaAPI module"));
#endif
        }

        /// <summary>
        /// Web server is not contactable. Either no Internet or an invalid URL
        /// </summary>
        public void CannotAccessWiki(string url,string systemMessage)
        {
            Errors.Add(new ErrorMessage(Module, 1, "Unable to contact Wiki URL " + url,systemMessage));
        }

        /// <summary>
        /// No file was grabbed from the internet page. Unknown reason.
        /// </summary>
        public void NoFileDownloaded()
        {
            Errors.Add(new ErrorMessage(Module,2,"No file downloaded"));
        }

        public void UnableToRetrieveDownload(string systemMessage)
        {
            Errors.Add(new ErrorMessage(Module, 3, "Unable to retrieve Download", systemMessage));
        }
    }
}
