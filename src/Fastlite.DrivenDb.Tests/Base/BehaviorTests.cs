using System.Linq;
using Fastlite.DrivenDb.Tests.Base.Contracts;
using Fastlite.DrivenDb.Tests.Base.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.DrivenDb.Tests.Base
{   
   public abstract class BehaviorTests
   {
      [TestMethod]
      public void DrivenDb_CanPopulatePropertiesViaNameMatchingFromAResultsetWithCaseSensativity()
      {
         using (var fixture = CreateFixture())
         {
            var sut = fixture.CreateAccessor()
               .Build();

            var actual = sut.Read(@"SELECT * FROM [TableInt32] WHERE [ColumnA] = 2")
               .As<TableInt32>()
               .Single();
            
            Assert.AreEqual(2, actual.ColumnA);  
         }
      }

      protected abstract IBehaviorFixture CreateFixture();
   }
}
