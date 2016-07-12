using System.Linq;
using Dapper;
using hMailServer.Repository.RelationalShared;
using MySql.Data.MySqlClient;

namespace hMailServer.Repository.MySQL
{
    public class RepositoryFactory : IRepositoryFactory
    {
        static RepositoryFactory()
        {
            var mappings = TypeColumnMappings.Create();

            foreach (var mapping in mappings)
            {
                Dapper.SqlMapper.SetTypeMap(
                   mapping.Type,
                    new CustomPropertyTypeMap(
                        mapping.Type,
                        (type, columnName) =>
                        {
                            if (mapping.FieldNameByColumnName.ContainsKey(columnName))
                            {
                                string fieldName = mapping.FieldNameByColumnName[columnName];

                                var properties = type.GetProperties();

                                return properties.FirstOrDefault(item => item.Name == fieldName);
                            }

                            return null;
                        }));
            }
        }

        private readonly string _connectionString;
        
        public RepositoryFactory(DatabaseConfiguration databaseConfiguration)
        {
            var connectionStringBuilder = new MySqlConnectionStringBuilder();
            connectionStringBuilder.UserID = databaseConfiguration.Username;
            connectionStringBuilder.Password = databaseConfiguration.Password;
            connectionStringBuilder.Server = databaseConfiguration.Server;
            connectionStringBuilder.Database = databaseConfiguration.Database;
            connectionStringBuilder.Port = databaseConfiguration.Port;

            _connectionString = connectionStringBuilder.ToString();
        }

        public IAccountRepository CreateAccountRepository()
        {
            return new AccountRepository(_connectionString);
        }
    }
}
