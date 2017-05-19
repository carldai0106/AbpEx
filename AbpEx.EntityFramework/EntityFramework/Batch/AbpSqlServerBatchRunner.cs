using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EntityFramework.Extensions;
using EntityFramework.Mapping;
using EntityFramework.Reflection;
using System.Data.SqlClient;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using EntityFramework.DynamicFilters;

namespace Abp.EntityFramework.Batch
{
    /// <summary>
    /// A batch execution runner for SQL Server.
    /// </summary>
    public class AbpSqlServerBatchRunner : IAbpBatchRunner, ITransientDependency
    {
        public int Update<TEntity>(
            DbContext dbContext,
            IAbpSession abpSession,
            EntityMap entityMap,
            ObjectQuery<TEntity> query,
            Expression<Func<TEntity, TEntity>> updateExpression
        ) where TEntity : class
        {
            return InnerUpdate(dbContext, abpSession, entityMap, query, updateExpression).Result;
        }

        public Task<int> UpdateAsync<TEntity>(
               DbContext dbContext,
               IAbpSession abpSession,
               EntityMap entityMap,
               ObjectQuery<TEntity> query,
               Expression<Func<TEntity, TEntity>> updateExpression
           ) where TEntity : class
        {
            return InnerUpdate(dbContext, abpSession, entityMap, query, updateExpression, true);
        }

        public int Delete<TEntity>(
               DbContext dbContext,
               IAbpSession abpSession,
               EntityMap entityMap,
               ObjectQuery<TEntity> query
           ) where TEntity : class
        {
            return InnerDelete(dbContext, abpSession, entityMap, query).Result;
        }

        public Task<int> DeleteAsync<TEntity>(
               DbContext dbContext,
               IAbpSession abpSession,
               EntityMap entityMap,
               ObjectQuery<TEntity> query
            ) where TEntity : class
        {
            return InnerDelete(dbContext, abpSession, entityMap, query, true);
        }

        private static Tuple<DbConnection, DbTransaction> GetStore(ObjectContext objContext)
        {
            // TODO, re-eval if this is needed
            var dbConnection = objContext.Connection;
            var entityConnection = dbConnection as EntityConnection;

            // by-pass entity connection
            if (entityConnection == null)
                return new Tuple<DbConnection, DbTransaction>(dbConnection, null);

            var connection = entityConnection.StoreConnection;

            // get internal transaction
            dynamic connectionProxy = new DynamicProxy(entityConnection);
            dynamic entityTransaction = connectionProxy.CurrentTransaction;
            if (entityTransaction == null)
                return new Tuple<DbConnection, DbTransaction>(connection, null);

            DbTransaction transaction = entityTransaction.StoreTransaction;
            return new Tuple<DbConnection, DbTransaction>(connection, transaction);
        }

