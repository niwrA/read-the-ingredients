using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WikiAccess
{
    /// <summary>
    /// General interface to Wikidata
    /// </summary>
    public class WikidataIO : WikimediaApi
    {
        public async Task<string> GetSPARQL(string query)
        {
            var wikiClient = new HttpClient();

            var encoded = Uri.EscapeDataString(query);
            string fullURL = _SPARQLurl + encoded + "&format=json";

            string httpResponseBody = "";

            try
            {
                //Send the GET request
                var httpResponse = await wikiClient.GetAsync(fullURL);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }

            return httpResponseBody;
        }

        protected override string APIurl { get { return @"http://www.Wikidata.org/w/api.php?"; } }
        private string _SPARQLurl { get { return @"https://query.wikidata.org/sparql?query="; } }
        protected override string Parameters
        {
            get
            {
                string Param = "action =" + Action;
                if (Format != "") Param += "&format=" + Format;
                if (Sites != "") Param += "&sites=" + Sites;
                if (Sitefilter != "") Param += "&sitefilter=" + Sitefilter;
                if (Ids != 0) Param += "&ids=Q" + Ids.ToString();
                if (Props != "") Param += "&props=" + Props;
                if (Languages != "") Param += "&languages=" + Languages;
                if (Search != "") Param += "&search=" + Search;

                return Param;
            }
        }
        private WikidataIOErrorLog WikidataErrors { get; set; }
        private ErrorLog ExternalErrors { get; set; }

        public string Action { get; set; }
        public string Format { get; set; }
        public string Sites { get; set; }
        public string Sitefilter { get; set; }
        public int Ids { get; set; }
        public string Props { get; set; }
        public string Languages { get; set; }
        public string Search { get; set; }
        public string[] ClaimsRequired { get; set; }

        public WikidataIO()
            : base()
        {
            WikidataErrors = new WikidataIOErrorLog();
        }

        public WikidataFields GetData()
        {
            if (GrabPage())
            {
                WikidataExtract Item = new WikidataExtract(Content, ClaimsRequired);
                ExternalErrors = Item.WikidataExtractErrors;
                if (Item.Success)
                    return Item.Fields;
                else
                    return null;
            }
            else
            {
                WikidataErrors.UnableToRetrieveData();
                return null;
            }
        }

        public List<ErrorLog> GetErrors()
        {
            List<ErrorLog> Logs = new List<ErrorLog>();
            Logs.Add(APIErrors);
            Logs.Add(WikidataErrors);
            Logs.Add(ExternalErrors);
            return Logs;
        }
    }
}
