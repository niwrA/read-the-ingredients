using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WikiAccessFacadeShared
{
    public class WikiAccessFacade : IWikiAccessFacade
    {
        public WikiAccess.IWikidataFields GetWikiDataItem(int id)
        {
            var io = new WikiAccess.WikidataIO();
            io.Action = "wbgetentities";
            io.Format = "json";
            io.Sites = "";
            io.Ids = id;
            io.Props = "claims|descriptions|labels|sitelinks";
            io.Languages = "";
            //            io.ClaimsRequired = new string[5] { "P31", "P27", "P21", "P569", "P570" };

            var fields = io.GetData();
            return fields;
        }

        public async Task<string> GetWikiDataImages(int id)
        {
            var io = new WikiAccess.WikidataIO();
            var query = @"SELECT ?pic WHERE { wd:Q" + id.ToString() + " wdt:P18 ?pic }";
            var result = await io.GetSPARQL(query);
            return result;
        }

        public async Task<string> GetFoodIngredientImages()
        {
            var io = new WikiAccess.WikidataIO();
            var query = @"SELECT ?ingredient ?pic WHERE { ?ingredient wdt:P31 wd:Q27643250 . ?ingredient wdt:P18 ?pic }";
            var result = await io.GetSPARQL(query);
            return result;
            
        }

        public async Task<string> GetWikiDataFoodIngredients(string languageCode)
        {
            var io = new WikiAccess.WikidataIO();
            var query = @"SELECT ?ingredient ?ingredientLabel ?alias WHERE{ ?ingredient wdt:P31 wd:Q27643250 OPTIONAL { ?ingredient skos:altLabel ?alias FILTER (LANG (?alias) = '" + languageCode + "') } SERVICE wikibase:label { bd:serviceParam wikibase:language '" + languageCode + "' }}";
            var result = await io.GetSPARQL(query);
            return result;
        }

        public async Task<string> GetWikiDataIngredientENumbers(string languageCode)
        {
            var io = new WikiAccess.WikidataIO();
            var query = @"SELECT ?ingredient ?ingredientLabel ?alias WHERE{ ?ingredient wdt:P31 wd:Q27643250 . ?ingredient wdt:P628 ?enumber SERVICE wikibase:label { bd:serviceParam wikibase:language '" + languageCode + "' }}";
            var result = await io.GetSPARQL(query);
            return result;
        }
    }
}