        private static string GetSelectSql<TEntity>(
            DbContext dbContext,
            IAbpSession abpSession,
            ObjectQuery<TEntity> query,
            EntityMap entityMap,
            DbCommand command
            ) where TEntity : class
        {
            // TODO change to esql?

            // changing query to only select keys
            var selector = new StringBuilder(50);
            selector.Append("new(");
            foreach (var propertyMap in entityMap.KeyMaps)
            {
                if (selector.Length > 4)
                    selector.Append((", "));

                selector.Append(propertyMap.PropertyName);
            }
            selector.Append(")");

            var selectQuery = DynamicQueryable.Select(query, selector.ToString());
            var objectQuery = selectQuery as ObjectQuery;

            if (objectQuery == null)
                throw new ArgumentException("The query must be of type ObjectQuery.", nameof(query));

            var innerJoinSql = objectQuery.ToTraceString();

            // create parameters
            foreach (var objectParameter in objectQuery.Parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = objectParameter.Name;
                parameter.Value = objectParameter.Value ?? DBNull.Value;

                command.Parameters.Add(parameter);
            }

            var parameters = new List<SqlParameter>();
            var reg = new Regex(@"([\[?\w +\]?\.\[?\w+\]?]+(?:\s*\=\s*\@\w+))", RegexOptions.Compiled);

            var matches = reg.Matches(innerJoinSql).Cast<Match>().Where(x =>
                (
                    x.Value.Contains("TenantId") ||
                    x.Value.Contains("IsDeleted")
                )
                && x.Value.Contains("@DynamicFilterParam_")).ToList();

            var abpContext = dbContext as DbContext;

            if (abpContext != null)
            {                
                foreach (var item in matches)
                {
                    if (item.Value.Contains("TenantId"))
                    {
                        var dynamicFilterParam = item.Value.Split('=').ElementAt(1).Trim();
                        // TenantId, Accourding to TenantId to filter records
                        var para1 = new SqlParameter(dynamicFilterParam, SqlDbType.Int)
                        {
                            Value = abpSession.TenantId
                        };

                        var dfp = dynamicFilterParam.Split('_').ElementAt(1);
                        string strDfp;
                        if (dfp.Length == 6)
                        {
                            var number = int.Parse(dfp) + 1;
                            strDfp = $"{number:D6}";
                        }
                        else
                        {
                            strDfp = (int.Parse(dfp) + 1).ToString();
                        }

                        var para2 = new SqlParameter("@DynamicFilterParam_" + strDfp, SqlDbType.Bit);
                        // if true enabled tenant filter; if false disabled tenant filter
                        if (abpContext.IsFilterEnabled(AbpDataFilters.MayHaveTenant) ||
                            abpContext.IsFilterEnabled(AbpDataFilters.MustHaveTenant))
                        {
                            para2.Value = DBNull.Value;
                        }
                        else
                        {
                            para2.Value = 1;//disabled tenant filter
                        }
                        if (parameters.All(x => x.ParameterName != para1.ParameterName))
                        {
                            parameters.Add(para1);
                        }
                        if (parameters.All(x => x.ParameterName != para2.ParameterName))
                        {
                            parameters.Add(para2);
                        }
                    }

                    if (item.Value.Contains("IsDeleted"))
                    {
                        var dynamicFilterParam = item.Value.Split('=').ElementAt(1).Trim();
                        // IsDeleted = false, It will get records that do not soft deleted records.
                        var para1 = new SqlParameter(dynamicFilterParam, SqlDbType.Bit)
                        {
                            Value = false
                        };


                        var dfp = dynamicFilterParam.Split('_').ElementAt(1);
                        string strDfp;
                        if (dfp.Length == 6)
                        {
                            var number = int.Parse(dfp) + 1;
                            strDfp = $"{number:D6}";
                        }
                        else
                        {
                            strDfp = (int.Parse(dfp) + 1).ToString();
                        }

                        var para2 = new SqlParameter("@DynamicFilterParam_" + strDfp, SqlDbType.Bit);

                        // if true enabled SoftDelete; if false disabled SoftDelete
                        if (abpContext.IsFilterEnabled(AbpDataFilters.SoftDelete))
                        {
                            para2.Value = DBNull.Value;
                        }
                        else
                        {
                            para2.Value = 1;
                        }

                        if (parameters.All(x => x.ParameterName != para1.ParameterName))
                        {
                            parameters.Add(para1);
                        }
                        if (parameters.All(x => x.ParameterName != para2.ParameterName))
                        {
                            parameters.Add(para2);
                        }
                    }
                }
            }

            command.Parameters.AddRange(parameters.ToArray());

            return innerJoinSql;
        }

