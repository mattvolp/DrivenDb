using System.IO;
using System.Linq;
using Fastlite.DrivenDb.Core.Contracts;
using Fastlite.DrivenDb.Data.Tests.Base.Infrastructure;
using Fastlite.DrivenDb.Data.Tests.Base.Tables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.DrivenDb.Data.Tests.Base
{
   public abstract class DbEntityTests : DbTestClass
   {
      [TestMethod]
      public void DbEntity_SerializationTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable")
               .OrderBy(e => e.MyNumber)
               .ToArray();

            entities[1].Entity.Delete();
            entities[2].MyString = "Three Three Three";

            using (var memory = new MemoryStream())
            {
               DataContracts.Serialize(entities, memory);

               memory.Position = 0;

               var hydrated = DataContracts.Deserialize<MyTable[]>(memory);

               Assert.IsTrue(hydrated.Length == 3);
               Assert.IsTrue(hydrated[0].Entity.State == EntityState.Current);
               Assert.IsTrue(hydrated[1].Entity.State == EntityState.Deleted);
               Assert.IsTrue(hydrated[2].Entity.State == EntityState.Modified);
               Assert.IsTrue(hydrated[2].Entity.Changes.Contains("MyString"));
               Assert.IsTrue(hydrated[2].MyString == "Three Three Three");
            }
         }
      }

      [TestMethod]
      public void DbEntity_PartialPropertiesWithoutColumnAttributesWillBeScriptedIntoSql()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entity = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable")
               .First();

            entity.MyNumber = 100;
            entity.MyString = "100";
            entity.PartialValue = 100;

            Assert.IsFalse(entity.Entity.Changes.Contains("PartialValue"));
            Asserts.DoesNotThrow(() =>
               accessor.WriteEntity(entity)
               );
         }
      }
   }
}
