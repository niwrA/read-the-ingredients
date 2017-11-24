using IngredientLanguageNamesShared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using System.IO;
using NutrientLanguageNameShared;
using Newtonsoft.Json;

namespace IngredientLanguageNameTaxonomyImporterShared
{
    public class TaxonomyImporter
    {
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
            public List<WDIngredientTyped> Ingredients { get; set; }
        }
        public class ResultSet
        {
            [JsonProperty("results")]
            public Results Results { get; set; }
        }
        public class WDIngredient
        {
            private string _wikiDataId;
            [JsonProperty("ingredient")]
            public string WikiDataId { get { return _wikiDataId; } set { _wikiDataId = value.Replace("http://www.wikidata.org/entity/Q", ""); } }
            [JsonProperty("ingredientLabel")]
            public string Label { get; set; }
            [JsonProperty("alias")]
            public string Alias { get; internal set; }
            public bool IsPreferred { get; internal set; }
        }
        public class WDIngredientTyped
        {
            [JsonProperty("ingredient")]
            public ValueObject WikiDataId { get; set; }
            [JsonProperty("ingredientLabel")]
            public ValueObject Label { get; set; }
            [JsonProperty("alias")]
            public ValueObject Alias { get; internal set; }
            public bool IsPreferred { get; internal set; }
        }


        private IngredientLanguageNames.IIngredientLanguageNameRepository _repo;
        public TaxonomyImporter(IngredientLanguageNames.IIngredientLanguageNameRepository repo)
        {
            _repo = repo;
        }
        public async void Import(string fileName)
        {
            string text = await loadFile(fileName);
            Guid euAdditiveGuid = Guid.NewGuid();
            int wikidata_id = 0;
            var reader = new StringReader(text);
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }

                if (string.IsNullOrEmpty(line))
                {
                    euAdditiveGuid = Guid.NewGuid();
                }

                if (line.Contains(":"))
                {
                    var parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        var language = parts[0];
                        var names = parts[1];
                        var namesList = names.Split(',');
                        if (language == "wikidata_id")
                        {
                            int.TryParse(names, out wikidata_id);
                        }
                        foreach (var name in namesList)
                        {
                            AddTranslation(euAdditiveGuid, language, name.Trim());
                        }
                    }
                }
            }
        }

        private Dictionary<string, Guid> WikiDataIdToGuid { get; set; } = new Dictionary<string, Guid>();
        private Dictionary<string, string> WikiDataIdToAlias { get; set; } = new Dictionary<string, string>();
        public async void ImportJson(string fileName, List<string> languages)
        {
            bool isCloudLoadSucceeded = false;
            // todo: ask system if offline, to prevent having to timeout
            try
            {
                var wikiData = new WikiAccessFacadeShared.WikiAccessFacade();
                foreach (var language in languages)
                {
                    string json = await wikiData.GetWikiDataFoodIngredients(language);
                    var importResults = JsonConvert.DeserializeObject<ResultSet>(json);
                    foreach (var result in importResults.Results.Ingredients)
                    {
                        Guid guid;
                        if (!WikiDataIdToGuid.TryGetValue(result.WikiDataId.cleanValue, out guid))
                        {
                            guid = Guid.NewGuid();
                            WikiDataIdToGuid.Add(result.WikiDataId.cleanValue, guid);
                        }
                        AddTranslation(guid, language, result);
                    }
                }
                _repo.PersistChanges();
                // todo: persist retrieved data for offline situation
                isCloudLoadSucceeded = true;
            }
            catch (Exception)
            {
                // cloud load failed. Todo: user feedback?
            }
            if(!isCloudLoadSucceeded)
            {
                // load from file
                //string json = await loadFile(@"Import/" + language + "/" + fileName);
            }
        }

        public static Encoding GetEncoding(byte[] bom)
        {
            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }
        private async Task<string> loadFile(string fileName)
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///" + fileName));
            var inputStream = await file.OpenReadAsync();

            IBuffer buffer = await FileIO.ReadBufferAsync(file);
            DataReader reader = DataReader.FromBuffer(buffer);
            byte[] fileContent = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(fileContent);
            string text = GetEncoding(new byte[4] { fileContent[0], fileContent[1], fileContent[2], fileContent[3] }).GetString(fileContent);
            return text;
        }

        private static async Task<System.IO.StringReader> LoadFile(string fileName)
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var uri = new Uri("ms-appx:///" + fileName);
            Windows.Storage.StorageFile sampleFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
            string text = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
            var reader = new System.IO.StringReader(text);
            return reader;
        }

        private void AddTranslation(Guid? ingredientGuid, string language, string name)
        {
            var state = _repo.CreateIngredientLanguageNameState();
            state.IngredientGuid = ingredientGuid.Value;
            state.Language = language.Trim();
            state.Name = name.Trim();
            _repo.UpdateIngredientLanguageNameState(state);
        }

        private void AddTranslation(Guid ingredientGuid, string language, WDIngredientTyped ingredient)
        {
            var state = _repo.CreateIngredientLanguageNameState();
            bool isAliasSaved = false;
            state.IngredientGuid = ingredientGuid;
            state.Language = language.Trim();

            state.WikiDataId = int.Parse(ingredient.WikiDataId.cleanValue);
            state.IsPreferred = false;
            if (ingredient.Alias!=null)
            {
                string name = ingredient.Alias.Value;
                state.Name = name.Trim();
                _repo.UpdateIngredientLanguageNameState(state);
                isAliasSaved = true;
            }

            string existingLabel;
            var labelUniqueCacheId = language + ingredient.WikiDataId.cleanValue + ingredient.Label.Value;
            if (!WikiDataIdToAlias.TryGetValue(labelUniqueCacheId, out existingLabel))
            {
                if(isAliasSaved)
                {
                    state = _repo.CreateIngredientLanguageNameState();
                    state.IngredientGuid = ingredientGuid;
                    state.Language = language.Trim();

                    state.WikiDataId = int.Parse(ingredient.WikiDataId.cleanValue);
                }

                WikiDataIdToAlias.Add(labelUniqueCacheId, ingredient.Label.Value);
                state.Name = ingredient.Label.Value;
                state.IsPreferred = true;
                _repo.UpdateIngredientLanguageNameState(state);
            }
        }
    }
}
