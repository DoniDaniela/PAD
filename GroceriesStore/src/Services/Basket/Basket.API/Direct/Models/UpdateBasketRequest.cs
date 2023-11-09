namespace Basket.API.Direct.Models;

public class UpdateBasketRequest
{
    public string BuyerId { get; set; }

    public IEnumerable<UpdateBasketRequestItemData> Items { get; set; }

    public IEnumerable<CatalogItem> catalogItems { get; set; }
}
