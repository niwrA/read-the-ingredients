using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using IngredientsParserShared;

namespace IngredientsParserTests
{
    public class IncredientsParserTests
    {
        private IngredientsParser _parser;
        private string DefaultTestIngredients = "rietsuiker*, plantaardige olie* (zonnebloem, palm), 13% hazelnoot*, 7.5% magere cacaopoeder*, magere _melk_poeder*, emulgator (_soja_lecithine), vanille*. *Van biologische oorsprong.";
        public IncredientsParserTests()
        {
            _parser = new IngredientsParser();
        }

        [Fact]
        [Trait("Group", "Allergant")]
        public void DetectsAllergant_IfWholeIngredientIsAllergant()
        {
            var test = "_allergant_";
            var result = _parser.ParseAllergents(ref test);
            Assert.Equal(1, result.Count);
            Assert.Equal("allergant", result[0]);
            Assert.Equal("allergant", test);
        }

        [Fact]
        [Trait("Group", "Allergant")]
        public void DetectsAllergant_IfIngredientContainsAllergant_StartsWith()
        {
            var test = "_allergant_something";
            var result = _parser.ParseAllergents(ref test);
            Assert.Equal(1, result.Count);
            Assert.Equal("allergant", result[0]);
            Assert.Equal("allergantsomething", test);
        }

        [Fact]
        [Trait("Group", "Allergant")]
        public void DetectsAllergant_IfIngredientContainsAllergant_EndsWith()
        {
            var test = "something_allergant_";
            var result = _parser.ParseAllergents(ref test);
            Assert.Equal(1, result.Count);
            Assert.Equal("allergant", result[0]);
            Assert.Equal("somethingallergant", test);
        }

        [Fact]
        [Trait("Group", "Percentage")]
        public void DetectsPercentage_IfIngredientContainsPercentage()
        {
            var test = "at least 37% something";
            var result = _parser.ParsePercentage(ref test);
            Assert.Equal("37%", result);
            Assert.Equal("at least something", test);
        }

        [Fact]
        [Trait("Group", "Percentage")]
        public void DetectsPercentage_IfIngredientContainsPercentage_StartsWith()
        {
            var test = "37% something";
            var result = _parser.ParsePercentage(ref test);
            Assert.Equal("37%", result);
            Assert.Equal("something", test);
        }

        [Fact]
        [Trait("Group", "Percentage")]
        public void DetectsPercentage_IfIngredientContainsPercentage_EndsWith()
        {
            var test = "something 37%";
            var result = _parser.ParsePercentage(ref test);
            Assert.Equal("37%", result);
            Assert.Equal("something", test);
        }

        [Fact]
        [Trait("Group", "Percentage")]
        public void DetectsPercentage_IfIngredientContainsPercentage_WithSpace()
        {
            var test = "something 37 %";
            var result = _parser.ParsePercentage(ref test);
            Assert.Equal("37 %", result);
            Assert.Equal("something", test);
        }

        [Fact]
        [Trait("Group", "Percentage")]
        public void DetectsPercentage_IfIngredientContainsPercentage_WithDecimal()
        {
            var test = "at least 37.12% something";
            var result = _parser.ParsePercentage(ref test);
            Assert.Equal("37.12%", result);
            Assert.Equal("at least something", test);
        }

        [Fact]
        [Trait("Group", "Percentage")]
        public void DetectsPercentage_IfIngredientContainsPercentage_WithDecimalAsComma()
        {
            var test = "at least 37,12% something";
            var result = _parser.ParsePercentage(ref test);
            Assert.Equal("37,12%", result);
            Assert.Equal("at least something", test);
        }

        [Fact]
        [Trait("Group", "Detail")]
        public void DetectsDetails_IfIngredientContainsOneDetail()
        {
            var test = "something (detail)";
            var result = _parser.ParseDetails(ref test, new List<IngredientsParser.ParsedAnnotation>());
            Assert.Equal(1, result.Ingredients.Count);
            Assert.Equal("detail", result.Ingredients[0].Text);
            Assert.Equal("something", test);
        }

        [Fact]
        [Trait("Group", "Detail")]
        public void DetectsDetails_IfIngredientContainsDetails()
        {
            var test = "something (detail, detail2)";
            var result = _parser.ParseDetails(ref test, new List<IngredientsParser.ParsedAnnotation>());
            Assert.Equal(2, result.Ingredients.Count);
            Assert.Equal("detail", result.Ingredients[0].Text);
            Assert.Equal("detail2", result.Ingredients[1].Text);
            Assert.Equal("something", test);
        }

