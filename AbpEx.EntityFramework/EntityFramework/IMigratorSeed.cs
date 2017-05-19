namespace Abp.EntityFramework
{
    public interface IMigratorSeed<TDbContext>
    {
        bool IsSeed { get; }

        void SeedData(TDbContext context);
    }
}
