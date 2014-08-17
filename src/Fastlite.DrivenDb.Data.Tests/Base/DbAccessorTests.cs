using System;
using System.IO;
using System.Linq;
using System.Transactions;
using Fastlite.DrivenDb.Core.Contracts;
using Fastlite.DrivenDb.Core.Contracts.Exceptions;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;
using Fastlite.DrivenDb.Data.Access;
using Fastlite.DrivenDb.Data.Tests.Base.Infrastructure;
using Fastlite.DrivenDb.Data.Tests.Base.Tables;
using Xunit;

namespace Fastlite.DrivenDb.Data.Tests.Base
{
   public abstract class DbAccessorTests
   {
      [Fact]
      public void Undelete_RestoresPreviousNewState()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var record = accessor.ReadEntities<MyTable>(@"SELECT * FROM MyTable")
               .Select(t => t.ToNew())
               .First();

            record.Entity.Delete();

            Assert.True(record.Entity.State == EntityState.Deleted);

            record.Entity.Undelete();

            Assert.True(record.Entity.State == EntityState.New);
         }
      }

      [Fact]
      public void Undelete_RestoresPreviousCurrentState()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var record = accessor.ReadEntities<MyTable>(@"SELECT * FROM MyTable")
               .First();

            record.Entity.Delete();

            Assert.True(record.Entity.State == EntityState.Deleted);

            record.Entity.Undelete();

            Assert.True(record.Entity.State == EntityState.Current);
         }
      }

      [Fact]
      public void Undelete_RestoresPreviousUpdateState()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var record = accessor.ReadEntities<MyTable>(@"SELECT * FROM MyTable")
               .Select(t => t.ToUpdate())
               .First();

            record.Entity.Delete();

            Assert.True(record.Entity.State == EntityState.Deleted);

            record.Entity.Undelete();

            Assert.True(record.Entity.State == EntityState.Modified);
         }
      }

      [Fact]
      public void ToUpdate_ProvidesUpdatableEntities()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var records = accessor.ReadEntities<MyTable>(@"SELECT * FROM MyTable")
               .Select(t => t.ToUpdate())
               .ToArray();

            Assert.True(records[0].MyIdentity == 1);
            Assert.True(records[1].MyIdentity == 2);
            Assert.True(records[2].MyIdentity == 3);

            Assert.DoesNotThrow(() => accessor.WriteEntities(records));
         }
      }

      [Fact]
      public void ToNew_ProvidesInsertableEntities()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var records = accessor.ReadEntities<MyTable>(@"SELECT * FROM MyTable")
               .Select(t => t.ToNew())
               .ToArray();

            Assert.True(records[0].MyIdentity == 0);
            Assert.True(records[1].MyIdentity == 0);
            Assert.True(records[2].MyIdentity == 0);

            accessor.WriteEntities(records);

            Assert.True(records.Length == 3);
            Assert.True(records[0].MyIdentity == 4);
            Assert.True(records[1].MyIdentity == 5);
            Assert.True(records[2].MyIdentity == 6);
         }
      }

      [Fact]
      public void FallbackReadEntitiesWithNullTest()
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

            Assert.True(entities.Length == 0);
         }
      }

      [Fact]
      public void FallbackReadEntitiesWithoutValuesTest()
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

            Assert.True(entities.Length == 0);
         }
      }

      [Fact]
      public void FallbackReadEntitiesWithValuesTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var identities = new[] {1, 2, 3};

            var entities = accessor.Fallback.ReadEntities<MyTable>(
               "SELECT * FROM MyTable WHERE MyIdentity IN (@0)"
               , identities
               ).ToArray();

            Assert.True(entities.Length == 3);
         }
      }

      [Fact]
      public void Partial_PropertiesWithoutColumnAttributesWillBeScriptedIntoSql()
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

            Assert.False(entity.Entity.Changes.Contains("PartialValue"));
            Assert.DoesNotThrow(() =>
               accessor.WriteEntity(entity)
               );
         }
      }

      [Fact]
      public void TransactionScope_AvoidsExecutionWhenAllEntitiesAreCurrent()
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

      [Fact]
      public void DbScope_AvoidsExecutionWhenAllEntitiesAreCurrent()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable");

            using (var scope = accessor.CreateScope())
            {
               scope.WriteEntities(entities);
               scope.Commit();
            }
         }
      }

      [Fact]
      public void TransactionScope_ExecuteCommitsSuccessfully()
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

            //Assert.True(entities.All(e => e.MyNumber == 555));
            Assert.True(entities.All(e => e.MyString == "testeroo"));
         }
      }

      [Fact]
      public void TransactionScope_ExecuteRollsbackSuccessfully()
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

            //Assert.True(entities.All(e => e.MyNumber != 555));
            Assert.True(entities.All(e => e.MyString != "testeroo"));
         }
      }

      [Fact]
      public void ReadValues_StringsReadSuccessfully()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var values = accessor.ReadValues<string>("SELECT MyString FROM MyTable");

            Assert.True(values.Contains("One"));
            Assert.True(values.Contains("Two"));
            Assert.True(values.Contains("Three"));
         }
      }

      [Fact]
      public void DbScope_ExecuteCommitsSuccessfully()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            using (var scope = accessor.CreateScope())
            {
               scope.Execute("UPDATE MyTable SET MyString = 'testeroo'");
               scope.Execute("UPDATE MyTable SET MyNumber = 555");
               scope.Commit();
            }

            var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable");

            Assert.True(entities.All(e => e.MyNumber == 555));
            Assert.True(entities.All(e => e.MyString == "testeroo"));
         }
      }

      [Fact]
      public void DbScope_ExecuteRollsbackSuccessfully()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            using (var scope = accessor.CreateScope())
            {
               scope.Execute("UPDATE MyTable SET MyString = 'testeroo'");
               scope.Execute("UPDATE MyTable SET MyNumber = 555");
            }

            var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable");

            Assert.True(entities.All(e => e.MyNumber != 555));
            Assert.True(entities.All(e => e.MyString != "testeroo"));
         }
      }
      
      [Fact]
      public void ReadEntitiesTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable")
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

      [Fact]
      public void ParallelReadEntitiesTest()
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

      [Fact]
      public void ReadTypeTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadType<MyTableType>("SELECT * FROM MyTable")
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

      [Fact]
      public void ReadTypeTestWithFields()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadType<MyTableType2>("SELECT * FROM MyTable")
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

      [Fact]
      public void InsertEntitiesTest()
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

                  Assert.True(changes.Length == 3);
                  Assert.True(changes[0].ChangeType == DbChangeType.Inserted);
                  Assert.True(changes[1].ChangeType == DbChangeType.Inserted);
                  Assert.True(changes[2].ChangeType == DbChangeType.Inserted);
                  Assert.True(changes[0].AffectedTable == "MyTable");
                  Assert.True(changes[1].AffectedTable == "MyTable");
                  Assert.True(changes[2].AffectedTable == "MyTable");
                  //Assert.True(changes[0].Identity[0].Equals(4L));
                  //Assert.True(changes[1].Identity[0].Equals(5L));
                  //Assert.True(changes[2].Identity[0].Equals(6L));
                  Assert.True(changes[0].AffectedColumns == null);
                  Assert.True(changes[1].AffectedColumns == null);
                  Assert.True(changes[2].AffectedColumns == null);
               };

            accessor.WriteEntities(new[] {gnu4, gnu5, gnu6});

            Assert.True(gnu4.MyIdentity == 4);
            Assert.True(gnu4.Entity.State == EntityState.Current);
            Assert.True(gnu5.MyIdentity == 5);
            Assert.True(gnu5.Entity.State == EntityState.Current);
            Assert.True(gnu6.MyIdentity == 6);
            Assert.True(gnu6.Entity.State == EntityState.Current);
         }
      }

      [Fact]
      public void InsertUnknownTypeEntitiesTest()
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

                  Assert.True(changes.Length == 3);
                  Assert.True(changes[0].ChangeType == DbChangeType.Inserted);
                  Assert.True(changes[1].ChangeType == DbChangeType.Inserted);
                  Assert.True(changes[2].ChangeType == DbChangeType.Inserted);
                  Assert.True(changes[0].AffectedTable == "MyTable");
                  Assert.True(changes[1].AffectedTable == "MyTable");
                  Assert.True(changes[2].AffectedTable == "MyTable");
                  //Assert.True(changes[0].Identity[0].Equals(4L));
                  //Assert.True(changes[1].Identity[0].Equals(5L));
                  //Assert.True(changes[2].Identity[0].Equals(6L));
                  Assert.True(changes[0].AffectedColumns == null);
                  Assert.True(changes[1].AffectedColumns == null);
                  Assert.True(changes[2].AffectedColumns == null);
               };

            accessor.WriteEntities(new IDbEntity[] {gnu4, gnu5, gnu6});

            Assert.True(gnu4.MyIdentity == 4);
            Assert.True(gnu4.Entity.State == EntityState.Current);
            Assert.True(gnu5.MyIdentity == 5);
            Assert.True(gnu5.Entity.State == EntityState.Current);
            Assert.True(gnu6.MyIdentity == 6);
            Assert.True(gnu6.Entity.State == EntityState.Current);
         }
      }

      [Fact]
      public void DeleteEntitiesTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable")
               .OrderBy(e => e.MyNumber)
               .ToArray();

            Assert.True(entities.Length == 3);

            entities[1].Entity.Delete();

            accessor.Deleted += (s, a) =>
               {
                  var changes = a.Changes.ToArray();

                  Assert.True(changes.Length == 1);
                  Assert.True(changes[0].ChangeType == DbChangeType.Deleted);
                  //Assert.True(changes[0].Identity[0].Equals(2L));
                  Assert.True(changes[0].AffectedTable == "MyTable");
                  Assert.True(changes[0].AffectedColumns == null);
               };

            accessor.WriteEntities(entities);

            Assert.True(entities[1].Entity.State == EntityState.Deleted);

            entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable")
               .OrderBy(e => e.MyNumber)
               .ToArray();

            Assert.True(entities.Length == 2);
            Assert.True(entities[0].MyIdentity == 1);
            Assert.True(entities[0].MyNumber == 1);
            Assert.True(entities[0].MyString == "One");
            Assert.True(entities[1].MyIdentity == 3);
            Assert.True(entities[1].MyNumber == 3);
            Assert.True(entities[1].MyString == "Three");
         }
      }

      [Fact]
      public void UpdateEntitiesTest()
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

                  Assert.True(changes.Length == 1);
                  Assert.True(changes[0].ChangeType == DbChangeType.Updated);
                  //Assert.True(changes[0].Identity[0].Equals(3L));
                  Assert.True(changes[0].AffectedTable == "MyTable");
                  Assert.True(changes[0].AffectedColumns.Contains("MyString"));
               };

            accessor.WriteEntities(entities);

            entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable")
               .OrderBy(e => e.MyNumber)
               .ToArray();

            Assert.True(entities[2].MyIdentity == 3);
            Assert.True(entities[2].MyNumber == 3);
            Assert.True(entities[2].MyString == "I said Three!");
         }
      }

      [Fact]
      public void ReadRelatedTest()
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

            Assert.True(children.Length == 3);

            children = accessor.ReadRelated<MyTable, MyChildren>(entities[1])
               .On(e => new {e.MyIdentity}, c => new {c.HisIdentity})
               .ToArray();

            Assert.True(children.Length == 0);

            children = accessor.ReadRelated<MyTable, MyChildren>(entities[2])
               .On(e => new {e.MyIdentity}, c => new {c.HisIdentity})
               .ToArray();

            Assert.True(children.Length == 1);

            children = accessor.ReadRelated<MyTable, MyChildren>(entities)
               .On(e => new {e.MyIdentity}, c => new {c.HisIdentity})
               .ToArray();

            Assert.True(children.Length == 4);
         }
      }

      [Fact]
      public void SerializationTest()
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

               Assert.True(hydrated.Length == 3);
               Assert.True(hydrated[0].Entity.State == EntityState.Current);
               Assert.True(hydrated[1].Entity.State == EntityState.Deleted);
               Assert.True(hydrated[2].Entity.State == EntityState.Modified);
               Assert.True(hydrated[2].Entity.Changes.Contains("MyString"));
               Assert.True(hydrated[2].MyString == "Three Three Three");
            }
         }
      }

      [Fact]
      public void ReadAnonymousTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadAnonymous(new {MyIdentity = 0L, MyString = "", MyNumber = 0L}, "SELECT * FROM MyTable").ToArray();

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

      [Fact]
      public void WriteIdentity32()
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

            Assert.True(gnu.MyIdentity == 1);
         }
      }

      [Fact]
      public void WriteIdentity64()
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

            Assert.True(gnu.MyIdentity == 4);
         }
      }

      [Fact]
      public void WriteIdentityMixed()
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

            Assert.True(gnutable1.MyIdentity == 4);
            Assert.True(gnufriend1.MyIdentity == 1);
            Assert.True(gnutable2.MyIdentity == 5);
            Assert.True(gnufriend2.MyIdentity == 2);
         }
      }

      [Fact]
      public void ParameterTest()
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

      [Fact]
      public void ParameterTest2()
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

               Assert.True(false);
            }
            catch (ArgumentException e)
            {
               Assert.True(true, e.Message);
            }
         }
      }

      [Fact]
      public void DrivenRecordTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var records = accessor.ReadEntities<MyTableRecord>(@"SELECT * FROM MyTable")
               .ToArray();

            Assert.True(records.Length == 3);
            Assert.True(records[0].MyIdentity == 1);
            Assert.True(records[0].MyNumber == 1);
            Assert.True(records[0].MyString == "One");
            Assert.True(records[1].MyIdentity == 2);
            Assert.True(records[1].MyNumber == 2);
            Assert.True(records[1].MyString == "Two");
            Assert.True(records[2].MyIdentity == 3);
            Assert.True(records[2].MyNumber == 3);
            Assert.True(records[2].MyString == "Three");
         }
      }

      [Fact]
      public void UnrelatedPropertyTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var records = accessor.ReadEntities<MyTablePartial>(@"SELECT * FROM MyTable")
               .ToArray();

            Assert.True(records.Length == 3);
            Assert.True(records[0].MyIdentity == 1);
            Assert.True(records[0].MyNumber == 1);
            Assert.True(records[0].MyString == "One");
            Assert.True(records[0].UnrelatedProperty == "yo");
            Assert.True(records[1].MyIdentity == 2);
            Assert.True(records[1].MyNumber == 2);
            Assert.True(records[1].MyString == "Two");
            Assert.True(records[1].UnrelatedProperty == "yo");
            Assert.True(records[2].MyIdentity == 3);
            Assert.True(records[2].MyNumber == 3);
            Assert.True(records[2].MyString == "Three");
            Assert.True(records[2].UnrelatedProperty == "yo");
         }
      }

      [Fact]
      public void UnrelatedPropertyTest2()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var record = accessor.ReadIdentity<MyTablePartial, int>(1);

            Assert.True(record.MyIdentity == 1);
            Assert.True(record.MyNumber == 1);
            Assert.True(record.MyString == "One");
            Assert.True(record.UnrelatedProperty == "yo");
         }
      }

      [Fact]
      public void InactiveExtensionTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithNoExtensions()
               .Build();

            Assert.Throws<InactiveExtensionException>(() =>
               accessor.ReadEntities<MyTable>(
                  "SELECT * FROM MyTable WHERE MyIdentity IN (@0)"
                  , new[] {1, 2, 3}
                  ));
         }
      }

      [Fact]
      public void MissingResultTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithNoExtensions()
               .Build();

            Assert.Throws<MissingResultException>(() =>
               accessor.ReadEntities<MyTable, MyChildren>("SELECT * FROM MyTable")
               );
         }
      }
      
      [Fact]
      public void UpdateMultipleValuesTest()
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

            Assert.True(entities[2].MyIdentity == 3);
            Assert.True(entities[2].MyNumber == 33);
            Assert.True(entities[2].MyString == "I said Three!");
         }
      }

      [Fact]
      public void AllowUnmappedColumnsPassTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithExtensions(AccessorExtension.AllowUnmappedColumns)
               .Build();

            Assert.DoesNotThrow(() => accessor.ReadEntities<MyTableSlim>("SELECT * FROM MyTable"));
         }
      }

      [Fact]
      public void AllowUnmappedColumnsFailTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithNoExtensions()
               .Build();

            Assert.Throws<InactiveExtensionException>(() =>
               accessor.ReadEntities<MyTableSlim>("SELECT * FROM MyTable")
               );
         }
      }

      [Fact]
      public void AllowCaseInsensitiveColumnMappingPassTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithExtensions(AccessorExtension.CaseInsensitiveColumnMapping)
               .Build();

            Assert.DoesNotThrow(() =>
               accessor.ReadEntities<MyTable>(
                  "SELECT MyIdentity as 'myidentity', MyString as 'mystring', MyNumber as 'mynumber' FROM MyTable")
               );
         }
      }

      [Fact]
      public void AllowCaseInsensitiveColumnMappingFailTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithNoExtensions()
               .Build();

            Assert.Throws<InactiveExtensionException>(() =>
               accessor.ReadEntities<MyTable>(
                  @"SELECT MyIdentity as 'myidentity', MyString as 'mystring', MyNumber as 'mynumber' FROM MyTable")
               );
         }
      }

      [Fact]
      public void DefaultAttributeNameToPropertyName()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            Assert.DoesNotThrow(() =>
               accessor.ReadEntities<MyTableWithoutColumnNames>(
                  "SELECT MyIdentity as 'myidentity', MyString as 'mystring', MyNumber as 'mynumber' FROM MyTable")
               );
         }
      }

      [Fact]
      public void NullableReadValueTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var value = accessor.ReadValue<int?>("SELECT Value1 FROM NullableTest WHERE Value1 = 1");

            Assert.True(value.HasValue);
            Assert.Equal(value.GetValueOrDefault(), 1);
         }
      }

      [Fact]
      public void NullableReadNullValueTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var value = accessor.ReadValue<int?>("SELECT Value1 FROM NullableTest WHERE Value1 IS NULL");

            Assert.False(value.HasValue);
         }
      }

      [Fact]
      public void NullableReadNoRowTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var value = accessor.ReadValue<int?>("SELECT Value1 FROM NullableTest WHERE Value1 = 999");

            Assert.False(value.HasValue);
         }
      }

      [Fact]
      public void NullableReadValuesTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var values = accessor.ReadValues<int?>("SELECT Value1 FROM NullableTest");

            Assert.True(values.Count() == 4);
            Assert.True(values.Count(v => v.HasValue && v.Value == 1) == 1);
            Assert.True(values.Count(v => v.HasValue && v.Value == 2) == 1);
            Assert.True(values.Count(v => !v.HasValue) == 2);
         }
      }

      [Fact]
      public void SelectIdentityWithLinqTableAttribute()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            Assert.DoesNotThrow(() => accessor.ReadIdentity<MyTableSlim, int>(1));
         }
      }

      [Fact]
      public void DbScopeCommitsData()
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

            using (var scope = accessor.CreateScope())
            {
               scope.WriteEntity(gnu4);
               scope.WriteEntities(new[] {gnu5, gnu6});
               scope.Commit();
            }

            var committed = accessor.ReadEntities<MyTable>(
               @"SELECT * FROM MyTable"
               );

            Assert.Equal(6, committed.Count());
         }
      }

      [Fact]
      public void DbScopeRollsbackData()
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

            using (var scope = accessor.CreateScope())
            {
               scope.WriteEntity(gnu4);
               scope.WriteEntities(new[] {gnu5, gnu6});
            }

            var committed = accessor.ReadEntities<MyTable>(
               @"SELECT * FROM MyTable"
               );

            Assert.Equal(3, committed.Count());
         }
      }

      [Fact]
      public void NoPrimaryKeyReadTest()
      {
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadEntities<MyNopkTable>(
               @"SELECT * FROM MyNopkTable"
               );

            Assert.Equal(3, entities.Count());
         }
      }

      [Fact]
      public void NoPrimaryKeyWriteTest()
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

            Assert.Equal(6, entities.Count());
         }
      }

      [Fact]
      public void ReadEntitiesWithCustomPropertyNamesTest()
      {         
         using (var fixture = CreateFixture())
         {
            var accessor = fixture.CreateAccessor()
               .WithAllExtensions()
               .Build();

            var entities = accessor.ReadEntities<MyCustomNameTable>("SELECT * FROM MyTable")
               .ToArray();

            Assert.True(entities.Length == 3);
            Assert.True(entities[0].MyIdentitY == 1);
            Assert.True(entities[0].MyNUMBER == 1);
            Assert.True(entities[0].MyStringCustom == "One");
            Assert.True(entities[1].MyIdentitY == 2);
            Assert.True(entities[1].MyNUMBER == 2);
            Assert.True(entities[1].MyStringCustom == "Two");
            Assert.True(entities[2].MyIdentitY == 3);
            Assert.True(entities[2].MyNUMBER == 3);
            Assert.True(entities[2].MyStringCustom == "Three");
         }
      }

      protected abstract IDbTestFixture CreateFixture();      
   }
}