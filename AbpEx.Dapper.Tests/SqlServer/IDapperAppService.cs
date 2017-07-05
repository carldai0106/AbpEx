using Abp.Application.Services;
using Abp.Dapper.Tests.Entities;
using System.Collections.Generic;

namespace Abp.Dapper.Tests.SqlServer
{
    public interface IDapperAppService : IApplicationService
    {      
        void BatchInsert(IEnumerable<Person> entities);

        void BatchUpdate(string oldFirstName, string newFirstName);

        int Count(string fieldName, object fieldValue = null);

        void Delete(int id);

        void Delete(Person entity);

        void Delete(string fieldName, object fieldValue);

        Person Get(int id);

        IEnumerable<Person> GetList();

        IEnumerable<Person> GetList(string fieldName, object fieldValue);

        IEnumerable<Person> GetListPaged(string fieldName, object fieldValue, int pageNumber, int itemsPerPage,  string sortingProperty, bool ascending = true);


        IEnumerable<Person> GetListPaged(string fieldName, object fieldValue, int pageNumber, int itemsPerPage, bool ascending = true);


        IEnumerable<Person> GetSet(string fieldName, object fieldValue, int firstResult, int maxResults, string sortingProperty, bool ascending = true);


        IEnumerable<Person> GetSet(string fieldName, object fieldValue, int firstResult, int maxResults, bool ascending = true);

        void Insert(Person entity);

        int InsertAndGetId(Person entity);

        IEnumerable<Person> Query(string query, object parameters);

        IEnumerable<TAny> Query<TAny>(string query, object parameters) where TAny : class;

        IEnumerable<TAny> Query<TAny>(string query) where TAny : class;

        void Update(Person entity);

        int Execute(string sql, object parameters = null);

        IEnumerable<Person> GetByTenantId(int? tenantId);

        IEnumerable<Person> GetDisabledAllFilter();

        IEnumerable<Person> GetDisabledTenant();

        IEnumerable<Person> GetDisabledSoftDelete();

        IEnumerable<Person> BatchInsertRollBack(IEnumerable<Person> entities);

        void InsertRollBack(IEnumerable<Person> entities);
    }
}
