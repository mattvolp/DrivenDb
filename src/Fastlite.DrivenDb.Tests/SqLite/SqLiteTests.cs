using Fastlite.DrivenDb.Tests.Base;
using Fastlite.DrivenDb.Tests.Base.Infrastructure;
using Fastlite.DrivenDb.Tests.SqLite.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.DrivenDb.Tests.SqLite
{   
   [TestClass]
   public sealed class SqLiteTests : BehaviorTests
   {
      protected override IBehaviorFixture CreateFixture()
      {
         return new SqLiteFixture();        
      }
   }
}
