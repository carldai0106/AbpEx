using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Demo.Batch.Application.Customers.Dto;

namespace Demo.Batch.Application.Customers
{
    public interface ICustomerAppService : IApplicationService
    {
        Task<ListResultDto<CustomerListDto>> GetCustomers();

        Task BatchCreate(IEnumerable<CreateOrUpdateCustomerInput> input);

        Task<ListResultDto<CustomerListDto>> GetCustomerByName(string firstName, string lastName);

        Task BatchDelete(IEnumerable<long> list);

        Task<ListResultDto<CustomerListDto>> GetCustomerByAge(int assignedAge);

        Task<int> BatchUpdateForCustomerAge(int filterAge, int subtractAge);
    }
}
