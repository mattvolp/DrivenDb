using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Transactions;
using System.Xml;
using Xunit;

namespace DrivenDb.Tests.Language.Interfaces
{
   public abstract class IDbAccessorTests
   {
      [Fact]
      public void Undelete_RestoresPreviousNewState()
      {
         string key;

         var accessor = CreateAccessor(out key);
         var record = accessor.ReadEntities<MyTable>(@"SELECT * FROM MyTable")
            .Select(t => t.ToNew())
            .First();

         record.Entity.Delete();

         Assert.True(record.Entity.State == EntityState.Deleted);

         record.Entity.Undelete();

         Assert.True(record.Entity.State == EntityState.New);

         DestroyAccessor(key);
      }

      [Fact]
      public void Undelete_RestoresPreviousCurrentState()
      {
         string key;

         var accessor = CreateAccessor(out key);
         var record = accessor.ReadEntities<MyTable>(@"SELECT * FROM MyTable")
            .First();

         record.Entity.Delete();

         Assert.True(record.Entity.State == EntityState.Deleted);

         record.Entity.Undelete();

         Assert.True(record.Entity.State == EntityState.Current);

         DestroyAccessor(key);
      }

      [Fact]
      public void Undelete_RestoresPreviousUpdateState()
      {
         string key;

         var accessor = CreateAccessor(out key);
         var record = accessor.ReadEntities<MyTable>(@"SELECT * FROM MyTable")
            .Select(t => t.ToUpdate())
            .First();

         record.Entity.Delete();

         Assert.True(record.Entity.State == EntityState.Deleted);

         record.Entity.Undelete();

         Assert.True(record.Entity.State == EntityState.Modified);

         DestroyAccessor(key);
      }

      [Fact]
      public void ToUpdate_ProvidesUpdatableEntities()
      {
         string key;

         var accessor = CreateAccessor(out key);
         var records = accessor.ReadEntities<MyTable>(@"SELECT * FROM MyTable")
            .Select(t => t.ToUpdate())
            .ToArray();

         Assert.True(records[0].MyIdentity == 1);
         Assert.True(records[1].MyIdentity == 2);
         Assert.True(records[2].MyIdentity == 3);

         Assert.DoesNotThrow(() => accessor.WriteEntities(records));

         DestroyAccessor(key);
      }

      [Fact]
      public void ToNew_ProvidesInsertableEntities()
      {
         string key;

         var accessor = CreateAccessor(out key);
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

         DestroyAccessor(key);
      }

      [Fact]
      public void FallbackReadEntitiesWithNullTest()
      {
         string key;

         var accessor = CreateAccessor(out key);
         var entities = accessor.Fallback.ReadEntities<MyTable>(
            "SELECT * FROM MyTable WHERE MyIdentity IN (@0)"
            , default(int[])
            ).ToArray();

         Assert.True(entities.Length == 0);

         DestroyAccessor(key);
      }

      [Fact]
      public void FallbackReadEntitiesWithoutValuesTest()
      {
         string key;

         var accessor = CreateAccessor(out key);
         var identities = new int[0];

         var entities = accessor.Fallback.ReadEntities<MyTable>(
            "SELECT * FROM MyTable WHERE MyIdentity IN (@0)"
            , identities
            ).ToArray();

         Assert.True(entities.Length == 0);

         DestroyAccessor(key);
      }

      [Fact]
      public void FallbackReadEntitiesWithValuesTest()
      {
         string key;

         var accessor = CreateAccessor(out key);
         var identities = new[] { 1, 2, 3 };

         var entities = accessor.Fallback.ReadEntities<MyTable>(
            "SELECT * FROM MyTable WHERE MyIdentity IN (@0)"
            , identities
            ).ToArray();

         Assert.True(entities.Length == 3);

         DestroyAccessor(key);
      }

      [Fact]
      public void Partial_PropertiesWithoutColumnAttributesWillBeScriptedIntoSql()
      {
         string key;

         var accessor = CreateAccessor(out key);
         var entity = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable")
            .First();

         entity.MyNumber = 100;
         entity.MyString = "100";
         entity.PartialValue = 100;

         Assert.False(entity.Entity.Changes.Contains("PartialValue"));
         Assert.DoesNotThrow(() =>
            accessor.WriteEntity(entity)
            );

         DestroyAccessor(key);
      }

      [Fact]
      public void TransactionScope_AvoidsExecutionWhenAllEntitiesAreCurrent()
      {
         string key;

         var accessor = CreateAccessor(out key);
         var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable");

         using (var scope = new TransactionScope())
         {
            accessor.WriteEntities(entities);
            scope.Complete();
         }

         DestroyAccessor(key);
      }

