using System.Data.SqlClient;
using Fastlite.DrivenDb.Data.Access;
using Fastlite.DrivenDb.Data.Tests.Base.Infrastructure;

namespace Fastlite.DrivenDb.Data.Tests.MsSql.Infrastructure
{
   public sealed class MsSqlTestFixture : IDbTestFixture
   {
      private const string MASTER_CSTRING = @"Integrated Security=SSPI;Initial Catalog=master;Data Source=localhost";
      private const string TEST_CSTRING = @"Integrated Security=SSPI;Initial Catalog=DrivenDbTest;Data Source=localhost";

      public MsSqlTestFixture()
      {
         CreateDatabase();
      }

      public DbAccessorBuilder CreateAccessor()
      {
         var factory = new MsSqlAccessorFactory();

         return new DbAccessorBuilder(null, factory);
      }

      public void Dispose()
      {
         DestroyDatabase();
      }

      private static void CreateDatabase()
      {
         SqlConnection.ClearAllPools();

         var master = DbFactory.CreateAccessor(
            DbAccessorType.MsSql, AccessorExtension.All,
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
             DbAccessorType.MsSql, AccessorExtension.All,
             () => new SqlConnection(TEST_CSTRING)
             );

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
               ");
      }

      private static void DestroyDatabase()
      {
         SqlConnection.ClearAllPools();

         var accessor = DbFactory.CreateAccessor(
             DbAccessorType.MsSql, () => new SqlConnection(MASTER_CSTRING)
             );

         accessor.Execute("DROP DATABASE [DrivenDbTest]");
      }
   }
}
