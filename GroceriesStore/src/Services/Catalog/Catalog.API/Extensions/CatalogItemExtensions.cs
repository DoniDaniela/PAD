namespace GroceriesStore.Services.Catalog.API.Model;

public static class CatalogItemExtensions
{
    public static void FillProductUrl(this CatalogItem item, string picBaseUrl)
    {
        if (item != null)
        {
            item.PictureUri = picBaseUrl.Replace("[0]", item.Id.ToString());
        }
    }
}
