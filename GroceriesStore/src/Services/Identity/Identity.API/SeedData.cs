namespace GroceriesStore.Services.Identity.API;

public class SeedData
{
    public static async Task EnsureSeedData(IServiceScope scope, IConfiguration configuration, ILogger logger)
    {
        var retryPolicy = CreateRetryPolicy(configuration, logger);
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await retryPolicy.ExecuteAsync(async () =>
        {
            await context.Database.MigrateAsync();

            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var alice = await userMgr.FindByNameAsync("alice");

            if (alice == null)
            {
                alice = new ApplicationUser
                {
                    UserName = "dana",
                    Email = "doni.daniela01@gmail.com",
                    EmailConfirmed = true,
                    CardHolderName = "Doni Daniela",
                    CardNumber = "4012888888881881",
                    CardType = 1,
                    City = "Chisinau",
                    Country = "MD",
                    Expiration = "12/24",
                    Id = Guid.NewGuid().ToString(),
                    LastName = "Doni",
                    Name = "Dana",
                    PhoneNumber = "1234567890",
                    ZipCode = "MD2009",
                    State = "MD",
                    Street = "Dr. Viilor 29",
                    SecurityNumber = "123"
                };

                var result = userMgr.CreateAsync(alice, "Pass123$").Result;

                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                logger.LogDebug("alice created");
            }
            else
            {
                logger.LogDebug("alice already exists");
            }

            
        });
    }

    private static AsyncPolicy CreateRetryPolicy(IConfiguration configuration, ILogger logger)
    {
        var retryMigrations = false;
        bool.TryParse(configuration["RetryMigrations"], out retryMigrations);

        // Only use a retry policy if configured to do so.
        // When running in an orchestrator/K8s, it will take care of restarting failed services.
        if (retryMigrations)
        {
            return Policy.Handle<Exception>().
                WaitAndRetryForeverAsync(
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, retry, timeSpan) => logger.LogWarning(exception, "Error migrating database (retry attempt {retry})", retry));
        }

        return Policy.NoOpAsync();
    }
}
