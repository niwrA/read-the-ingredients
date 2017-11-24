using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;

namespace WikiAccess
{
    /// <summary>
    /// Class to retrieve a Wikipedia page
    /// </summary>
    public class WikipediaIO : WikimediaApi
    {
        protected override string APIurl { get { return @"http://en.wikipedia.org/w/api.php?"; } }
        protected override string Parameters
        {
            get
            {
                string Param = "action=" + Action;
                if (Titles != "") Param += "&titles=" + WebUtility.UrlEncode(Titles);
                if (Format != "") Param += "&format=" + Format;
                if (Redirects != "") Param += "&redirects";
                if (Export != "") Param += "&export";
                if (ExportNoWrap != "") Param += "&exportnowrap";

                return Param;
            }
        }

        public List<string[]> TemplatesUsed;
        public List<string> CategoriesUsed;

        public string Article { get; set; }
        public string Action { get; set; }
        public string Titles { get; set; }
        public string Format { get; set; }
        public string Redirects { get; set; }
        public string Export { get; set; }
        public string ExportNoWrap { get; set; }
        public string PageTitle { get; set; }
        private WikipediaIOErrorLog WikipediaErrors { get; set; }

        public WikipediaIO()
            : base()
        {
            WikipediaErrors = new WikipediaIOErrorLog();
            TemplatesUsed = new List<string[]>();
            CategoriesUsed = new List<string>();
        }

        /// <summary>
        /// Download a page from Wikipedia and process
        /// </summary>
        /// <returns>True = success</returns>
        public bool GetData()
        {
            if (GrabPage())
            {
                if (ExtractXML())
                {
                    ExtractCategories();
                    ExtractTemplates();
                    return true;
                }
                else
                {
                    WikipediaErrors.UnableToParseXML();
                    return false;
                }
            }
            else
            {
                WikipediaErrors.UnableToRetrieveData();
                return false;
            }
        }

        /// <summary>
        /// Extract the article from the downloaded XML content
        /// </summary>
        /// <returns></returns>
        private bool ExtractXML()
        {
            bool WikipediaArticleExists = false;

            using (XmlReader Reader = XmlReader.Create(new StringReader(Content)))
            {
                string[] thisElementName = new string[5];
                while (Reader.Read())
                {
                    switch (Reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            thisElementName[Reader.Depth] = Reader.Name;
                            break;

                        case XmlNodeType.Text:
                            if (thisElementName[0] == "mediawiki")
                            {
                                if (thisElementName[1] == "page")
                                {
                                    if (thisElementName[2] == "title")
                                    {
                                        PageTitle = Reader.Value;
                                    }

                                    if (thisElementName[2] == "revision")
                                    {
                                        if (thisElementName[3] == "text")
                                        {
                                            WikipediaArticleExists = true;
                                            Article = RemoveHTMLcomments(ReplaceDash(RemoveTerminators(Reader.Value)));
                                        }
                                    }

                                }
                            }
                            break;
                    }
                }

                if (!WikipediaArticleExists)
                {
                    WikipediaErrors.ArticleNotExists();
                    return false;
                }
                else
                    return true;
            }
        }

        private string RemoveHTMLcomments(string OriginalText)
        {
            string ThisText = OriginalText;
            int CommentStart = 0;

            do
            {
                CommentStart = ThisText.IndexOf("<!--", StringComparison.Ordinal);
                if (CommentStart != -1)
                {
                    int CommentEnd = ThisText.IndexOf("-->", CommentStart + 3, StringComparison.Ordinal);

                    if (CommentEnd == -1 || CommentEnd < CommentStart)
                    {
                        WikipediaErrors.UnbalancedHTMLcomment();
                        return ThisText;
                    }
                    else
                    {
                        string Comment = ThisText.Substring(CommentStart, CommentEnd - CommentStart + 3);
                        ThisText = ThisText.Replace(Comment, "");
                    }
                }

            } while (CommentStart != -1);

            return ThisText;
        }

        private string RemoveTerminators(string originalText)
        {
            string newText = originalText;
            newText = newText.Replace("\u000a", string.Empty);
            newText = newText.Replace("\u000b", string.Empty);
            newText = newText.Replace("\u000c", string.Empty);
            newText = newText.Replace("\u000d", string.Empty);
            newText = newText.Replace("\u0085", string.Empty);
            newText = newText.Replace("\u2028", string.Empty);
            newText = newText.Replace("\u2029", string.Empty);
            return newText;
        }

        private string ReplaceDash(string original)
        {
            string Output = original;

            Output = Output.Replace("\u058A", "-");
            Output = Output.Replace("\u05BE", "-");
            Output = Output.Replace("\u1400", "-");
            Output = Output.Replace("\u1806", "-");
            Output = Output.Replace("\u2010", "-");

            Output = Output.Replace("\u2011", "-");
            Output = Output.Replace("\u2012", "-");
            Output = Output.Replace("\u2013", "-");
            Output = Output.Replace("\u2014", "-");
            Output = Output.Replace("\u2015", "-");

            Output = Output.Replace("\u2E17", "-");
            Output = Output.Replace("\u2E1A", "-");
            Output = Output.Replace("\u2E3A", "-");
            Output = Output.Replace("\u2E3B", "-");
            Output = Output.Replace("\u301C", "-");

            Output = Output.Replace("\u3030", "-");
            Output = Output.Replace("\u30A0", "-");
            Output = Output.Replace("\uFE31", "-");
            Output = Output.Replace("\uFE32", "-");
            Output = Output.Replace("\uFE58", "-");

            Output = Output.Replace("\uFE63", "-");
            Output = Output.Replace("\uFF0D", "-");

            return Output;
        }

