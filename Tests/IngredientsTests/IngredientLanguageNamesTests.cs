using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using IngredientLanguageNamesShared;
using IngredientShared;

namespace IngredientsTests
{
    public class IngredientLanguageNamesTests
    {
        [Fact]
        [Trait("Group", "Ingredient")]
        public void CanCreateIngredientLanguageName_CallsCreateIngredientLanguageNameStateOnRepository_AndSetsGuidsOnState()
        {
            var repoStub = Substitute.For<IngredientLanguageNames.IIngredientLanguageNameRepository>();
            var stateStub = Substitute.For<IngredientLanguageName.IIngredientLanguageNameState>();
            var ingredientLanguageNames = new IngredientLanguageNames(repoStub);
            var ingredientGuid = Guid.NewGuid();
            var language = "en";
            var name = "butter";

            repoStub.CreateIngredientLanguageNameState().Returns(stateStub);

            ingredientLanguageNames.CreateIngredientLanguageName(ingredientGuid, name, language);

            repoStub.Received().CreateIngredientLanguageNameState();

            stateStub.Received().IngredientGuid = ingredientGuid;
            stateStub.Received().Language = language;
            stateStub.Received().Name = name;
        }

        [Fact]
        [Trait("Group", "IngredientLanguageName")]
        public void IngredientLanguageName_ReflectsStateCorrectly()
        {
            var stateStub = Substitute.For<IngredientLanguageName.IIngredientLanguageNameState>();
            var ingredientLanguageName = new IngredientLanguageName(stateStub);
            var ingredientGuid = Guid.NewGuid();
            var language = "en";
            var name = "butter";

            stateStub.IngredientGuid.Returns(ingredientGuid);
            stateStub.Language.Returns(language);
            stateStub.Name.Returns(name);

            Assert.Equal(ingredientGuid, ingredientLanguageName.IngredientGuid);
            Assert.Equal(language, ingredientLanguageName.Language);
            Assert.Equal(name, ingredientLanguageName.Name);
        }

        [Fact]
        [Trait("Group", "IngredientLanguageNames")]
        public void CanGetTranslations()
        {
            var repoStub = Substitute.For<IngredientLanguageNames.IIngredientLanguageNameRepository>();
            var ingredientLanguageNames = new IngredientLanguageNames(repoStub);
            var butterEnglishStub = Substitute.For<IngredientLanguageName.IIngredientLanguageNameState>();
            var butterDutchStub = Substitute.For<IngredientLanguageName.IIngredientLanguageNameState>();

            butterEnglishStub.Name = "butter";
            butterEnglishStub.Language = "en";
            butterDutchStub.Name = "boter";
            butterDutchStub.Language = "nl";

            repoStub.GetTranslations(butterEnglishStub.Name, butterEnglishStub.Language, butterDutchStub.Language).Returns(new List<IngredientLanguageName.IIngredientLanguageNameState> { butterDutchStub });

            var ingredientGuid = Guid.NewGuid();
            var languageNameGuid = Guid.NewGuid();

            var translations = ingredientLanguageNames.TranslateIngredient("butter","en","nl");
            Assert.Equal(1, translations.Count());
            Assert.Equal("boter", translations.First().Name);
        }
    }
}
