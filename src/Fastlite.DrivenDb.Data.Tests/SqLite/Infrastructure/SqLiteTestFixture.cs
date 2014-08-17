using System;
using System.Data.SQLite;
using System.IO;
using Fastlite.DrivenDb.Data.Access;
using Fastlite.DrivenDb.Data.Tests.Base.Infrastructure;

namespace Fastlite.DrivenDb.Data.Tests.SqLite.Infrastructure
{
   internal class SqLiteTestFixture : IDbTestFixture
   {
      private readonly string _database;

      public SqLiteTestFixture()
      {
         _database = Path.GetTempFileName();

         CreateDatabase(_database);
      }

      public DbAccessorBuilder CreateAccessor()
      {
         var factory = new SqLiteAccessorFactory();

         return new DbAccessorBuilder(_database, factory);
      }

      public void Dispose()
      {
         File.Delete(_database);
      }

      private static void CreateDatabase(string _database)
      {         
         var accessor = DbFactory.CreateAccessor(
             DbAccessorType.SqLite, AccessorExtension.All, () => new SQLiteConnection(String.Format("Data Source={0};Version=3;New=True", _database))
             );

         accessor.Execute(@"
                CREATE TABLE MyTable
                (
                   [MyIdentity] INTEGER PRIMARY KEY AUTOINCREMENT,
                   [MyString] TEXT NULL,
                   [MyNumber] INTEGER NOT NULL
                );

                CREATE TABLE MyChildren
                (
                   [HisIdentity] INTEGER,
                   [MyString] TEXT NULL
                );

                CREATE TABLE MyFriend
                (
                   [MyIdentity] INTEGER PRIMARY KEY AUTOINCREMENT,
                   [MyString] TEXT NULL,
                   [MyNumber] INTEGER NOT NULL
                );

                INSERT INTO MyTable VALUES (null, 'One', 1);
                INSERT INTO MyTable VALUES (null, 'Two', 2);
                INSERT INTO MyTable VALUES (null, 'Three', 3);

                INSERT INTO MyChildren VALUES (1, 'Child 1/3');
                INSERT INTO MyChildren VALUES (1, 'Child 2/3');
                INSERT INTO MyChildren VALUES (1, 'Child 3/3');
                INSERT INTO MyChildren VALUES (3, 'Child 1/1');

                CREATE TABLE [NullableTest] (
                  [Value1] INT NULL
                );

                INSERT INTO [NullableTest] VALUES (null);
                INSERT INTO [NullableTest] VALUES (1);
                INSERT INTO [NullableTest] VALUES (null);
                INSERT INTO [NullableTest] VALUES (2);

                CREATE TABLE MyNopkTable
                (
                   [MyString] TEXT NULL,
                   [MyNumber] INTEGER NOT NULL
                );

                INSERT INTO MyNopkTable VALUES ('One', 1);
                INSERT INTO MyNopkTable VALUES ('Two', 2);
                INSERT INTO MyNopkTable VALUES ('Three', 3);
                ");
      }
   }
}
