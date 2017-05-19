using System.Reflection;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Modules;
using Abp.Runtime.DataAnnotations;

namespace Abp
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpExModule : AbpModule
    {       
        private static readonly object SyncObj = new object();

        public override void PreInitialize()
        {
            IocManager.RegisterIfNot<IAbpExConfiguration, AbpExConfiguration>();
            IocManager.RegisterIfNot<IDataAnnotationLocalizable, DataAnnotationLocalization>();

            //we need register it as static when we run unit-test.
            lock (SyncObj)
            {
                if (!Dependency.IocManager.Instance.IsRegistered<IAbpExConfiguration>())
                {
                    Dependency.IocManager.Instance.RegisterIfNot<IAbpExConfiguration, AbpExConfiguration>();
                }

                if (!Dependency.IocManager.Instance.IsRegistered<IDataAnnotationLocalizable>())
                {
                    Dependency.IocManager.Instance.RegisterIfNot<IDataAnnotationLocalizable, DataAnnotationLocalization>();
                }
            }          
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}