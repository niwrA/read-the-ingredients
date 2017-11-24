using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using IngredientShared;
using NSubstitute;

namespace IngredientsTests
{
    public class IngredientStateMock : Ingredient.IIngredientState
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
    }

    public class IngredientsTests
    {
        [Fact]
        [Trait("Group", "Ingredient")]
        public void CanCreateIngredient_CallsCreateIngredientStateOnRepository_AndSetsGuidOnState()
        {
            var repoStub = Substitute.For<Ingredients.IIngredientRepository>();
            var stateStub = Substitute.For<Ingredient.IIngredientState>();
            var ingredients = new Ingredients(repoStub);

            repoStub.CreateIngredientState().Returns(stateStub);

            ingredients.CreateIngredient();

            repoStub.Received().CreateIngredientState();
            stateStub.Received().Guid = Arg.Is<Guid>(a => a != null && a != Guid.Empty);
        }

        [Fact]
        [Trait("Group", "Ingredient")]
        public void Ingredient_ReflectsStateCorrectly()
        {
            var stateStub = Substitute.For<Ingredient.IIngredientState>();
            var ingredient = new Ingredient(stateStub);
            var guid = Guid.NewGuid();
            stateStub.Guid.Returns(guid);

            Assert.Equal(guid, ingredient.Guid);
        }
    }
}
