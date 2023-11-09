using Basket.API.Direct.Models;

namespace Basket.API.Direct.Services;

public interface IBasketService
{
    Task<BasketData> GetByIdAsync(string id);

    Task UpdateAsync(BasketData currentBasket);
}
