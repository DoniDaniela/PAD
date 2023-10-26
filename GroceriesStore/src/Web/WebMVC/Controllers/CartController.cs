namespace GroceriesStore.WebMVC.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly IBasketService _basketSvc;
    private readonly ICatalogService _catalogSvc;
    private readonly IIdentityParser<ApplicationUser> _appUserParser;

    public CartController(IBasketService basketSvc, ICatalogService catalogSvc, IIdentityParser<ApplicationUser> appUserParser)
    {
        _basketSvc = basketSvc;
        _catalogSvc = catalogSvc;
        _appUserParser = appUserParser;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var vm = await _basketSvc.GetBasket(user);

            return View(vm);
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }

        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Index(Dictionary<string, int> quantities)
    {
        try
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var action = "update";
            var id = "";
            Basket basket;
            if (Request.Form.ContainsKey("deleteItem"))
            {
                action = "deleteItem";
                id = Request.Form["deleteItem"];
            }
            else if (Request.Form.ContainsKey("deleteBasket"))
            {
                action = "deleteBasket";
            }
            switch (action)
            {
                case "deleteItem":
                    basket = await _basketSvc.DeleteItem(user, id);
                    break;
                case "deleteBasket":
                    basket = await _basketSvc.Delete(user);
                    break;
                default:
                    basket = await _basketSvc.SetQuantities(user, quantities);
                    break;
            }

        }
        catch (Exception ex)
        {
            HandleException(ex);
        }

        return View();
    }

    public async Task<IActionResult> AddToCart(CatalogItem productDetails)
    {
        try
        {
            if (productDetails?.Id != null)
            {
                var user = _appUserParser.Parse(HttpContext.User);
                await _basketSvc.AddItemToBasket(user, productDetails.Id);
            }
            return RedirectToAction("Index", "Catalog");
        }
        catch (Exception ex)
        {
            // Catch error when Basket.api is in circuit-opened mode                 
            HandleException(ex);
        }

        return RedirectToAction("Index", "Catalog", new { errorMsg = ViewBag.BasketInoperativeMsg });
    }

    private void HandleException(Exception ex)
    {
        ViewBag.BasketInoperativeMsg = $"Basket Service is inoperative {ex.GetType().Name} - {ex.Message}";
    }
}
