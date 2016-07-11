using System.Linq;
using Dapper;
using hMailServer.Repository.RelationalShared;

namespace hMailServer.Repository.MySQL
{
    public class RepositoryFactory : IRepositoryFactory
    {
        public RepositoryFactory()
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

        public IAccountRepository CreateAccountRepository(string connectionString)
        {
            return new AccountRepository(connectionString);
        }
    }
}
