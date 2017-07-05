using Abp.Dapper.Tests.Entities;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Abp.Dapper.Tests.SqlServer
{
    public class DapperAppService_Tests : AbpDapperTestBase
    {     
        private IDapperAppService _appService;

        public DapperAppService_Tests(ITestOutputHelper output) : base(output)
        {           
            _appService = Resolve<IDapperAppService>();
        }

        private List<Person> GetRecords()
        {
            var persons = new List<Person>
            {
                new Person {
                   FirstName = "Jody",
                    LastName = "Sutton",
                    Age = 20,
                    TenantId = 1,
                    IsDeleted = false
                },
                new Person {
                    FirstName = "Emerson",
                    LastName = "Duncan",
                    Age = 23,
                    TenantId = 1,
                    IsDeleted = false},
                new Person {
                    FirstName = "Brook",
                    LastName = "Fraser",
                    Age = 25,
                    TenantId = 1,
                    IsDeleted = false},
                new Person {
                    FirstName = "Casey",
                    LastName = "Hart",
                    Age = 19,
                    TenantId = 1,
                    IsDeleted = false},
                new Person {
                    FirstName = "Jaden",
                    LastName = "Moore",
                    Age = 18,
                    TenantId = 1,
                    IsDeleted = false
                },
                new Person {
                    FirstName = "Val",
                    LastName = "Hart",
                    Age = 30,
                    TenantId = 1,
                    IsDeleted = false
                },
                new Person {
                    FirstName = "Val",
                    LastName = "Cardenas",
                    Age = 30,
                    TenantId = 1,
                    IsDeleted = false
                },
                new Person {
                    FirstName = "Caden",
                    LastName = "Murphy",
                    Age = 32,
                    TenantId = 1,
                    IsDeleted = true
                },
                new Person {
                    FirstName = "Brice",
                    LastName = "Crane",
                    Age = 26,
                    TenantId = 2,
                    IsDeleted = false
                },
                new Person {
                    FirstName = "Leslie",
                    LastName = "Lyons",
                    Age = 29,
                    TenantId = 2,
                    IsDeleted = false
                }
            };

            return persons;
        }       

        [Fact]
        public void Integration_Test()
        {
            var persons = GetRecords();
            _appService.BatchInsert(persons);

            //Get TenantId = 1, there is a person marked IsDeleted = true;
            var count = _appService.Count(string.Empty, null);
            count.ShouldBe(7);

            //Expression<Func<Person, bool>> p = x => x.FirstName == "Val"
            count = _appService.Count("FirstName", "Val");
            count.ShouldBe(2);

            //Expression<Func<Person, bool>> p = x => x.FirstName == "Val"
            //Action<Person> a = x => new Person { FirstName = "Carl" };
            _appService.BatchUpdate("Val", "Carl");

            //Expression<Func<Person, bool>> p = x => x.FirstName == "Carl"
            count = _appService.Count("FirstName", "Carl");
            count.ShouldBe(2);

            //Expression<Func<Person, bool>> p = x => x.FirstName == "Val"
            count = _appService.Count("FirstName", "Val");
            count.ShouldBe(0);

            //get all records when tenantid = 1 and isdeleted = false
            var list = _appService.GetList();
            var entity = list.FirstOrDefault();

            entity.IsDeleted.ShouldBe(false);

            //delete a record by id, actually it will be set IsDeleted = true;
            //It will be logic deleted
            _appService.Delete(entity.Id);

            //get a record by id
            entity = _appService.Query("select * from person where id = @id", new Person { Id = entity.Id }).FirstOrDefault();
            entity.IsDeleted.ShouldBe(true);

            list = _appService.GetList();
            entity = list.FirstOrDefault();

            //delete a record by entity
            _appService.Delete(entity);
            entity = _appService.Query("select * from person where id = @id", new Person { Id = entity.Id }).FirstOrDefault();
            entity.IsDeleted.ShouldBe(true);

            //delete all recods if first name is "carl";
            //Expression<Func<Person, bool>> p = x => x.FirstName == "Carl"
            _appService.Delete("FirstName", "Carl");
            list = _appService.GetList("FirstName", "Carl");
            list.Count().ShouldBe(0);

            count = _appService.Execute("update person set firstname = 'Val', isdeleted = 0 where firstname = 'Carl' ");
            count.ShouldBe(2);

            //paging
            count = _appService.Count(string.Empty);
            int pageSize = 2;
            int pageCount = count % pageSize == 0 ? count / pageSize : (count / pageSize) + 1;

            for (int i = 0; i < pageCount; i++)
            {
                list = _appService.GetListPaged(string.Empty, string.Empty, i, pageSize);
                if (i == pageCount - 1)
                {
                    list.Count().ShouldBe(pageCount * pageSize - count);
                }
                else
                {
                    list.Count().ShouldBe(2);
                }
            }

            //get a set
            var list1 = _appService.GetSet(string.Empty, string.Empty, 1, 5, true);
            list1.Count().ShouldBe(5);

            var list2 = _appService.GetSet(string.Empty, string.Empty, 1, 5, false);
            list2.Count().ShouldBe(5);

            list1.FirstOrDefault().Id.ShouldNotBe(list2.FirstOrDefault().Id);

            Output.WriteLine(JsonConvert.SerializeObject(list1, Formatting.Indented));

            _appService.Insert(new Person
            {
                Age = 60,
                FirstName = "Hello",
                LastName = "Dapper",
                TenantId = 1
            });

            list = _appService.GetList("FirstName", "Hello");
            list.FirstOrDefault().LastName.ShouldBe("Dapper");
            list.FirstOrDefault().TenantId.ShouldBe(1);

            entity = list.FirstOrDefault();

            entity.LastName = "World";
            _appService.Update(entity);

            _appService.Get(entity.Id).LastName.ShouldBe("World");
        }

        [Fact]
        public void GetByTenantId()
        {
            var persons = GetRecords();
            _appService.BatchInsert(persons);

            var list = _appService.GetByTenantId(1);
            list.Count().ShouldBe(7);

            list = _appService.GetByTenantId(2);
            list.Count().ShouldBe(2);
        }
        [Fact]
        public void GetDisabledAllFilter()
        {
            var persons = GetRecords();
            _appService.BatchInsert(persons);

            var list = _appService.GetDisabledAllFilter();
            list.Count().ShouldBe(10);
        }
        [Fact]
        public void GetDisabledTenant()
        {
            var persons = GetRecords();
            _appService.BatchInsert(persons);

            var list = _appService.GetDisabledTenant();
            list.Count().ShouldBe(9);
        }
        [Fact]
        public void GetDisabledSoftDelete()
        {
            var persons = GetRecords();
            _appService.BatchInsert(persons);

            var list = _appService.GetDisabledSoftDelete();
            list.Count().ShouldBe(8);
        }
        [Fact]
        public void BatchInsertRollBack()
        {
            var persons = GetRecords();
            Should.Throw<Exception>(() =>
            {
                _appService.BatchInsertRollBack(persons);
            }).Message.ShouldBe("Rollback");

            _appService.GetList().Count().ShouldBe(0);
        }

        [Fact]
        public void InsertRollBack()
        {
            var persons = GetRecords();
            Should.Throw<Exception>(() =>
            {
                _appService.InsertRollBack(persons);
            }).Message.ShouldBe("Rollback");

            _appService.GetList().Count().ShouldBe(0);
        }

        [Fact]
        public void Update()
        {
            var persons = GetRecords();
            _appService.BatchInsert(persons);

            _appService.Update(new Person
            {
                Id = 1,
                FirstName = "Carl1",
                LastName = "Dai1"
            });
        }
    }
}
