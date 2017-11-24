using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngredientShared;
using IngredientLanguageNamesShared;
using EUAdditiveLanguageNamesShared;
using ProductsControllerShared;
using NutrientLanguageNameShared;

namespace ReadTheIngredientsUWP.Repositories
{
    //public class MainRepository //: IMainRepository reenable once we start using this again
    //{
    //    private class IngredientState : Ingredient.IIngredientState
    //    {
    //        public Guid Guid { get; set; }
    //    }

    //    private Dictionary<Guid, Ingredient.IIngredientState> _ingredientStateDictionary;

    //    private class IngredientLanguageNameState : IngredientLanguageName.IIngredientLanguageNameState
    //    {
    //        public IngredientLanguageNameState()
    //        {
    //        }
    //        public Guid Guid { get; set; }
    //        public Guid IngredientGuid { get; set; }
    //        public string Language { get; set; }
    //        public string Name { get; set; }

    //        public string UNII { get; set; }
    //        public int WikiDataId { get; set; }
    //        public bool IsPreferred { get; set; }
    //    }

    //    private Dictionary<Guid, IngredientLanguageName.IIngredientLanguageNameState> _ingredientLanguageNameStateDictionary;

    //    private class EUAdditiveLanguageNameState : EUAdditiveLanguageName.IEUAdditiveLanguageNameState
    //    {
    //        public EUAdditiveLanguageNameState()
    //        {
    //        }
    //        public Guid Guid { get; set; }
    //        public Guid EUAdditiveGuid { get; set; }
    //        public string EUNumber { get; set; }
    //        public string Language { get; set; }
    //        public string Name { get; set; }
    //    }

    //    private Dictionary<Guid, EUAdditiveLanguageName.IEUAdditiveLanguageNameState> _EUAdditiveLanguageNameStateDictionary;

    //    public MainRepository()
    //    {
    //        _ingredientStateDictionary = new Dictionary<Guid, Ingredient.IIngredientState>();
    //        _ingredientLanguageNameStateDictionary = new Dictionary<Guid, IngredientLanguageName.IIngredientLanguageNameState>();
    //        _EUAdditiveLanguageNameStateDictionary = new Dictionary<Guid, EUAdditiveLanguageName.IEUAdditiveLanguageNameState>();
    //    }

    //    public Ingredient.IIngredientState CreateIngredientState()
    //    {
    //        var state = new IngredientState();
    //        state.Guid = Guid.NewGuid();
    //        _ingredientStateDictionary.Add(state.Guid, state);
    //        return state;
    //    }

    //    public void PersistChanges()
    //    {
    //       //noop
    //    }

    //    public IEnumerable<IngredientLanguageName.IIngredientLanguageNameState> GetTranslations(Guid ingredientGuid)
    //    {
    //        var translations = _ingredientLanguageNameStateDictionary.Values.Where(w => w.IngredientGuid == ingredientGuid);
    //        return translations.ToList();
    //    }

    //    public IEnumerable<IngredientLanguageName.IIngredientLanguageNameState> GetTranslations(string name, string fromLanguage, string toLanguage)
    //    {
    //        var euAdditiveGuids = _ingredientLanguageNameStateDictionary.Values.Where(w => w.Name == name && w.Language == fromLanguage).Select(s => s.IngredientGuid);
    //        var translations = _ingredientLanguageNameStateDictionary.Values.Where(w => euAdditiveGuids.Contains(w.IngredientGuid) && w.Language == toLanguage);
    //        //translations = _ingredientLanguageNameStateDictionary.Values.Where(w => w.Name == "plantaardige oliën");

    //        return translations.ToList();
    //    }

    //    public IngredientLanguageName.IIngredientLanguageNameState CreateIngredientLanguageNameState()
    //    {
    //        var state = new IngredientLanguageNameState();
    //        state.Guid = Guid.NewGuid();
    //        _ingredientLanguageNameStateDictionary.Add(state.Guid, state);
    //        return state;
    //    }

    //    public IEnumerable<EUAdditiveLanguageName.IEUAdditiveLanguageNameState> GetEUAdditiveTranslations(Guid euAdditiveGuid)
    //    {
    //        var translations = _EUAdditiveLanguageNameStateDictionary.Values.Where(w => w.EUAdditiveGuid == euAdditiveGuid);
    //        return translations.ToList();
    //    }

    //    public EUAdditiveLanguageName.IEUAdditiveLanguageNameState CreateEUAdditiveLanguageNameState()
    //    {
    //        var state = new EUAdditiveLanguageNameState();
    //        state.Guid = Guid.NewGuid();
    //        _EUAdditiveLanguageNameStateDictionary.Add(state.Guid, state);
    //        return state;
    //    }

    //    IEnumerable<EUAdditiveLanguageName.IEUAdditiveLanguageNameState> EUAdditiveLanguageNames.IEUAdditiveLanguageNameRepository.GetEUAdditiveTranslations(string euNumber, string toLanguage)
    //    {
    //        var translations = _EUAdditiveLanguageNameStateDictionary.Values.Where(w => w.EUNumber == euNumber && w.Language == toLanguage);
    //        return translations.ToList();
    //    }

    //    public IEnumerable<EUAdditiveLanguageName.IEUAdditiveLanguageNameState> GetEUAdditiveByLanguageName(string name, string fromLanguage)
    //    {
    //        var translations = _EUAdditiveLanguageNameStateDictionary.Values.Where(w => w.Name.ToLower() == name.ToLower() && w.Language == fromLanguage);
    //        //translations = _EUAdditiveLanguageNameStateDictionary.Values.Where(w => w.EUNumber == "E470a");
    //        return translations.ToList();
    //    }

    //    public void UpdateIngredientLanguageNameState(IngredientLanguageName.IIngredientLanguageNameState state)
    //    {
    //        // noop
    //    }

    //    public void UpdateEUAdditiveNameState(EUAdditiveLanguageName.IEUAdditiveLanguageNameState state)
    //    {
    //        // noop
    //    }

    //    IEnumerable<NutrientLanguageName.INutrientLanguageNameState> NutrientLanguageNames.INutrientLanguageNameRepository.GetNutrientTranslations(Guid guid)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public NutrientLanguageName.INutrientLanguageNameState CreateNutrientLanguageNameState()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    IEnumerable<NutrientLanguageName.INutrientLanguageNameState> NutrientLanguageNames.INutrientLanguageNameRepository.GetNutrientTranslations(string name, string fromLanguage, string toLanguage)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void UpdateNutrientLanguageNameState(NutrientLanguageName.INutrientLanguageNameState state)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
