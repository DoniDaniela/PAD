namespace GroceriesStore.Services.Basket.API.Model;

public interface IBasketRepository
{
    Task<CustomerBasket> GetBasketAsync(string customerId);
    IEnumerable<string> GetUsers();
    Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
    Task<bool> DeleteBasketAsync(string id);

    Task<CustomerBasket> DeleteBasketItemAsync(string basketItemId, string customerId);
}

