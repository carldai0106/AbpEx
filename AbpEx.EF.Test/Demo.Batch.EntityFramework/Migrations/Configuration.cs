using System.Collections.Generic;
using Abp;
using Abp.EntityFramework;
using Demo.Batch.EntityFramework;
using Demo.Batch.Migrations.Seed;

namespace Demo.Batch.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<BatchDbContext>, IMigratorSeed<BatchDbContext>
    {
        private Dictionary<string, string> _dbInitCfg;

        public bool IsSeed
        {
            get
            {
                if (_dbInitCfg == null)
                {
                    return false;
                }

                string value;
                if (_dbInitCfg.TryGetValue("BatchDemo", out value))
                {
                    return value == "1";
                }

                return false;
            }
        }

        public Configuration()
        {
            _dbInitCfg = DbConfiguration.GetDbInitCfg();
             #if UnitTest            
                AutomaticMigrationsEnabled = false;
                AutomaticMigrationDataLossAllowed = true;            
            #else                        
                AutomaticMigrationsEnabled = false;
                AutomaticMigrationDataLossAllowed = true;
            #endif
            ContextKey = "BatchDemo";
        }

        protected override void Seed(BatchDbContext context)
        {
            SeedData(context);
        }

        private void InitDatabase(BatchDbContext context)
        {
            new BatchDbInit(context).Create();
        }

        public void SeedData(BatchDbContext context)
        {
#if UnitTest
                InitDatabase(context);
                return;
#endif

            if (_dbInitCfg == null)
            {
                InitDatabase(context);
                _dbInitCfg = new Dictionary<string, string> { { "BatchDemo", "1" } };
                DbConfiguration.SaveDbInitCfg(_dbInitCfg);
                return;
            }

            if (IsSeed)
            {
                return;
            }

            InitDatabase(context);
            _dbInitCfg["BatchDemo"] = "1";
            DbConfiguration.SaveDbInitCfg(_dbInitCfg);
        }
    }
}
