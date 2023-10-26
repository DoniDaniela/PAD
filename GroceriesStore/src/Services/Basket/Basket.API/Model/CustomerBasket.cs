namespace GroceriesStore.Services.Basket.API.Model;

public class CustomerBasket
{
    public int Id { get; set; }

    public string BuyerId { get; set; }

    public virtual List<BasketItem> Items { get; set; } = new List<BasketItem>();

    public CustomerBasket()
    {

    }

    public CustomerBasket(string customerId)
    {
        BuyerId = customerId;
    }
}

