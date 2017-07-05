using Abp.Dapper.Repositories;
using Abp.Dapper.Tests.Entities;
using Abp.Domain.Uow;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Abp.Dapper.Tests.SqlServer
{
    public class DapperRepository_Tests : AbpDapperTestBase
    {       
        private readonly IDapperRepository<Person> _personDapperRepository;      
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DapperRepository_Tests(ITestOutputHelper output) : base(output)
        {
            _personDapperRepository = Resolve<IDapperRepository<Person>>();           
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public void Dapper_Repository_Tests()
        {
            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin())
            {
                //---Insert operation should work and tenant, creation audit properties must be set---------------------
                _personDapperRepository.Insert(new Person
                {
                    Age = 52,
                    FirstName = "Dapper",
                    LastName = "Abpex",
                    TenantId = 1
                });
                var insertedPerson = _personDapperRepository.FirstOrDefault(x => x.FirstName == "Dapper");

                insertedPerson.ShouldNotBeNull();
                insertedPerson.TenantId.ShouldBe(AbpSession.TenantId);
                insertedPerson.CreationTime.ShouldNotBeNull();
                insertedPerson.CreatorUserId.ShouldBe(AbpSession.UserId);

                //----Update operation should work and Modification Audits should be set---------------------------                
                var personToUpdate = insertedPerson;
                personToUpdate.FirstName = "Dapper_Updated";
                _personDapperRepository.Update(personToUpdate);

                personToUpdate.ShouldNotBeNull();
                personToUpdate.TenantId.ShouldBe(AbpSession.TenantId);
                personToUpdate.LastModificationTime.ShouldNotBeNull();
                personToUpdate.LastModifierUserId.ShouldBe(AbpSession.UserId);
                
                //----Select * from syntax should work---------------------------------
                IEnumerable<Person> persons = _personDapperRepository.Query("select * from person");
                persons.Count().ShouldBeGreaterThan(0);              

                //------Soft Delete should work for Dapper--------------
                _personDapperRepository.Insert(new Person
                {
                    Age = 44,
                    FirstName = "SoftDelete",
                    LastName = "Abpex",
                    TenantId = 1
                });

                var toSoftDeletePerson = _personDapperRepository.FirstOrDefault(x => x.FirstName == "SoftDelete");
                _personDapperRepository.Delete(toSoftDeletePerson);

                toSoftDeletePerson.IsDeleted.ShouldBe(true);
                toSoftDeletePerson.DeleterUserId.ShouldBe(AbpSession.UserId);
                toSoftDeletePerson.TenantId.ShouldBe(AbpSession.TenantId);

                var softDeletedPerson = _personDapperRepository.FirstOrDefault(x => x.FirstName == "SoftDelete");
                softDeletedPerson.ShouldBeNull();

                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {                    
                    var softDeletedPersonWhenFilterDisabled = _personDapperRepository.Single(x => x.FirstName == "SoftDelete");
                    softDeletedPersonWhenFilterDisabled.ShouldNotBeNull();
                }

                //using (AbpSession.Use(2, 266))
                //{
                //    int personWithTenant2Id = _personDapperRepository.InsertAndGetId(new Person
                //    {
                //        Age = 44,
                //        FirstName = "Tenant2",
                //        LastName = "Abpex",
                //        TenantId = 2
                //    });

                //    var personWithTenant2 = _personDapperRepository.GetList(x => x.FirstName == "Tenant2").FirstOrDefault();

                //    personWithTenant2.TenantId.ShouldBe(2); //Not sure about that?,Because we changed TenantId to 2 in this scope !!! Abp.TenantId = 2 now NOT 1 !!!
                //}

                using (_unitOfWorkManager.Current.SetTenantId(3))
                {
                    int personWithTenant3Id = _personDapperRepository.InsertAndGetId(new Person
                    {
                        Age = 44,
                        FirstName = "Tenant3",
                        LastName = "Abpex",
                        TenantId = 3
                    });

                    var personWithTenant3 = _personDapperRepository.Get(personWithTenant3Id);

                    personWithTenant3.TenantId.ShouldBe(3);
                }

                var personWithTenantId3FromDapper = _personDapperRepository.FirstOrDefault(x => x.FirstName == "Tenant3");
                personWithTenantId3FromDapper.ShouldBeNull();

                using (_unitOfWorkManager.Current.SetTenantId(3))
                {
                    var personWithTenantId3FromDapperInsideTenantScope = _personDapperRepository.FirstOrDefault(x => x.FirstName == "Tenant3");
                    personWithTenantId3FromDapperInsideTenantScope.ShouldNotBeNull();
                }

                //About issue-#2091
                using (_unitOfWorkManager.Current.SetTenantId(AbpSession.TenantId))
                {
                    int personWithTenantId40 = _personDapperRepository.InsertAndGetId(new Person
                    {
                        Age = 44,
                        FirstName = "Tenant3",
                        LastName = "Abpex",
                        TenantId = AbpSession.TenantId
                    });

                    var entity = _personDapperRepository.Get(personWithTenantId40);

                    entity.TenantId.ShouldBe(AbpSession.TenantId);
                    entity.CreatorUserId.ShouldBe(AbpSession.UserId);
                }

                uow.Complete();
            }
        }
    }
}
