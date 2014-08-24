using Fastlite.DrivenDb.Data.Access;
using Fastlite.DrivenDb.Data.Access.Interfaces;
using Fastlite.DrivenDb.Data.Tests.Base.Infrastructure;
using MySql.Data.MySqlClient;

namespace Fastlite.DrivenDb.Data.Tests.MySql.Infrastructure
{
   internal sealed class MySqlAccessorFactory : IDbAccessorFactory
   {
      public IDbAccessor Create(string database, AccessorOptions options)
      {
         return DbFactory.CreateAccessor(
            DbAccessorType.MySql, options,
            () => new MySqlConnection("Server=localhost;User Id=root;Password=;Database=DrivenDbTest")
            );
      }
   }
}
