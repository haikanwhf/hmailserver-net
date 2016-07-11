using hMailServer.Repository;
using hMailServer.Repository.MySQL;
using StructureMap;

namespace hMailServer.Application
{
    class DependencyRegistry : Registry
    {
        public DependencyRegistry(Configuration configuration)
        {
            var repositoryFactory = new RepositoryFactory();

            For<IAccountRepository>()
                .Use(() => repositoryFactory.CreateAccountRepository(configuration.DatabaseConnectionString));
        }
    }
}
