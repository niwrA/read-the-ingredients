using EUAdditiveLanguageNamesShared;
using IngredientLanguageNamesShared;
using IngredientShared;
using ProductsControllerShared;
using SQLite.Net;
using SQLite.Net.Attributes;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NutrientLanguageNameShared;
using UserSettingsShared;
using IngredientImagesShared;

namespace ReadTheIngredientsUWP.Repositories
{
    public class IngredientState : Ingredient.IIngredientState
    {
        [PrimaryKey]
        public Guid Guid { get; set; }
    }
    public class IngredientLanguageNameState : IngredientLanguageName.IIngredientLanguageNameState
    {
        [PrimaryKey]
        public Guid Guid { get; set; }

        [Indexed]
        public Guid IngredientGuid { get; set; }

        [Indexed]
        public string Language { get; set; }

        [Indexed]
        public string Name { get; set; }
        public string UNII { get; set; }
        public int WikiDataId { get; set; }
        public bool IsPreferred { get; set; }

    }
    public class NutrientLanguageNameState : NutrientLanguageName.INutrientLanguageNameState
    {
        [PrimaryKey]
        public Guid Guid { get; set; }

        [Indexed]
        public string Language { get; set; }

        [Indexed]
        public string Name { get; set; }

        [Indexed]
        public Guid NutrientGuid { get; set; }

        public int Order { get; set; }
    }
    public class EUAdditiveLanguageNameState : EUAdditiveLanguageName.IEUAdditiveLanguageNameState
    {
        [PrimaryKey]
        public Guid Guid { get; set; }
        [Indexed]
        public Guid EUAdditiveGuid { get; set; }

        public string EUNumber { get; set; }
        [Indexed]
        public string Language { get; set; }
        [Indexed]
        public string Name { get; set; }
    }
    public class IngredientImageState : IngredientImages.IIngredientImageState
    {
        [PrimaryKey]
        public Guid Guid { get; set; }
        public string Url { get; set; }
        [Indexed]
        public int WikiDataId { get; set; }
    }

    public class UserSettingState : IUserSettingState
    {
        public Guid Guid { get; set; }
        public string Namespace { get; set; }
        public string Setting { get; set; }
        public string Value { get; set; }
        public bool IsSet { get; set; }
    }

    public class MainRepositorySQLLite : IMainRepository
    {
        private const string DefaultPath = "openfooddata.sqllite";
        private string _path = "openfooddatamain.sqllite";
        private SQLiteConnection _conn;
        public bool DatabaseReady
        {
            get { return System.IO.File.Exists(_path);  }
        }
        public MainRepositorySQLLite()
        {
            _path = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, DefaultPath);
            if (System.IO.File.Exists(_path))
            {
                System.IO.File.Delete(_path);
            }
            _conn = new SQLiteConnection(new SQLitePlatformWinRT(), _path);
            //var cstring = new SQLiteConnectionString("Data Source =:memory:; Version = 3; New = True;", true, null, null, SQLite.Net.Interop.SQLiteOpenFlags.Create);
            var conn = _conn;

            //conn.CreateTable<IngredientState>();
            conn.CreateTable<IngredientLanguageNameState>();
            conn.CreateTable<NutrientLanguageNameState>();
            conn.CreateTable<EUAdditiveLanguageNameState>();
            conn.CreateTable<UserSettingState>();
            conn.CreateTable<IngredientImageState>();

        }
        public EUAdditiveLanguageName.IEUAdditiveLanguageNameState CreateEUAdditiveLanguageNameState()
        {
            var state = new EUAdditiveLanguageNameState();
            state.Guid = Guid.NewGuid();
            _conn.Insert(state);
            return state;
        }

        public IngredientLanguageName.IIngredientLanguageNameState CreateIngredientLanguageNameState()
        {
            var state = new IngredientLanguageNameState();
            state.Guid = Guid.NewGuid();
            _conn.Insert(state);
            return state;
        }

        public Ingredient.IIngredientState CreateIngredientState()
        {
            var state = new IngredientState();
            state.Guid = Guid.NewGuid();
            _conn.Insert(state);
            return state;
        }
        public void UpdateIngredientState(Ingredient.IIngredientState state)
        {
            _conn.Update(state);
        }

        public IEnumerable<EUAdditiveLanguageName.IEUAdditiveLanguageNameState> GetEUAdditiveByLanguageName(string name, string fromLanguage)
        {
            var translations = _conn.Table<EUAdditiveLanguageNameState>().Where(w => (w.Name.ToLower() == name.ToLower() || w.EUNumber == name) && w.Language == fromLanguage);
            //translations = _EUAdditiveLanguageNameStateDictionary.Values.Where(w => w.EUNumber == "E470a");
            return translations.ToList();
        }

