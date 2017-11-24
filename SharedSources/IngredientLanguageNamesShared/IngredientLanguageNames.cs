using System;
using System.Collections.Generic;
using System.Text;
using IngredientShared;

namespace IngredientLanguageNamesShared
{
    public interface IIngredientLanguageName
    {
        string Language { get; }
        Guid IngredientGuid { get; }
        string Name { get; }
    }
    public class IngredientLanguageName : IIngredientLanguageName
    {
        public interface IIngredientLanguageNameState
        {
            Guid Guid { get; set; }
            Guid IngredientGuid { get; set; }
            string Name { get; set; }
            string Language { get; set; }
            int WikiDataId { get; set; }
            string UNII { get; set; }
            bool IsPreferred { get; set; }
        }

        private IIngredientLanguageNameState _state;

        public IngredientLanguageName(IIngredientLanguageNameState state)
        {
            _state = state;
        }

        public Guid IngredientGuid { get { return _state.IngredientGuid; } }
        public string Name { get { return _state.Name; } }
        public string Language { get { return _state.Language; } }

        public int WikiDataId { get { return _state.WikiDataId; } }
    }

    public interface IIngredientLanguageNames
    {
        IngredientLanguageName CreateIngredientLanguageName(Guid ingredientGuid, string name, string culture);
        IEnumerable<IngredientLanguageName> GetLanguageNamesForIngredient(Guid guid);
        IEnumerable<IngredientLanguageName> TranslateIngredient(string name, string fromCulture, string toCulture);
    }
    public class IngredientLanguageNames : IIngredientLanguageNames
    {
        public interface IIngredientLanguageNameRepository
        {
            IEnumerable<IngredientLanguageName.IIngredientLanguageNameState> GetTranslations(Guid guid);
            IngredientLanguageName.IIngredientLanguageNameState CreateIngredientLanguageNameState();
            IEnumerable<IngredientLanguageName.IIngredientLanguageNameState> GetTranslations(string name, string fromLanguage, string toLanguage);
            void UpdateIngredientLanguageNameState(IngredientLanguageName.IIngredientLanguageNameState state);
            void PersistChanges();
        }

        public IngredientLanguageName CreateIngredientLanguageName(Guid ingredientGuid, string name, string language)
        {
            var state = _repo.CreateIngredientLanguageNameState();
            state.IngredientGuid = ingredientGuid;
            state.Name = name;
            state.Language = language;

            return new IngredientLanguageName(state);
        }

        private IIngredientLanguageNameRepository _repo;
        public IngredientLanguageNames(IIngredientLanguageNameRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<IngredientLanguageName> GetLanguageNamesForIngredient(Guid guid)
        {
            var ingredientLanguageNameStates = _repo.GetTranslations(guid);
            return WrapStates(ingredientLanguageNameStates);
        }

        public IEnumerable<IngredientLanguageName> TranslateIngredient(string name, string fromCulture, string toCulture)
        {
            var ingredientLanguageNameStates =  _repo.GetTranslations(name, fromCulture, toCulture);
            return WrapStates(ingredientLanguageNameStates);
        }

        private IEnumerable<IngredientLanguageName> WrapStates(IEnumerable<IngredientLanguageName.IIngredientLanguageNameState> ingredientLanguageNameStates)
        {
            var ingredientLanguageNames = new List<IngredientLanguageName>();
            foreach(var state in ingredientLanguageNameStates)
            {
                ingredientLanguageNames.Add(new IngredientLanguageName(state));
            }
            return ingredientLanguageNames;
        }
    }
}
