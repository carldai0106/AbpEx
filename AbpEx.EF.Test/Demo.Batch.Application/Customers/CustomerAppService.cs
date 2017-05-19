using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System.Linq;
using Abp.EntityFramework.Extensions;
using Demo.Batch.Application.Customers.Dto;
using Demo.Batch.Customers;

namespace Demo.Batch.Application.Customers
{
    public class CustomerAppService : BatchAppServiceBase, ICustomerAppService
    {
        private readonly IRepository<Customer, long> _repository;
        public CustomerAppService(
            IRepository<Customer, long> repository)
        {
            _repository = repository;
        }

        public async Task<ListResultDto<CustomerListDto>> GetCustomers()
        {
            var list = await _repository.GetAllListAsync();
            return new ListResultDto<CustomerListDto>(list.MapTo<List<CustomerListDto>>());
        }

        public async Task BatchCreate(IEnumerable<CreateOrUpdateCustomerInput> input)
        {
            var list = input.Select(x =>
            {
                var entity = new Customer
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age

                };

                var prop = typeof(Customer).GetProperty("TenantId");
                prop?.SetValue(entity, 1);

                return entity;
            });

            await _repository.BatchInsertAsync(list);
        }

        public async Task<ListResultDto<CustomerListDto>> GetCustomerByName(string firstName, string lastName)
        {
            var list = await _repository.GetAllListAsync(x => x.FirstName.Contains(firstName) && x.LastName.Contains(lastName));

            return new ListResultDto<CustomerListDto>(list.MapTo<List<CustomerListDto>>());
        }

        public async Task BatchDelete(IEnumerable<long> input)
        {
            await _repository.BatchDeleteAsync(x => input.Contains(x.Id));
        }

        public async Task<ListResultDto<CustomerListDto>> GetCustomerByAge(int assignedAge)
        {
            var list = await _repository.GetAllListAsync(x => x.Age == assignedAge);
            return new ListResultDto<CustomerListDto>(list.MapTo<List<CustomerListDto>>());
        }

        public async Task<int> BatchUpdateForCustomerAge(int filterAge, int subtractAge)
        {
            return await _repository.BatchUpdateAsync(x => x.Age == filterAge, u => new Customer
            {
                Age = u.Age - subtractAge,
                FirstName = u.FirstName + " FirstName",
                LastName = u.LastName + " LastName"
            });
        }
       
    }
}
