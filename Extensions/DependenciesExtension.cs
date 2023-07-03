using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderProject.Repositories;
using OrderProject.Repositories.Contracts;
using OrderProject.Services;
using OrderProject.Services.Contracts;

namespace OrderProject.Extension;

public static class DependenciesExtension
{
    public static void AddSqlConnection(this IServiceCollection services, string connectionString)
    {
        services.AddScoped(x => new SqlConnection(connectionString));
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.TryAddTransient<ICustomerRepository, CustomerRepository>();
        services.AddTransient<IPromoCodeRepository, PromoCodeRepository>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddTransient<IDeliveryFeeService, DeliveryFeeService>();
    }
}