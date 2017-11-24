using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using ProductsControllerShared;
using System.Globalization;

namespace ProductsControllerTests
{
    public class ProductsControllerTests
    {
        [Theory]
        [InlineData("en", "nl")]
        [InlineData("nl", "fr")]
        [InlineData("fr", "de")]
        [InlineData("de", "en")]
        [Trait("Group", "NextLanguage")]
        public void NextLanguage_Succeeds(string current, string next)
        {
            var supportedLanguages = new List<string> { "en", "nl", "fr", "de" };
            var sut = new ProductsControllerTestBuilder()
                .WithSupportedLanguages(supportedLanguages)
                .WithProductLanguage(current)
                .Build();

            Assert.Equal(current, sut.LanguageCode);
            sut.NextLanguage();
            Assert.Equal(next, sut.LanguageCode);
        }

        [Fact]
        [Trait("Group", "NextLanguage")]
        public void NextLanguage_Succeeds_WithoutProduct()
        {
            var supportedLanguages = new List<string> { "en", "nl", "fr", "de" };
            var sut = new ProductsControllerTestBuilder()
                .WithSupportedLanguages(supportedLanguages)
                .WithoutProduct()
                .Build();

            Assert.Equal("en", sut.LanguageCode);
            sut.NextLanguage();
            Assert.Equal("nl", sut.LanguageCode);
        }


        // todo: rewrite with mocked usersettings
        [Theory]
        [InlineData("en-US", "en")]
        [InlineData("fr-FR", "fr")]
        [Trait("Group", "CorrectDefaultLanguage")]
        public void LanguageDefaultsToUserLanguage(string culture, string language)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(culture);

            var sut = new ProductsControllerTestBuilder().WithProductLanguage(language).Build();

            Assert.Equal(language, sut.LanguageCode);
        }
    }

    public class ProductsControllerTestBuilder
    {
        private IList<string> _supportedLanguages = new List<string> { "en", "nl", "fr", "de", "es" };
        private string _productLanguage = "en";
        private string _barcode = "12345678";
        private OpenFoodFactsContract.OpenFoodFactsProductDTO _offProduct ;
        private bool _setProduct = true;

        public ProductsController Build()
        {
            var mainFake = Substitute.For<IMainRepository>();
            var wikiFake = Substitute.For<IWikiDataRepository>();
            var sut = new ProductsController(mainFake, _supportedLanguages, wikiFake);

            if(_setProduct)
            {
                _offProduct = new OpenFoodFactsContract.OpenFoodFactsProductDTO { OriginalLanguage = _productLanguage };
                sut.SetProduct(_offProduct, _barcode);
            }

            return sut;
        }

        public ProductsControllerTestBuilder WithSupportedLanguages(IList<string> languages)
        {
            _supportedLanguages = languages;
            return this;
        }

        public ProductsControllerTestBuilder WithProductLanguage(string language)
        {
            _productLanguage = language;
            return this;
        }

        public ProductsControllerTestBuilder WithoutProduct()
        {
            _setProduct = false;
            return this;
        }
    }
}
