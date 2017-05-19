using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Batch.Application.Customers;
using Demo.Batch.Application.Customers.Dto;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using System.Linq;

namespace Demo.Batch.Tests.Customers
{
    public class CustomerAppService_Tests : BatchTestBase
    {
        private readonly ICustomerAppService _service;

        public CustomerAppService_Tests(ITestOutputHelper output) : 
            base(output)
        {
            _service = Resolve<ICustomerAppService>();
        }

        [Fact, TestPriority(1)]
        public async Task GetCustomers_Test()
        {
            var list = await _service.GetCustomers();

            var json = JsonConvert.SerializeObject(list, Formatting.Indented);

            Output.WriteLine(json);
        }

        [Fact, TestPriority(2)]
        public async Task BatchCreate_Test()
        {
            var rs = await _service.GetCustomers();
            var count = rs.Items.Count;

            var list = new List<CreateOrUpdateCustomerInput>
            {
                new CreateOrUpdateCustomerInput
                {
                    FirstName = "Carl",
                    LastName = "Dai",
                    Age = 31
                },
                new CreateOrUpdateCustomerInput
                {
                    FirstName = "Perry",
                    LastName = "Yu",
                    Age = 31
                }
            };

            await _service.BatchCreate(list);
            rs = await _service.GetCustomers();
            rs.Items.Count.ShouldBe(count + list.Count);
        }

        //Notice : Run this method before you must run Batch Insert method.
        [Fact, TestPriority(3)]
        public async Task BatchDelete_Test()
        {
            var rsItem1 = await _service.GetCustomerByName("Carl", "Dai");
            var rsItem2 = await _service.GetCustomerByName("Perry", "Yu");

            var list = new List<long>
            {
                rsItem1.Items.ElementAt(0).Id,
                rsItem2.Items.ElementAt(0).Id
            };

            await _service.BatchDelete(list);

            var rs = await _service.GetCustomers();
            var count = rs.Items.Count(x => list.Contains(x.Id));

            count.ShouldBe(0);
        }

        [Fact]
        public void UtcTimeConvert()
        {
            Output.WriteLine(DateTime.Parse("2017-03-11 16:00").ToUniversalTime().ToString());
            Output.WriteLine(DateTime.Parse("2017-03-12 16:00").ToUniversalTime().ToString());
        }

        [Fact, TestPriority(4)]
        public async Task BatchUpdate_Test()
        {
            var list = new List<CreateOrUpdateCustomerInput>
            {
                new CreateOrUpdateCustomerInput
                {
                    FirstName = "Carl",
                    LastName = "Dai",
                    Age = 120
                },
                new CreateOrUpdateCustomerInput
                {
                    FirstName = "Perry",
                    LastName = "Yu",
                    Age = 120
                }
            };

            await _service.BatchCreate(list);

            var rs = await _service.GetCustomerByAge(120);
            //get all records count by age is 120
            var count120 = rs.Items.Count;

            //120 - 20 = 100
            await _service.BatchUpdateForCustomerAge(120, 20);

            rs = await _service.GetCustomerByAge(100);
            //get all records count by age is 100
            var count100 = rs.Items.Count;

            count120.ShouldBe(count100);

            var rsItem1 = await _service.GetCustomerByName("Carl", "Dai");
            var rsItem2 = await _service.GetCustomerByName("Perry", "Yu");

            var ids = new List<long>
            {
                rsItem1.Items.ElementAt(0).Id,
                rsItem2.Items.ElementAt(0).Id
            };

            await _service.BatchDelete(ids);

            rs = await _service.GetCustomerByAge(100);
            rs.Items.Count.ShouldBe(0);
        }
    }
}
