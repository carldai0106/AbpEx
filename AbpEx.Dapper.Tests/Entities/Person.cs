using System.Collections.Generic;
using Abp.Domain.Entities.Auditing;
using DapperExtensions.Mapper;
using Abp.Domain.Entities;

namespace Abp.Dapper.Tests.Entities
{
    public class Person : FullAuditedEntity<int>, IMayHaveTenant
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public IEnumerable<Phone> Phones { get; private set; }
        public int? TenantId { get; set; }
    }

    public class Phone
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }

    public class PersonMapper : ClassMapper<Person>
    {
        public PersonMapper()
        {
            Table("Person");
            Map(m => m.Id).Key(KeyType.Identity);
            Map(m => m.Phones).Ignore();
            AutoMap();
        }
    }
}
