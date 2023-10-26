namespace WebMVC.Infrastructure;

public static class API
{

    public static class Purchase
    {
        public static string AddItemToBasket(string baseUri) => $"{baseUri}/basket/items";
        public static string UpdateBasketItem(string baseUri) => $"{baseUri}/basket/items";

        public static string DeleteBasketItem(string baseUri) => $"{baseUri}/basket/items";

        public static string DeleteBasket(string baseUri) => $"{baseUri}/basket/items";

    }

    public static class Basket
    {
        public static string GetBasket(string baseUri, string basketId) => $"{baseUri}/{basketId}";
        public static string UpdateBasket(string baseUri) => baseUri;
        public static string CheckoutBasket(string baseUri) => $"{baseUri}/checkout";
        public static string CleanBasket(string baseUri, string basketId) => $"{baseUri}/{basketId}";
    }

    public static class Catalog
    {
        public static string GetAllCatalogItems(string baseUri, int page, int take, int? brand, int? type)
        {
            var filterQs = "";

            if (type.HasValue)
            {
                var brandQs = (brand.HasValue) ? brand.Value.ToString() : string.Empty;
                filterQs = $"/type/{type.Value}/brand/{brandQs}";

            }
            else if (brand.HasValue)
            {
                var brandQs = (brand.HasValue) ? brand.Value.ToString() : string.Empty;
                filterQs = $"/type/all/brand/{brandQs}";
            }
            else
            {
                filterQs = string.Empty;
            }

            return $"{baseUri}items{filterQs}?pageIndex={page}&pageSize={take}";
        }

        public static string GetAllBrands(string baseUri)
        {
            return $"{baseUri}catalogBrands";
        }

        public static string GetAllTypes(string baseUri)
        {
            return $"{baseUri}catalogTypes";
        }
    }
}