        private async Task<int> BatchHandle<TEntity>(
            DbContext dbContext,
            IAbpSession abpSession,
            EntityMap entityMap,
            ObjectQuery<TEntity> query,
            Func<ObjectContext, DbCommand, StringBuilder, string, string> customFunc,
            bool async = false
            ) where TEntity : class
        {
            DbConnection dbConnection = null;
            DbTransaction dbTransaction = null;
            DbCommand dbCommand = null;
            var ownConnection = false;
            var ownTransaction = false;
            try
            {
                var objContext = (dbContext as IObjectContextAdapter).ObjectContext;

                // get store connection and transaction
                var store = GetStore(objContext);
                dbConnection = store.Item1;
                dbTransaction = store.Item2;

                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                    ownConnection = true;
                }

                // use existing transaction or create new
                if (dbTransaction == null)
                {
                    dbTransaction = dbConnection.BeginTransaction();
                    ownTransaction = true;
                }

                dbCommand = dbConnection.CreateCommand();
                dbCommand.Transaction = dbTransaction;
                if (objContext.CommandTimeout.HasValue)
                    dbCommand.CommandTimeout = objContext.CommandTimeout.Value;

                var innerSelect = GetSelectSql(dbContext, abpSession, query, entityMap, dbCommand);
                var sqlBuilder = new StringBuilder(innerSelect.Length * 2);

                var command = customFunc(objContext, dbCommand, sqlBuilder, innerSelect);

                dbCommand.CommandText = command;

                var result = async
                    ? await dbCommand.ExecuteNonQueryAsync().ConfigureAwait(false)
                    : dbCommand.ExecuteNonQuery();

                // only commit if created transaction
                if (ownTransaction)
                    dbTransaction.Commit();

                return result;
            }
            finally
            {
                if (dbCommand != null)
                    dbCommand.Dispose();
                if (dbTransaction != null && ownTransaction)
                    dbTransaction.Dispose();
                if (dbConnection != null && ownConnection)
                    dbConnection.Close();
            }
        }

        private Task<int> InnerUpdate<TEntity>(
                                                DbContext dbContext,
                                                IAbpSession abpSession,
                                                EntityMap entityMap,
                                                ObjectQuery<TEntity> query,
                                                Expression<Func<TEntity, TEntity>> updateExpression,
                                                bool async = false
                                            ) where TEntity : class
        {
            Func<ObjectContext, DbCommand, StringBuilder, string, string> customFunc = (objContext, dbCommand, sqlBuilder, innerSql) =>
            {
                sqlBuilder.Append("UPDATE ");
                sqlBuilder.Append(entityMap.TableName);
                sqlBuilder.AppendLine(" SET ");

                var memberInitExpression = updateExpression.Body as MemberInitExpression;
                if (memberInitExpression == null)
                    throw new ArgumentException("The update expression must be of type MemberInitExpression.", nameof(updateExpression));

                var nameCount = 0;
                var wroteSet = false;
                foreach (var binding in memberInitExpression.Bindings)
                {
                    if (wroteSet)
                        sqlBuilder.AppendLine(", ");

                    var propertyName = binding.Member.Name;
                    var columnName = entityMap.PropertyMaps
                        .Where(p => p.PropertyName == propertyName)
                        .Select(p => p.ColumnName)
                        .FirstOrDefault();


                    var memberAssignment = binding as MemberAssignment;
                    if (memberAssignment == null)
                        throw new ArgumentException("The update expression MemberBinding must only by type MemberAssignment.", nameof(updateExpression));

                    var memberExpression = memberAssignment.Expression;

                    ParameterExpression parameterExpression = null;
                    memberExpression.Visit((ParameterExpression p) =>
                    {
                        if (p.Type == entityMap.EntityType)
                            parameterExpression = p;

                        return p;
                    });


                    if (parameterExpression == null)
                    {
                        object value;

                        if (memberExpression.NodeType == ExpressionType.Constant)
                        {
                            var constantExpression = memberExpression as ConstantExpression;
                            if (constantExpression == null)
                                throw new ArgumentException(
                                    "The MemberAssignment expression is not a ConstantExpression.", nameof(updateExpression));

                            value = constantExpression.Value;
                        }
                        else
                        {
                            var lambda = Expression.Lambda(memberExpression, null);
                            value = lambda.Compile().DynamicInvoke();
                        }

                        if (value != null)
                        {
                            var parameterName = "p__update__" + nameCount++;
                            var parameter = dbCommand.CreateParameter();
                            parameter.ParameterName = parameterName;
                            parameter.Value = value;
                            dbCommand.Parameters.Add(parameter);

                            sqlBuilder.AppendFormat("[{0}] = @{1}", columnName, parameterName);
                        }
                        else
                        {
                            sqlBuilder.AppendFormat("[{0}] = NULL", columnName);
                        }
                    }
                    else
                    {
                        var objectSet = objContext.CreateObjectSet<TEntity>();
                        var typeArguments = new[] { entityMap.EntityType, memberExpression.Type };

                        var constantExpression = Expression.Constant(objectSet);
                        var lambdaExpression = Expression.Lambda(memberExpression, parameterExpression);

                        var selectExpression = Expression.Call(
                            typeof(Queryable),
                            "Select",
                            typeArguments,
                            constantExpression,
                            lambdaExpression);

                        // create query from expression
                        var selectQuery = objectSet.CreateQuery(selectExpression, entityMap.EntityType);
                        var sql = selectQuery.ToTraceString();

                        // parse select part of sql to use as update
                        var regex = @"SELECT\s*\r\n\s*(?<ColumnValue>.+)?\s*AS\s*(?<ColumnAlias>\[\w+\])\r\n\s*FROM\s*(?<TableName>\[\w+\]\.\[\w+\]|\[\w+\])\s*AS\s*(?<TableAlias>\[\w+\])";
                        var match = Regex.Match(sql, regex);
                        if (!match.Success)
                            throw new ArgumentException("The MemberAssignment expression could not be processed.", nameof(updateExpression));

                        var value = match.Groups["ColumnValue"].Value;
                        var alias = match.Groups["TableAlias"].Value;

                        value = value.Replace(alias + ".", "j0.");
                        //value = value.Replace(alias + ".", "");
                        foreach (var objectParameter in selectQuery.Parameters)
                        {
                            string parameterName = "p__update__" + nameCount++;

                            var parameter = dbCommand.CreateParameter();
                            parameter.ParameterName = parameterName;
                            parameter.Value = objectParameter.Value ?? DBNull.Value;
                            dbCommand.Parameters.Add(parameter);

                            value = value.Replace(objectParameter.Name, parameterName);
                        }
                        sqlBuilder.AppendFormat("[{0}] = {1}", columnName, value);
                    }
                    wroteSet = true;
                }

                sqlBuilder.AppendLine(" ");
                sqlBuilder.AppendFormat("FROM {0} AS j0 INNER JOIN (", entityMap.TableName);
                sqlBuilder.AppendLine();
                sqlBuilder.AppendLine(innerSql);
                sqlBuilder.Append(") AS j1 ON (");

                bool wroteKey = false;
                foreach (var keyMap in entityMap.KeyMaps)
                {
                    if (wroteKey)
                        sqlBuilder.Append(" AND ");

                    sqlBuilder.AppendFormat("j0.[{0}] = j1.[{0}]", keyMap.ColumnName);
                    wroteKey = true;
                }
                sqlBuilder.Append(")");

                return sqlBuilder.ToString();
            };

            return BatchHandle(dbContext, abpSession, entityMap, query, customFunc, async);
        }

