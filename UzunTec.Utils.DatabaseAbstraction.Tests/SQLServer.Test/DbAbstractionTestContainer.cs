using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace UzunTec.Utils.DatabaseAbstraction.Test
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
            this.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            this.Register<IDbQueryBase>(delegate () { return this.dbQueryBase; }, Lifestyle.Singleton);
            this.Register<DBUser>(Lifestyle.Singleton);
            this.Register<UserQueryClient>(Lifestyle.Singleton);
            this.Verify();
        }
    }
}
