﻿using System;
using System.Threading.Tasks;
using Abp;
using Abp.Collections;
using Abp.EntityFramework;
using Abp.Modules;
using Abp.PlugIns;
using Abp.TestBase;
using Demo.Batch.Application;
using Demo.Batch.Configuration;
using Demo.Batch.EntityFramework;
using Demo.Batch.Migrations.Seed;
using Effort;
using EntityFramework.DynamicFilters;
using Xunit.Abstractions;

namespace Demo.Batch.Tests
{
    public class BatchTestBase : AbpIntegratedTestBase<BatchCoreModule>
    {
        protected readonly ITestOutputHelper Output;
        protected BatchTestBase(ITestOutputHelper output)
        {
            UsingDbContext(context =>
            {
                new BatchDbInit(context).Create();
            });

            Output = output;

            LoginAsTenant();
        }

        public void LoginAsTenant()
        {
            AbpSession.TenantId = 1;
            AbpSession.UserId = 1;
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            var visitConnection = DbConnectionFactory.CreateTransient();
            if (!LocalIocManager.IsRegistered<IBatchDataModuleConfiguration>())
            {
                LocalIocManager.Register<IBatchDataModuleConfiguration, BatchDataModuleConfiguration>();
                Resolve<IBatchDataModuleConfiguration>().Connection = visitConnection;
            }

            AbpBootstrapper.PlugInSources.Add(new PlugInTypeListSource(
                  typeof(AbpExModule),
                  typeof(AbpExEntityFrameworkModule),
                  typeof(BatchCoreModule),
                  typeof(BatchDataModule),
                  typeof(BatchApplicationModule)
              ));
        }

        protected void UsingDbContext(Action<BatchDbContext> action)
        {
            using (var context = LocalIocManager.Resolve<BatchDbContext>())
            {
                context.DisableAllFilters();
                action(context);
                context.SaveChanges();
            }
        }

        protected async Task UsingDbContextAsync(Action<BatchDbContext> action)
        {
            using (var context = LocalIocManager.Resolve<BatchDbContext>())
            {
                context.DisableAllFilters();
                action(context);
                await context.SaveChangesAsync();
            }
        }

        protected T UsingDbContext<T>(Func<BatchDbContext, T> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<BatchDbContext>())
            {
                context.DisableAllFilters();
                result = func(context);
                context.SaveChanges();
            }

            return result;
        }

        protected async Task<T> UsingDbContextAsync<T>(Func<BatchDbContext, Task<T>> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<BatchDbContext>())
            {
                context.DisableAllFilters();
                result = await func(context);
                await context.SaveChangesAsync();
            }

            return result;
        }
    }
}
