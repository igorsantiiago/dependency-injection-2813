using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OrderProject.Models;
using RestSharp;

namespace OrderProject.Controllers;

public class OrderProjectController : ControllerBase
{
    [Route("v1/orders")]
    [HttpPost]
    public async Task<IActionResult> Place(string customerId, string zipCode, string promoCode, int[] products)
    {
        // # 1 - Recupera o cliente
        Customer customer = null;
        await using (var conn = new SqlConnection("CONN_STRING"))
        {
            const string query = "SELECT [Id], [Name], [Email] FROM CUSTOMER WHERE ID=@id";
            customer = await conn.QueryFirstAsync<Customer>(query, new { id = customerId });
        }

        // # 2 - Calcula o frete
        decimal deliveryFee = 0;

        var client = new RestClient("https://consultafrete.io/cep/");
        var request = new RestRequest().AddJsonBody(new { zipCode });

        deliveryFee = await client.PostAsync<decimal>(request, new CancellationToken());

        if (deliveryFee < 5)
            deliveryFee = 5;

        // # 3 - Calcula o total de Produtos
        decimal subTotal = 0;

        const string getProductQuery = "SELECT [Id], [Name], [Price] FROM PRODUCT WHERE ID=@id";

        for (var p = 0; p < products.Length; p++)
        {
            Product product;
            await using (var conn = new SqlConnection("CONN_STRING"))
            {
                product = await conn.QueryFirstAsync<Product>(getProductQuery, new { Id = p });
            }

            subTotal += product.Price;
        }

        // # 4 - Aplica o cupom de desconto
        decimal discount = 0;

        await using (var conn = new SqlConnection("CONN_STRING"))
        {
            const string query = "SELECT * FROM PROMO_CODES WHERE CODE=@code";
            var promo = await conn.QueryFirstAsync<PromoCode>(query, new { code = promoCode });

            if (promo.ExpireDate > DateTime.Now)
                discount = promo.Value;
        }

        // # 5 - Gera o pedido
        var order = new Order
        {
            Code = Guid.NewGuid().ToString().ToUpper().Substring(0, 8),
            Date = DateTime.Now,
            DeliveryFee = deliveryFee,
            Discount = discount,
            Products = products,
            SubTotal = subTotal
        };

        // # 6 - Calcula o total
        order.Total = subTotal - discount + deliveryFee;

        // # 7 - Retorna
        return Ok(new
        {
            Message = $"Pedido {order.Code} gerado com sucesso!"
        });

    }
}