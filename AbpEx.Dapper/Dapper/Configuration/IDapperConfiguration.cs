using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using DapperExtensions.Sql;
using Abp.Dapper.DbContext;

namespace Abp.Dapper.Configuration
{
    public interface IDapperConfiguration
    {
        IList<Assembly> MappingAssemblies { get; }

        Type DefaultMapper { get; }

        IDbConnection DbConnection { get; }

        ISqlDialect Dialect { get; }

        ISqlGenerator SqlGenerator { get; }

        IDapperConfiguration UseClassMapper(Type typeOfMapper);

        IDapperConfiguration FromAssembly(IList<Assembly> mappingAssemblies);

        IDapperConfiguration UseConnection(IDbConnection connection);

        IDapperConfiguration UseSqlDialect(ISqlDialect dialect);

        IDapperDbContext Build();
    }
}
