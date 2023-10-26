namespace GroceriesStore.WebMVC.Services;

public interface IIdentityParser<T>
{
    T Parse(IPrincipal principal);
}
