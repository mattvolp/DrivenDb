using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using DrivenDb.MsSql;
using Xunit;

namespace DrivenDb.Tests.Language.MsSql
{
   public class MsSqlScripterTests
   {
      private readonly MsSqlScripter _scripter;

      public MsSqlScripterTests()
      {
         var db = new Db(AccessorExtension.Common, () => null);
         _scripter = new MsSqlScripter(db, () => new MsSqlBuilder());
      }

      [Fact]
      public void ColumnsWithTrailingSpacesInTheNameDeleteWithoutError()
      {
         var sut = new MySpacedIndentityClass();

         sut.Entity.SetIdentity(1);
         sut.Entity.Reset();
         sut.Entity.Delete();

         using (var command = new SqlCommand())
         {
            _scripter.ScriptDelete(command, 0, sut);

            Assert.Contains("WHERE [MyIdentity ] = ", command.CommandText);
         }
      }

      [Fact]
      public void ColumnsWithTrailingSpacesInTheNameUpdateWithoutError()
      {
         var sut = new MySpacedIndentityClass();

         sut.Entity.SetIdentity(1);
         sut.Entity.Reset();
         sut.MyValue = "TEST";

         using (var command = new SqlCommand())
         {
            _scripter.ScriptUpdate(command, 0, sut);

            Assert.Contains("WHERE [MyIdentity ] = ", command.CommandText);
         }
      }

      [Fact]
      public void ScriptExecuteFact()
      {
         var query = @"UPDATE [Foo] SET [Bar] = @0 WHERE [Baz] IN (@1)";
         var enumerable = new[] { 1, 2, 3 };

         using (var command = new SqlCommand())
         {
            _scripter.ScriptExecute(command, query, 1, enumerable);
            Assert.Equal(@"UPDATE [Foo] SET [Bar] = @0 WHERE [Baz] IN (@1,@2,@3)", command.CommandText.Trim());
            Assert.Equal(4, command.Parameters.Count);
            Assert.Equal("@1", command.Parameters[1].ParameterName);
            Assert.Equal(1, command.Parameters[1].Value);
            Assert.Equal("@2", command.Parameters[2].ParameterName);
            Assert.Equal(2, command.Parameters[2].Value);
         }
      }

      [Fact]
      public void ScriptExecuteFact2()
      {
         var query = @"UPDATE [Foo] SET [Bar] = @0 WHERE [Baz] IN (@1) AND [Buz] = @2";
         var enumerable = new[] { 1, 2, 3 };

         using (var command = new SqlCommand())
         {
            _scripter.ScriptExecute(command, query, 1, enumerable, "foo");
            Assert.Equal(@"UPDATE [Foo] SET [Bar] = @0 WHERE [Baz] IN (@1,@2,@3) AND [Buz] = @4", command.CommandText.Trim());
            Assert.Equal(5, command.Parameters.Count);
            Assert.Equal("@4", command.Parameters[4].ParameterName);
            Assert.Equal("foo", command.Parameters[4].Value);
         }
      }

      [Fact]
      public void ScriptExecuteFact3()
      {
         var query = @"UPDATE [Foo] SET [Bar] = 12 WHERE [Baz] IN (@0) AND [Buz] = @1";
         var enumerable = new[] { 3, 2, 1 };

         using (var command = new SqlCommand())
         {
            _scripter.ScriptExecute(command, query, enumerable, "foo");
            Assert.Equal(@"UPDATE [Foo] SET [Bar] = 12 WHERE [Baz] IN (@0,@1,@2) AND [Buz] = @3", command.CommandText.Trim());
            Assert.Equal(4, command.Parameters.Count);
            Assert.Equal("@0", command.Parameters[0].ParameterName);
            Assert.Equal(3, command.Parameters[0].Value);
            Assert.Equal("@3", command.Parameters[3].ParameterName);
            Assert.Equal("foo", command.Parameters[3].Value);
         }
      }

      [Fact]
      public void ScriptExecuteFact4()
      {
         var query = @"UPDATE [Foo] SET [Bar] = @0 WHERE [Baz] IN (@1)";
         var enumerable = new[] { "abc", "def" };

         using (var command = new SqlCommand())
         {
            _scripter.ScriptExecute(command, query, 1, enumerable);
            Assert.Equal(@"UPDATE [Foo] SET [Bar] = @0 WHERE [Baz] IN (@1,@2)", command.CommandText.Trim());
            Assert.Equal(3, command.Parameters.Count);
            Assert.Equal("@1", command.Parameters[1].ParameterName);
            Assert.Equal("abc", command.Parameters[1].Value);
            Assert.Equal("@2", command.Parameters[2].ParameterName);
            Assert.Equal("def", command.Parameters[2].Value);
         }
      }

