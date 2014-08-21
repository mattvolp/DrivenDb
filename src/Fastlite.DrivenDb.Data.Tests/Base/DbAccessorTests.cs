using System;
using System.Linq;
using System.Transactions;
using Fastlite.DrivenDb.Core.Contracts;
using Fastlite.DrivenDb.Core.Contracts.Exceptions;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;
using Fastlite.DrivenDb.Data.Access;
using Fastlite.DrivenDb.Data.Tests.Base.Infrastructure;
using Fastlite.DrivenDb.Data.Tests.Base.Tables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.DrivenDb.Data.Tests.Base
{
   public abstract class DbAccessorTests : DbTestClass
   {            
      [TestMethod]
      public void DbAccessor_TransactionScopeAvoidsExecutionWhenAllEntitiesAreCurrent()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable");

            using (var scope = new TransactionScope())
            {
               accessor.WriteEntities(entities);
               scope.Complete();
            }
         }
      }

      [TestMethod]
      public void DbAccessor_TransactionScopeExecuteCommitsSuccessfully()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            using (var scope = new TransactionScope())
            {
               accessor.Execute("UPDATE MyTable SET MyString = 'testeroo'");
               // causes the sqlite database to be locked, i don't understand this yet
               //accessor.Execute("UPDATE MyTable SET MyNumber = 555");  
               scope.Complete();
            }

            var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable");

            //Assert.IsTrue(entities.All(e => e.MyNumber == 555));
            Assert.IsTrue(entities.All(e => e.MyString == "testeroo"));
         }
      }

      [TestMethod]
      public void DbAccessor_TransactionScopeExecuteRollsbackSuccessfully()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            try
            {
               using (new TransactionScope())
               {
                  accessor.Execute("UPDATE MyTable SET MyString = 'testeroo'");
                  // causes the sqlite database to be locked, i don't understand this yet
                  //accessor.Execute("UPDATE MyTable SET MyNumber = 555");  

                  throw new Exception();
               }
            }
            catch
            {
            }

            var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable");

            //Assert.IsTrue(entities.All(e => e.MyNumber != 555));
            Assert.IsTrue(entities.All(e => e.MyString != "testeroo"));
         }
      }

      [TestMethod]
      public void DbAccessor_ReadValuesStringsReadSuccessfully()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var values = accessor.ReadValues<string>("SELECT MyString FROM MyTable");

            Assert.IsTrue(values.Contains("One"));
            Assert.IsTrue(values.Contains("Two"));
            Assert.IsTrue(values.Contains("Three"));
         }
      }

      [TestMethod]
      public void DbAccessor_ReadEntitiesTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable")
               .ToArray();

            Assert.IsTrue(entities.Length == 3);
            Assert.IsTrue(entities[0].MyIdentity == 1);
            Assert.IsTrue(entities[0].MyNumber == 1);
            Assert.IsTrue(entities[0].MyString == "One");
            Assert.IsTrue(entities[1].MyIdentity == 2);
            Assert.IsTrue(entities[1].MyNumber == 2);
            Assert.IsTrue(entities[1].MyString == "Two");
            Assert.IsTrue(entities[2].MyIdentity == 3);
            Assert.IsTrue(entities[2].MyNumber == 3);
            Assert.IsTrue(entities[2].MyString == "Three");
         }
      }

      [TestMethod]
      public void DbAccessor_ReadTypeTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadType<MyTableType>("SELECT * FROM MyTable")
               .ToArray();

            Assert.IsTrue(entities.Length == 3);
            Assert.IsTrue(entities[0].MyIdentity == 1);
            Assert.IsTrue(entities[0].MyNumber == 1);
            Assert.IsTrue(entities[0].MyString == "One");
            Assert.IsTrue(entities[1].MyIdentity == 2);
            Assert.IsTrue(entities[1].MyNumber == 2);
            Assert.IsTrue(entities[1].MyString == "Two");
            Assert.IsTrue(entities[2].MyIdentity == 3);
            Assert.IsTrue(entities[2].MyNumber == 3);
            Assert.IsTrue(entities[2].MyString == "Three");
         }
      }

      [TestMethod]
      public void DbAccessor_ReadTypeTestWithFields()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadType<MyTableType2>("SELECT * FROM MyTable")
               .ToArray();

            Assert.IsTrue(entities.Length == 3);
            Assert.IsTrue(entities[0].MyIdentity == 1);
            Assert.IsTrue(entities[0].MyNumber == 1);
            Assert.IsTrue(entities[0].MyString == "One");
            Assert.IsTrue(entities[1].MyIdentity == 2);
            Assert.IsTrue(entities[1].MyNumber == 2);
            Assert.IsTrue(entities[1].MyString == "Two");
            Assert.IsTrue(entities[2].MyIdentity == 3);
            Assert.IsTrue(entities[2].MyNumber == 3);
            Assert.IsTrue(entities[2].MyString == "Three");
         }
      }

      [TestMethod]
      public void DbAccessor_InsertEntitiesTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var gnu4 = new MyTable()
               {
                  MyNumber = 4,
                  MyString = "Four"
               };
            var gnu5 = new MyTable()
               {
                  MyNumber = 5,
                  MyString = "Five"
               };
            var gnu6 = new MyTable()
               {
                  MyNumber = 6,
                  MyString = "Six"
               };

            accessor.Inserted += (s, a) =>
               {
                  var changes = a.Changes.ToArray();

                  Assert.IsTrue(changes.Length == 3);
                  Assert.IsTrue(changes[0].ChangeType == DbChangeType.Inserted);
                  Assert.IsTrue(changes[1].ChangeType == DbChangeType.Inserted);
                  Assert.IsTrue(changes[2].ChangeType == DbChangeType.Inserted);
                  Assert.IsTrue(changes[0].AffectedTable == "MyTable");
                  Assert.IsTrue(changes[1].AffectedTable == "MyTable");
                  Assert.IsTrue(changes[2].AffectedTable == "MyTable");
                  //Assert.IsTrue(changes[0].Identity[0].Equals(4L));
                  //Assert.IsTrue(changes[1].Identity[0].Equals(5L));
                  //Assert.IsTrue(changes[2].Identity[0].Equals(6L));
                  Assert.IsTrue(changes[0].AffectedColumns == null);
                  Assert.IsTrue(changes[1].AffectedColumns == null);
                  Assert.IsTrue(changes[2].AffectedColumns == null);
               };

            accessor.WriteEntities(new[] {gnu4, gnu5, gnu6});

            Assert.IsTrue(gnu4.MyIdentity == 4);
            Assert.IsTrue(gnu4.Entity.State == EntityState.Current);
            Assert.IsTrue(gnu5.MyIdentity == 5);
            Assert.IsTrue(gnu5.Entity.State == EntityState.Current);
            Assert.IsTrue(gnu6.MyIdentity == 6);
            Assert.IsTrue(gnu6.Entity.State == EntityState.Current);
         }
      }

      [TestMethod]
      public void DbAccessor_InsertUnknownTypeEntitiesTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var gnu4 = new MyTable()
               {
                  MyNumber = 4,
                  MyString = "Four"
               };
            var gnu5 = new MyTable()
               {
                  MyNumber = 5,
                  MyString = "Five"
               };
            var gnu6 = new MyTable()
               {
                  MyNumber = 6,
                  MyString = "Six"
               };

            accessor.Inserted += (s, a) =>
               {
                  var changes = a.Changes.ToArray();

                  Assert.IsTrue(changes.Length == 3);
                  Assert.IsTrue(changes[0].ChangeType == DbChangeType.Inserted);
                  Assert.IsTrue(changes[1].ChangeType == DbChangeType.Inserted);
                  Assert.IsTrue(changes[2].ChangeType == DbChangeType.Inserted);
                  Assert.IsTrue(changes[0].AffectedTable == "MyTable");
                  Assert.IsTrue(changes[1].AffectedTable == "MyTable");
                  Assert.IsTrue(changes[2].AffectedTable == "MyTable");
                  //Assert.IsTrue(changes[0].Identity[0].Equals(4L));
                  //Assert.IsTrue(changes[1].Identity[0].Equals(5L));
                  //Assert.IsTrue(changes[2].Identity[0].Equals(6L));
                  Assert.IsTrue(changes[0].AffectedColumns == null);
                  Assert.IsTrue(changes[1].AffectedColumns == null);
                  Assert.IsTrue(changes[2].AffectedColumns == null);
               };

            accessor.WriteEntities(new IDbEntity[] {gnu4, gnu5, gnu6});

            Assert.IsTrue(gnu4.MyIdentity == 4);
            Assert.IsTrue(gnu4.Entity.State == EntityState.Current);
            Assert.IsTrue(gnu5.MyIdentity == 5);
            Assert.IsTrue(gnu5.Entity.State == EntityState.Current);
            Assert.IsTrue(gnu6.MyIdentity == 6);
            Assert.IsTrue(gnu6.Entity.State == EntityState.Current);
         }
      }

      [TestMethod]
      public void DbAccessor_DeleteEntitiesTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable")
               .OrderBy(e => e.MyNumber)
               .ToArray();

            Assert.IsTrue(entities.Length == 3);

            entities[1].Entity.Delete();

            accessor.Deleted += (s, a) =>
               {
                  var changes = a.Changes.ToArray();

                  Assert.IsTrue(changes.Length == 1);
                  Assert.IsTrue(changes[0].ChangeType == DbChangeType.Deleted);
                  //Assert.IsTrue(changes[0].Identity[0].Equals(2L));
                  Assert.IsTrue(changes[0].AffectedTable == "MyTable");
                  Assert.IsTrue(changes[0].AffectedColumns == null);
               };

            accessor.WriteEntities(entities);

            Assert.IsTrue(entities[1].Entity.State == EntityState.Deleted);

            entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable")
               .OrderBy(e => e.MyNumber)
               .ToArray();

            Assert.IsTrue(entities.Length == 2);
            Assert.IsTrue(entities[0].MyIdentity == 1);
            Assert.IsTrue(entities[0].MyNumber == 1);
            Assert.IsTrue(entities[0].MyString == "One");
            Assert.IsTrue(entities[1].MyIdentity == 3);
            Assert.IsTrue(entities[1].MyNumber == 3);
            Assert.IsTrue(entities[1].MyString == "Three");
         }
      }

      [TestMethod]
      public void DbAccessor_UpdateEntitiesTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable")
               .OrderBy(e => e.MyNumber)
               .ToArray();

            entities[2].MyString = "I said Three!";

            accessor.Updated += (s, a) =>
               {
                  var changes = a.Changes.ToArray();

                  Assert.IsTrue(changes.Length == 1);
                  Assert.IsTrue(changes[0].ChangeType == DbChangeType.Updated);
                  //Assert.IsTrue(changes[0].Identity[0].Equals(3L));
                  Assert.IsTrue(changes[0].AffectedTable == "MyTable");
                  Assert.IsTrue(changes[0].AffectedColumns.Contains("MyString"));
               };

            accessor.WriteEntities(entities);

            entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable")
               .OrderBy(e => e.MyNumber)
               .ToArray();

            Assert.IsTrue(entities[2].MyIdentity == 3);
            Assert.IsTrue(entities[2].MyNumber == 3);
            Assert.IsTrue(entities[2].MyString == "I said Three!");
         }
      }

      [TestMethod]
      public void DbAccessor_ReadRelatedTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable")
               .OrderBy(e => e.MyNumber)
               .ToArray();

            var children = accessor.ReadRelated<MyTable, MyChildren>(entities[0])
               .On(e => new {e.MyIdentity}, c => new {c.HisIdentity})
               .ToArray();

            Assert.IsTrue(children.Length == 3);

            children = accessor.ReadRelated<MyTable, MyChildren>(entities[1])
               .On(e => new {e.MyIdentity}, c => new {c.HisIdentity})
               .ToArray();

            Assert.IsTrue(children.Length == 0);

            children = accessor.ReadRelated<MyTable, MyChildren>(entities[2])
               .On(e => new {e.MyIdentity}, c => new {c.HisIdentity})
               .ToArray();

            Assert.IsTrue(children.Length == 1);

            children = accessor.ReadRelated<MyTable, MyChildren>(entities)
               .On(e => new {e.MyIdentity}, c => new {c.HisIdentity})
               .ToArray();

            Assert.IsTrue(children.Length == 4);
         }
      }

      [TestMethod]
      public void DbAccessor_ReadAnonymousTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadAnonymous(new {MyIdentity = 0L, MyString = "", MyNumber = 0L}, "SELECT * FROM MyTable").ToArray();

            Assert.IsTrue(entities.Length == 3);
            Assert.IsTrue(entities[0].MyIdentity == 1);
            Assert.IsTrue(entities[0].MyNumber == 1);
            Assert.IsTrue(entities[0].MyString == "One");
            Assert.IsTrue(entities[1].MyIdentity == 2);
            Assert.IsTrue(entities[1].MyNumber == 2);
            Assert.IsTrue(entities[1].MyString == "Two");
            Assert.IsTrue(entities[2].MyIdentity == 3);
            Assert.IsTrue(entities[2].MyNumber == 3);
            Assert.IsTrue(entities[2].MyString == "Three");
         }
      }

      [TestMethod]
      public void DbAccessor_WriteIdentity32()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var gnu = new MyFriend()
               {
                  MyNumber = 1,
                  MyString = "One"
               };

            accessor.WriteEntity(gnu);

            Assert.IsTrue(gnu.MyIdentity == 1);
         }
      }

      [TestMethod]
      public void DbAccessor_WriteIdentity64()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var gnu = new MyTable()
               {
                  MyNumber = 4,
                  MyString = "Four"
               };

            accessor.WriteEntity(gnu);

            Assert.IsTrue(gnu.MyIdentity == 4);
         }
      }

      [TestMethod]
      public void DbAccessor_WriteIdentityMixed()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var gnutable1 = new MyTable()
               {
                  MyNumber = 4,
                  MyString = "Four"
               };
            var gnufriend1 = new MyFriend()
               {
                  MyNumber = 1,
                  MyString = "One"
               };
            var gnutable2 = new MyTable()
               {
                  MyNumber = 5,
                  MyString = "Five"
               };
            var gnufriend2 = new MyFriend()
               {
                  MyNumber = 2,
                  MyString = "Two"
               };

            accessor.WriteEntities(new IDbEntity[] {gnutable1, gnufriend1, gnutable2, gnufriend2});

            Assert.IsTrue(gnutable1.MyIdentity == 4);
            Assert.IsTrue(gnufriend1.MyIdentity == 1);
            Assert.IsTrue(gnutable2.MyIdentity == 5);
            Assert.IsTrue(gnufriend2.MyIdentity == 2);
         }
      }

      [TestMethod]
      public void DbAccessor_ParameterTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            accessor.ReadEntities<MyTable>(@"
               SELECT * FROM MyTable
               WHERE MyIdentity = @0
                  OR MyIdentity IN (@1)
                  OR MyIdentity = @2
                  OR MyIdentity = @3
                  OR MyIdentity = @4
                  OR MyIdentity = @5
                  OR MyIdentity = @6
                  OR MyIdentity = @7
                  OR MyIdentity = @8
                  OR MyIdentity = @9
                  OR MyIdentity = @10
                  OR MyIdentity = @11
                  OR MyIdentity = @2
                  OR MyIdentity IN (@1)"
               , 1
               , new[] {9, 8, 7, 6}, 2, 9, 9, 9, 9, 9, 9, 9, 9, 9
               );
         }
      }

      [TestMethod]
      public void DbAccessor_ParameterTest2()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            try
            {
               accessor.ReadEntities<MyTable>(
                  @"SELECT * FROM MyTable WHERE MyIdentity IN (@1)",
                  new int[0]
                  );

               Assert.IsTrue(false);
            }
            catch (ArgumentException e)
            {
               Assert.IsTrue(true, e.Message);
            }
         }
      }

      [TestMethod]
      public void DbAccessor_DrivenRecordTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var records = accessor.ReadEntities<MyTableRecord>(@"SELECT * FROM MyTable")
               .ToArray();

            Assert.IsTrue(records.Length == 3);
            Assert.IsTrue(records[0].MyIdentity == 1);
            Assert.IsTrue(records[0].MyNumber == 1);
            Assert.IsTrue(records[0].MyString == "One");
            Assert.IsTrue(records[1].MyIdentity == 2);
            Assert.IsTrue(records[1].MyNumber == 2);
            Assert.IsTrue(records[1].MyString == "Two");
            Assert.IsTrue(records[2].MyIdentity == 3);
            Assert.IsTrue(records[2].MyNumber == 3);
            Assert.IsTrue(records[2].MyString == "Three");
         }
      }

      [TestMethod]
      public void DbAccessor_UnrelatedPropertyTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var records = accessor.ReadEntities<MyTablePartial>(@"SELECT * FROM MyTable")
               .ToArray();

            Assert.IsTrue(records.Length == 3);
            Assert.IsTrue(records[0].MyIdentity == 1);
            Assert.IsTrue(records[0].MyNumber == 1);
            Assert.IsTrue(records[0].MyString == "One");
            Assert.IsTrue(records[0].UnrelatedProperty == "yo");
            Assert.IsTrue(records[1].MyIdentity == 2);
            Assert.IsTrue(records[1].MyNumber == 2);
            Assert.IsTrue(records[1].MyString == "Two");
            Assert.IsTrue(records[1].UnrelatedProperty == "yo");
            Assert.IsTrue(records[2].MyIdentity == 3);
            Assert.IsTrue(records[2].MyNumber == 3);
            Assert.IsTrue(records[2].MyString == "Three");
            Assert.IsTrue(records[2].UnrelatedProperty == "yo");
         }
      }

      [TestMethod]
      public void DbAccessor_UnrelatedPropertyTest2()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var record = accessor.ReadIdentity<MyTablePartial, int>(1);

            Assert.IsTrue(record.MyIdentity == 1);
            Assert.IsTrue(record.MyNumber == 1);
            Assert.IsTrue(record.MyString == "One");
            Assert.IsTrue(record.UnrelatedProperty == "yo");
         }
      }

      [TestMethod]
      public void DbAccessor_InactiveExtensionTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithNoExtensions()
               .Build();

            Asserts.Throws<InactiveExtensionException>(() =>
               accessor.ReadEntities<MyTable>(
                  "SELECT * FROM MyTable WHERE MyIdentity IN (@0)"
                  , new[] {1, 2, 3}
                  ));
         }
      }

      [TestMethod]
      public void DbAccessor_MissingResultTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithNoExtensions()
               .Build();

            Asserts.Throws<MissingResultException>(() =>
               accessor.ReadEntities<MyTable, MyChildren>("SELECT * FROM MyTable")
               );
         }
      }
      
      [TestMethod]
      public void DbAccessor_UpdateMultipleValuesTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable")
               .OrderBy(e => e.MyNumber)
               .ToArray();

            entities[2].MyString = "I said Three!";
            entities[2].MyNumber = 33;

            accessor.WriteEntities(entities);

            entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable")
               .OrderBy(e => e.MyNumber)
               .ToArray();

            Assert.IsTrue(entities[2].MyIdentity == 3);
            Assert.IsTrue(entities[2].MyNumber == 33);
            Assert.IsTrue(entities[2].MyString == "I said Three!");
         }
      }

      [TestMethod]
      public void DbAccessor_AllowUnmappedColumnsPassTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithExtensions(AccessorExtension.AllowUnmappedColumns)
               .Build();

            Asserts.DoesNotThrow(() => accessor.ReadEntities<MyTableSlim>("SELECT * FROM MyTable"));
         }
      }

      [TestMethod]
      public void DbAccessor_AllowUnmappedColumnsFailTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithNoExtensions()
               .Build();

            Asserts.Throws<InactiveExtensionException>(() =>
               accessor.ReadEntities<MyTableSlim>("SELECT * FROM MyTable")
               );
         }
      }

      [TestMethod]
      public void DbAccessor_AllowCaseInsensitiveColumnMappingPassTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithExtensions(AccessorExtension.CaseInsensitiveColumnMapping)
               .Build();

            Asserts.DoesNotThrow(() =>
               accessor.ReadEntities<MyTable>(
                  "SELECT MyIdentity as 'myidentity', MyString as 'mystring', MyNumber as 'mynumber' FROM MyTable")
               );
         }
      }

      [TestMethod]
      public void DbAccessor_AllowCaseInsensitiveColumnMappingFailTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithNoExtensions()
               .Build();

            Asserts.Throws<InactiveExtensionException>(() =>
               accessor.ReadEntities<MyTable>(
                  @"SELECT MyIdentity as 'myidentity', MyString as 'mystring', MyNumber as 'mynumber' FROM MyTable")
               );
         }
      }

      [TestMethod]
      public void DbAccessor_DefaultAttributeNameToPropertyName()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            Asserts.DoesNotThrow(() =>
               accessor.ReadEntities<MyTableWithoutColumnNames>(
                  "SELECT MyIdentity as 'myidentity', MyString as 'mystring', MyNumber as 'mynumber' FROM MyTable")
               );
         }
      }

      [TestMethod]
      public void DbAccessor_NullableReadValueTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var value = accessor.ReadValue<int?>("SELECT Value1 FROM NullableTest WHERE Value1 = 1");

            Assert.IsTrue(value.HasValue);
            Assert.AreEqual(value.GetValueOrDefault(), 1);
         }
      }

      [TestMethod]
      public void DbAccessor_NullableReadNullValueTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var value = accessor.ReadValue<int?>("SELECT Value1 FROM NullableTest WHERE Value1 IS NULL");

            Assert.IsFalse(value.HasValue);
         }
      }

      [TestMethod]
      public void DbAccessor_NullableReadNoRowTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var value = accessor.ReadValue<int?>("SELECT Value1 FROM NullableTest WHERE Value1 = 999");

            Assert.IsFalse(value.HasValue);
         }
      }

      [TestMethod]
      public void DbAccessor_NullableReadValuesTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var values = accessor.ReadValues<int?>("SELECT Value1 FROM NullableTest");

            Assert.IsTrue(values.Count() == 4);
            Assert.IsTrue(values.Count(v => v.HasValue && v.Value == 1) == 1);
            Assert.IsTrue(values.Count(v => v.HasValue && v.Value == 2) == 1);
            Assert.IsTrue(values.Count(v => !v.HasValue) == 2);
         }
      }

      [TestMethod]
      public void DbAccessor_SelectIdentityWithLinqTableAttribute()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            Asserts.DoesNotThrow(() => accessor.ReadIdentity<MyTableSlim, int>(1));
         }
      }

      [TestMethod]
      public void DbAccessor_NoPrimaryKeyReadTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadEntities<MyNopkTable>(
               @"SELECT * FROM MyNopkTable"
               );

            Assert.AreEqual(3, entities.Count());
         }
      }

      [TestMethod]
      public void DbAccessor_NoPrimaryKeyWriteTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var gnu = new[]
               {
                  new MyNopkTable() {MyNumber = 7, MyString = "Seven"},
                  new MyNopkTable() {MyNumber = 8, MyString = "Eight"},
                  new MyNopkTable() {MyNumber = 9, MyString = "Nine"},
               };

            accessor.WriteEntities(gnu);

            var entities = accessor.ReadEntities<MyNopkTable>(
               @"SELECT * FROM MyNopkTable"
               );

            Assert.AreEqual(6, entities.Count());
         }
      }

      [TestMethod]
      public void DbAccessor_ReadEntitiesWithCustomPropertyNamesTest()
      {         
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadEntities<MyCustomNameTable>("SELECT * FROM MyTable")
               .ToArray();

            Assert.IsTrue(entities.Length == 3);
            Assert.IsTrue(entities[0].MyIdentitY == 1);
            Assert.IsTrue(entities[0].MyNUMBER == 1);
            Assert.IsTrue(entities[0].MyStringCustom == "One");
            Assert.IsTrue(entities[1].MyIdentitY == 2);
            Assert.IsTrue(entities[1].MyNUMBER == 2);
            Assert.IsTrue(entities[1].MyStringCustom == "Two");
            Assert.IsTrue(entities[2].MyIdentitY == 3);
            Assert.IsTrue(entities[2].MyNUMBER == 3);
            Assert.IsTrue(entities[2].MyStringCustom == "Three");
         }
      }
   }
}