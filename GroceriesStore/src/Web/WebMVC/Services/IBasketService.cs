namespace GroceriesStore.WebMVC.Services;

using GroceriesStore.WebMVC.ViewModels;

public interface IBasketService
{
    Task<Basket> GetBasket(ApplicationUser user);
    Task AddItemToBasket(ApplicationUser user, int productId);
    Task<Basket> UpdateBasket(Basket basket);
    Task<Basket> SetQuantities(ApplicationUser user, Dictionary<string, int> quantities);

    Task<Basket> DeleteItem(ApplicationUser user, string id);

    Task<Basket> Delete(ApplicationUser user);

}