      [Fact]
      public void ScriptExecuteFact5()
      {
         var query = @"UPDATE [Foo] SET [Bar] = @1 WHERE [Baz] IN (@0)";
         var enumerable = new[] { "abc", "def" };

         using (var command = new SqlCommand())
         {
            _scripter.ScriptExecute(command, query, enumerable, 1);
            Assert.Equal(@"UPDATE [Foo] SET [Bar] = @2 WHERE [Baz] IN (@0,@1)", command.CommandText.Trim());
            Assert.Equal(3, command.Parameters.Count);
            Assert.Equal("@0", command.Parameters[0].ParameterName);
            Assert.Equal("abc", command.Parameters[0].Value);
            Assert.Equal("@1", command.Parameters[1].ParameterName);
            Assert.Equal("def", command.Parameters[1].Value);
         }
      }

      [Fact]
      public void ScriptExecuteFact6()
      {
         var query = @"UPDATE [Foo] SET [Bar] = @0 WHERE [Baz] IN (@1)";
         var enumerable = new[] { "abc" };

         using (var command = new SqlCommand())
         {
            _scripter.ScriptExecute(command, query, 1, enumerable);
            Assert.Equal(query, command.CommandText.Trim());
            Assert.Equal(2, command.Parameters.Count);
            Assert.Equal("@1", command.Parameters[1].ParameterName);
            Assert.Equal("abc", command.Parameters[1].Value);
         }
      }

      [Fact]
      public void ScriptSelectFact()
      {
         var query = @"SELECT * FROM [Foo] WHERE [Bar] IN (@0) AND [Baz] IN (@1)";
         var enumerable1 = new[] { "abc", "def" };
         var enumerable2 = new[] { 5, 6, 7 };

         using (var command = new SqlCommand())
         {
            _scripter.ScriptSelect(command, query, enumerable1, enumerable2);
            Assert.Equal(@"SELECT * FROM [Foo] WHERE [Bar] IN (@0,@1) AND [Baz] IN (@2,@3,@4)", command.CommandText.Trim());
            Assert.Equal(5, command.Parameters.Count);
            Assert.Equal("@1", command.Parameters[1].ParameterName);
            Assert.Equal("def", command.Parameters[1].Value);
            Assert.Equal("@3", command.Parameters[3].ParameterName);
            Assert.Equal(6, command.Parameters[3].Value);
         }
      }

      [Fact]
      public void ScriptSelectFact2()
      {
         var query = @"SELECT * FROM [Foo] WHERE [Bar] IN (@0) AND [Baz] IN (@1)";
         var enumerable1 = new[] { "abc", "def" };
         var enumerable2 = new[] { 5, 6, 7 };

         using (var command = new SqlCommand())
         {
            _scripter.ScriptSelect(command, query, enumerable2, enumerable1);
            Assert.Equal(@"SELECT * FROM [Foo] WHERE [Bar] IN (@0,@1,@2) AND [Baz] IN (@3,@4)", command.CommandText.Trim());
            Assert.Equal(5, command.Parameters.Count);
            Assert.Equal("@1", command.Parameters[1].ParameterName);
            Assert.Equal(6, command.Parameters[1].Value);
            Assert.Equal("@3", command.Parameters[3].ParameterName);
            Assert.Equal("abc", command.Parameters[3].Value);
         }
      }

      [Fact]
      public void ScriptSelectFact3()
      {
         var query = @"SELECT * FROM [Foo] WHERE [Bar] IN (@0) AND [Baz] IN (@0)";
         var enumerable = new[] { 5, 6, 7 };

         using (var command = new SqlCommand())
         {
            _scripter.ScriptSelect(command, query, enumerable);
            Assert.Equal(@"SELECT * FROM [Foo] WHERE [Bar] IN (@0,@1,@2) AND [Baz] IN (@0,@1,@2)", command.CommandText.Trim());
            Assert.Equal(3, command.Parameters.Count);
            Assert.Equal("@1", command.Parameters[1].ParameterName);
            Assert.Equal(6, command.Parameters[1].Value);
            Assert.Equal("@2", command.Parameters[2].ParameterName);
            Assert.Equal(7, command.Parameters[2].Value);
         }
      }

