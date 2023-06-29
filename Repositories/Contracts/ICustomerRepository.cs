using OrderProject.Models;

namespace OrderProject.Repositories.Contracts;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(string customerId);
}