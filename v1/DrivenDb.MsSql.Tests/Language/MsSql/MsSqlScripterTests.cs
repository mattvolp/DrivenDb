using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace DrivenDb.Tests.Language.MsSql
{
    public class MsSqlScripterTests 
    {
        //[Test]
        //public void ScriptDeleteTest()
        //{
        //    var db = new Db(AccessorExtension.All, null);
        //    var scripter = new MsSqlScripter(db);
        //    var command = new SqlCommand();
        //    var entity = new MyTable();

        //    entity.Entity.SetIdentity(9);

        //    scripter.ScriptDelete(command, 0, entity);

        //    Assert.True(command.CommandText.Trim() == @"DELETE FROM [MyTable] WHERE [MyIdentity] = @0_0;");
        //    Assert.True(command.Parameters.Count == 1);
        //    Assert.True(command.Parameters[0].ParameterName == "@0_0");
        //    Assert.True(command.Parameters[0].Value.Equals(9));
        //}

        //[Test]
        //public void ScriptDeleteTest2()
        //{
        //    var db = new Db(AccessorExtension.All, null);
        //    var scripter = new MsSqlScripter(db);
        //    var command = new SqlCommand();
        //    var entity = new MyTable2();

        //    entity.MyKey1 = 1;
        //    entity.MyKey2 = 2;

        //    scripter.ScriptDelete(command, 0, entity);

        //    Assert.True(command.CommandText.Trim() == @"DELETE FROM [MyTable2] WHERE [MyKey1] = @0_0 AND [MyKey2] = @0_1;");
        //    Assert.True(command.Parameters.Count == 2);
        //    Assert.True(command.Parameters[0].ParameterName == "@0_0");
        //    Assert.True(command.Parameters[0].Value.Equals(1));
        //    Assert.True(command.Parameters[1].ParameterName == "@0_1");
        //    Assert.True(command.Parameters[1].Value.Equals(2));
        //}

        //[Test]
        //public void ScriptIdentitySelectTest()
        //{
        //    var db = new Db(AccessorExtension.All, null);
        //    var scripter = new MsSqlScripter(db);
        //    var command = new SqlCommand();

        //    scripter.ScriptIdentitySelect<MyTable>(command, 9);

        //    Console.WriteLine(command.CommandText);

        //    Assert.True(command.CommandText.Trim() == @"SELECT [MyIdentity], [MyString] FROM [MyTable] WHERE [MyIdentity] = @0;");
        //    Assert.True(command.Parameters.Count == 1);
        //    Assert.True(command.Parameters[0].ParameterName == "@0");
        //    Assert.True(command.Parameters[0].Value.Equals(9));
        //}

        //[Test]
        //public void ScriptIdentitySelectTest2()
        //{
        //    var db = new Db(AccessorExtension.All, null);
        //    var scripter = new MsSqlScripter(db);
        //    var command = new SqlCommand();

        //    scripter.ScriptIdentitySelect<MyTable2>(command, 1, 2);

        //    Assert.True(command.CommandText.Trim() == @"SELECT [MyKey1], [MyKey2], [MyString] FROM [MyTable2] WHERE [MyKey1] = @0 AND [MyKey2] = @1;");
        //    Assert.True(command.Parameters.Count == 2);
        //    Assert.True(command.Parameters[0].ParameterName == "@0");
        //    Assert.True(command.Parameters[0].Value.Equals(1));
        //    Assert.True(command.Parameters[1].ParameterName == "@1");
        //    Assert.True(command.Parameters[1].Value.Equals(2));
        //}

        //[Test]
        //public void ScriptInsertTest()
        //{
        //    var db = new Db(AccessorExtension.All, null);
        //    var scripter = new MsSqlScripter(db);
        //    var command = new SqlCommand();
        //    var entity = new MyTable();

        //    entity.MyString = "test";

        //    scripter.ScriptInsert(command, 0, entity);

        //    Assert.True(command.CommandText.Trim() == @"INSERT INTO [MyTable] ([MyString]) OUTPUT 0, INSERTED.MyIdentity VALUES (@0_0);");
        //    Assert.True(command.Parameters.Count == 1);
        //    Assert.True(command.Parameters[0].ParameterName == "@0_0");
        //    Assert.True(command.Parameters[0].Value.Equals("test"));
        //}

        //[Test]
        //public void ScriptInsertTest2()
        //{
        //    var db = new Db(AccessorExtension.All, null);
        //    var scripter = new MsSqlScripter(db);
        //    var command = new SqlCommand();
        //    var entity = new MyTable2();

        //    entity.MyKey1 = 1;
        //    entity.MyKey2 = 2;
        //    entity.MyString = "test";

        //    scripter.ScriptInsert(command, 0, entity);

        //    Assert.True(command.CommandText.Trim() == @"INSERT INTO [MyTable2] ([MyKey1], [MyKey2], [MyString]) VALUES (@0_0, @0_1, @0_2);");
        //    Assert.True(command.Parameters.Count == 3);
        //    Assert.True(command.Parameters[0].ParameterName == "@0_0");
        //    Assert.True(command.Parameters[0].Value.Equals(1));
        //    Assert.True(command.Parameters[1].ParameterName == "@0_1");
        //    Assert.True(command.Parameters[1].Value.Equals(2));
        //    Assert.True(command.Parameters[2].ParameterName == "@0_2");
        //    Assert.True(command.Parameters[2].Value.Equals("test"));
        //}

        //[Test]
        //public void ScriptUpdateTest()
        //{
        //    var db = new Db(AccessorExtension.All, null);
        //    var scripter = new MsSqlScripter(db);
        //    var command = new SqlCommand();
        //    var entity = new MyTable();

        //    entity.Entity.SetIdentity(9);
        //    entity.Entity.Reset();
        //    entity.MyString = "test";

        //    scripter.ScriptUpdate(command, 0, entity);

        //    Assert.True(command.CommandText.Trim() == @"UPDATE [MyTable] SET [MyString] = @0_0 WHERE [MyIdentity] = @0_1;");
        //    Assert.True(command.Parameters.Count == 2);
        //    Assert.True(command.Parameters[0].ParameterName == "@0_0");
        //    Assert.True(command.Parameters[0].Value.Equals("test"));
        //    Assert.True(command.Parameters[1].ParameterName == "@0_1");
        //    Assert.True(command.Parameters[1].Value.Equals(9));
        //}

        //[Test]
        //public void ScriptUpdateTest2()
        //{
        //    var db = new Db(AccessorExtension.All, null);
        //    var scripter = new MsSqlScripter(db);
        //    var command = new SqlCommand();
        //    var entity = new MyTable2();

        //    entity.MyKey1 = 1;
        //    entity.MyKey2 = 2;
        //    entity.Entity.Reset();

        //    entity.MyString = "test";

        //    scripter.ScriptUpdate(command, 0, entity);

        //    Assert.True(command.CommandText.Trim() == @"UPDATE [MyTable2] SET [MyString] = @0_0 WHERE [MyKey1] = @0_1 AND [MyKey2] = @0_2;");
        //    Assert.True(command.Parameters.Count == 3);
        //    Assert.True(command.Parameters[0].ParameterName == "@0_0");
        //    Assert.True(command.Parameters[0].Value.Equals("test"));
        //    Assert.True(command.Parameters[1].ParameterName == "@0_1");
        //    Assert.True(command.Parameters[1].Value.Equals(1));
        //    Assert.True(command.Parameters[2].ParameterName == "@0_2");
        //    Assert.True(command.Parameters[2].Value.Equals(2));
        //}

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

        #endregion
    }
}
