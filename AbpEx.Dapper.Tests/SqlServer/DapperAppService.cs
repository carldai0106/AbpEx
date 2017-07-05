using Abp.Application.Services;
using Abp.Dapper.Repositories;
using System.Collections.Generic;
using System;
using Abp.Domain.Uow;
using Abp.Dapper.Tests.Entities;
using System.Linq.Expressions;
using Abp.Utils;
using Abp.Extensions;

namespace Abp.Dapper.Tests.SqlServer
{
    public class DapperAppService : ApplicationService, IDapperAppService
    {
        private readonly IDapperRepository<Person> _repository;
        private IUnitOfWorkManager _uowManager;       
        public DapperAppService(
            IDapperRepository<Person> repository,
            IUnitOfWorkManager uowManager
            )
        {
            _repository = repository;
            _uowManager = uowManager;            
        }       

        public void BatchInsert(IEnumerable<Person> entities)
        {
            _repository.BatchInsert(entities);
        }

        public void BatchUpdate(string oldFirstName, string newFirstName)
        {
            _repository.BatchUpdate(x => x.FirstName == oldFirstName, b => b.FirstName = newFirstName);
        }

        private Expression<Func<Person, bool>> GetPerdicate(string fieldName, object fieldValue = null)
        {
            Expression<Func<Person, bool>> predicate = null;
            if (fieldName.IsNullOrEmpty() || fieldValue == null)
            {
                predicate = null;
            }
            else
            {
                var propType = typeof(Person).GetProperty(fieldName);
                predicate = ExpressionUtils.MakePredicate<Person>(fieldName, fieldValue, propType.PropertyType);
            }

            return predicate;
        }

        public int Count(string fieldName, object fieldValue = null)
        {
            var predicate = GetPerdicate(fieldName, fieldValue );
            return _repository.Count(predicate);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public void Delete(Person entity)
        {
            _repository.Delete(entity);
        }

        public void Delete(string fieldName, object fieldValue)
        {
            var propType = typeof(Person).GetProperty(fieldName);
            var predicate = ExpressionUtils.MakePredicate<Person>(fieldName, fieldValue, propType.PropertyType);
            _repository.Delete(predicate);
        }

        public int Execute(string sql, object parameters = null)
        {
            return _repository.Execute(sql, parameters);
        }

        public Person Get(int id)
        {
            return _repository.Get(id);
        }

        public IEnumerable<Person> GetList()
        {
            return _repository.GetList();
        }

        public IEnumerable<Person> GetList(string fieldName, object fieldValue)
        {
            var predicate = GetPerdicate(fieldName, fieldValue);
            return _repository.GetList(predicate);
        }

        public IEnumerable<Person> GetListPaged(string fieldName, object fieldValue, int pageNumber, int itemsPerPage, string sortingProperty, bool ascending = true)
        {
            var predicate = GetPerdicate(fieldName, fieldValue);
            return _repository.GetListPaged(predicate, pageNumber, itemsPerPage, sortingProperty, ascending);
        }

        public IEnumerable<Person> GetListPaged(string fieldName, object fieldValue, int pageNumber, int itemsPerPage, bool ascending = true)
        {
            var predicate = GetPerdicate(fieldName, fieldValue);
            return _repository.GetListPaged(predicate, pageNumber, itemsPerPage, ascending, x => x.Id);
        }

        public IEnumerable<Person> GetSet(string fieldName, object fieldValue, int firstResult, int maxResults, string sortingProperty, bool ascending = true)
        {
            var predicate = GetPerdicate(fieldName, fieldValue);
            return _repository.GetSet(predicate, firstResult, maxResults, sortingProperty, ascending);
        }

        public IEnumerable<Person> GetSet(string fieldName, object fieldValue, int firstResult, int maxResults, bool ascending = true)
        {
            var predicate = GetPerdicate(fieldName, fieldValue);
            return _repository.GetSet(predicate, firstResult, maxResults,  ascending, x => x.Id);
        }

        public void Insert(Person entity)
        {
             _repository.Insert(entity);
        }

        public int InsertAndGetId(Person entity)
        {
            return _repository.InsertAndGetId(entity);
        }

        public IEnumerable<Person> Query(string query, object parameters)
        {
            return _repository.Query(query, parameters);
        }

        public IEnumerable<TAny> Query<TAny>(string query, object parameters) where TAny : class
        {
            return _repository.Query<TAny>(query, parameters);
        }

        public IEnumerable<TAny> Query<TAny>(string query) where TAny : class
        {
            return _repository.Query<TAny>(query);
        }

        public void Update(Person entity)
        {
            _repository.Update(entity);
        }

        public IEnumerable<Person> GetByTenantId(int? tenantId)
        {
            using (var uow = UnitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return _repository.GetList();
            }
        }

        public IEnumerable<Person> GetDisabledAllFilter()
        {
            using (var uow = UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant, AbpDataFilters.SoftDelete))
            {
                return _repository.GetList();
            }
        }

        public IEnumerable<Person> GetDisabledTenant()
        {
            using (var uow = UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                return _repository.GetList();
            }
        }

        public IEnumerable<Person> GetDisabledSoftDelete()
        {
            using (var uow = UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                return _repository.GetList();
            }
        }

        public IEnumerable<Person> BatchInsertRollBack(IEnumerable<Person> entities)
        {
            _repository.BatchInsert(entities);
            throw new Exception("Rollback");
        }

        public void InsertRollBack(IEnumerable<Person> entities)
        {
            var i = 0;
            foreach (var item in entities)
            {
                _repository.Insert(item);
                if (i == 2)
                    throw new Exception("Rollback");
                i++;
            }
        }
    }
}
