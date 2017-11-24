using System;
using System.Collections.Generic;
using System.Text;

namespace NutrientLanguageNameShared
{
    public interface INutrientLanguageName
    {
        string Language { get; }
        Guid NutrientGuid { get; }
        string Name { get; }
    }
    public class NutrientLanguageName : INutrientLanguageName
    {
        public interface INutrientLanguageNameState
        {
            Guid Guid { get; set; }
            Guid NutrientGuid { get; set; }
            string Name { get; set; }
            string Language { get; set; }
            int Order { get; set; }
        }

        private INutrientLanguageNameState _state;

        public NutrientLanguageName(INutrientLanguageNameState state)
        {
            _state = state;
        }

        public Guid NutrientGuid { get { return _state.NutrientGuid; } }
        public string Name { get { return _state.Name; } }
        public string Language { get { return _state.Language; } }
        public int Order { get { return _state.Order; } }
    }

    public interface INutrientLanguageNames
    {
        NutrientLanguageName CreateNutrientLanguageName(Guid nutrientGuid, string name, string culture);
        IEnumerable<NutrientLanguageName> GetLanguageNamesForNutrient(Guid guid);
        IEnumerable<NutrientLanguageName> TranslateNutrient(string name, string fromCulture, string toCulture);
    }
    public class NutrientLanguageNames : INutrientLanguageNames
    {
        public interface INutrientLanguageNameRepository
        {
            IEnumerable<NutrientLanguageName.INutrientLanguageNameState> GetNutrientTranslations(Guid guid);
            NutrientLanguageName.INutrientLanguageNameState CreateNutrientLanguageNameState();
            IEnumerable<NutrientLanguageName.INutrientLanguageNameState> GetNutrientTranslations(string name, string fromLanguage, string toLanguage);
            void UpdateNutrientLanguageNameState(NutrientLanguageName.INutrientLanguageNameState state);
        }

        public NutrientLanguageName CreateNutrientLanguageName(Guid nutrientGuid, string name, string language)
        {
            var state = _repo.CreateNutrientLanguageNameState();
            state.NutrientGuid = nutrientGuid;
            state.Name = name;
            state.Language = language;

            return new NutrientLanguageName(state);
        }

        private INutrientLanguageNameRepository _repo;
        public NutrientLanguageNames(INutrientLanguageNameRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<NutrientLanguageName> GetLanguageNamesForNutrient(Guid guid)
        {
            var nutrientLanguageNameStates = _repo.GetNutrientTranslations(guid);
            return WrapStates(nutrientLanguageNameStates);
        }

        public IEnumerable<NutrientLanguageName> TranslateNutrient(string name, string fromCulture, string toCulture)
        {
            var nutrientLanguageNameStates = _repo.GetNutrientTranslations(name, fromCulture, toCulture);
            return WrapStates(nutrientLanguageNameStates);
        }

        private IEnumerable<NutrientLanguageName> WrapStates(IEnumerable<NutrientLanguageName.INutrientLanguageNameState> nutrientLanguageNameStates)
        {
            var nutrientLanguageNames = new List<NutrientLanguageName>();
            foreach (var state in nutrientLanguageNameStates)
            {
                nutrientLanguageNames.Add(new NutrientLanguageName(state));
            }
            return nutrientLanguageNames;
        }
    }
}
