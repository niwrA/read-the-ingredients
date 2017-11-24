using EUAdditiveLanguageNamesShared;
using IngredientLanguageNamesShared;
using NutrientLanguageNameShared;
using ProductsControllerShared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngredientShared;
using Microsoft.EntityFrameworkCore;

namespace SDKTemplate.Repositories
{
   
    public class MainRepositoryEntityFrameworkCore //: IMainRepository <-- reenable once we start using this again
    {
        public class IngredientLanguageNameState : IngredientLanguageName.IIngredientLanguageNameState
        {
            [Key]
            public Guid Guid { get; set; }
            [Required]
            public Guid IngredientGuid { get; set; }
            [Required]
            public string Language { get; set; }
            [Required]
            public string Name { get; set; }
            public string UNII { get; set; }
            public int WikiDataId { get; set; }
            public bool IsPreferred { get; set; }

        }
        public class NutrientLanguageNameState : NutrientLanguageName.INutrientLanguageNameState
        {
            [Key]
            public Guid Guid { get; set; }
            public string Language { get; set; }
            public string Name { get; set; }
            public Guid NutrientGuid { get; set; }
            public int Order { get; set; }
        }
        public class EUAdditiveLanguageNameState : EUAdditiveLanguageName.IEUAdditiveLanguageNameState
        {
            [Key]
            public Guid Guid { get; set; }
            public Guid EUAdditiveGuid { get; set; }

            public string EUNumber { get; set; }
            public string Language { get; set; }
            public string Name { get; set; }

        }

        public class IngredientState : Ingredient.IIngredientState
        {
            [Key]
            public Guid Guid { get; set; }
        }

        public class ProductScannerContext : DbContext
        {
            public DbSet<IngredientLanguageNameState> IngredientLanguageNameStates { get; set; }
            public DbSet<NutrientLanguageNameState> NutrientLanguageNameStates { get; set; }
            public DbSet<EUAdditiveLanguageNameState> EUAdditiveLanguageNameStates { get; set; }
            public DbSet<IngredientState> IngredientStates { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlite("Filename=ProductScannerEFC.db");
            }

            //protected override void OnModelCreating(ModelBuilder modelBuilder)
            //{
            //    // Make SensorId required
            //    modelBuilder.Entity<Sensor>()
            //        .Property(b => b.SensorId)
            //        .IsRequired();

            //    // Make TimeStamp required
            //    modelBuilder.Entity<Ambience>()
            //        .Property(b => b.AmbienceId)
            //        .IsRequired();
            //}
        }

        private ProductScannerContext _context;

        public MainRepositoryEntityFrameworkCore()
        {
            _context = new ProductScannerContext();
            _context.Database.Migrate();
        }
        public Ingredient.IIngredientState CreateIngredientState()
        {
            var ingredient = new IngredientState();
            ingredient.Guid = Guid.NewGuid();
            _context.IngredientStates.Add(ingredient);
            return ingredient;
        }

        public void PersistChanges()
        {
            _context.SaveChanges();
        }

        public IEnumerable<IngredientLanguageName.IIngredientLanguageNameState> GetTranslations(Guid guid)
        {
            throw new NotImplementedException();
        }

        public IngredientLanguageName.IIngredientLanguageNameState CreateIngredientLanguageNameState()
        {
            var ingredient = new IngredientLanguageNameState();
            ingredient.Guid = Guid.NewGuid();
            _context.IngredientLanguageNameStates.Add(ingredient);
            return ingredient;
        }

        public IEnumerable<IngredientLanguageName.IIngredientLanguageNameState> GetTranslations(string name, string fromLanguage, string toLanguage)
        {
            var ingredientGuids = _context.IngredientLanguageNameStates.Where(w => w.Name == name && w.Language == fromLanguage).Select(s => s.IngredientGuid);
            var translations = _context.IngredientLanguageNameStates.Where(w => ingredientGuids.Contains(w.IngredientGuid) && w.Language == toLanguage);
            //translations = _ingredientLanguageNameStateDictionary.Values.Where(w => w.Name == "plantaardige oliën");
            var test = _context.IngredientLanguageNameStates.First();
            return translations.ToList();
        }

        public void UpdateIngredientLanguageNameState(IngredientLanguageName.IIngredientLanguageNameState state)
        {

        }

        public IEnumerable<EUAdditiveLanguageName.IEUAdditiveLanguageNameState> GetEUAdditiveTranslations(Guid guid)
        {
            throw new NotImplementedException();
        }

        public EUAdditiveLanguageName.IEUAdditiveLanguageNameState CreateEUAdditiveLanguageNameState()
        {
            var euAdditive = new EUAdditiveLanguageNameState();
            euAdditive.Guid = Guid.NewGuid();
            _context.EUAdditiveLanguageNameStates.Add(euAdditive);
            return euAdditive;
        }

        public IEnumerable<EUAdditiveLanguageName.IEUAdditiveLanguageNameState> GetEUAdditiveTranslations(string euNumber, string toLanguage)
        {
            var translations = _context.EUAdditiveLanguageNameStates.Where(w => w.EUNumber == euNumber && w.Language == toLanguage);
            return translations.ToList();
        }

        public IEnumerable<EUAdditiveLanguageName.IEUAdditiveLanguageNameState> GetEUAdditiveByLanguageName(string name, string fromLanguage)
        {
            var translations = _context.EUAdditiveLanguageNameStates.Where(w => (w.Name.ToLower() == name.ToLower() || w.EUNumber == name) && w.Language == fromLanguage);
            //translations = _EUAdditiveLanguageNameStateDictionary.Values.Where(w => w.EUNumber == "E470a");
            return translations.ToList();
        }

        public void UpdateEUAdditiveNameState(EUAdditiveLanguageName.IEUAdditiveLanguageNameState state)
        {
        }

        public IEnumerable<NutrientLanguageName.INutrientLanguageNameState> GetNutrientTranslations(Guid guid)
        {
            throw new NotImplementedException();
        }

        public NutrientLanguageName.INutrientLanguageNameState CreateNutrientLanguageNameState()
        {
            var nutrient = new NutrientLanguageNameState();
            nutrient.Guid = Guid.NewGuid();
            _context.NutrientLanguageNameStates.Add(nutrient);
            return nutrient;
        }

        public IEnumerable<NutrientLanguageName.INutrientLanguageNameState> GetNutrientTranslations(string name, string fromLanguage, string toLanguage)
        {
            var nutrientGuids = _context.NutrientLanguageNameStates.Where(w => w.Name == name && w.Language == fromLanguage).Select(s => s.NutrientGuid);
            var translations = _context.NutrientLanguageNameStates.Where(w => nutrientGuids.Contains(w.NutrientGuid) && w.Language == toLanguage);
            //translations = _ingredientLanguageNameStateDictionary.Values.Where(w => w.Name == "plantaardige oliën");
            var test = _context.NutrientLanguageNameStates.First();
            return translations.ToList();
        }

        public void UpdateNutrientLanguageNameState(NutrientLanguageName.INutrientLanguageNameState state)
        {

        }
    }
}
