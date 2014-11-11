using System.ComponentModel;
using System.Data.Linq.Mapping;
using DrivenDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DrivenDbp.Tests
{
   [TestClass]
   public class DbRecordTests
   {
      [TestMethod]
      public void DbRecord_PortableShowsPreferenceTowardsDbColumnAttributes()
      {
         var actual = new ClassB();

         Assert.AreEqual("NumberY", actual.Record.Columns["Number"].Name);
         Assert.AreEqual("TextY", actual.Record.Columns["Text"].Name);         
      }

      [TestMethod]
      public void DbRecord_PortableShowsPreferenceTowardsDbTableAttributes()
      {
         var actual = new ClassB();

         Assert.IsTrue(actual.Record.Table.Schema == "yyy");
         Assert.IsTrue(actual.Record.Table.Name == "ClassY");
      }

      [TestMethod]
      public void DbRecord_PortableCanFindLinqTableAttributes()
      {
         var actual = new ClassA();

         Assert.IsTrue(actual.Record.Table.Schema == "dbo");
         Assert.IsTrue(actual.Record.Table.Name == "ClassA");         
      }

      [TestMethod]
      public void DbRecord_PortableCanFindLinqColumnAttributes()
      {
         var actual = new ClassA();

         Assert.IsTrue(actual.Record.Columns.ContainsKey("Number"));
         Assert.IsTrue(actual.Record.Columns["Number"].IsPrimaryKey);
         Assert.IsTrue(actual.Record.Columns["Number"].IsDbGenerated);
         Assert.IsTrue(actual.Record.Columns.ContainsKey("Text"));
         Assert.IsFalse(actual.Record.Columns["Text"].IsPrimaryKey);
         Assert.IsFalse(actual.Record.Columns["Text"].IsDbGenerated);
      }

      [Table(Name = "dbo.ClassA")]      
      internal class ClassA : DbEntity<ClassA>, INotifyPropertyChanged
      {         
         [Column(Name = "Number", DbType = "int", IsPrimaryKey = true, IsDbGenerated = true, CanBeNull = false)]
         public int Number
         {
            get;
            set;
         }

         [Column(Name = "Text", DbType = "varchar(max)", IsPrimaryKey = false, IsDbGenerated = false, CanBeNull = true)]
         public string Text
         {
            get;
            set;
         }

         public event PropertyChangedEventHandler PropertyChanged = delegate { };
      }

      [Table(Name = "xxx.ClassX")]
      [DbTable(Schema = "yyy", Name = "ClassY")]
      internal class ClassB : DbEntity<ClassB>, INotifyPropertyChanged
      {
         [Column(Name = "NumberX", DbType = "int", IsPrimaryKey = true, IsDbGenerated = true, CanBeNull = false)]
         [DbColumn(Name = "NumberY", IsPrimaryKey = true, IsDbGenerated = true)]
         public int Number
         {
            get;
            set;
         }

         [Column(Name = "TextX", DbType = "varchar(max)", IsPrimaryKey = false, IsDbGenerated = false, CanBeNull = true)]
         [DbColumn(Name = "TextY", IsPrimaryKey = false, IsDbGenerated = false)]
         public string Text
         {
            get;
            set;
         }

         public event PropertyChangedEventHandler PropertyChanged = delegate { };
      }
   }
}
