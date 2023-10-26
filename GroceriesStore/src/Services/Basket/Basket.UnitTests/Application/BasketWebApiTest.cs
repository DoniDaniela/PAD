namespace UnitTest.Basket.Application;

using GroceriesStore.Services.Basket.API.Model;

public class BasketWebApiTest
{
    private readonly Mock<IBasketRepository> _basketRepositoryMock;
    private readonly Mock<IBasketIdentityService> _identityServiceMock;
    private readonly Mock<ILogger<BasketController>> _loggerMock;

    public BasketWebApiTest()
    {
        _basketRepositoryMock = new Mock<IBasketRepository>();
        _identityServiceMock = new Mock<IBasketIdentityService>();
        _loggerMock = new Mock<ILogger<BasketController>>();
    }

    [Fact]
    public async Task Get_customer_basket_success()
    {
        //Arrange
        var fakeCustomerId = "1";
        var fakeCustomerBasket = GetCustomerBasketFake(fakeCustomerId);

        _basketRepositoryMock.Setup(x => x.GetBasketAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(fakeCustomerBasket));
        _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId);

        //Act
        var basketController = new BasketController(
            _loggerMock.Object,
            _basketRepositoryMock.Object,
            _identityServiceMock.Object);

        var actionResult = await basketController.GetBasketByIdAsync(fakeCustomerId);

        //Assert
        Assert.Equal((actionResult.Result as OkObjectResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
        Assert.Equal((((ObjectResult)actionResult.Result).Value as CustomerBasket).BuyerId, fakeCustomerId);
    }

    [Fact]
    public async Task Post_customer_basket_success()
    {
        //Arrange
        var fakeCustomerId = "1";
        var fakeCustomerBasket = GetCustomerBasketFake(fakeCustomerId);

        _basketRepositoryMock.Setup(x => x.UpdateBasketAsync(It.IsAny<CustomerBasket>()))
            .Returns(Task.FromResult(fakeCustomerBasket));
        _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId);

        //Act
        var basketController = new BasketController(
            _loggerMock.Object,
            _basketRepositoryMock.Object,
            _identityServiceMock.Object);

        var actionResult = await basketController.UpdateBasketAsync(fakeCustomerBasket);

        //Assert
        Assert.Equal((actionResult.Result as OkObjectResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
        Assert.Equal((((ObjectResult)actionResult.Result).Value as CustomerBasket).BuyerId, fakeCustomerId);
    }

    private CustomerBasket GetCustomerBasketFake(string fakeCustomerId)
    {
        return new CustomerBasket(fakeCustomerId)
        {
            Items = new List<BasketItem>()
            {
                new BasketItem()
            }
        };
    }
}
