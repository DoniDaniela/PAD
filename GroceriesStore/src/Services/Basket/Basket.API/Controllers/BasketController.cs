using Basket.API.Model;

namespace GroceriesStore.Services.Basket.API.Controllers;

[Route("api/v1/[controller]")]
//[Authorize]
[ApiController]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _repository;
    private readonly IIdentityService _identityService;
    private readonly ILogger<BasketController> _logger;

    public BasketController(
        ILogger<BasketController> logger,
        IBasketRepository repository,
        IIdentityService identityService)
    {
        _logger = logger;
        _repository = repository;
        _identityService = identityService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync(string id)
    {
        var basket = await _repository.GetBasketAsync(id);

        return Ok(basket ?? new CustomerBasket(id));
    }

    [HttpPost]
    public async Task<ActionResult<CustomerBasket>> UpdateBasketAsync([FromBody] CustomerBasket value)
    {
        return Ok(await _repository.UpdateBasketAsync(value));
    }


    // DELETE api/values/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task DeleteBasketByIdAsync(string id)
    {
        await _repository.DeleteBasketAsync(id);
    }

    [HttpDelete("{id}/{itemId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task DeleteBasketItemByIdAsync(string id, string itemId)
    {
        await _repository.DeleteBasketItemAsync(itemId, id);
    }

}
