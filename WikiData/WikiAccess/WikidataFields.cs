using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikiAccess
{
    /// <summary>
    /// Container to hold a single item grabbed from Wikidata
    /// </summary>
    public class WikidataFields : IWikidataFields
    {
        public int Id { get; set; }
        public IDictionary<string, string> WikipediaLinks { get; set; }
        public IDictionary<string, string> Labels { get; set; }
        public IDictionary<string, string> Description { get; set; }
        public IList<KeyValuePair<int, WikidataClaim>> Claims { get; set; }  // Cannot use Dictionary as can have multiple claims per item

        public WikidataFields()
        {
            WikipediaLinks = new Dictionary<string, string>();
            Labels = new Dictionary<string, string>();
            Description = new Dictionary<string, string>();
            Claims = new List<KeyValuePair<int, WikidataClaim>>();
        }
    }
}
