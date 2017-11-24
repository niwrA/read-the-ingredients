using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFoodFactsContract
{

    [JsonObject("nutriments")]
    public class OpenFoodFactsNutrimentsDTO
    {
        [JsonProperty("energy_100g")]
        public string Energy100g { get; set; }
        [JsonProperty("energy_unit")]
        public string EnergyUnit { get; set; }
        [JsonProperty("fat_100g")]
        public string Fat100g { get; set; }
        [JsonProperty("fat_unit")]
        public string FatUnit { get; set; }
        [JsonProperty("saturated-fat_100g")]
        public string SaturatedFat100g { get; set; }
        [JsonProperty("saturated-fat_unit")]
        public string SaturatedFatUnit { get; set; }
        [JsonProperty("carbohydrates_100g")]
        public string Carbohydrates100g { get; set; }
        [JsonProperty("carbohydrates_unit")]
        public string CarbohydratesUnit { get; set; }
        [JsonProperty("proteins_100g")]
        public string Proteins100g { get; set; }
        [JsonProperty("proteins_unit")]
        public string ProteinsUnit { get; set; }
        [JsonProperty("sugars_100g")]
        public string Sugars100g { get; set; }
        [JsonProperty("sugars_unit")]
        public string SugarsUnit { get; set; }
        [JsonProperty("salt_100g")]
        public string Salt100g { get; set; }
        [JsonProperty("salt_unit")]
        public string SaltUnit { get; set; }
        [JsonProperty("fiber_100g")]
        public string Fibers100g { get; set; }
        [JsonProperty("fiber_unit")]
        public string FibersUnit { get; set; }
    }

    [JsonObject("product")]
    public class OpenFoodFactsProductDTO
    {
        [JsonProperty("product_name")]
        public string Name { get; set; }
        [JsonProperty("product_name_nl")]
        public string NameNL { get; set; }
        [JsonProperty("product_name_en")]
        public string NameEN { get; set; }
        [JsonProperty("product_name_fr")]
        public string NameFR { get; set; }
        [JsonProperty("lang")]
        public string OriginalLanguage { get; set; }
        //{
        //    get
        //    {
        //        if (!string.IsNullOrWhiteSpace(IngredientsTextNL))
        //        {
        //            return "nl";
        //        }
        //        if (!string.IsNullOrWhiteSpace(IngredientsTextFR))
        //        {
        //            return "fr";
        //        }
        //        if (!string.IsNullOrWhiteSpace(IngredientsTextDE))
        //        {
        //            return "de";
        //        }
        //        if (!string.IsNullOrWhiteSpace(IngredientsTextES))
        //        {
        //            return "es";
        //        }
        //        if (!string.IsNullOrWhiteSpace(IngredientsTextEN))
        //        {
        //            return "en";
        //        }
        //        return "en";
        //    }
        //}


        [JsonProperty("quantity")]
        public string Quantity { get; set; }

        [JsonProperty("ingredients_text")]
        public string IngredientsText { get; set; }
        [JsonProperty("ingredients_text_nl")]
        public string IngredientsTextNL { get; set; }
        [JsonProperty("ingredients_text_en")]
        public string IngredientsTextEN { get; set; }
        [JsonProperty("ingredients_text_fr")]
        public string IngredientsTextFR { get; set; }
        [JsonProperty("ingredients_text_de")]
        public string IngredientsTextDE { get; set; }
        [JsonProperty("ingredients_text_es")]
        public string IngredientsTextES { get; set; }

        public string DisplayIngredients
        {
            get
            {
                var text = IngredientsTextNL;
                if (string.IsNullOrEmpty(text))
                {
                    text = IngredientsText;
                }
                if (string.IsNullOrEmpty(text))
                {
                    text = IngredientsTextEN;
                }
                return text;
            }
        }

        public string DisplayName
        {
            get
            {
                var name = NameNL;
                if (string.IsNullOrEmpty(name))
                {
                    name = Name;
                }
                if (string.IsNullOrEmpty(name))
                {
                    name = NameEN;
                }
                return name;
            }
        }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty("image_small_url")]
        public string ImageSmallUrl { get; set; }

        [JsonProperty("image_front_url")]
        public string ImageFrontUrl { get; set; }

        [JsonProperty("image_front_small_url")]
        public string ImageFrontSmallUrl { get; set; }

        //[JsonProperty("nutriments")]
        //public OpenFoodFactsNutrimentsDTO Nutriments { get; set; }

        [JsonProperty("nutriments")]
        public Dictionary<string, string> NutrimentsDictionary { get; set; }

        [JsonProperty("languages_codes")]
        public Dictionary<string, string> LanguagesDictionary { get; set; }

        public string DisplayImageUrl
        {
            get
            {
                var url = ImageFrontSmallUrl;
                if (string.IsNullOrEmpty(url))
                {
                    url = ImageSmallUrl;
                }
                return url;
            }
        }
    }
    public class OpenFoodFactsResultDTO
    {
        [JsonProperty("Status")]
        public int Status { get; set; }
        [JsonProperty("product")]
        public OpenFoodFactsProductDTO Product { get; set; }
    }
    public class OpenFoodFactsSearchResultDTO
    {
        [JsonProperty("page_size")]
        public int PageSize { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("skip")]
        public int Skip { get; set; }
        [JsonProperty("products")]
        public List<OpenFoodFactsProductDTO> Products { get; set; }
    }
}
