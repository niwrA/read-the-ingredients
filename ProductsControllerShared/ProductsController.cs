using IngredientLanguageNamesShared;
using OpenFoodFactsContract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using IngredientsParserShared;
using EUAdditiveLanguageNamesShared;
using IngredientShared;
using System.Collections.ObjectModel;
using NutrientLanguageNameShared;
using UserSettingsShared;
using UserSettingsControllerShared;
using IngredientImagesShared;

namespace ProductsControllerShared
{
    public interface IMainRepository : IngredientLanguageNames.IIngredientLanguageNameRepository, EUAdditiveLanguageNames.IEUAdditiveLanguageNameRepository, NutrientLanguageNames.INutrientLanguageNameRepository, IUserSettingRepository, IngredientImages.IIngredientImageRepository
    {

    }
    public interface IWikiDataUrl
    {
        Uri Url { get; set; }
    }
    public interface IWikiDataRepository
    {
        IEnumerable<IWikiDataUrl> GetWikiDataImages(int Id);
    }
    public class ProductsController : INotifyPropertyChanged
    {
        private IMainRepository _repo { get; set; }
        private IWikiDataRepository _wikiDataRepo { get; set; }
        private UserSettingsController _userSettingsController;
        public ProductsController(IMainRepository repo, IList<string> languages, IWikiDataRepository wikiDataRepo)
        {
            _repo = repo;
            _wikiDataRepo = wikiDataRepo;
            _userSettingsController = new UserSettingsController(new UserSettings(repo), languages);
            SupportedLanguages = languages;
            LanguageCode = _userSettingsController.UserLanguage;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _languageCode = "en";
        public string LanguageCode
        {
            get { return _languageCode; }
            set
            {
                _languageCode = value;
                OnPropertyChanged();
                //Translate();
            }
        }

        private ProductViewModel _product;
        public ProductViewModel Product
        {
            get { return _product; }
            set
            {
                _product = value;
                OnPropertyChanged();
                //Translate();
            }
        }

        // todo: move processing to separate object
        #region processing 
        private void TranslateIngredient(IngredientLanguageNames translator, string name, string displayName, IngredientViewModel ingredient)
        {
            var nutrientTranslations = translator.TranslateIngredient(name, Product.OriginalLanguage, Product.LanguageCode);
            if (nutrientTranslations.Any())
            {
                displayName = nutrientTranslations.First().Name;
            }
            ingredient.DisplayName = displayName;
        }

        internal void ParseIngredients()
        {
            var parser = new IngredientsParser();
            List<IngredientsParser.ParsedAnnotation> annotations = new List<IngredientsParser.ParsedAnnotation>();
            var _parsedIngredients = parser.Parse(_currentProduct.DisplayIngredients, annotations);

            var translator = new IngredientLanguageNames(_repo);
            var nutrientTranslator = new NutrientLanguageNames(_repo);
            var euNumbers = new EUAdditiveLanguageNames(_repo);

            foreach (var parsedIngredient in _parsedIngredients.Ingredients)
            {
                Product.Ingredients.Add(new IngredientViewModel(parsedIngredient, Product));
            }
            var additives = DetectEUNumbers(_parsedIngredients.Ingredients, euNumbers).OrderBy(w => w.EUNumber);
            foreach (var additive in additives)
            {
                Product.EUAdditives.Add(new EUAdditiveViewModel(additive.EUNumber, additive.DisplayName));
            }
            var allergens = CollectAllergens(_parsedIngredients.Ingredients);
            foreach (var allergen in allergens)
            {
                Product.Allergens.Add(new AllergenViewModel(allergen.Name, allergen.DisplayName));
            }
            ProcessNutrients();
            Product.Translate();
        }

        internal void GetDetails(IngredientViewModel ingredient)
        {
            var ingredientImages = new IngredientImages(_repo);
            var result = ingredientImages.GetIngredientImages(ingredient.WikiDataId);
            foreach (var url in result)
            {
                var uri = new Uri(url.Url);
                ingredient.ImageUrl = uri;
            }
        }

        private void ProcessNutrients()
        {
            if (_currentProduct.NutrimentsDictionary != null)
            {
                var subcategoryNames = new List<string> { "saturaged-fat" };
                //var nutrients = new List<NutrientViewModel>();
                foreach (var item in _currentProduct.NutrimentsDictionary)
                {
                    if (!item.Key.Contains("sodium") && !item.Key.Contains("score"))
                    {
                        if (item.Key.EndsWith("_100g"))
                        {
                            // get unit
                            var name = item.Key.Replace("_100g", "");
                            var displayName = name;
                            var unitKey = item.Key.Replace("_100g", "_unit");
                            string unitValue = "";
                            bool subCategory = false;
                            _currentProduct.NutrimentsDictionary.TryGetValue(unitKey, out unitValue);
                            if (subcategoryNames.Contains(item.Key))
                            {
                                subCategory = true;
                            }
                            var nutrientViewModel = new NutrientViewModel(item.Key, item.Value, unitValue, subCategory, 1);
                            //displayName = TranslateNutrient(nutrientTranslator, name, displayName, nutrientViewModel);
                            //nutrients.Add(nutrientViewModel);
                            Product.Nutrients.Add(nutrientViewModel);
                        }
                    }
                }
            }

        }

        private string TranslateNutrient(NutrientLanguageNames nutrientTranslator, string name, string displayName, NutrientViewModel nutrientViewModel)
        {
            var nutrientTranslations = nutrientTranslator.TranslateNutrient(name, "label", Product.LanguageCode);
            int order = 1;
            if (nutrientTranslations.Any())
            {
                displayName = nutrientTranslations.First().Name;
                order = nutrientTranslations.First().Order;
            }
            nutrientViewModel.DisplayName = displayName;
            return displayName;
        }

        private void TranslateIngredients(IEnumerable<IngredientsParser.ParsedIngredient> ingredients, IngredientLanguageNames translator)
        {
            if (ingredients != null)
            {
                foreach (var ingredient in ingredients)
                {
                    ingredient.DisplayName = GetFirstTranslation(translator, ingredient.Name);
                    TranslateIngredients(ingredient.Ingredients, translator);
                }
            }
        }
        private IEnumerable<EUAdditiveViewModel> DetectEUNumbers(IEnumerable<IngredientsParser.ParsedIngredient> ingredients, EUAdditiveLanguageNames euNumbers)
        {
            var detected = new List<EUAdditiveViewModel>();

            if (ingredients != null)
            {
                foreach (var ingredient in ingredients)
                {
                    var euNumber = DetectEUNumber(ingredient, euNumbers);
                    if (euNumber != null)
                    {
                        //Product.EUAdditives.Add(euNumber); // todo: one or the other - this is nice for live detection
                        detected.Add(euNumber);
                    }
                    detected.AddRange(DetectEUNumbers(ingredient.Ingredients, euNumbers));
                }
            }
            return detected;
        }

        private IEnumerable<AllergenViewModel> CollectAllergens(IEnumerable<IngredientsParser.ParsedIngredient> ingredients)
        {
            var detected = new List<AllergenViewModel>();

            if (ingredients != null)
            {
                foreach (var ingredient in ingredients)
                {
                    foreach (var allergen in ingredient.Allergens)
                    {
                        detected.Add(new AllergenViewModel(allergen, allergen));
                    }

                    var allergens = CollectAllergens(ingredient.Ingredients);
                    detected.AddRange(allergens);
                }
            }
            return detected;
        }

        private EUAdditiveViewModel DetectEUNumber(IngredientsParser.ParsedIngredient ingredient, EUAdditiveLanguageNames euNumbers)
        {
            var results = euNumbers.GetEUAdditiveNumber(ingredient.Name, Product.LanguageCode);
            if (results.Count() == 1)
            {
                return new EUAdditiveViewModel(results.First().EUNumber, results.First().Name);
            }
            return null;
        }

        private string GetFirstTranslation(IngredientLanguageNames translator, string name)
        {
            // todo: from product ingredient language to selected language
            var translations = translator.TranslateIngredient(name, Product.OriginalLanguage, Product.LanguageCode);
            if (translations.Any())
            {
                return translations.First().Name;
            }
            return name.ToLower();
        }
        #endregion

        public void SetProduct(OpenFoodFactsProductDTO product, string barcode)
        {
            _currentProduct = product;
            LanguageCode = product.OriginalLanguage;
            Product = new ProductViewModel(product, _repo, barcode);
        }

        private OpenFoodFactsProductDTO _currentProduct;

        public IList<string> SupportedLanguages { get; internal set; }

        public void NextLanguage()
        {
            var current = LanguageCode;
            var i = SupportedLanguages.IndexOf(current) + 1;
            if (i >= SupportedLanguages.Count) { i = 0; }
            LanguageCode = SupportedLanguages[i];
            if (Product != null)
            {
                Product.LanguageCode = SupportedLanguages[i];
            }
        }

        internal void SetNewProduct(string detectedBarcode, string path)
        {
            var newProduct = new ProductViewModel(detectedBarcode, path);
            Product = newProduct;
            
        }
    }
}