        private Task<int> InnerDelete<TEntity>(
            DbContext dbContext,
            IAbpSession abpSession,
            EntityMap entityMap,
            ObjectQuery<TEntity> query,
            bool async = false
            ) where TEntity : class
        {

            Func<ObjectContext, DbCommand, StringBuilder, string, string> customFunc = (objContext, dbCommand, sqlBuilder, innerSql) =>
            {
                sqlBuilder.Append("DELETE ");
                sqlBuilder.Append(entityMap.TableName);
                sqlBuilder.AppendLine();

                sqlBuilder.AppendFormat("FROM {0} AS j0 INNER JOIN (", entityMap.TableName);
                sqlBuilder.AppendLine();
                sqlBuilder.AppendLine(innerSql);
                sqlBuilder.Append(") AS j1 ON (");

                var wroteKey = false;
                foreach (var keyMap in entityMap.KeyMaps)
                {
                    if (wroteKey)
                        sqlBuilder.Append(" AND ");

                    sqlBuilder.AppendFormat("j0.[{0}] = j1.[{0}]", keyMap.ColumnName);
                    wroteKey = true;
                }
                sqlBuilder.Append(")");

                return sqlBuilder.ToString();
            };

            return BatchHandle(dbContext, abpSession, entityMap, query, customFunc, async);
        }
    }
}