        public IEnumerable<EUAdditiveLanguageName.IEUAdditiveLanguageNameState> GetEUAdditiveTranslations(Guid guid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EUAdditiveLanguageName.IEUAdditiveLanguageNameState> GetEUAdditiveTranslations(string euNumber, string toLanguage)
        {
            var translations = _conn.Table<EUAdditiveLanguageNameState>().Where(w => w.EUNumber == euNumber && w.Language == toLanguage);
            return translations.ToList();
        }

        public IEnumerable<IngredientLanguageName.IIngredientLanguageNameState> GetTranslations(Guid guid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IngredientLanguageName.IIngredientLanguageNameState> GetTranslations(string name, string fromLanguage, string toLanguage)
        {
            var ingredientGuids = _conn.Table<IngredientLanguageNameState>().Where(w => w.Name.ToLower() == name.ToLower() && w.Language == fromLanguage).Select(s => s.IngredientGuid);
            var translations = _conn.Table<IngredientLanguageNameState>().Where(w => ingredientGuids.Contains(w.IngredientGuid) && w.Language == toLanguage && w.IsPreferred);
            //translations = _ingredientLanguageNameStateDictionary.Values.Where(w => w.Name == "plantaardige oliën");
            //var test = _conn.Table<IngredientLanguageNameState>().First();
            return translations.ToList();
        }

        public void PersistChanges()
        {
            _conn.Commit();
            _conn.BeginTransaction();
        }

        public void BeginTransaction()
        {
            _conn.BeginTransaction();
        }

        public void Commit()
        {
            _conn.Commit();
        }

        public void UpdateIngredientLanguageNameState(IngredientLanguageName.IIngredientLanguageNameState state)
        {
            _conn.Update(state);
        }

        public void UpdateEUAdditiveNameState(EUAdditiveLanguageName.IEUAdditiveLanguageNameState state)
        {
            _conn.Update(state);
        }

        IEnumerable<NutrientLanguageName.INutrientLanguageNameState> NutrientLanguageNames.INutrientLanguageNameRepository.GetNutrientTranslations(Guid guid)
        {
            throw new NotImplementedException();
        }

        public NutrientLanguageName.INutrientLanguageNameState CreateNutrientLanguageNameState()
        {
            var state = new NutrientLanguageNameState();
            state.Guid = Guid.NewGuid();
            _conn.Insert(state);
            return state;
        }

        IEnumerable<NutrientLanguageName.INutrientLanguageNameState> NutrientLanguageNames.INutrientLanguageNameRepository.GetNutrientTranslations(string name, string fromLanguage, string toLanguage)
        {
            var ingredientGuids = _conn.Table<NutrientLanguageNameState>().Where(w => w.Name.ToLower() == name.ToLower() && w.Language == fromLanguage).Select(s => s.NutrientGuid);
            var translations = _conn.Table<NutrientLanguageNameState>().Where(w => ingredientGuids.Contains(w.NutrientGuid) && w.Language == toLanguage);
            //translations = _ingredientLanguageNameStateDictionary.Values.Where(w => w.Name == "plantaardige oliën");
            //var test = _conn.Table<NutrientLanguageNameState>().First();
            return translations.ToList();
        }

        public void UpdateNutrientLanguageNameState(NutrientLanguageName.INutrientLanguageNameState state)
        {
            _conn.Update(state);
        }

        public IUserSettingState CreateUserSettingState()
        {
            var state = new UserSettingState();
            state.Guid = Guid.NewGuid();
            _conn.Insert(state);
            return state;
        }

        public void UpdateUserSettingState(IUserSettingState state)
        {
            _conn.Update(state);
        }

        public void DeleteUserSettingState(Guid guid)
        {
            _conn.Delete(guid);
        }

        public IUserSettingState GetUserSettingState(string ns, string setting)
        {
            return _conn.Table<UserSettingState>().SingleOrDefault(w=>w.Namespace == ns && w.Setting == setting);
        }

        public IngredientImages.IIngredientImageState CreateIngredientImageState()
        {
            var state = new IngredientImageState();
            state.Guid = Guid.NewGuid();
            _conn.Insert(state);
            return state;
        }

        public void UpdateIngredientImageState(IngredientImages.IIngredientImageState state)
        {
            _conn.Update(state);
        }

        public IEnumerable<IngredientImages.IIngredientImageState> GetIngredientImageStates(int wikiDataId)
        {
            return _conn.Table<IngredientImageState>().Where(w => w.WikiDataId == wikiDataId).ToList();
        }
    }
}
