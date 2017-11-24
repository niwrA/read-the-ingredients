using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikiAccess
{
    /// <summary>
    /// Class to store errors pertaining to Wikipedia IO
    /// </summary>
    public class WikipediaIOErrorLog : ErrorLog
    {
        public string Module { get { return "W"; } }
        public List<ErrorMessage> Errors { get; set; }

        public WikipediaIOErrorLog()
        {
            Errors = new List<ErrorMessage>();
#if DEBUG
            Errors.Add(new ErrorMessage(Module, 0, "WikipediaIO module"));
#endif
        }

        public void UnableToRetrieveData()
        {
            Errors.Add(new ErrorMessage(Module, 1, "Unable to retrieve data"));
        }

        public void UnableToParseXML()
        {
            Errors.Add(new ErrorMessage(Module, 2, "Unable to parse XML"));
        }
        
        public void ArticleNotExists()
        {
            Errors.Add(new ErrorMessage(Module, 3, "Wikipedia article does not exist"));
        }

        public void UnbalancedHTMLcomment()
        {
            Errors.Add(new ErrorMessage(Module,4, "Unbalanced HTML comments in article"));
        }

        public void UnbalancedCategoryBrackets()
        {
            Errors.Add(new ErrorMessage(Module,5, "Unbalanced Category brackets"));
        }

        public void UnbalancedTemplateBrackets()
        {
            Errors.Add(new ErrorMessage(Module,6, "Unbalanced Template brackets"));
        }

        public void UnableToExtractTemplate(string templateName)
        {
            Errors.Add(new ErrorMessage(Module, 7, "Unable to extract template " + templateName));
        }
    }
}
