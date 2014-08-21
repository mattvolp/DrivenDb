using System;
using System.Linq;
using Fastlite.DrivenDb.Data.Access.MsSql.Interfaces;
using Fastlite.DrivenDb.Data.Tests.Base;
using Fastlite.DrivenDb.Data.Tests.Base.Infrastructure;
using Fastlite.DrivenDb.Data.Tests.Base.Tables;
using Fastlite.DrivenDb.Data.Tests.MsSql.Infrastructure;
using Fastlite.DrivenDb.Data.Tests.MsSql.Tables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if ALL_TESTS

namespace Fastlite.DrivenDb.Data.Tests.MsSql
{
   [TestClass]
   public class MsSqlAccessorTests : DbAccessorTests
   {
      [TestMethod]
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

            Assert.AreEqual(4, entities[0].MyIdentity);
            Assert.AreEqual(5, entities[1].MyIdentity);
            Assert.AreEqual(6, entities[2].MyIdentity);
         }
      }

      [TestMethod]
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

            Assert.AreEqual(expected.Date, actual.PartyDate.Date);
            Assert.AreEqual(expected.Date, actual.PartyDateTime.Date);
            Assert.AreEqual(expected.Date, actual.PartyDateTime2.Date);

            Assert.AreEqual(expected.TimeOfDay, actual.PartyTime);
            Assert.AreEqual(null, actual.PartyTime2);
            Assert.AreEqual(expected.TimeOfDay, actual.PartyDateTime.TimeOfDay);
            Assert.AreEqual(expected.TimeOfDay, actual.PartyDateTime2.TimeOfDay);
         }
      }

      [TestMethod]
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

            Assert.AreEqual(4, entities[0].MyIdentity);
            Assert.AreEqual(5, entities[1].MyIdentity);
            Assert.AreEqual(6, entities[2].MyIdentity);
         }
      }

      [TestMethod]
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

            Assert.AreEqual(1, existing.Count());

            var first = existing.First();

            Assert.IsNull(first.Value2);
            Assert.IsNull(first.Value3);

            var value = GetString(first.Value1);

            Assert.AreEqual("This is a test", value);
         }
      }

      [TestMethod]
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

            Assert.AreEqual(1, existing.Count());

            var first = existing.First();
            var value = GetString(first.Test);

            Assert.AreEqual("This is a test", value);
         }
      }

      [TestMethod]
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

            Assert.AreEqual(1, existing.Count());

            var first = existing.First();
            var value = first.Test;

            Assert.AreEqual("This is a test", value);
         }
      }

      [TestMethod]
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

            var changes = accessor.WriteEntitiesAndOutputDeleted(entities, new { MyNumber = 0L, MyString = "" }).ToArray();

            Assert.AreEqual(changes[0].Item2.MyNumber, 1);
            Assert.AreEqual(changes[0].Item2.MyString, "One");
            Assert.AreEqual(changes[1].Item2.MyNumber, 2);
            Assert.AreEqual(changes[1].Item2.MyString, "Two");
            Assert.AreEqual(changes[2].Item2.MyNumber, 3);
            Assert.AreEqual(changes[2].Item2.MyString, "Three");
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

#endif