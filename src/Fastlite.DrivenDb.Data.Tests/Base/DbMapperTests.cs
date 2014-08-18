using System.Linq;
using Fastlite.DrivenDb.Data.Tests.Base.Infrastructure;
using Fastlite.DrivenDb.Data.Tests.Base.Tables;
using Xunit;

namespace Fastlite.DrivenDb.Data.Tests.Base
{
   public abstract class DbMapperTests : DbTestClass
   {
      [Fact]
      public void ParallelMapper_ReadEntitiesTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.Parallel.ReadEntities<MyTable>("SELECT * FROM MyTable")
               .ToArray();

            Assert.True(entities.Length == 3);
            Assert.True(entities[0].MyIdentity == 1);
            Assert.True(entities[0].MyNumber == 1);
            Assert.True(entities[0].MyString == "One");
            Assert.True(entities[1].MyIdentity == 2);
            Assert.True(entities[1].MyNumber == 2);
            Assert.True(entities[1].MyString == "Two");
            Assert.True(entities[2].MyIdentity == 3);
            Assert.True(entities[2].MyNumber == 3);
            Assert.True(entities[2].MyString == "Three");
         }
      }
   }
}