      [Fact]
      public void ScriptSelectFact4()
      {
         var query = @"SELECT * FROM [Foo] WHERE [Bar] IN (@0) AND [Buz] = @1 AND [Baz] IN (@0)";
         var enumerable = new[] { 5, 6, 7 };

         using (var command = new SqlCommand())
         {
            _scripter.ScriptSelect(command, query, enumerable, "other");
            Assert.Equal(@"SELECT * FROM [Foo] WHERE [Bar] IN (@0,@1,@2) AND [Buz] = @3 AND [Baz] IN (@0,@1,@2)", command.CommandText.Trim());
            Assert.Equal(4, command.Parameters.Count);
            Assert.Equal("@3", command.Parameters[3].ParameterName);
            Assert.Equal("other", command.Parameters[3].Value);
            Assert.Equal("@1", command.Parameters[1].ParameterName);
            Assert.Equal(6, command.Parameters[1].Value);
         }
      }

      [Fact]
      public void ScriptDeleteFact()
      {
         var command = new SqlCommand();
         var entity = new MyTable();

         entity.Entity.SetIdentity(9);

         _scripter.ScriptDelete(command, 0, entity);

         Assert.Equal(@"DELETE FROM [MyTable] WHERE [MyIdentity] = @0_0;", command.CommandText.Trim());
         Assert.Equal(1, command.Parameters.Count);
         Assert.Equal("@0_0", command.Parameters[0].ParameterName);
         Assert.True(command.Parameters[0].Value.Equals(9));
      }

      [Fact]
      public void ScriptDeleteFact2()
      {
         var command = new SqlCommand();
         var entity = new MyTable2();

         entity.MyKey1 = 1;
         entity.MyKey2 = 2;

         _scripter.ScriptDelete(command, 0, entity);

         Assert.Equal(@"DELETE FROM [MyTable2] WHERE [MyKey1] = @0_0 AND [MyKey2] = @0_1;", command.CommandText.Trim());
         Assert.Equal(2, command.Parameters.Count);
         Assert.Equal("@0_0", command.Parameters[0].ParameterName);
         Assert.True(command.Parameters[0].Value.Equals(1));
         Assert.Equal("@0_1", command.Parameters[1].ParameterName);
         Assert.True(command.Parameters[1].Value.Equals(2));
      }

      [Fact]
      public void ScriptIdentitySelectFact()
      {
         var command = new SqlCommand();

         _scripter.ScriptIdentitySelect<MyTable>(command, 9);

         Assert.Equal(@"SELECT [MyIdentity], [MyString] FROM [MyTable] WHERE [MyIdentity] = @0;", command.CommandText.Trim());
         Assert.Equal(1, command.Parameters.Count);
         Assert.Equal("@0", command.Parameters[0].ParameterName);
         Assert.True(command.Parameters[0].Value.Equals(9));
      }

      [Fact]
      public void ScriptIdentitySelectFact2()
      {
         var command = new SqlCommand();

         _scripter.ScriptIdentitySelect<MyTable2>(command, 1, 2);

         Assert.Equal(@"SELECT [MyKey1], [MyKey2], [MyString] FROM [MyTable2] WHERE [MyKey1] = @0 AND [MyKey2] = @1;", command.CommandText.Trim());
         Assert.Equal(2, command.Parameters.Count);
         Assert.Equal("@0", command.Parameters[0].ParameterName);
         Assert.True(command.Parameters[0].Value.Equals(1));
         Assert.Equal("@1", command.Parameters[1].ParameterName);
         Assert.True(command.Parameters[1].Value.Equals(2));
      }

      [Fact]
      public void ScriptInsertFact()
      {
         var command = new SqlCommand();
         var entity = new MyTable();

         entity.MyString = "test";

         _scripter.ScriptInsert(command, 0, entity, true);

         Assert.Equal(@"INSERT INTO [MyTable] ([MyString]) OUTPUT 0, INSERTED.MyIdentity VALUES (@0_0);", command.CommandText.Trim());
         Assert.Equal(1, command.Parameters.Count);
         Assert.Equal("@0_0", command.Parameters[0].ParameterName);
         Assert.True(command.Parameters[0].Value.Equals("test"));
      }

