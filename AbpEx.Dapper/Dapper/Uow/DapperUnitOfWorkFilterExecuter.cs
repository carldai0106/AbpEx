using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Reflection;
using System;
using System.Linq;
using Abp.Dapper.DbContext;

namespace Abp.Dapper.Uow
{
    public class DapperUnitOfWorkFilterExecuter : IDapperUnitOfWorkFilterExecuter
    {
        public void ApplyCurrentFilters(IUnitOfWork unitOfWork, DapperDbContext dbContext)
        {
            foreach (var filter in unitOfWork.Filters)
            {
                //if (filter.IsEnabled)
                //{
                //    dbContext.EnableFilter(filter.FilterName);
                //}
                //else
                //{
                //    dbContext.DisableFilter(filter.FilterName);
                //}

                //foreach (var filterParameter in filter.FilterParameters)
                //{
                //    if (TypeHelper.IsFunc<object>(filterParameter.Value))
                //    {
                //        dbContext.SetFilterScopedParameterValue(filter.FilterName, filterParameter.Key, (Func<object>)filterParameter.Value);
                //    }
                //    else
                //    {
                //        dbContext.SetFilterScopedParameterValue(filter.FilterName, filterParameter.Key, filterParameter.Value);
                //    }
                //}
            }
        }

        public void ApplyDisableFilter(IUnitOfWork unitOfWork, string filterName)
        {

            var contexts = unitOfWork.As<DapperUnitOfWork>().GetAllActiveDbContexts();

            

            var queryFilter = context.QueryFilters.FirstOrDefault(x => x.FilterName == filterName);
            if (queryFilter != null)
                queryFilter.IsEnabled = false;
        }

        public void ApplyEnableFilter(IUnitOfWork unitOfWork, string filterName)
        {
            //var context = unitOfWork.As<DapperUnitOfWork>().GetDapperDbContext();
            //var queryFilter = context.QueryFilters.FirstOrDefault(x => x.FilterName == filterName);
            //if (queryFilter != null)
            //    queryFilter.IsEnabled = true;
        }

        public void ApplyFilterParameterValue(IUnitOfWork unitOfWork, string filterName, string parameterName, object value)
        {
            //var context = unitOfWork.As<DapperUnitOfWork>().GetDapperDbContext();
            //var filter = context.QueryFilters.FirstOrDefault(x => x.FilterName == filterName);
            //if (TypeHelper.IsFunc<object>(value))
            //{
            //    var func = (Func<object>)value;
            //    var rs = func();
            //    filter?.SetFilterValue(rs);
            //}
            //else
            //{
            //    filter?.SetFilterValue(value);
            //}
        }
    }
}
