using System;
using System.Collections.Generic;
using System.Text;

namespace EUAdditiveLanguageNamesShared
{
    public interface IEUAdditiveLanguageName
    {
        string Language { get; }
        Guid EUAdditiveGuid { get; }
        string Name { get; }
    }
    public class EUAdditiveLanguageName : IEUAdditiveLanguageName
    {
        public interface IEUAdditiveLanguageNameState
        {
            Guid Guid { get; set; }
            Guid EUAdditiveGuid { get; set; }
            string EUNumber { get; set; }
            string Name { get; set; }
            string Language { get; set; }
        }

        private IEUAdditiveLanguageNameState _state;

        public EUAdditiveLanguageName(IEUAdditiveLanguageNameState state)
        {
            _state = state;
        }

        public Guid EUAdditiveGuid { get { return _state.EUAdditiveGuid; } }
        public string EUNumber { get { return _state.EUNumber; } }
        public string Name { get { return _state.Name; } }
        public string Language { get { return _state.Language; } }
    }

    public interface IEUAdditiveLanguageNames
    {
        EUAdditiveLanguageName CreateEUAdditiveLanguageName(Guid ingredientGuid, string euNumber, string name, string culture);
        IEnumerable<EUAdditiveLanguageName> GetLanguageNamesForEUAdditive(Guid guid);
        IEnumerable<EUAdditiveLanguageName> TranslateEUAdditive(string euNumber, string toCulture);
        IEnumerable<EUAdditiveLanguageName> GetEUAdditiveNumber(string name, string fromLanguage);
    }
    public class EUAdditiveLanguageNames : IEUAdditiveLanguageNames
    {
        public interface IEUAdditiveLanguageNameRepository
        {
            IEnumerable<EUAdditiveLanguageName.IEUAdditiveLanguageNameState> GetEUAdditiveTranslations(Guid guid);
            EUAdditiveLanguageName.IEUAdditiveLanguageNameState CreateEUAdditiveLanguageNameState();
            IEnumerable<EUAdditiveLanguageName.IEUAdditiveLanguageNameState> GetEUAdditiveTranslations(string euNumber, string toLanguage);
            IEnumerable<EUAdditiveLanguageName.IEUAdditiveLanguageNameState> GetEUAdditiveByLanguageName(string name, string fromLanguage);
            void UpdateEUAdditiveNameState(EUAdditiveLanguageName.IEUAdditiveLanguageNameState state);
        }

        public EUAdditiveLanguageName CreateEUAdditiveLanguageName(Guid ingredientGuid, string euNumber, string name, string language)
        {
            var state = _repo.CreateEUAdditiveLanguageNameState();
            state.EUAdditiveGuid = ingredientGuid;
            state.EUNumber = euNumber;
            state.Name = name;
            state.Language = language;

            return new EUAdditiveLanguageName(state);
        }

        private IEUAdditiveLanguageNameRepository _repo;
        public EUAdditiveLanguageNames(IEUAdditiveLanguageNameRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<EUAdditiveLanguageName> GetLanguageNamesForEUAdditive(Guid guid)
        {
            var ingredientLanguageNameStates = _repo.GetEUAdditiveTranslations(guid);
            return WrapStates(ingredientLanguageNameStates);
        }

        public IEnumerable<EUAdditiveLanguageName> TranslateEUAdditive(string name, string toCulture)
        {
            var ingredientLanguageNameStates = _repo.GetEUAdditiveTranslations(name, toCulture);
            return WrapStates(ingredientLanguageNameStates);
        }

        private IEnumerable<EUAdditiveLanguageName> WrapStates(IEnumerable<EUAdditiveLanguageName.IEUAdditiveLanguageNameState> ingredientLanguageNameStates)
        {
            var ingredientLanguageNames = new List<EUAdditiveLanguageName>();
            foreach (var state in ingredientLanguageNameStates)
            {
                ingredientLanguageNames.Add(new EUAdditiveLanguageName(state));
            }
            return ingredientLanguageNames;
        }

        public IEnumerable<EUAdditiveLanguageName> GetEUAdditiveNumber(string name, string fromLanguage)
        {
            var states = _repo.GetEUAdditiveByLanguageName(name, fromLanguage);
            return WrapEUAdditiveStates(states);
        }

        private IEnumerable<EUAdditiveLanguageName> WrapEUAdditiveStates(IEnumerable<EUAdditiveLanguageName.IEUAdditiveLanguageNameState> ingredientLanguageNameStates)
        {
            var ingredientLanguageNames = new List<EUAdditiveLanguageName>();
            foreach (var state in ingredientLanguageNameStates)
            {
                ingredientLanguageNames.Add(new EUAdditiveLanguageName(state));
            }
            return ingredientLanguageNames;
        }
    }
}
