namespace UnitTest.Basket.Application;

public class CartControllerTest
{
    private readonly Mock<ICatalogService> _catalogServiceMock;
    private readonly Mock<IBasketService> _basketServiceMock;
    private readonly Mock<IIdentityParser<ApplicationUser>> _identityParserMock;
    private readonly Mock<HttpContext> _contextMock;

    public CartControllerTest()
    {
        _catalogServiceMock = new Mock<ICatalogService>();
        _basketServiceMock = new Mock<IBasketService>();
        _identityParserMock = new Mock<IIdentityParser<ApplicationUser>>();
        _contextMock = new Mock<HttpContext>();
    }

    [Fact]
    public async Task Add_to_cart_success()
    {
        //Arrange
        var fakeCatalogItem = GetFakeCatalogItem();

        _basketServiceMock.Setup(x => x.AddItemToBasket(It.IsAny<ApplicationUser>(), It.IsAny<int>()))
            .Returns(Task.FromResult(1));

        //Act
        var orderController = new CartController(_basketServiceMock.Object, _catalogServiceMock.Object, _identityParserMock.Object);
        orderController.ControllerContext.HttpContext = _contextMock.Object;
        var actionResult = await orderController.AddToCart(fakeCatalogItem);

        //Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(actionResult);
        Assert.Equal("Catalog", redirectToActionResult.ControllerName);
        Assert.Equal("Index", redirectToActionResult.ActionName);
    }

    private BasketModel GetFakeBasket(string buyerId)
    {
        return new BasketModel()
        {
            BuyerId = buyerId
        };
    }

    private CatalogItem GetFakeCatalogItem()
    {
        return new CatalogItem()
        {
            Id = 1,
            Name = "fakeName",
            CatalogBrand = "fakeBrand",
            CatalogType = "fakeType",
            CatalogBrandId = 2,
            CatalogTypeId = 5,
            Price = 20
        };
    }
}
