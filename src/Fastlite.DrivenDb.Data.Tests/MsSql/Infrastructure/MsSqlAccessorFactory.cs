using System.Data.SqlClient;
using Fastlite.DrivenDb.Data.Access;
using Fastlite.DrivenDb.Data.Access.Interfaces;
using Fastlite.DrivenDb.Data.Tests.Base.Infrastructure;

namespace Fastlite.DrivenDb.Data.Tests.MsSql.Infrastructure
{
   public sealed class MsSqlAccessorFactory : IDbAccessorFactory
   {
      private const string TEST_CSTRING = @"Integrated Security=SSPI;Initial Catalog=DrivenDbTest;Data Source=localhost";

      public IDbAccessor Create(string database, AccessorExtension extensions)
      {
         return DbFactory.CreateAccessor(
            DbAccessorType.MsSql, extensions,
            () => new SqlConnection(TEST_CSTRING)
            );
      }
   }
}
