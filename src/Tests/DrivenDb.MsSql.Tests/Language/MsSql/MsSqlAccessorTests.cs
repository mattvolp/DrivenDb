using System.Data;
using DrivenDb.MsSql;
using DrivenDb.MsSql.Tests.Language.MsSql;
using DrivenDb.Tests.Language.Interfaces;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using Xunit;

namespace DrivenDb
{
   public class MsSqlAccessorTests : IDbAccessorTests
   {
      [Fact]
      public void SchemaOverrideCanBeSwitchedAtRuntime()
      {
         var filename = "";
         var accessor = (IMsSqlAccessor) CreateAccessor(out filename);

         var schema1s = accessor.ReadEntities<SchemaTable>(
            @"SELECT * FROM [one].[SchemaTable1]"
            ).ToArray();

         var schema2s = schema1s.Select(s => s.ToNew())
            .ToArray();

         schema2s.ForEach(s => s.Record.TableOverride = new DbTableAttribute() {HasTriggers = false, Schema = "two", Name = "SchemaTable2"});         
         accessor.WriteEntities(schema2s);

         var actual = accessor.ReadEntities<SchemaTable>(
            @"SELECT * FROM [two].[SchemaTable2]"
            ).ToArray();
         
         Assert.Equal("one", actual[0].Text);
         Assert.Equal("two", actual[1].Text);
         Assert.Equal("three", actual[2].Text);

         DestroyAccessor(filename);
      }

      [Fact]
      public void WriteTransactionWithScopeIdentityTest()
      {
         var filename = "";
         var accessor = (IMsSqlAccessor) CreateAccessor(out filename);

         var entities = new MyTable[]
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

         DestroyAccessor(filename);
      }

      [Fact]
      public void ReadEntitysWithTimespanColumnSucceeds()
      {
         var filename = "";
         var accessor = (IMsSqlAccessor) CreateAccessor(out filename);

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

         DestroyAccessor(filename);
      }

      [Fact]
      public void WriteEntitiesWithScopeIdentityTest()
      {
         var filename = "";
         var accessor = (IMsSqlAccessor) CreateAccessor(out filename);

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

         DestroyAccessor(filename);
      }

      [Fact]
      public void VarbinaryTest()
      {
         var filename = "";
         var accessor = CreateAccessor(out filename);

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

         DestroyAccessor(filename);
      }
      
      [Fact]
      public void ImageTest()
      {
         var filename = "";
         var accessor = CreateAccessor(out filename);

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

         DestroyAccessor(filename);
      }

      [Fact]
      public void TextTest()
      {
         var filename = "";
         var accessor = CreateAccessor(out filename);

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

         DestroyAccessor(filename);
      }

