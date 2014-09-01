using Fastlite.DrivenDb.Data.Tests._2._0.Infrastructure;
using Fastlite.DrivenDb.Data.Tests._2._0.SqLite.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.DrivenDb.Data.Tests._2._0.SqLite
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
