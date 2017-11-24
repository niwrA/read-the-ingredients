using System.Collections.Generic;

namespace WikiAccess
{
    public interface IWikidataFields
    {
        IList<KeyValuePair<int, WikidataClaim>> Claims { get; set; }
        IDictionary<string, string> Description { get; set; }
        int Id { get; set; }
        IDictionary<string, string> Labels { get; set; }
        IDictionary<string, string> WikipediaLinks { get; set; }
    }
}