      [Fact]
      public void ScriptInsertFact2()
      {
         var command = new SqlCommand();
         var entity = new MyTable2();

         entity.MyKey1 = 1;
         entity.MyKey2 = 2;
         entity.MyString = "test";

         _scripter.ScriptInsert(command, 0, entity, false);

         Assert.Equal(@"INSERT INTO [MyTable2] ([MyKey1], [MyKey2], [MyString]) VALUES (@0_0, @0_1, @0_2);", command.CommandText.Trim());
         Assert.Equal(3, command.Parameters.Count);
         Assert.Equal("@0_0", command.Parameters[0].ParameterName);
         Assert.True(command.Parameters[0].Value.Equals(1));
         Assert.Equal("@0_1", command.Parameters[1].ParameterName);
         Assert.True(command.Parameters[1].Value.Equals(2));
         Assert.Equal("@0_2", command.Parameters[2].ParameterName);
         Assert.True(command.Parameters[2].Value.Equals("test"));
      }

      [Fact]
      public void ScriptUpdateFact()
      {
         var command = new SqlCommand();
         var entity = new MyTable();

         entity.Entity.SetIdentity(9);
         entity.Entity.Reset();
         entity.MyString = "test";

         _scripter.ScriptUpdate(command, 0, entity);

         Assert.Equal(@"UPDATE [MyTable] SET [MyString] = @0_0 WHERE [MyIdentity] = @0_1;", command.CommandText.Trim());
         Assert.Equal(2, command.Parameters.Count);
         Assert.Equal("@0_0", command.Parameters[0].ParameterName);
         Assert.True(command.Parameters[0].Value.Equals("test"));
         Assert.Equal("@0_1", command.Parameters[1].ParameterName);
         Assert.True(command.Parameters[1].Value.Equals(9));
      }

      [Fact]
      public void ScriptUpdateFact2()
      {
         var command = new SqlCommand();
         var entity = new MyTable2();

         entity.MyKey1 = 1;
         entity.MyKey2 = 2;
         entity.Entity.Reset();

         entity.MyString = "test";

         _scripter.ScriptUpdate(command, 0, entity);

         Assert.Equal(@"UPDATE [MyTable2] SET [MyString] = @0_0 WHERE [MyKey1] = @0_1 AND [MyKey2] = @0_2;", command.CommandText.Trim());
         Assert.Equal(3, command.Parameters.Count);
         Assert.Equal("@0_0", command.Parameters[0].ParameterName);
         Assert.True(command.Parameters[0].Value.Equals("test"));
         Assert.Equal("@0_1", command.Parameters[1].ParameterName);
         Assert.True(command.Parameters[1].Value.Equals(1));
         Assert.Equal("@0_2", command.Parameters[2].ParameterName);
         Assert.True(command.Parameters[2].Value.Equals(2));
      }

      #region --- NESTED -----------------------------------------------------------------------

      [Table(Name = "MyTable")]
      internal class MyTable : DbEntity<MyTable>, INotifyPropertyChanged
      {
         private int m_MyIdentity;
         private string m_MyString;

         [Column(IsDbGenerated = true, IsPrimaryKey = true, Name = "MyIdentity")]
         public int MyIdentity
         {
            get { return m_MyIdentity; }
            set
            {
               m_MyIdentity = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyIdentity"));
            }
         }

         [Column(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyString")]
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

      [Table(Name = "MyTable2")]
      internal class MyTable2 : DbEntity<MyTable2>, INotifyPropertyChanged
      {
         private int m_MyKey1;
         private int m_MyKey2;
         private string m_MyString;

         [Column(IsDbGenerated = false, IsPrimaryKey = true, Name = "MyKey1")]
         public int MyKey1
         {
            get { return m_MyKey1; }
            set
            {
               m_MyKey1 = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyKey1"));
            }
         }
         [Column(IsDbGenerated = false, IsPrimaryKey = true, Name = "MyKey2")]
         public int MyKey2
         {
            get { return m_MyKey2; }
            set
            {
               m_MyKey2 = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyKey2"));
            }
         }

         [Column(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyString")]
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

      [DbTable(Name = "MyTable")]
      [DataContract]
      private class MySpacedIndentityClass : DbEntity<MySpacedIndentityClass>, INotifyPropertyChanged
      {
         [DataMember]
         private int m_MyIdentity;

         [DataMember]
         private string m_MyValue;

         [DbColumn(IsDbGenerated = true, IsPrimaryKey = true, Name = "MyIdentity ")] // note: space on the end is on purpose
         public int MyIdentity
         {
            get { return m_MyIdentity; }
            set { m_MyIdentity = value; }
         }

         [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "MyValue")]
         public string MyValue
         {
            get { return m_MyValue; }
            set
            {
               m_MyValue = value;
               PropertyChanged(this, new PropertyChangedEventArgs("MyValue"));
            }
         }

         public event PropertyChangedEventHandler PropertyChanged;
      }

      #endregion
   }
}
