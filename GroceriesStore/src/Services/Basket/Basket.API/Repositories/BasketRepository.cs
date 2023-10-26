using Basket.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Basket.API.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly ILogger<BasketRepository> _logger;
    private readonly BasketContext _basketContext;

    public BasketRepository(ILogger<BasketRepository> logger, BasketContext basketContext)
    {
        _logger = logger;
        _basketContext = basketContext;
    }

    public async Task<bool> DeleteBasketAsync(string id)
    {
        return true; // await _database.KeyDeleteAsync(id);
    }

    public IEnumerable<string> GetUsers()
    {
        return Enumerable.Empty<string>();
        //var server = GetServer();
        //var data = server.Keys();

        //return data?.Select(k => k.ToString());
    }

    public async Task<CustomerBasket> GetBasketAsync(string customerId)
    {
        return await _basketContext.Baskets
            .Include(x => x.Items)
            .FirstOrDefaultAsync(b => b.BuyerId == customerId);
        //return new CustomerBasket()
        //{
        //    BuyerId = customerId,
        //    Items = new List<BasketItem>()
        //};
        //var data = await _database.StringGetAsync(customerId);

        //if (data.IsNullOrEmpty)
        //{
        //    return null;
        //}

        //return JsonSerializer.Deserialize<CustomerBasket>(data, JsonDefaults.CaseInsensitiveOptions);
    }

    public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
    {
        var savedBasket = await GetBasketAsync(basket.BuyerId);
        if (savedBasket != null)
        {
            savedBasket.Items = basket.Items;
            savedBasket.Items.ForEach(x => x.BasketId = basket.Id);
            savedBasket.Items.RemoveAll(x => x.Quantity < 0);
            _basketContext.Update(savedBasket);
        }
        else
        {
            savedBasket = new CustomerBasket()
            {
                BuyerId = basket.BuyerId,
                Items = basket.Items
            };
            _basketContext.Add(savedBasket);
        }

        await _basketContext.SaveChangesAsync();

        //basket.Items.ForEach(x => x.BasketId = savedBasket.Id);
        //_basketContext.BasketItems.AddRange(basket.Items);

        //await _basketContext.SaveChangesAsync();

        //var created = await _database.StringSetAsync(basket.BuyerId, JsonSerializer.Serialize(basket, JsonDefaults.CaseInsensitiveOptions));

        //if (!created)
        //{
        //    _logger.LogInformation("Problem occur persisting the item.");
        //    return null;
        //}

        //_logger.LogInformation("Basket item persisted successfully.");

        return await GetBasketAsync(savedBasket.BuyerId);
    }

    public async Task<CustomerBasket> DeleteBasketItemAsync(string basketItemId, string customerId)
    {
        var savedBasket = await GetBasketAsync(customerId);
        if (savedBasket != null)
        {
            savedBasket.Items.RemoveAll(x => x.Id == basketItemId);
            _basketContext.Update(savedBasket);
            await _basketContext.SaveChangesAsync();
        }


        //basket.Items.ForEach(x => x.BasketId = savedBasket.Id);
        //_basketContext.BasketItems.AddRange(basket.Items);

        //await _basketContext.SaveChangesAsync();

        //var created = await _database.StringSetAsync(basket.BuyerId, JsonSerializer.Serialize(basket, JsonDefaults.CaseInsensitiveOptions));

        //if (!created)
        //{
        //    _logger.LogInformation("Problem occur persisting the item.");
        //    return null;
        //}

        //_logger.LogInformation("Basket item persisted successfully.");

        return await GetBasketAsync(customerId);
    }

}
