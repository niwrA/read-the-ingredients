using IngredientImagesShared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace IngredientImageImporterShared
{
    public class TaxonomyImporter
    {
        private IngredientImages.IIngredientImageRepository _repo;
        public TaxonomyImporter(IngredientImages.IIngredientImageRepository repo)
        {
            _repo = repo;
        }

        public class ValueObject
        {
            private string _value;
            public string cleanValue
            {
                get { return Value.Replace("http://www.wikidata.org/entity/Q", ""); }
                set { Value = value.Replace("http://www.wikidata.org/entity/Q", ""); }
            }
            [JsonProperty("value")]
            public string Value { get; set; }
            [JsonProperty("type")]
            public string Type { get; set; }
        }
        public class Results
        {
            [JsonProperty("bindings")]
            public List<WDImageTyped> Images { get; set; }
        }
        public class ResultSet
        {
            [JsonProperty("results")]
            public Results Results { get; set; }
        }

        public class WDImageTyped
        {
            [JsonProperty("ingredient")]
            public ValueObject WikiDataId { get; set; }
            [JsonProperty("pic")]
            public ValueObject Image { get; set; }
        }


        public async void ImportJson(string fileName)
        {
            bool isCloudLoadSucceeded = false;
            // todo: ask system if offline, to prevent having to timeout
            try
            {
                var wikiData = new WikiAccessFacadeShared.WikiAccessFacade();
                string json = await wikiData.GetFoodIngredientImages();
                var importResults = JsonConvert.DeserializeObject<ResultSet>(json);
                foreach (var result in importResults.Results.Images)
                {
                    if(result.Image!=null)
                    {
                        var state = _repo.CreateIngredientImageState();

                        state.Url = result.Image.Value;
                        state.WikiDataId = int.Parse(result.WikiDataId.cleanValue);
                        //Debug.WriteLine(state.WikiDataId.ToString());
                        _repo.UpdateIngredientImageState(state);
                    }
                }
                _repo.PersistChanges();
                // todo: persist retrieved data for offline situation
                isCloudLoadSucceeded = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // cloud load failed. Todo: user feedback?
            }
            if (!isCloudLoadSucceeded)
            {
                // load from file
                //string json = await loadFile(@"Import/" + language + "/" + fileName);
            }
        }
    }
}