        [Fact]
        [Trait("Group", "Normalise")]
        public void NormalisesPercentage_WhenEnclosedByParenthesis()
        {
            var test = "something (37%), something";
            var result = _parser.NormalisePercentages(test);
            Assert.Equal("something 37%, something", result);
        }

        [Fact]
        [Trait("Group", "Detail")]
        public void IgnoresDetails_IfIngredientContainsDetailsWithAlternativeNameIndicatedByEqualSign()
        {
            var test = "something (=a random thing)";
            var result = _parser.ParseDetails(ref test, new List<IngredientsParser.ParsedAnnotation>());
            Assert.Equal(0, result.Ingredients.Count);
        }

        [Fact]
        [Trait("Group", "Detail")]
        public void DetectsDetails_IfIngredientContainsDetailsRecursive()
        {
            var test = "something (detail, detail2 (recdetail, recdetail2))";
            var result = _parser.ParseDetails(ref test, new List<IngredientsParser.ParsedAnnotation>());
            Assert.Equal(2, result.Ingredients.Count);
            Assert.Equal("detail", result.Ingredients[0].Name);
            Assert.Equal("detail2", result.Ingredients[1].Name);
            Assert.Equal(2, result.Ingredients[1].Ingredients.Count);
            Assert.Equal("recdetail", result.Ingredients[1].Ingredients[0].Name);
            Assert.Equal("recdetail2", result.Ingredients[1].Ingredients[1].Name);
            Assert.Equal("something", test);
        }

        [Fact]
        [Trait("Group", "Detail")]
        public void DetectsDetails_IfIngredientContainsDetailsRecursive_WithAlternatingSquareBrackets()
        {
            var test = "something (detail, detail2 [recdetail, recdetail2])";
            var result = _parser.ParseDetails(ref test, new List<IngredientsParser.ParsedAnnotation>());
            Assert.Equal(2, result.Ingredients.Count);
            Assert.Equal("detail", result.Ingredients[0].Name);
            Assert.Equal("detail2", result.Ingredients[1].Name);
            Assert.Equal(2, result.Ingredients[1].Ingredients.Count);
            Assert.Equal("recdetail", result.Ingredients[1].Ingredients[0].Name);
            Assert.Equal("recdetail2", result.Ingredients[1].Ingredients[1].Name);
            Assert.Equal("something", test);
        }

        [Fact(Skip ="Not yet supported")] // todo
        [Trait("Group", "All")]
        public void DetectsDetails_IfIngredientContainsComponentDetails_IndicatedByColon()
        {
            var test = "zucchini, tomato.\nMilk: water, calcium.";
            var result = _parser.Parse(test, new List<IngredientsParser.ParsedAnnotation>());
            Assert.Equal(3, result.Ingredients.Count);
            Assert.Equal("zucchini", result.Ingredients[0].Name);
            Assert.Equal("tomato", result.Ingredients[1].Name);
            Assert.Equal("milk", result.Ingredients[2].Name);
            Assert.Equal(2, result.Ingredients[2].Ingredients.Count);
            Assert.Equal("water", result.Ingredients[1].Ingredients[0].Name);
            Assert.Equal("calcium", result.Ingredients[1].Ingredients[1].Name);
        }

        //[Fact]
        //[Trait("Group", "All")]
        //public void DetectsDetails_IfIngredientContainsDetails_IndicatedByColon()
        //{
        //    var test = "zucchini, tomato, milk: water, calcium.";
        //    var result = _parser.Parse(test, new List<IngredientsParser.ParsedAnnotation>());
        //    Assert.Equal(3, result.Ingredients.Count);
        //    Assert.Equal("zucchini", result.Ingredients[0].Name);
        //    Assert.Equal("tomato", result.Ingredients[1].Name);
        //    Assert.Equal("milk", result.Ingredients[2].Name);
        //    Assert.Equal(2, result.Ingredients[2].Ingredients.Count);
        //    Assert.Equal("water", result.Ingredients[1].Ingredients[0].Name);
        //    Assert.Equal("calcium", result.Ingredients[1].Ingredients[1].Name);
        //}