      [Fact]
      public void DbScope_AvoidsExecutionWhenAllEntitiesAreCurrent()
      {
         string key;

         var accessor = CreateAccessor(out key);
         var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable");

         using (var scope = accessor.CreateScope())
         {
            scope.WriteEntities(entities);
            scope.Commit();
         }

         DestroyAccessor(key);
      }

      [Fact]
      public void TransactionScope_ExecuteCommitsSuccessfully()
      {
         string key;

         var accessor = CreateAccessor(out key);

         using (var scope = new TransactionScope())
         {
            accessor.Execute("UPDATE MyTable SET MyString = 'testeroo'");
            //accessor.Execute("UPDATE MyTable SET MyNumber = 555");  // causes the sqlite database to be locked, i don't understand this yet
            scope.Complete();
         }

         var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable");

         //Assert.True(entities.All(e => e.MyNumber == 555));
         Assert.True(entities.All(e => e.MyString == "testeroo"));

         DestroyAccessor(key);
      }

      [Fact]
      public void TransactionScope_ExecuteRollsbackSuccessfully()
      {
         string key;

         var accessor = CreateAccessor(out key);

         try
         {
            using (var scope = new TransactionScope())
            {
               accessor.Execute("UPDATE MyTable SET MyString = 'testeroo'");
               //accessor.Execute("UPDATE MyTable SET MyNumber = 555");  // causes the sqlite database to be locked, i don't understand this yet

               throw new Exception();
            }
         }
         catch
         {
         }

         var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable");

         //Assert.True(entities.All(e => e.MyNumber != 555));
         Assert.True(entities.All(e => e.MyString != "testeroo"));

         DestroyAccessor(key);
      }

      [Fact]
      public void ReadValues_StringsReadSuccessfully()
      {
         string key;

         var accessor = CreateAccessor(out key);
         var values = accessor.ReadValues<string>("SELECT MyString FROM MyTable");

         Assert.True(values.Contains("One"));
         Assert.True(values.Contains("Two"));
         Assert.True(values.Contains("Three"));

         DestroyAccessor(key);
      }

      [Fact]
      public void DbScope_ExecuteCommitsSuccessfully()
      {
         string key;

         var accessor = CreateAccessor(out key);

         using (var scope = accessor.CreateScope())
         {
            scope.Execute("UPDATE MyTable SET MyString = 'testeroo'");
            scope.Execute("UPDATE MyTable SET MyNumber = 555");
            scope.Commit();
         }

         var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable");

         Assert.True(entities.All(e => e.MyNumber == 555));
         Assert.True(entities.All(e => e.MyString == "testeroo"));

         DestroyAccessor(key);
      }

      [Fact]
      public void DbScope_ExecuteRollsbackSuccessfully()
      {
         string key;

         var accessor = CreateAccessor(out key);

         using (var scope = accessor.CreateScope())
         {
            scope.Execute("UPDATE MyTable SET MyString = 'testeroo'");
            scope.Execute("UPDATE MyTable SET MyNumber = 555");
         }

         var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable");

         Assert.True(entities.All(e => e.MyNumber != 555));
         Assert.True(entities.All(e => e.MyString != "testeroo"));

         DestroyAccessor(key);
      }

      //[Fact]
      //public void InsertEntitiesWithoutReturnIdTest()
      //{
      //   string key;

      //   var accessor = CreateAccessor(out key);
      //   var gnu4 = new MyTable()
      //      {
      //         MyNumber = 4,
      //         MyString = "Four"
      //      };
      //   var gnu5 = new MyTable()
      //      {
      //         MyNumber = 5,
      //         MyString = "Five"
      //      };
      //   var gnu6 = new MyTable()
      //      {
      //         MyNumber = 6,
      //         MyString = "Six"
      //      };

      //   accessor.WriteEntities(new[] { gnu4, gnu5, gnu6 }, false);

      //   Assert.True(gnu4.MyIdentity == 0);
      //   Assert.True(gnu4.Entity.State == EntityState.New);
      //   Assert.True(gnu5.MyIdentity == 0);
      //   Assert.True(gnu5.Entity.State == EntityState.New);
      //   Assert.True(gnu6.MyIdentity == 0);
      //   Assert.True(gnu6.Entity.State == EntityState.New);

      //   DestroyAccessor(key);
      //}

      [Fact]
      public void ReadEntitiesTest()
      {
         string key;

         var accessor = CreateAccessor(out key);
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

         DestroyAccessor(key);
      }

      [Fact]
      public void ParallelReadEntitiesTest()
      {
         string key;

         var accessor = CreateAccessor(out key);
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

         DestroyAccessor(key);
      }

      [Fact]
      public void ReadTypeTest()
      {
         string key;

         var accessor = CreateAccessor(out key);
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

         DestroyAccessor(key);
      }

      [Fact]
      public void ReadTypeTestWithFields()
      {
         string key;

         var accessor = CreateAccessor(out key);
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

         DestroyAccessor(key);
      }

