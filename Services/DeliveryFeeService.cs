using OrderProject.Services.Contracts;
using RestSharp;

namespace OrderProject.Services;

public class DeliveryFeeService : IDeliveryFeeService
{
    private readonly Configuration _configuration;

    public DeliveryFeeService(Configuration configuration)
        => _configuration = configuration;

    public async Task<decimal> GetDeliveryFeeAsync(string zipCode)
    {
        // var client = new RestClient("https://consultafrete.io/cep/");
        var client = new RestClient(_configuration.DeliveryFeeServiceUrl);
        var request = new RestRequest().AddJsonBody(new { zipCode });

        var response = await client.PostAsync<decimal>(request, new CancellationToken());

        return response < 5 ? 5 : response;
    }
}