        private void ExtractCategories()
        {
            int catStart = Article.IndexOf("[[Category:", StringComparison.Ordinal);

            while (catStart > 0)
            {
                int catNextPipe = Article.IndexOf("|", catStart, StringComparison.Ordinal);
                int catNextClose = Article.IndexOf("]]", catStart, StringComparison.Ordinal);
                int catFinish = 0;

                if (catNextPipe < catNextClose && catNextPipe > 0)
                {
                    catFinish = catNextPipe;
                }
                else
                {
                    catFinish = catNextClose;
                }

                if (catStart != -1 && catFinish != -1 && catFinish > catStart)
                {
                    CategoriesUsed.Add(Article.Substring(catStart + 11, catFinish - catStart - 11).ToLower().Trim());
                    catStart = Article.IndexOf("[[Category:", catFinish, StringComparison.Ordinal);
                }
                else
                {
                    WikipediaErrors.UnbalancedCategoryBrackets();
                    catStart = -99;
                }
            }
        }

        private void ExtractTemplates()
        {
            int TplStart = Article.IndexOf("{{", StringComparison.Ordinal);

            while (TplStart >= 0)
            {
                int TplNextPipe = Article.IndexOf("|", TplStart, StringComparison.Ordinal);
                int TplNextClose = Article.IndexOf("}}", TplStart, StringComparison.Ordinal);
                int TplFinish = 0;

                if (TplNextPipe < TplNextClose && TplNextPipe > 0)
                {
                    TplFinish = TplNextPipe;
                }
                else
                {
                    TplFinish = TplNextClose;
                }

                if (TplStart != -1 && TplFinish != -1 && TplFinish > TplStart)
                {
                    string[] thisTemplate = new string[2];
                    
                    thisTemplate[0] = Article.Substring(TplStart + 2, TplFinish - TplStart - 2).ToLower().Trim();
                    thisTemplate[1] = getFullTemplate(TplStart);
                        TemplatesUsed.Add(thisTemplate);


                    TplStart = Article.IndexOf("{{", TplFinish, StringComparison.Ordinal);
                }
                else
                {
                    WikipediaErrors.UnbalancedTemplateBrackets();
                    TplStart = -99;
                }
            }
        }
        /// <summary>
        /// Function to remove Wikipedia style [[links]]
        /// </summary>
        /// <param name="OriginalText"></param>
        /// <param name="RevisedText"></param>
        /// <returns></returns>
        public static bool DelinkText(string OriginalText, out string RevisedText)
        {
            string thisText = OriginalText;

            int linkStart = 0;

            // Look for first [[ and ]], replace them with text
            // repeat until no more [[ or ]]
            do
            {
                linkStart = thisText.IndexOf("[[", StringComparison.Ordinal);
                if (linkStart != -1)
                {
                    //Found start
                    int linkEnd = thisText.IndexOf("]]", StringComparison.Ordinal);

                    if (linkEnd == -1 || linkEnd < linkStart)
                    {
                        //Did not find close
                        RevisedText = OriginalText;
                        return false;
                    }
                    else
                    {
                        // Found link, extract text
                        string link = thisText.Substring(linkStart, linkEnd - linkStart + 2);
                        string newlink = link.Substring(2, link.Length - 4);

                        // If its piped, remove left side
                        int pipe = newlink.IndexOf("|", StringComparison.Ordinal);
                        if (pipe != -1)
                        {
                            newlink = newlink.Substring(pipe + 1);
                        }
                        // Replace [[link]] with newlink
                        thisText = thisText.Replace(link, newlink);
                    }
                }
                else
                {
                    //Did not find a [[, probably finished
                    //but first check there are no closing ]]
                    int linkClose = thisText.IndexOf("]]", StringComparison.Ordinal);
                    if (linkClose != -1)
                    {
                        RevisedText = OriginalText;
                        return false;
                    }
                }

            } while (linkStart != -1);

            RevisedText = thisText;
            return true;
        }

        public List<ErrorLog> GetErrors()
        {
            List<ErrorLog> Logs = new List<ErrorLog>();
            Logs.Add(APIErrors);
            Logs.Add(WikipediaErrors);
            return Logs;
        }

        private string getFullTemplate(int startpoint)
        {
            int Endpoint = 0;
            int LeftBraceCount = 0;
            int RightBraceCount = 0;

            /* There might be a template within the template, so we just cant search for next }}
            * instead I count all further occurances of {{ or }}, when they balance I have the full original template.
            * In case the template is {{{{Hello}}wave}} I do an i++ to avoid matching 2+3 bracket.  */
            for (int i = startpoint; i < Article.Length - 1; i++)
            {
                if (Article.Substring(i, 2) == "{{")
                {
                    LeftBraceCount++;
                    i++;
                }
                if (Article.Substring(i, 2) == "}}")
                {
                    RightBraceCount++;
                    i++;
                }

                if (LeftBraceCount == RightBraceCount)
                {
                    Endpoint = i;
                    break;
                }
            }

            string TemplateText = "";

            if (LeftBraceCount != RightBraceCount)
            {
                int PipePos = Article.IndexOf("|",startpoint);
                WikipediaErrors.UnableToExtractTemplate(Article.Substring(startpoint+2,PipePos-startpoint-2));
                return null;
            }

            if (DelinkText(Article.Substring(startpoint + 2, Endpoint - startpoint - 3), out TemplateText))
            {
                return TemplateText;
            }
            else
            {
                return null;
            }

        }
    }
}
