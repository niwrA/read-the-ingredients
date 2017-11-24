using IngredientLanguageNamesShared;
using IngredientsParserShared;
using NutrientLanguageNameShared;
using OpenFoodFactsContract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ProductsControllerShared
{
    public abstract class NotifyPropertyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ProductViewModel : NotifyPropertyBase
    {
        private OpenFoodFactsProductDTO _offProduct;
        private IMainRepository _repo;
        public ProductViewModel(string barcode, string barcodePath)
        {
            _offProduct = new OpenFoodFactsProductDTO { };
            Barcode = barcode;
            BarcodePath = barcodePath;
            if (!string.IsNullOrWhiteSpace(OriginalLanguage))
            {
                LanguageCode = OriginalLanguage;
            }
        }
        public ProductViewModel(OpenFoodFactsProductDTO offProduct, IMainRepository repo, string barcode)
        {
            _offProduct = offProduct;
            _repo = repo;
            Barcode = barcode;
            Ingredients = new ObservableCollection<IngredientViewModel>();
            Nutrients = new ObservableCollection<NutrientViewModel>();
            EUAdditives = new ObservableCollection<EUAdditiveViewModel>();
            Allergens = new ObservableCollection<AllergenViewModel>();
            if (!string.IsNullOrWhiteSpace(OriginalLanguage))
            {
                LanguageCode = OriginalLanguage;
            }

        }

        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_offProduct.DisplayName))
                {
                    return "openfoodfact";
                }
                return _offProduct.DisplayName;
            }
        }
        public string Quantity { get { return _offProduct.Quantity; } }
        public string ImageUrl { get { return _offProduct.DisplayImageUrl; } }

        private ObservableCollection<IngredientViewModel> _ingredients;
        public ObservableCollection<IngredientViewModel> Ingredients { get { return _ingredients; } set { _ingredients = value; OnPropertyChanged(); } }

        private ObservableCollection<EUAdditiveViewModel> _euAdditives;
        public ObservableCollection<EUAdditiveViewModel> EUAdditives { get { return _euAdditives; } set { _euAdditives = value; OnPropertyChanged(); } }

        private ObservableCollection<AllergenViewModel> _allergens;
        public ObservableCollection<AllergenViewModel> Allergens { get { return _allergens; } set { _allergens = value; OnPropertyChanged(); } }

        private ObservableCollection<NutrientViewModel> _nutrients;
        public ObservableCollection<NutrientViewModel> Nutrients { get { return _nutrients; } set { _nutrients = value; OnPropertyChanged(); } }

        private string _languageCode = "nl"; // todo: get from product found
        public string LanguageCode
        {
            get { return _languageCode; }
            set
            {
                _languageCode = value;
                OnPropertyChanged();
                Translate();
            }
        }

        public string Barcode { get; private set; }
        public string FormattedBarcode
        {
            get
            {
                if (Barcode.Length == 8) { return Barcode.Substring(0, 4) + " " + Barcode.Substring(4); }
                else if (Barcode.Length == 13)
                {
                    return Barcode.Substring(0, 1) + " " + Barcode.Substring(1, 6) + " " + Barcode.Substring(7);
                }
                return Barcode;
            }
        }


        private string _nutrientsTitle = "";
        public string NutrientsTitle
        {
            get { return _nutrientsTitle; }
            set { _nutrientsTitle = value; OnPropertyChanged(); }
        }

        private string _ingredientsTitle = "";
        public string IngredientsTitle
        {
            get { return _ingredientsTitle; }
            set { _ingredientsTitle = value; OnPropertyChanged(); }
        }

        private string _unknownProductTitle = "";
        public string UnknownProductTitle
        {
            get { return _unknownProductTitle; }
            set { _unknownProductTitle = value; OnPropertyChanged(); }
        }


        public string OriginalLanguage { get { return _offProduct.OriginalLanguage; } }

        public string BarcodePath { get; private set; }

        internal void Expand(IngredientViewModel ingredient)
        {
            if (ingredient.HasChildren)
            {
                var index = this.Ingredients.IndexOf(ingredient);
                var expandedIngredient = new ExpandedIngredientViewModel(ingredient.ParsedIngredient, ingredient.Product);
                expandedIngredient.DisplayName = ingredient.DisplayName;
                //this.Ingredients[index] = expandedIngredient;
                //TranslateIngredients(expandedIngredient.Ingredients, null);

                this.Ingredients[index] = new ExpandedIngredientViewModel(ingredient.ParsedIngredient, ingredient.Product);
                TranslateIngredients(this.Ingredients, null);

                //below fades out and in
                //this.Ingredients.Remove(ingredient);
                //this.Ingredients.Insert(index, new ExpandedIngredientViewModel(ingredient.ParsedIngredient, ingredient.Product));
            }
        }

        internal void Collapse(ExpandedIngredientViewModel ingredient)
        {
            var index = this.Ingredients.IndexOf(ingredient);
            this.Ingredients[index] = new IngredientViewModel(ingredient.ParsedIngredient, ingredient.Product);
            TranslateIngredients(this.Ingredients, null);
        }

        public void Translate()
        {
            TranslateIngredients(this.Ingredients, null);
            TranslateNutrients(this.Nutrients);
            TranslateAllergens(this.Allergens);
            TranslateCaptions();
        }

        private void TranslateAllergens(ObservableCollection<AllergenViewModel> allergens)
        {
            var translator = new IngredientLanguageNames(_repo);
            foreach (var allergen in allergens)
            {
                TranslateAllergen(translator, allergen.Name, allergen.Name, allergen);
            }
        }

        private void TranslateAllergen(IngredientLanguageNames translator, string name, string displayName, AllergenViewModel allergen)
        {
            var nutrientTranslations = translator.TranslateIngredient(name, OriginalLanguage, LanguageCode);
            if (nutrientTranslations.Any())
            {
                displayName = nutrientTranslations.First().Name;
            }
            allergen.DisplayName = displayName;
        }

        private void TranslateCaptions()
        {
            if (LanguageCode == "nl")
            {
                NutrientsTitle = "Voedingswaarde per 100 g ℮";
                IngredientsTitle = "Ingrediënten";
                UnknownProductTitle = "Onbekend product. Voeg het product toe op nl.openfoodfacts.org ...";
            }
            else if (LanguageCode == "fr")
            {
                NutrientsTitle = "Composition nutritionnelle pour 100 g ℮";
                IngredientsTitle = "Ingrédients";
                UnknownProductTitle = "Produit inconnu. Ajouter la produit a fr.openfoodfacts.org ...";
            }
            else if (LanguageCode == "de")
            {
                NutrientsTitle = "Nährwertinformationen";
                IngredientsTitle = "Zutaten";
                UnknownProductTitle = "Unbekanntes Nahrungsmittel: de.openfoodfacts.org ...";
            }
            else if (LanguageCode == "es")
            {
                NutrientsTitle = "Información nutricional";
                IngredientsTitle = "Producto desconocido: es.openfoodfacts.org ...";
            }
            else
            {
                NutrientsTitle = "Nutritional value per 100 g ℮";
                IngredientsTitle = "Ingredients";
                UnknownProductTitle = "Unknown product. Please add to en.openfoodfacts.org ...";
            }
        }

        internal void TranslateIngredients(ObservableCollection<IngredientViewModel> ingredients, IIngredientLanguageNames translator)
        {
            if (ingredients != null)
            {
                if (translator == null) translator = new IngredientLanguageNames(_repo);

                foreach (var ingredient in ingredients)
                {
                    TranslateIngredient(translator, ingredient.Name, ingredient.Name, ingredient);
                    if (ingredient is ExpandedIngredientViewModel)
                    {
                        TranslateIngredients(((ExpandedIngredientViewModel)ingredient).Ingredients, translator);
                    }
                }
            }
        }
        private void TranslateIngredient(IIngredientLanguageNames translator, string name, string displayName, IngredientViewModel ingredient)
        {
            var ingredientTranslations = translator.TranslateIngredient(name, OriginalLanguage, LanguageCode);
            if (ingredientTranslations.Any())
            {
                displayName = ingredientTranslations.First().Name;
                ingredient.WikiDataId = ingredientTranslations.First().WikiDataId;
            }
            ingredient.DisplayName = displayName;
        }

        private void TranslateNutrients(ObservableCollection<NutrientViewModel> nutrients)
        {
            if (nutrients != null)
            {
                var nutrientTranslator = new NutrientLanguageNames(_repo);
                foreach (var nutrient in nutrients)
                {
                    var name = nutrient.Name.Replace("_100g", "");
                    TranslateNutrient(nutrientTranslator, name, name, nutrient);
                }
                var sortedNutrients = nutrients.OrderBy(o => o.Order).ToList();
                Nutrients.Clear();
                foreach (var nutrient in sortedNutrients)
                {
                    Nutrients.Add(nutrient);
                }
            }
        }

        private string TranslateNutrient(NutrientLanguageNames nutrientTranslator, string name, string displayName, NutrientViewModel nutrientViewModel)
        {
            var nutrientTranslations = nutrientTranslator.TranslateNutrient(name, "label", this.LanguageCode);
            if (nutrientTranslations.Any())
            {
                displayName = nutrientTranslations.First().Name;
                nutrientViewModel.Order = nutrientTranslations.First().Order;
                nutrientViewModel.IsSubCategory = !(nutrientViewModel.Order == 1 || nutrientViewModel.Order % 10 == 0);
            }
            nutrientViewModel.DisplayName = displayName;
            return displayName;
        }
    }

    public class IngredientViewModel : NotifyPropertyBase
    {
        public ProductViewModel Product { get; set; }
        public IngredientViewModel Parent { get; set; }
        public IngredientViewModel(IngredientsParser.ParsedIngredient parsedIngredient, ProductViewModel productVm)
        {
            ParsedIngredient = parsedIngredient;
            Product = productVm;
        }
        public string Name { get { return ParsedIngredient.Name; } }
        public string PercentageText { get { return ParsedIngredient.PercentageText; } }
        public bool HasChildren { get { return ParsedIngredient.Ingredients.Any(); } }
        public bool IsRoot { get { return Parent == null; } }
        private IngredientsParser.ParsedIngredient _parsedIngredient;
        internal IngredientsParser.ParsedIngredient ParsedIngredient
        {
            get
            {
                return _parsedIngredient;
            }

            set
            {
                _parsedIngredient = value;
                OnPropertyChanged();
            }
        }

        private string _displayName;
        public string DisplayName { get { return _displayName; } set { _displayName = value; OnPropertyChanged(); } }

        public int WikiDataId { get; set; }
        private Uri _imageUrl;
        public Uri ImageUrl { get { return _imageUrl; } set { _imageUrl = value; OnPropertyChanged(); } }

        public void Expand(IngredientViewModel ingredient)
        {
            if (Parent == null)
            {
                Product.Expand(ingredient);
            }
            else
            {
                Parent.Expand(ingredient);
            }
        }

        internal void Collapse(ExpandedIngredientViewModel ingredient)
        {
            if (Parent == null)
            {
                Product.Collapse(ingredient);
            }
            else
            {
                Parent.Collapse(ingredient);
            }
        }
    }

    public class ExpandedIngredientViewModel : IngredientViewModel
    {
        private ObservableCollection<IngredientViewModel> _ingredients;
        public ObservableCollection<IngredientViewModel> Ingredients
        {
            get { return _ingredients; }
            set { _ingredients = value; OnPropertyChanged(); }
        }

        public ExpandedIngredientViewModel(IngredientsParser.ParsedIngredient parsedIngredient, ProductViewModel productVm) : base(parsedIngredient, productVm)
        {
            Ingredients = new ObservableCollection<IngredientViewModel>();
            foreach (var ingredient in ParsedIngredient.Ingredients)
            {
                var ingredientVm = new IngredientViewModel(ingredient, this.Product);
                ingredientVm.Parent = this;
                Ingredients.Add(ingredientVm);
            }
        }
    }

    public class EUAdditiveViewModel : NotifyPropertyBase
    {
        public EUAdditiveViewModel(string euNumber, string displayName)
        {
            EUNumber = euNumber;
            DisplayName = displayName;
        }
        public string EUNumber { get; set; }
        public string DisplayName { get; set; }
    }

    public class AllergenViewModel : NotifyPropertyBase
    {
        public AllergenViewModel(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }
        public string Name { get; set; }
        private string _displayName;
        public string DisplayName { get { return _displayName; } set { _displayName = value; OnPropertyChanged(); } }
    }

    public class NutrientViewModel : NotifyPropertyBase
    {
        public NutrientViewModel(string name, string value, string unit, bool isSubCategory, int order)
        {
            Name = name;
            DisplayName = name; // for translations later
            Value = value;
            DoubleValue = double.Parse(value);
            Unit = unit;
            Order = order;
        }
        private double DoubleValue { get; set; }

        public string Name { get; set; }
        private string _displayName;
        public string DisplayName { get { return _displayName; } set { _displayName = value; OnPropertyChanged(); } }
        public string DisplayValue
        {
            get
            {
                switch (Unit)
                {
                    case "mg":
                        return (DoubleValue * 1000).ToString();
                    case "µg":
                        return (DoubleValue * 1000000).ToString();
                }
                return Value;
            }
        }
        public string Value { get; set; }
        public string Unit { get; set; }
        private bool _isSubCategory;
        public bool IsSubCategory { get { return _isSubCategory; } set { _isSubCategory = value; OnPropertyChanged(); } }
        private int _order;
        public int Order { get { return _order; } set { _order = value; OnPropertyChanged(); OnPropertyChanged("IsSubCategory"); } }
    }
}