      [Fact]
      public void InsertEntitiesTest()
      {
         string key;

         var accessor = CreateAccessor(out key);
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

         accessor.WriteEntities(new[] { gnu4, gnu5, gnu6 });

         Assert.True(gnu4.MyIdentity == 4);
         Assert.True(gnu4.Entity.State == EntityState.Current);
         Assert.True(gnu5.MyIdentity == 5);
         Assert.True(gnu5.Entity.State == EntityState.Current);
         Assert.True(gnu6.MyIdentity == 6);
         Assert.True(gnu6.Entity.State == EntityState.Current);

         DestroyAccessor(key);
      }

      [Fact]
      public void InsertUnknownTypeEntitiesTest()
      {
         string key;

         var accessor = CreateAccessor(out key);
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

         accessor.WriteEntities(new IDbEntity[] { gnu4, gnu5, gnu6 });

         Assert.True(gnu4.MyIdentity == 4);
         Assert.True(gnu4.Entity.State == EntityState.Current);
         Assert.True(gnu5.MyIdentity == 5);
         Assert.True(gnu5.Entity.State == EntityState.Current);
         Assert.True(gnu6.MyIdentity == 6);
         Assert.True(gnu6.Entity.State == EntityState.Current);

         DestroyAccessor(key);
      }

      [Fact]
      public void DeleteEntitiesTest()
      {
         string key;

         var accessor = CreateAccessor(out key);
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

         DestroyAccessor(key);
      }

      [Fact]
      public void UpdateEntitiesTest()
      {
         string key;

         var accessor = CreateAccessor(out key);
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

         DestroyAccessor(key);
      }

      [Fact]
      public void ReadRelatedTest()
      {
         string key;

         var accessor = CreateAccessor(out key);
         var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable")
            .OrderBy(e => e.MyNumber)
            .ToArray();

         var children = accessor.ReadRelated<MyTable, MyChildren>(entities[0])
            .On(e => new { e.MyIdentity }, c => new { c.HisIdentity })
            .ToArray();

         Assert.True(children.Length == 3);

         children = accessor.ReadRelated<MyTable, MyChildren>(entities[1])
            .On(e => new { e.MyIdentity }, c => new { c.HisIdentity })
            .ToArray();

         Assert.True(children.Length == 0);

         children = accessor.ReadRelated<MyTable, MyChildren>(entities[2])
            .On(e => new { e.MyIdentity }, c => new { c.HisIdentity })
            .ToArray();

         Assert.True(children.Length == 1);

         children = accessor.ReadRelated<MyTable, MyChildren>(entities)
            .On(e => new { e.MyIdentity }, c => new { c.HisIdentity })
            .ToArray();

         Assert.True(children.Length == 4);

         DestroyAccessor(key);
      }

      [Fact]
      public void SerializationTest()
      {
         string key;

         var accessor = CreateAccessor(out key);
         var entities = accessor.ReadEntities<MyTable>("SELECT * FROM MyTable")
            .OrderBy(e => e.MyNumber)
            .ToArray();

         entities[1].Entity.Delete();
         entities[2].MyString = "Three Three Three";

         using (var memory = new MemoryStream())
         {
            SerializeDataContractXml(entities, memory);

            memory.Position = 0;

            var hydrated = DeserializeDataContractXml<MyTable[]>(memory);

            Assert.True(hydrated.Length == 3);
            Assert.True(hydrated[0].Entity.State == EntityState.Current);
            Assert.True(hydrated[1].Entity.State == EntityState.Deleted);
            Assert.True(hydrated[2].Entity.State == EntityState.Modified);
            Assert.True(hydrated[2].Entity.Changes.Contains("MyString"));
            Assert.True(hydrated[2].MyString == "Three Three Three");
         }

         DestroyAccessor(key);
      }

      [Fact]
      public void ReadAnonymousTest()
      {
         string key;

         var accessor = CreateAccessor(out key);
         var entities = accessor.ReadAnonymous(new { MyIdentity = 0L, MyString = "", MyNumber = 0L }, "SELECT * FROM MyTable").ToArray();

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

         DestroyAccessor(key);
      }

      [Fact]
      public void WriteIdentity32()
      {
         string key;

         var accessor = CreateAccessor(out key);
         var gnu = new MyFriend()
            {
               MyNumber = 1,
               MyString = "One"
            };

         accessor.WriteEntity(gnu);

         Assert.True(gnu.MyIdentity == 1);

         DestroyAccessor(key);
      }

      [Fact]
      public void WriteIdentity64()
      {
         string key;

         var accessor = CreateAccessor(out key);
         var gnu = new MyTable()
            {
               MyNumber = 4,
               MyString = "Four"
            };

         accessor.WriteEntity(gnu);

         Assert.True(gnu.MyIdentity == 4);

         DestroyAccessor(key);
      }