        [Theory]
        [Trait("Group", "Annotation")]
        [InlineData("*")]
        [InlineData("**")]
        [InlineData("#")]
        public void DetectsAnnotation_IfIngredientContainsAnnotation(string identifier)
        {
            var annotations = new List<IngredientsParser.ParsedAnnotation>();
            annotations.Add(new IngredientsParser.ParsedAnnotation { Identifier = identifier, Text = "Detected" });

            var test = "something" + identifier;
            var result = _parser.ParseAnnotations(ref test, annotations);

            Assert.Equal(1, result.Count);
            Assert.Equal(identifier, result[0].Identifier);
            Assert.Equal("Detected", result[0].Text);
            Assert.Equal("something", test);
        }

        [Theory]
        [Trait("Group", "All")]
        [InlineData("rietsuiker *, plantaardige olie * (zonnebloem, palm), 13% hazelnoot*, 7.5% magere cacaopoeder*, magere _melk_poeder*, emulgator(_soja_lecithine), vanille*. *Van biologische oorsprong.")]
        public void CanParseFullIngredientsList(string ingredients)
        {
            var result = _parser.Parse(ingredients, new List<IngredientsParser.ParsedAnnotation>());
            var ingredientResults = result.Ingredients;
            var annotationResults = result.Annotations;

            Assert.Equal(7, result.Ingredients.Count);
            Assert.Equal(1, result.Annotations.Count);
            Assert.Equal("rietsuiker", ingredientResults[0].Name);
            Assert.Equal(1, ingredientResults[0].Annotations.Count);
            Assert.Equal("plantaardige olie", ingredientResults[1].Name);
            Assert.Equal(1, ingredientResults[1].Annotations.Count);
            Assert.Equal(2, ingredientResults[1].Ingredients.Count);
            Assert.Equal("hazelnoot", ingredientResults[2].Name);
            Assert.Equal(1, ingredientResults[2].Annotations.Count);
            Assert.Equal("13%", ingredientResults[2].PercentageText);
            Assert.Equal("magere cacaopoeder", ingredientResults[3].Name);
            Assert.Equal(1, ingredientResults[3].Annotations.Count);
            Assert.Equal("7.5%", ingredientResults[3].PercentageText);
            Assert.Equal("magere melkpoeder", ingredientResults[4].Name);
            Assert.Equal(1, ingredientResults[4].Annotations.Count);
            Assert.Equal("emulgator", ingredientResults[5].Name);
            Assert.Equal(0, ingredientResults[5].Annotations.Count);
            Assert.Equal(1, ingredientResults[5].Ingredients.Count);
            Assert.Equal("vanille", ingredientResults[6].Name);
            Assert.Equal(1, ingredientResults[6].Annotations.Count);
            Assert.Equal("vanille", ingredientResults[6].Name);
            Assert.Equal("*", annotationResults[0].Identifier);
            Assert.Equal("Van biologische oorsprong.", annotationResults[0].Text);

        }

        [Theory]
        [Trait("Group", "All")]
        [InlineData("salt, sugar cane.")]
        public void CanParseFullIngredientsList_EndingWithDot(string ingredients)
        {
            var result = _parser.Parse(ingredients, new List<IngredientsParser.ParsedAnnotation>());
            var ingredientResults = result.Ingredients;
            var annotationResults = result.Annotations;

            Assert.Equal(2, result.Ingredients.Count);
            Assert.Equal("salt", ingredientResults[0].Name);
            Assert.Equal("sugar cane", ingredientResults[1].Name);
        }


        [Theory]
        [Trait("Group", "All")]
        [InlineData("Une cuillère de soupe de farine de _blé_, une cuillère de sucre roux de canne, une noix de _beurre_ frais 20,8 %, du chocolat 17 % (sucre, pâte de cacao, beurre de cacao, cacao maigre en pudre, émulsifiant : lécithines (_soja_), arôme naturel de vanille), du jaune d’_œufs_, de l'_œuf_ frais entier et une pointe de sel de Guérande 0,5 %.")]
        public void CanParseFullIngredientsList_French(string ingredients)
        {
            var result = _parser.Parse(ingredients, new List<IngredientsParser.ParsedAnnotation>());

            Assert.Equal(6, result.Ingredients.Count);
            Assert.Equal("Une cuillère de soupe de farine de blé", result.Ingredients[0].Name);
        }
    }
}
