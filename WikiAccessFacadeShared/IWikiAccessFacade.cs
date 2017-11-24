using System.Threading.Tasks;
using WikiAccess;

namespace WikiAccessFacadeShared
{
    public interface IWikiAccessFacade
    {
        Task<string> GetWikiDataFoodIngredients(string languageCode);
        Task<string> GetWikiDataIngredientENumbers(string languageCode);
        Task<string> GetFoodIngredientImages();
    }
}