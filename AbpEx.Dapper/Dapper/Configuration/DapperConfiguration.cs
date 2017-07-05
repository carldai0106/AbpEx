using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using DapperExtensions;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using Abp.Dapper.DbContext;

namespace Abp.Dapper.Configuration
{
    public class DapperConfiguration : IDapperConfiguration
    {
        public static IDapperConfiguration Instance { get; private set; }
        public IList<Assembly> MappingAssemblies { get; private set; }
        public Type DefaultMapper { get; private set; }
        public IDbConnection DbConnection { get; private set; }
        public ISqlDialect Dialect { get; private set; }
        public ISqlGenerator SqlGenerator { get; private set; }

        static DapperConfiguration()
        {
            Instance = new DapperConfiguration();
        }

        public IDapperDbContext Build()
        {
            if (DefaultMapper == null)
                DefaultMapper = typeof (AutoClassMapper<>);

            if (Dialect == null)
                throw new ArgumentNullException("Dialect is null.");

            var config = new DapperExtensionsConfiguration(DefaultMapper, MappingAssemblies, Dialect);
            var sqlGenerator = new SqlGeneratorImpl(config);

            SqlGenerator = sqlGenerator;

            var context = new DapperDbContext(DbConnection, sqlGenerator);

            return context;
        }

        public IDapperConfiguration FromAssembly(IList<Assembly> mappingAssemblies)
        {
            MappingAssemblies = (List<Assembly>) (mappingAssemblies ?? new List<Assembly>());
            return this;
        }

        public IDapperConfiguration UseConnection(IDbConnection connection)
        {
            DbConnection = connection;
            return this;
        }

        public IDapperConfiguration UseClassMapper(Type typeOfMapper)
        {
            if (typeof(IClassMapper).IsAssignableFrom(typeOfMapper) == false)
                throw new NullReferenceException("Mapping is not type of IClassMapper");

            DefaultMapper = typeOfMapper;
            return this;
        }

        public IDapperConfiguration UseSqlDialect(ISqlDialect dialect)
        {
            Dialect = dialect;
            return this;
        }
    }
}
