using Fastlite.DrivenDb.Data.Tests.Base;
using Fastlite.DrivenDb.Data.Tests.Base.Infrastructure;
using Fastlite.DrivenDb.Data.Tests.SqLite.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.DrivenDb.Data.Tests.SqLite
{
   [TestClass]
   public class SqLiteFallbackTests : FallbackAccessorTests
   {
      protected override IDbTestFixture CreateFixture()
      {
         return new SqLiteTestFixture();
      }
   }
}