      [Fact]
      public void WriteIdentityMixed()
      {
         string key;

         var accessor = CreateAccessor(out key);
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

         accessor.WriteEntities(new IDbEntity[] { gnutable1, gnufriend1, gnutable2, gnufriend2 });

         Assert.True(gnutable1.MyIdentity == 4);
         Assert.True(gnufriend1.MyIdentity == 1);
         Assert.True(gnutable2.MyIdentity == 5);
         Assert.True(gnufriend2.MyIdentity == 2);

         DestroyAccessor(key);
      }

      [Fact]
      public void ParameterTest()
      {
         string key;

         var accessor = CreateAccessor(out key);

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
               OR MyIdentity IN (@1)
            ",
            1, new[] { 9, 8, 7, 6 }, 2, 9, 9, 9, 9, 9, 9, 9, 9, 9
            );

         DestroyAccessor(key);
      }

      [Fact]
      public void ParameterTest2()
      {
         string key;

         var accessor = CreateAccessor(out key);

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

         DestroyAccessor(key);
      }

      //[Fact]
      //public void ParameterTest3()
      //{
      //    string key;

      //    var accessor = CreateAccessor(out key);

      //    try
      //    {
      //        accessor.ReadEntities<MyTable>(
      //              @"SELECT * FROM MyTable WHERE MyString = @0 OR MyString = @1",
      //              "test", null
      //              );

      //        Assert.True(true);
      //    }
      //    catch (Exception)
      //    {
      //        Assert.True(false);
      //    }

      //    DestroyAccessor(key);
      //}

      //[Fact]
      //public void ParameterTest4()
      //{
      //    string key;

      //    var accessor = CreateAccessor(out key);

      //    try
      //    {
      //        /* NOTE: passing a constant null as the sole parameter tricks the params method
      //         *       into thinking no values were passed, when one was intended.  be sure
      //         *       to pass DBNull.Value in these situations.  OR, better yet, come up with
      //         *       a clever way to solve this problem.
      //         */
      //        accessor.ReadEntities<MyTable>(
      //              @"SELECT * FROM MyTable WHERE MyString = @0",
      //              DBNull.Value
      //              );

      //        Assert.True(true);
      //    }
      //    catch (Exception)
      //    {
      //        Assert.True(false);
      //    }

      //    DestroyAccessor(key);
      //}

      //[Fact]
      //public void ParameterTest5()
      //{
      //    string key;

      //    var accessor = CreateAccessor(out key);

      //    try
      //    {
      //        var value = default(string);

      //        accessor.ReadEntities<MyTable>(
      //              @"SELECT * FROM MyTable WHERE MyString = @0",
      //            // ReSharper disable ExpressionIsAlwaysNull
      //              value
      //            // ReSharper restore ExpressionIsAlwaysNull
      //              );

      //        Assert.True(true);
      //    }
      //    catch (Exception)
      //    {
      //        Assert.True(false);
      //    }

      //    DestroyAccessor(key);
      //}

      [Fact]
      public void DrivenRecordTest()
      {
         string key;

         var accessor = CreateAccessor(out key);
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

         DestroyAccessor(key);
      }

      [Fact]
      public void UnrelatedPropertyTest()
      {
         string key;

         var accessor = CreateAccessor(out key);
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

         DestroyAccessor(key);
      }

      [Fact]
      public void UnrelatedPropertyTest2()
      {
         string key;

         var accessor = CreateAccessor(out key);
         var record = accessor.ReadIdentity<MyTablePartial, int>(1);

         Assert.True(record.MyIdentity == 1);
         Assert.True(record.MyNumber == 1);
         Assert.True(record.MyString == "One");
         Assert.True(record.UnrelatedProperty == "yo");

         DestroyAccessor(key);
      }

      [Fact]
      public void InactiveExtensionTest()
      {
         string key;

         var accessor = CreateAccessor(out key, AccessorExtension.None);

         Assert.Throws<InactiveExtensionException>(() => accessor.ReadEntities<MyTable>("SELECT * FROM MyTable WHERE MyIdentity IN (@0)", new[] { 1, 2, 3 }));

         DestroyAccessor(key);
      }

      [Fact]
      public void MissingResultTest()
      {
         string key;

         var accessor = CreateAccessor(out key, AccessorExtension.None);

         Assert.Throws<MissingResultException>(() => accessor.ReadEntities<MyTable, MyChildren>("SELECT * FROM MyTable"));

         DestroyAccessor(key);
      }

