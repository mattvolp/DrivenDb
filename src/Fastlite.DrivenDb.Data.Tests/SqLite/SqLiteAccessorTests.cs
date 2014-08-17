using Fastlite.DrivenDb.Data.Tests.Base;
using Fastlite.DrivenDb.Data.Tests.Base.Infrastructure;
using Fastlite.DrivenDb.Data.Tests.SqLite.Infrastructure;

namespace Fastlite.DrivenDb.Data.Tests.SqLite
{   
   public class SqLiteAccessorTests : DbAccessorTests
   {
      protected override IDbTestFixture CreateFixture()
      {
         return new SqLiteTestFixture();
      }     
   }
}