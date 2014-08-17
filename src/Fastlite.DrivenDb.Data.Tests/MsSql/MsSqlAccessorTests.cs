using System;
using System.Linq;
using Fastlite.DrivenDb.Data.Access.MsSql.Interfaces;
using Fastlite.DrivenDb.Data.Tests.Base;
using Fastlite.DrivenDb.Data.Tests.Base.Infrastructure;
using Fastlite.DrivenDb.Data.Tests.Base.Tables;
using Fastlite.DrivenDb.Data.Tests.MsSql.Infrastructure;
using Fastlite.DrivenDb.Data.Tests.MsSql.Tables;
using Xunit;

namespace Fastlite.DrivenDb.Data.Tests.MsSql
{
   public class MsSqlAccessorTests : DbAccessorTests
   {
      [Fact]
      public void WriteTransactionWithScopeIdentityTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = (IMsSqlAccessor) fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = new[]
               {
                  new MyTable() {MyNumber = 2, MyString = "2"},
                  new MyTable() {MyNumber = 3, MyString = "3"},
                  new MyTable() {MyNumber = 4, MyString = "4"},
               };

            using (var scope = accessor.CreateScope())
            {
               scope.WriteEntitiesUsingScopeIdentity(entities);
               scope.Commit();
            }

            Assert.Equal(4, entities[0].MyIdentity);
            Assert.Equal(5, entities[1].MyIdentity);
            Assert.Equal(6, entities[2].MyIdentity);
         }
      }

      [Fact]
      public void ReadEntitysWithTimespanColumnSucceeds()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = (IMsSqlAccessor) fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var actual = accessor.ReadEntities<TimeTable>(
               @"SELECT TOP 1 * FROM [TimeTable]"
               ).Single();

            var expected = new DateTime(1972, 8, 2, 6, 5, 33);

            Assert.Equal(expected.Date, actual.PartyDate.Date);
            Assert.Equal(expected.Date, actual.PartyDateTime.Date);
            Assert.Equal(expected.Date, actual.PartyDateTime2.Date);

            Assert.Equal(expected.TimeOfDay, actual.PartyTime);
            Assert.Equal(null, actual.PartyTime2);
            Assert.Equal(expected.TimeOfDay, actual.PartyDateTime.TimeOfDay);
            Assert.Equal(expected.TimeOfDay, actual.PartyDateTime2.TimeOfDay);
         }
      }

      [Fact]
      public void WriteEntitiesWithScopeIdentityTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = (IMsSqlAccessor) fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = new MyTable[]
               {
                  new MyTable() {MyNumber = 2, MyString = "2"},
                  new MyTable() {MyNumber = 3, MyString = "3"},
                  new MyTable() {MyNumber = 4, MyString = "4"},
               };

            accessor.WriteEntitiesUsingScopeIdentity(entities);

            Assert.Equal(4, entities[0].MyIdentity);
            Assert.Equal(5, entities[1].MyIdentity);
            Assert.Equal(6, entities[2].MyIdentity);
         }
      }

      [Fact]
      public void VarbinaryTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = (IMsSqlAccessor) fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var gnu = new VarbinaryTest()
               {
                  Value1 = GetBytes("This is a test")
               };

            accessor.WriteEntity(gnu);

            var existing = accessor.ReadEntities<VarbinaryTest>("SELECT * FROM [VarbinaryTest]");

            Assert.Equal(1, existing.Count());

            var first = existing.First();

            Assert.Null(first.Value2);
            Assert.Null(first.Value3);

            var value = GetString(first.Value1);

            Assert.Equal("This is a test", value);
         }
      }

      [Fact]
      public void ImageTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = (IMsSqlAccessor) fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var gnu = new ImageTable()
               {
                  Test = GetBytes("This is a test")
               };

            accessor.WriteEntity(gnu);

            var existing = accessor.ReadEntities<ImageTable>("SELECT * FROM [ImageTable]");

            Assert.Equal(1, existing.Count());

            var first = existing.First();
            var value = GetString(first.Test);

            Assert.Equal("This is a test", value);
         }
      }

      [Fact]
      public void TextTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = (IMsSqlAccessor) fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var gnu = new TextTable()
               {
                  Test = "This is a test"
               };

            accessor.WriteEntity(gnu);

            var existing = accessor.ReadEntities<TextTable>("SELECT * FROM [TextTable]");

            Assert.Equal(1, existing.Count());

            var first = existing.First();
            var value = first.Test;

            Assert.Equal("This is a test", value);
         }
      }

      [Fact]
      public void WriteEntitiesWithOutputTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = (IMsSqlAccessor) fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadEntities<MyTable>("SELECT * FROM [MyTable]")
               .ToList();

            entities[0].MyNumber = 100;
            entities[0].MyString = "100";
            entities[1].MyNumber = 200;
            entities[1].MyString = "200";
            entities[2].Entity.Delete();

            var gnu = new MyTable()
               {
                  MyNumber = 400,
                  MyString = "400"
               };

            entities.Add(gnu);

            var changes = accessor.WriteEntitiesAndOutputDeleted(entities, new {MyNumber = 0L, MyString = ""}).ToArray();

            Assert.Equal(changes[0].Item2.MyNumber, 1);
            Assert.Equal(changes[0].Item2.MyString, "One");
            Assert.Equal(changes[1].Item2.MyNumber, 2);
            Assert.Equal(changes[1].Item2.MyString, "Two");
            Assert.Equal(changes[2].Item2.MyNumber, 3);
            Assert.Equal(changes[2].Item2.MyString, "Three");
         }
      }

      private static byte[] GetBytes(string str)
      {
         byte[] bytes = new byte[str.Length * sizeof(char)];
         System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
         return bytes;
      }

      private static string GetString(byte[] bytes)
      {
         char[] chars = new char[bytes.Length / sizeof(char)];
         System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
         return new string(chars);
      }

      protected override IDbTestFixture CreateFixture()
      {
         return new MsSqlTestFixture();
      }
   }
}