      [Fact]
      public void InsertEntitiesTest2()
      {
         string key;

         var accessor = CreateAccessor(out key);
         var inserts = new List<MyBigTable>();
         //{
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //   new MyBigTable() {Property1 = "x", Property2 = "x", Property3 = "x", Property4 = "x", Property5 = "x", Property6 = "x", Property7 = "x", Property8 = "x", Property9 = "x", Property10 = "x", Property11 = "x", Property12 = "x",},
         //};

         Assert.DoesNotThrow(() => accessor.WriteEntities(inserts));

         DestroyAccessor(key);
      }

      [Fact]
      public void UpdateMultipleValuesTest()
      {
         string key;

         var accessor = CreateAccessor(out key);
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

         DestroyAccessor(key);
      }

      [Fact]
      public void AllowUnmappedColumnsPassTest()
      {
         string key;

         var accessor = CreateAccessor(out key, AccessorExtension.AllowUnmappedColumns);

         Assert.DoesNotThrow(() => accessor.ReadEntities<MyTableSlim>("SELECT * FROM MyTable"));

         DestroyAccessor(key);
      }

      [Fact]
      public void AllowUnmappedColumnsFailTest()
      {
         string key;

         var accessor = CreateAccessor(out key, AccessorExtension.None);

         Assert.Throws<InactiveExtensionException>(() => accessor.ReadEntities<MyTableSlim>("SELECT * FROM MyTable"));

         DestroyAccessor(key);
      }

      [Fact]
      public void AllowCaseInsensitiveColumnMappingPassTest()
      {
         string key;

         var accessor = CreateAccessor(out key, AccessorExtension.CaseInsensitiveColumnMapping);

         Assert.DoesNotThrow(() => accessor.ReadEntities<MyTable>("SELECT MyIdentity as 'myidentity', MyString as 'mystring', MyNumber as 'mynumber' FROM MyTable"));

         DestroyAccessor(key);
      }

      [Fact]
      public void AllowCaseInsensitiveColumnMappingFailTest()
      {
         string key;

         var accessor = CreateAccessor(out key, AccessorExtension.None);

         Assert.Throws<InactiveExtensionException>(() => accessor.ReadEntities<MyTable>("SELECT MyIdentity as 'myidentity', MyString as 'mystring', MyNumber as 'mynumber' FROM MyTable"));

         DestroyAccessor(key);
      }

      [Fact]
      public void DefaultAttributeNameToPropertyName()
      {
         string key;

         var accessor = CreateAccessor(out key, AccessorExtension.All);

         Assert.DoesNotThrow(() => accessor.ReadEntities<MyTableWithoutColumnNames>("SELECT MyIdentity as 'myidentity', MyString as 'mystring', MyNumber as 'mynumber' FROM MyTable"));

         DestroyAccessor(key);
      }

      [Fact]
      public void NullableReadValueTest()
      {
         string key;

         var accessor = CreateAccessor(out key, AccessorExtension.All);
         var value = accessor.ReadValue<int?>("SELECT Value1 FROM NullableTest WHERE Value1 = 1");

         Assert.True(value.HasValue);
         Assert.Equal(value.Value, 1);

         DestroyAccessor(key);
      }

      [Fact]
      public void NullableReadNullValueTest()
      {
         string key;

         var accessor = CreateAccessor(out key, AccessorExtension.All);
         var value = accessor.ReadValue<int?>("SELECT Value1 FROM NullableTest WHERE Value1 IS NULL");

         Assert.False(value.HasValue);

         DestroyAccessor(key);
      }

      [Fact]
      public void NullableReadNoRowTest()
      {
         string key;

         var accessor = CreateAccessor(out key, AccessorExtension.All);
         var value = accessor.ReadValue<int?>("SELECT Value1 FROM NullableTest WHERE Value1 = 999");

         Assert.False(value.HasValue);

         DestroyAccessor(key);
      }

      [Fact]
      public void NullableReadValuesTest()
      {
         string key;

         var accessor = CreateAccessor(out key, AccessorExtension.All);
         var values = accessor.ReadValues<int?>("SELECT Value1 FROM NullableTest");

         Assert.True(values.Count() == 4);
         Assert.True(values.Count(v => v.HasValue && v.Value == 1) == 1);
         Assert.True(values.Count(v => v.HasValue && v.Value == 2) == 1);
         Assert.True(values.Count(v => !v.HasValue) == 2);

         DestroyAccessor(key);
      }

      [Fact]
      public void SelectIdentityWithLinqTableAttribute()
      {
         string key;

         var accessor = CreateAccessor(out key, AccessorExtension.All);

         Assert.DoesNotThrow(() => accessor.ReadIdentity<MyTableSlim, int>(1));

         DestroyAccessor(key);
      }

      [Fact]
      public void DbScopeCommitsData()
      {
         string key;

         var accessor = CreateAccessor(out key);
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
            scope.WriteEntities(new[] { gnu5, gnu6 });
            scope.Commit();
         }

