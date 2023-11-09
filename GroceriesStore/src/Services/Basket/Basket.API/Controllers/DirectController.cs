using Basket.API.Direct.Models;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DirectController : ControllerBase
    {
        //private readonly IBasketService _basket;
        private readonly IBasketRepository _repository;

        public DirectController(ILogger<DirectController> logger,
        IBasketRepository repository,
        IIdentityService identityService)
        {
            //_basket = basketService;
            _repository = repository;
        }

        [HttpPost]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BasketData>> UpdateAllBasketAsync([FromBody] UpdateBasketRequest data)
        {
            if (data.Items == null || !data.Items.Any())
            {
                return BadRequest("Need to pass at least one basket line");
            }

            // Retrieve the current basket
            var basket = await _repository.GetBasketAsync(data.BuyerId) ?? new CustomerBasket(data.BuyerId);
            //var catalogItems = await _catalog.GetCatalogItemsAsync(data.Items.Select(x => x.ProductId));

            // group by product id to avoid duplicates
            var itemsCalculated = data
                    .Items
                    .GroupBy(x => x.ProductId, x => x, (k, i) => new { productId = k, items = i })
                    .Select(groupedItem =>
                    {
                        var item = groupedItem.items.First();
                        item.Quantity = groupedItem.items.Sum(i => i.Quantity);
                        return item;
                    });

            foreach (var bitem in itemsCalculated)
            {
                var catalogItem = data.catalogItems.SingleOrDefault(ci => ci.Id == bitem.ProductId);
                if (catalogItem == null)
                {
                    return BadRequest($"Basket refers to a non-existing catalog item ({bitem.ProductId})");
                }

                var itemInBasket = basket.Items.FirstOrDefault(x => x.ProductId == bitem.ProductId);
                if (itemInBasket == null)
                {
                    basket.Items.Add(new BasketItem()
                    {
                        Id = bitem.Id,
                        ProductId = catalogItem.Id,
                        ProductName = catalogItem.Name,
                        PictureUrl = catalogItem.PictureUri,
                        UnitPrice = catalogItem.Price,
                        Quantity = bitem.Quantity
                    });
                }
                else
                {
                    itemInBasket.Quantity = bitem.Quantity;
                }
            }

            await _repository.UpdateBasketAsync(basket);

            return Ok(basket);
        }

        [HttpPut]
        [Route("items")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BasketData>> UpdateQuantitiesAsync([FromBody] UpdateBasketItemsRequest data)
        {
            if (!data.Updates.Any())
            {
                return BadRequest("No updates sent");
            }

            // Retrieve the current basket
            var currentBasket = await _repository.GetBasketAsync(data.BasketId);
            if (currentBasket == null)
            {
                return BadRequest($"Basket with id {data.BasketId} not found.");
            }

            // Update with new quantities
            foreach (var update in data.Updates)
            {
                var basketItem = currentBasket.Items.SingleOrDefault(bitem => bitem.Id == update.BasketItemId);
                if (basketItem == null)
                {
                    return BadRequest($"Basket item with id {update.BasketItemId} not found");
                }
                basketItem.Quantity = update.NewQty;
            }

            // Save the updated basket
            await _repository.UpdateBasketAsync(currentBasket);

            return Ok(currentBasket);
        }

        [HttpPost]
        [Route("items")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> AddBasketItemAsync([FromBody] AddBasketItemRequest data)
        {
            if (data == null || data.Quantity == 0)
            {
                return BadRequest("Invalid payload");
            }

            // Step 1: Get the item from catalog
            //var item = await _catalog.GetCatalogItemAsync(data.CatalogItemId);

            // Step 2: Get current basket status
            var currentBasket = (await _repository.GetBasketAsync(data.BasketId)) ?? new CustomerBasket(data.BasketId);
            // Step 3: Search if exist product into basket
            var product = currentBasket.Items.SingleOrDefault(i => i.ProductId == data.catalogItem.Id);
            if (product != null)
            {
                // Step 4: Update quantity for product
                product.Quantity += data.Quantity;
            }
            else
            {
                // Step 4: Merge current status with new product
                currentBasket.Items.Add(new BasketItem()
                {
                    UnitPrice = data.catalogItem.Price,
                    PictureUrl = data.catalogItem.PictureUri,
                    ProductId = data.catalogItem.Id,
                    ProductName = data.catalogItem.Name,
                    Quantity = data.Quantity,
                    Id = Guid.NewGuid().ToString()
                });
            }

            // Step 5: Update basket
            await _repository.UpdateBasketAsync(currentBasket);

            return Ok();
        }

    }
}
