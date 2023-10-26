namespace Basket.FunctionalTests;

public class BasketScenarios :
    BasketScenarioBase
{
    [Fact]
    public async Task Post_basket_and_response_ok_status_code()
    {
        using var server = CreateServer();
        var content = new StringContent(BuildBasket(), UTF8Encoding.UTF8, "application/json");
        var uri = "/api/v1/basket/";
        var response = await server.CreateClient().PostAsync(uri, content);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_basket_and_response_ok_status_code()
    {
        using var server = CreateServer();
        var response = await server.CreateClient()
            .GetAsync(Get.GetBasket(1));
        response.EnsureSuccessStatusCode();
    }

    string BuildBasket()
    {
        var order = new CustomerBasket(AutoAuthorizeMiddleware.IDENTITY_ID);

        order.Items.Add(new BasketItem
        {
            ProductId = 1,
            ProductName = ".NET Bot Black Hoodie",
            UnitPrice = 10,
            Quantity = 1
        });

        return JsonSerializer.Serialize(order);
    }
}
