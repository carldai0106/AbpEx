using System;
using Abp.Domain.Entities;

namespace Abp.Dapper.Tests.Tests
{
    public class AA : Entity<int>
    {
        public string AAName { get; set; }
    }

    public class BB : Entity<long>
    {
        public string BBName { get; set; }
    }

    public class CC : Entity<Guid>
    {
        public string CCName { get; set; }
    }

    public class DD : Entity<string>
    {
        public string DDName { get; set; }
    }
}
