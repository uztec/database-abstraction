using SimpleInjector;

namespace UzunTec.Utils.DatabaseAbstraction.SQLServer.Test
{
    public class DbAbstractionTestContainer : Container
    {
        private readonly IDbQueryBase dbQueryBase;

        public DbAbstractionTestContainer(IDbQueryBase dbQueryBase)
        {
            this.dbQueryBase = dbQueryBase;
            this.Initialize();
        }

        private void Initialize()
        {
            this.Register<IDbQueryBase>(delegate () { return this.dbQueryBase; }, Lifestyle.Singleton);
            this.Register<DBUser>(Lifestyle.Singleton);
            this.Register<UserQueryClient>(Lifestyle.Singleton);
            this.Register<DBUserAsync>(Lifestyle.Singleton);
            this.Register<UserQueryClientAsync>(Lifestyle.Singleton);
            this.Verify();
        }
    }
}