         var committed = accessor.ReadEntities<MyTable>(
            @"SELECT * FROM MyTable"
            );

         Assert.Equal(6, committed.Count());

         DestroyAccessor(key);
      }

      [Fact]
      public void DbScopeRollsbackData()
      {
         string key;

         var accessor = CreateAccessor(out key);
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
            scope.WriteEntities(new[] { gnu5, gnu6 });
         }

         var committed = accessor.ReadEntities<MyTable>(
            @"SELECT * FROM MyTable"
            );

         Assert.Equal(3, committed.Count());

         DestroyAccessor(key);
      }

      [Fact]
      public void NoPrimaryKeyReadTest()
      {
         string key;

         var accessor = CreateAccessor(out key);

         var entities = accessor.ReadEntities<MyNopkTable>(
            @"SELECT * FROM MyNopkTable"
            );

         Assert.Equal(3, entities.Count());

         DestroyAccessor(key);
      }

      [Fact]
      public void NoPrimaryKeyWriteTest()
      {
         string key;

         var accessor = CreateAccessor(out key);

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

         DestroyAccessor(key);
      }

      [Fact]
      public void ReadEntitiesWithCustomPropertyNamesTest()
      {
         string key;

         var test = new MyCustomNameTable();

         var accessor = CreateAccessor(out key);
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

         DestroyAccessor(key);
      }

      #region --- PROTECTED -----------------------------------------------------------------------

      protected abstract IDbAccessor CreateAccessor(out string filename);

      protected abstract IDbAccessor CreateAccessor(out string filename, AccessorExtension extensions);

      protected abstract void DestroyAccessor(string key);

      #endregion --- PROTECTED -----------------------------------------------------------------------

      #region --- PRIVATE -------------------------------------------------------------------------

      private static void SerializeDataContractXml<T>(T instance, Stream stream)
      {
         using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, new UTF8Encoding(), false))
         {
            var serializer = new DataContractSerializer(typeof(T));
            serializer.WriteObject(writer, instance);
         }
      }

      private static T DeserializeDataContractXml<T>(Stream stream)
      {
         T instance;

         using (var reader = XmlDictionaryReader.CreateDictionaryReader(XmlReader.Create(stream)))
         {
            var deserializer = new DataContractSerializer(typeof(T));
            instance = (T) deserializer.ReadObject(reader);
         }

         return instance;
      }

      #endregion --- PRIVATE -------------------------------------------------------------------------

      #region --- NESTED --------------------------------------------------------------------------

      [DataContract]
      [DbTable(Name = "MyTable")]
      protected class MyTable : DbEntity<MyTable>, INotifyPropertyChanged
      {
         [DataMember]
         private long m_MyIdentity;

         [DataMember]
         private string m_MyString;

         [DataMember]
         private long m_MyNumber;

         private int m_PartialValue;

         [DbColumn(IsDbGenerated = true, IsPrimaryKey = true, Name = "MyIdentity")]
         public long MyIdentity
         {
            get { return m_MyIdentity; }
            set
            {
               m_MyIdentity = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyIdentity"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyString")]
         public string MyString
         {
            get { return m_MyString; }
            set
            {
               m_MyString = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyString"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyNumber")]
         public long MyNumber
         {
            get { return m_MyNumber; }
            set
            {
               m_MyNumber = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyNumber"));
            }
         }

         public int PartialValue
         {
            get { return m_PartialValue; }
            set
            {
               m_PartialValue = value;
               PropertyChanged(this, new PropertyChangedEventArgs("PartialValue"));
            }
         }

         public event PropertyChangedEventHandler PropertyChanged;
      }

      [DataContract]
      [DbTable(Name = "MyTable")]
      private class MyTableSlim : DbEntity<MyTableSlim>, INotifyPropertyChanged
      {
         [DataMember]
         private long m_MyIdentity;

         [DataMember]
         private string m_MyString;

         [DbColumn(IsDbGenerated = true, IsPrimaryKey = true, Name = "MyIdentity")]
         public long MyIdentity
         {
            get { return m_MyIdentity; }
            set
            {
               m_MyIdentity = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyIdentity"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyString")]
         public string MyString
         {
            get { return m_MyString; }
            set
            {
               m_MyString = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyString"));
            }
         }

         public event PropertyChangedEventHandler PropertyChanged;
      }

      [DataContract]
      [DbTable(Name = "MyChildren")]
      private class MyChildren : DbEntity<MyChildren>, INotifyPropertyChanged
      {
         [DataMember]
         private long m_HisIdentity;

         [DataMember]
         private string m_MyString;

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = true, Name = "HisIdentity")]
         public long HisIdentity
         {
            get { return m_HisIdentity; }
            set
            {
               m_HisIdentity = value;
               PropertyChanged(this, new PropertyChangedEventArgs("HisIdentity"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyString")]
         public string MyString
         {
            get { return m_MyString; }
            set
            {
               m_MyString = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyString"));
            }
         }

         public event PropertyChangedEventHandler PropertyChanged;
      }

      [DataContract]
      [DbTable(Name = "MyFriend")]
      private class MyFriend : DbEntity<MyFriend>, INotifyPropertyChanged
      {
         [DataMember]
         private int m_MyIdentity;

         [DataMember]
         private string m_MyString;

         [DataMember]
         private int m_MyNumber;

         [DbColumn(IsDbGenerated = true, IsPrimaryKey = true, Name = "MyIdentity")]
         public int MyIdentity
         {
            get { return m_MyIdentity; }
            set
            {
               m_MyIdentity = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyIdentity"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyString")]
         public string MyString
         {
            get { return m_MyString; }
            set
            {
               m_MyString = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyString"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyNumber")]
         public int MyNumber
         {
            get { return m_MyNumber; }
            set
            {
               m_MyNumber = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyNumber"));
            }
         }

         public event PropertyChangedEventHandler PropertyChanged;
      }

      [DataContract]
      [DbTable(Name = "MyTable")]
      private class MyTableRecord : DbRecord<MyTableRecord>
      {
         [DataMember]
         private long m_MyIdentity;

         [DataMember]
         private string m_MyString;

         [DataMember]
         private long m_MyNumber;

         [DbColumn(IsDbGenerated = true, IsPrimaryKey = true, Name = "MyIdentity")]
         public long MyIdentity
         {
            get { return m_MyIdentity; }
            set { m_MyIdentity = value; }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyString")]
         public string MyString
         {
            get { return m_MyString; }
            set { m_MyString = value; }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyNumber")]
         public long MyNumber
         {
            get { return m_MyNumber; }
            set { m_MyNumber = value; }
         }
      }

      [DataContract]
      [DbTable(Name = "MyTable")]
      private partial class MyTablePartial : DbRecord<MyTablePartial>
      {
         [DataMember]
         private long m_MyIdentity;

         [DataMember]
         private string m_MyString;

         [DataMember]
         private long m_MyNumber;

         [DbColumn(IsDbGenerated = true, IsPrimaryKey = true, Name = "MyIdentity")]
         public long MyIdentity
         {
            get { return m_MyIdentity; }
            set { m_MyIdentity = value; }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyString")]
         public string MyString
         {
            get { return m_MyString; }
            set { m_MyString = value; }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyNumber")]
         public long MyNumber
         {
            get { return m_MyNumber; }
            set { m_MyNumber = value; }
         }
      }

      private partial class MyTablePartial
      {
         public MyTablePartial()
         {
            UnrelatedProperty = "yo";
         }

         public string UnrelatedProperty
         {
            get;
            private set;
         }
      }

      [DbTable(Name = "MyBigTable")]
      private partial class MyBigTable : DbEntity<MyBigTable>, INotifyPropertyChanged
      {
         private long m_Id;
         private string m_Property1;
         private string m_Property2;
         private string m_Property3;
         private string m_Property4;
         private string m_Property5;
         private string m_Property6;
         private string m_Property7;
         private string m_Property8;
         private string m_Property9;
         private string m_Property10;
         private string m_Property11;
         private string m_Property12;

         [DbColumn(IsDbGenerated = true, IsPrimaryKey = true, Name = "Id")]
         public long Id
         {
            get { return m_Id; }
            set
            {
               m_Id = value;
               PropertyChanged(this, new PropertyChangedEventArgs("Id"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property1")]
         public string Property1
         {
            get { return m_Property1; }
            set
            {
               m_Property1 = value;
               PropertyChanged(this, new PropertyChangedEventArgs("Property1"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property2")]
         public string Property2
         {
            get { return m_Property2; }
            set
            {
               m_Property2 = value;
               PropertyChanged(this, new PropertyChangedEventArgs("Property2"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property3")]
         public string Property3
         {
            get { return m_Property3; }
            set
            {
               m_Property3 = value;
               PropertyChanged(this, new PropertyChangedEventArgs("Property3"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property4")]
         public string Property4
         {
            get { return m_Property4; }
            set
            {
               m_Property4 = value;
               PropertyChanged(this, new PropertyChangedEventArgs("Property4"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property5")]
         public string Property5
         {
            get { return m_Property5; }
            set
            {
               m_Property5 = value;
               PropertyChanged(this, new PropertyChangedEventArgs("Property5"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property6")]
         public string Property6
         {
            get { return m_Property6; }
            set
            {
               m_Property6 = value;
               PropertyChanged(this, new PropertyChangedEventArgs("Property6"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property7")]
         public string Property7
         {
            get { return m_Property7; }
            set
            {
               m_Property7 = value;
               PropertyChanged(this, new PropertyChangedEventArgs("Property7"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property8")]
         public string Property8
         {
            get { return m_Property8; }
            set
            {
               m_Property8 = value;
               PropertyChanged(this, new PropertyChangedEventArgs("Property8"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property9")]
         public string Property9
         {
            get { return m_Property9; }
            set
            {
               m_Property9 = value;
               PropertyChanged(this, new PropertyChangedEventArgs("Property9"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property10")]
         public string Property10
         {
            get { return m_Property10; }
            set
            {
               m_Property10 = value;
               PropertyChanged(this, new PropertyChangedEventArgs("Property10"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property11")]
         public string Property11
         {
            get { return m_Property11; }
            set
            {
               m_Property11 = value;
               PropertyChanged(this, new PropertyChangedEventArgs("Property11"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property12")]
         public string Property12
         {
            get { return m_Property12; }
            set
            {
               m_Property12 = value;
               PropertyChanged(this, new PropertyChangedEventArgs("Property12"));
            }
         }

         public event PropertyChangedEventHandler PropertyChanged;
      }

      [DataContract]
      [DbTable(Name = "MyTable")]
      private class MyTableWithoutColumnNames : DbEntity<MyTableWithoutColumnNames>, INotifyPropertyChanged
      {
         [DataMember]
         private long m_MyIdentity;

         [DataMember]
         private string m_MyString;

         [DataMember]
         private long m_MyNumber;

         [DbColumn(IsDbGenerated = true, IsPrimaryKey = true)]
         public long MyIdentity
         {
            get { return m_MyIdentity; }
            set
            {
               m_MyIdentity = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyIdentity"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false)]
         public string MyString
         {
            get { return m_MyString; }
            set
            {
               m_MyString = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyString"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false)]
         public long MyNumber
         {
            get { return m_MyNumber; }
            set
            {
               m_MyNumber = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyNumber"));
            }
         }

         public event PropertyChangedEventHandler PropertyChanged;
      }

      public class MyTableType
      {
         public long MyIdentity
         {
            get;
            set;
         }

         public string MyString
         {
            get;
            set;
         }

         public long MyNumber
         {
            get;
            set;
         }
      }

      public class MyTableType2
      {
         public long MyIdentity;
         public string MyString;
         public long MyNumber;
      }

      [DataContract]
      [DbTable(Name = "MyNopkTable")]
      protected class MyNopkTable : DbEntity<MyNopkTable>, INotifyPropertyChanged
      {
         [DataMember]
         private string m_MyString;

         [DataMember]
         private long m_MyNumber;

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyString")]
         public string MyString
         {
            get { return m_MyString; }
            set
            {
               m_MyString = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyString"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyNumber")]
         public long MyNumber
         {
            get { return m_MyNumber; }
            set
            {
               m_MyNumber = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyNumber"));
            }
         }

         public event PropertyChangedEventHandler PropertyChanged;
      }

      [DataContract]
      [DbTable(Name = "MyTable")]
      protected class MyCustomNameTable : DbEntity<MyCustomNameTable>, INotifyPropertyChanged
      {
         [DataMember]
         private long m_MyIdentity;

         [DataMember]
         private string m_MyString;

         [DataMember]
         private long m_MyNumber;

         [DbColumn(IsDbGenerated = true, IsPrimaryKey = true, Name = "MyIdentity")]
         public long MyIdentitY
         {
            get { return m_MyIdentity; }
            set
            {
               m_MyIdentity = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyIdentity"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyString")]
         public string MyStringCustom
         {
            get { return m_MyString; }
            set
            {
               m_MyString = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyString"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyNumber")]
         public long MyNUMBER
         {
            get { return m_MyNumber; }
            set
            {
               m_MyNumber = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyNumber"));
            }
         }

         public event PropertyChangedEventHandler PropertyChanged;
      }

      [DataContract]
      [DbTable(Name = "MyBad")]
      protected class MyBadTable : DbEntity<MyBadTable>, INotifyPropertyChanged
      {
         [DataMember]
         private long m_MyIdentity;

         [DataMember]
         private string m_MyString;

         [DataMember]
         private long m_MyNumber;

         [DbColumn(IsDbGenerated = true, IsPrimaryKey = true, Name = "MyIdentity")]
         public long MyIdentity
         {
            get { return m_MyIdentity; }
            set
            {
               m_MyIdentity = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyIdentity"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyString")]
         public string MyString
         {
            get { return m_MyString; }
            set
            {
               m_MyString = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyString"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyNumber")]
         public long MyNumber
         {
            get { return m_MyNumber; }
            set
            {
               m_MyNumber = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyNumber"));
            }
         }

         public event PropertyChangedEventHandler PropertyChanged;
      }

      #endregion --- NESTED --------------------------------------------------------------------------
   }
}