      [Fact]
      public void WriteEntitiesWithOutputTest()
      {
         var filename = "";
         var accessor = (IMsSqlAccessor) CreateAccessor(out filename);

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

         Assert.Equal(changes[0].Item2.MyNumber, 1);
         Assert.Equal(changes[0].Item2.MyString, "One");
         Assert.Equal(changes[1].Item2.MyNumber, 2);
         Assert.Equal(changes[1].Item2.MyString, "Two");
         Assert.Equal(changes[2].Item2.MyNumber, 3);
         Assert.Equal(changes[2].Item2.MyString, "Three");

         DestroyAccessor(filename);
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

      private const string MASTER_CSTRING = @"Integrated Security=SSPI;Initial Catalog=master;Data Source=localhost";
      private const string TEST_CSTRING = @"Integrated Security=SSPI;Initial Catalog=DrivenDbTest;Data Source=localhost";

      protected override IDbAccessor CreateAccessor(out string key)
      {
         return CreateAccessor(out key, AccessorExtension.All);
      }

      protected override IDbAccessor CreateAccessor(out string key, AccessorExtension extensions)
      {
         SqlConnection.ClearAllPools();

         key = null;

         var master = DbFactory.CreateAccessor(
              DbAccessorType.MsSql, extensions,
              () => new SqlConnection(MASTER_CSTRING)
              );

         master.Execute(@"
               IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'DrivenDbTest')
               BEGIN
                  DROP DATABASE DrivenDbTest
               END

               CREATE DATABASE [DrivenDbTest]"
            );

         var accessor = DbFactory.CreateAccessor(
             DbAccessorType.MsSql, extensions,
             () => new SqlConnection(TEST_CSTRING)
             );

         accessor.Execute(@"CREATE SCHEMA [one]");
         accessor.Execute(@"CREATE SCHEMA [two]");

         accessor.Execute(@"                                
                CREATE TABLE [MyTable]
                (
                   [MyIdentity] BIGINT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
                   [MyString] TEXT NULL,
                   [MyNumber] BIGINT NOT NULL
                );

                CREATE TABLE MyChildren
                (
                   [HisIdentity] BIGINT,
                   [MyString] TEXT NULL
                );

                CREATE TABLE MyFriend
                (
                   [MyIdentity] BIGINT IDENTITY(1,1)PRIMARY KEY CLUSTERED,
                   [MyString] TEXT NULL,
                   [MyNumber] BIGINT NOT NULL
                );

                INSERT INTO MyTable VALUES ('One', 1);
                INSERT INTO MyTable VALUES ('Two', 2);
                INSERT INTO MyTable VALUES ('Three', 3);

                INSERT INTO MyChildren VALUES (1, 'Child 1/3');
                INSERT INTO MyChildren VALUES (1, 'Child 2/3');
                INSERT INTO MyChildren VALUES (1, 'Child 3/3');
                INSERT INTO MyChildren VALUES (3, 'Child 1/1');

                CREATE TABLE MyBigTable
                (
                   Id BIGINT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
                   Property1    VARCHAR(50) NULL,
                   Property2    VARCHAR(50) NULL,
                   Property3    VARCHAR(50) NULL,
                   Property4    VARCHAR(50) NULL,
                   Property5    VARCHAR(50) NULL,
                   Property6    VARCHAR(50) NULL,
                   Property7    VARCHAR(50) NULL,
                   Property8    VARCHAR(50) NULL,
                   Property9    VARCHAR(50) NULL,
                   Property10   VARCHAR(50) NULL,
                   Property11   VARCHAR(50) NULL,
                   Property12   VARCHAR(50) NULL
                );

               CREATE TABLE [VarbinaryTest](
                  [Id] [int] IDENTITY(1,1) NOT NULL,
                  [Value1] [varbinary](50) NOT NULL,
                  [Value2] [varbinary](50) NULL,
                  [Value3] [varchar](50) NULL,
                CONSTRAINT [PK_VarbinaryTest] PRIMARY KEY CLUSTERED
               (
                  [Id] ASC
               )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
               ) ON [PRIMARY]
               CREATE TABLE [NullableTest] (
                  [Value1] INT NULL
               );

               INSERT INTO [NullableTest] VALUES (null);
               INSERT INTO [NullableTest] VALUES (1);
               INSERT INTO [NullableTest] VALUES (null);
               INSERT INTO [NullableTest] VALUES (2);

               CREATE TABLE [MyNopkTable]
               (
                   [MyString] TEXT NULL,
                   [MyNumber] BIGINT NOT NULL
               );

               INSERT INTO MyNopkTable VALUES ('One', 1);
               INSERT INTO MyNopkTable VALUES ('Two', 2);
               INSERT INTO MyNopkTable VALUES ('Three', 3);

               CREATE TABLE [TextTable]
               (
                  [Id] INT IDENTITY(1,1) NOT NULL,
                  [Test] TEXT
               )

               CREATE TABLE [ImageTable]
               (
                  [Id] INT IDENTITY(1,1) NOT NULL,
                  [Test] IMAGE
               )

               CREATE TABLE [TimeTable](
                  [PartyDate] [date] NOT NULL,
                  [PartyTime] [time](7) NOT NULL,
                  [PartyTime2] [time](7) NULL,
                  [PartyDateTime] [datetime] NOT NULL,
                  [PartyDateTime2] [datetime2](7) NOT NULL
               )

               INSERT INTO [dbo].[TimeTable] VALUES ('1972-08-02', '06:05:33', NULL, '1972-08-02 06:05:33', '1972-08-02 06:05:33')

               CREATE TABLE [one].[SchemaTable1]
               (
                  [Id] INT IDENTITY(1,1) NOT NULL,
                  [Text] VARCHAR(50)
               )

               INSERT INTO [one].[SchemaTable1] ([Text]) VALUES ('one')
               INSERT INTO [one].[SchemaTable1] ([Text]) VALUES ('two')
               INSERT INTO [one].[SchemaTable1] ([Text]) VALUES ('three')

               CREATE TABLE [two].[SchemaTable2]
               (
                  [Id] INT IDENTITY(1,1) NOT NULL,
                  [Text] VARCHAR(50)
               )
               ");

         return accessor;
      }

      protected override IDbDataParameter CreateParam<T>(string name, T value)
      {
         return new SqlParameter(name, value);
      }

      protected override void DestroyAccessor(string key)
      {
         SqlConnection.ClearAllPools();

         var accessor = DbFactory.CreateAccessor(
             DbAccessorType.MsSql, () => new SqlConnection(MASTER_CSTRING)
             );

         accessor.Execute("DROP DATABASE [DrivenDbTest]");
      }

      [DbTable(HasTriggers = false, Name = "SchemaTable1", Schema = "one")]
      private sealed class SchemaTable
         : DbEntity<SchemaTable>
         , INotifyPropertyChanged
      {
         private int _id;
         private string _text;

         [DbColumn(IsDbGenerated = true, IsPrimaryKey = true, Name = "Id")]
         public int Id
         {
            get { return _id; }
            set
            {
               _id = value;
               PropertyChanged(this, new PropertyChangedEventArgs("Id"));
            }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Text")]
         public string Text
         {
            get { return _text; }
            set
            {
               _text = value;
               PropertyChanged(this, new PropertyChangedEventArgs("Text"));
            }
         }

         public event PropertyChangedEventHandler PropertyChanged = delegate {};
      }
   }
}