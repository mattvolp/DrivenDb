using System.Linq;
using Fastlite.DrivenDb.Data.Tests.Base.Infrastructure;
using Fastlite.DrivenDb.Data.Tests.Base.Tables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.DrivenDb.Data.Tests.Base
{
   public abstract class FallbackAccessorTests : DbTestClass
   {
      [TestMethod]
      public void FallbackAccessor_ReadEntitiesWithNullTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.Fallback.ReadEntities<MyTable>(
               "SELECT * FROM MyTable WHERE MyIdentity IN (@0)"
               , default(int[])
               ).ToArray();

            Assert.IsTrue(entities.Length == 0);
         }
      }

      [TestMethod]
      public void FallbackAccessor_ReadEntitiesWithoutValuesTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var identities = new int[0];

            var entities = accessor.Fallback.ReadEntities<MyTable>(
               "SELECT * FROM MyTable WHERE MyIdentity IN (@0)"
               , identities
               ).ToArray();

            Assert.IsTrue(entities.Length == 0);
         }
      }

      [TestMethod]
      public void FallbackAccessor_ReadEntitiesWithValuesTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var identities = new[] { 1, 2, 3 };

            var entities = accessor.Fallback.ReadEntities<MyTable>(
               "SELECT * FROM MyTable WHERE MyIdentity IN (@0)"
               , identities
               ).ToArray();

            Assert.IsTrue(entities.Length == 3);
         }
      }
   }
}
