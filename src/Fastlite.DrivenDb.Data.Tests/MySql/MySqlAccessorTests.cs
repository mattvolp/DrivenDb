using Fastlite.DrivenDb.Data.Tests.Base;
using Fastlite.DrivenDb.Data.Tests.Base.Infrastructure;
using Fastlite.DrivenDb.Data.Tests.MySql.Infrastructure;

namespace Fastlite.DrivenDb.Data.Tests.MySql
{
   public class MySqlAccessorTests : DbAccessorTests
   {
      protected override IDbTestFixture CreateFixture()
      {
         return new MySqlTestFixture();
      }
   }
}