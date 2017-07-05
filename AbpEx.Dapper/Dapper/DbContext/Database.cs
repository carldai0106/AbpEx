using Dapper;
using DapperExtensions;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using System;
using System.Collections.Generic;
using System.Data;

namespace Abp.Dapper.DbContext
{
    public class Database
    {
        private readonly IDapperImplementor _dapper;

        private IDbTransaction _transaction;

        public int? CommandTimeout { get; set; }

        public bool HasActiveTransaction { get { return _transaction != null; } }

        public IDbConnection Connection { get; private set; }

        public Database(IDbConnection connection, ISqlGenerator sqlGenerator)
        {
            _dapper = new DapperImplementor(sqlGenerator);
            Connection = connection;

            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }        

        public IDbTransaction UseTransaction(IDbTransaction transaction)
        {
            _transaction = transaction;
            return _transaction;
        }

        //public void Dispose()
        //{
        //    if (Connection.State != ConnectionState.Closed)
        //    {                
        //        if (_transaction != null)
        //        {
        //            _transaction.Rollback();
        //        }

        //        Connection.Close();
        //    }
        //}

        public void Dispose()
        {
            if (Connection.State != ConnectionState.Closed)
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }

                Connection.Close();
            }
        }

        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _transaction = Connection.BeginTransaction(isolationLevel);
            return _transaction;
        }

        //public void Commit()
        //{
        //    if (_transaction != null)
        //    {
        //        _transaction.Commit();
        //        _transaction = null;
        //    }
        //}

        //public void Rollback()
        //{
        //    if (_transaction != null)
        //    {
        //        _transaction.Rollback();
        //        _transaction = null;
        //    }
        //}

        //public void RunInTransaction(Action action)
        //{
        //    BeginTransaction();
        //    try
        //    {
        //        action();
        //        Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (HasActiveTransaction)
        //        {
        //            Rollback();
        //        }

        //        throw ex;
        //    }
        //}

        //public T RunInTransaction<T>(Func<T> func)
        //{
        //    BeginTransaction();
        //    try
        //    {
        //        T result = func();
        //        Commit();
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (HasActiveTransaction)
        //        {
        //            Rollback();
        //        }

        //        throw ex;
        //    }
        //}

        public T Get<T>(dynamic id, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            return (T)_dapper.Get<T>(Connection, id, transaction, commandTimeout);
        }

        public T Get<T>(dynamic id, int? commandTimeout) where T : class
        {
            return (T)_dapper.Get<T>(Connection, id, _transaction, commandTimeout);
        }

        public void Insert<T>(IEnumerable<T> entities, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            _dapper.Insert<T>(Connection, entities, transaction, commandTimeout);
        }

        public void Insert<T>(IEnumerable<T> entities, int? commandTimeout) where T : class
        {
            _dapper.Insert<T>(Connection, entities, _transaction, commandTimeout);
        }

        public dynamic Insert<T>(T entity, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            return _dapper.Insert<T>(Connection, entity, transaction, commandTimeout);
        }

        public dynamic Insert<T>(T entity, int? commandTimeout) where T : class
        {
            return _dapper.Insert<T>(Connection, entity, _transaction, commandTimeout);
        }

        public bool Update<T>(T entity, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            return _dapper.Update<T>(Connection, entity, transaction, commandTimeout);
        }

        public bool Update<T>(T entity, int? commandTimeout) where T : class
        {
            return _dapper.Update<T>(Connection, entity, _transaction, commandTimeout);
        }

        public bool Delete<T>(T entity, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            return _dapper.Delete(Connection, entity, transaction, commandTimeout);
        }

        public bool Delete<T>(T entity, int? commandTimeout) where T : class
        {
            return _dapper.Delete(Connection, entity, _transaction, commandTimeout);
        }

        public bool Delete<T>(object predicate, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            return _dapper.Delete<T>(Connection, predicate, transaction, commandTimeout);
        }

        public bool Delete<T>(object predicate, int? commandTimeout) where T : class
        {
            return _dapper.Delete<T>(Connection, predicate, _transaction, commandTimeout);
        }

        public IEnumerable<T> GetList<T>(object predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
        {
            return _dapper.GetList<T>(Connection, predicate, sort, transaction, commandTimeout, buffered);
        }

        public IEnumerable<T> GetList<T>(object predicate, IList<ISort> sort, int? commandTimeout, bool buffered) where T : class
        {
            return _dapper.GetList<T>(Connection, predicate, sort, _transaction, commandTimeout, buffered);
        }

        public IEnumerable<T> GetPage<T>(object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
        {
            return _dapper.GetPage<T>(Connection, predicate, sort, page, resultsPerPage, transaction, commandTimeout, buffered);
        }

        public IEnumerable<T> GetPage<T>(object predicate, IList<ISort> sort, int page, int resultsPerPage, int? commandTimeout, bool buffered) where T : class
        {
            return _dapper.GetPage<T>(Connection, predicate, sort, page, resultsPerPage, _transaction, commandTimeout, buffered);
        }

        public IEnumerable<T> GetSet<T>(object predicate, IList<ISort> sort, int firstResult, int maxResults, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
        {
            return _dapper.GetSet<T>(Connection, predicate, sort, firstResult, maxResults, transaction, commandTimeout, buffered);
        }

        public IEnumerable<T> GetSet<T>(object predicate, IList<ISort> sort, int firstResult, int maxResults, int? commandTimeout, bool buffered) where T : class
        {
            return _dapper.GetSet<T>(Connection, predicate, sort, firstResult, maxResults, _transaction, commandTimeout, buffered);
        }

        public int Count<T>(object predicate, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            return _dapper.Count<T>(Connection, predicate, transaction, commandTimeout);
        }

        public int Count<T>(object predicate, int? commandTimeout) where T : class
        {
            return _dapper.Count<T>(Connection, predicate, _transaction, commandTimeout);
        }

        public IMultipleResultReader GetMultiple(GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout)
        {
            return _dapper.GetMultiple(Connection, predicate, transaction, commandTimeout);
        }

        public IMultipleResultReader GetMultiple(GetMultiplePredicate predicate, int? commandTimeout)
        {
            return _dapper.GetMultiple(Connection, predicate, _transaction, commandTimeout);
        }

        public void ClearCache()
        {
            _dapper.SqlGenerator.Configuration.ClearCache();
        }

        public Guid GetNextGuid()
        {
            return _dapper.SqlGenerator.Configuration.GetNextGuid();
        }

        public IClassMapper GetMap<T>() where T : class
        {
            return _dapper.SqlGenerator.Configuration.GetMap<T>();
        }

        public IEnumerable<T> Query<T>(string query, object parameters = null, bool buffered = true, int? commandTimeout = default(int?))
        {
            return Connection.Query<T>(query, parameters, _transaction, buffered, commandTimeout);
        }

        public IEnumerable<dynamic> Query(string query, object parameters = null, bool buffered = true, int? commandTimeout = default(int?))
        {
            return Connection.Query(query, parameters, _transaction, buffered, commandTimeout);
        }

        public int Execute(string sql, object parameters = null, int? commandTimeout = default(int?))
        {
            return Connection.Execute(sql, parameters, _transaction, commandTimeout);
        }
    }

}
