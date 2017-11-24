using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikiAccess
{
    class WikiAccessSample
    {
//        SELECT DISTINCT ?ingredient ?ingredientLabel ?unii ?alias WHERE {
// ?ingredient wdt:P652 ?unii.
//           OPTIONAL { ?ingredient skos:altLabel ?alias FILTER (LANG (?alias) = "nl").}
// SERVICE wikibase:label { 
//   bd:serviceParam wikibase:language "nl" .}    
//}
        static void Main(string[] args)
        {
            int Qcode = 15818798; 

            WikidataIO WIO = new WikidataIO();
            WIO.Action = "wbgetentities";
            WIO.Format = "json";
            WIO.Sites = "";
            WIO.Ids = Qcode;
            WIO.Props = "claims|descriptions|labels|sitelinks";
            WIO.Languages = "";
            WIO.ClaimsRequired = new string[5] { "P31", "P27", "P21", "P569", "P570" };

            WikidataFields Fields = WIO.GetData();

            //Console.WriteLine("-----Errors-----");
            List<ErrorLog> Errors = new List<ErrorLog>();
            Errors = WIO.GetErrors();

            foreach (ErrorLog thisLog in Errors)
            {
                if (thisLog != null)
                {
                    foreach (ErrorMessage Error in thisLog.Errors)
                    {
                        //Console.WriteLine(Error.ToString());
                    }
                }
            }


            if (Fields == null)
                return;

            string ThisName;
            if (!Fields.Labels.TryGetValue("en-gb", out ThisName))
                Fields.Labels.TryGetValue("en", out ThisName);

            string ThisDescription;
            if (!Fields.Description.TryGetValue("en-gb", out ThisDescription))
                Fields.Description.TryGetValue("en", out ThisDescription);

            string ThisWikipedia;
                Fields.WikipediaLinks.TryGetValue("enwiki", out ThisWikipedia);

            //Console.WriteLine(ThisName);
            //Console.WriteLine(ThisDescription);

            //Console.WriteLine("====================");


            WikipediaIO WPIO = new WikipediaIO();
            WPIO.Action = "query";
            WPIO.Export = "Yes";
            WPIO.ExportNoWrap = "Yes";
            WPIO.Format = "xml";
            WPIO.Redirects = "yes";
            WPIO.Titles = ThisWikipedia;

            if (WPIO.GetData())
            {
                List<string[]> Templates = WPIO.TemplatesUsed;
                List<string> Categories = WPIO.CategoriesUsed;

                //Console.WriteLine(WPIO.PageTitle);
                //Console.WriteLine(Templates.Count().ToString() + " templates");
                //Console.WriteLine(Categories.Count().ToString() + " categories");
            }

            List<ErrorLog> Errors2 = new List<ErrorLog>();
            Errors2 = WPIO.GetErrors();

            foreach (ErrorLog thisLog in Errors2)
            {
                if (thisLog != null)
                {
                    foreach (ErrorMessage Error in thisLog.Errors)
                    {
                        //Console.WriteLine(Error.ToString());
                    }
                }
            }
        }
    }
}
