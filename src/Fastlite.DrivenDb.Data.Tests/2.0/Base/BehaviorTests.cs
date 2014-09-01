using System.Linq;
using Fastlite.DrivenDb.Data.Tests._2._0.Base.Contracts;
using Fastlite.DrivenDb.Data.Tests._2._0.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.DrivenDb.Data.Tests._2._0
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

            var actual = sut.Read(@"SELECT * FROM [TableInt32] WHERE [Column1] = 2")
               .As<TableInt32>()
               .Single();
            
            Assert.IsTrue(false);  
         }
      }

      protected abstract IBehaviorFixture CreateFixture();
   }
}
