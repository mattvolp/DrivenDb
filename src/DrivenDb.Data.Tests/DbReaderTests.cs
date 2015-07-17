using DrivenDb.MsSql;
using DrivenDb.Testing.Internal.SqLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DrivenDb.Data.Tests
{
   [TestClass]
   public class DbReaderTests
   {
      [TestMethod]
      public void DbReaderReadsSuccessfullyFireTest()
      {
         // TODO: forget all this.  i dont need to test with generated entities.  there are tests specifically for that now.  
         //       this can be custom classes etc...
         var records = new[]
            {
               new {Table = "TestTable", Id = 1, Value = "one"},
               new {Table = "TestTable", Id = 2, Value = "two"},
               new {Table = "TestTable", Id = 3, Value = "three"},
            };

         using (var fixture = new SqLiteFixture()            
            .Publish(records)
            .AllScriptingOptions()
            .CreateTable("TestTable")
            .AffixColumn("Id", MsSqlType.Int)
            .AffixColumn("Value", MsSqlType.Nvarchar)            
            .Build())
         {
            var type = fixture.CreateProxy("TestTable");
            var sut = fixture.CreateReader();
            var actual = () sut.Read(type, "SELECT * FROM TestTable");

            Assert.AreEqual("IS CRAP", actual);
         }
      }

      //internal class RecordCollection
      //   : List<Record>
      //{
         
      //}

      //internal class Record
      //   : Dictionary<string, object>
      //{
      //   private readonly string _tablename;

      //   public Record(string tablename)
      //   {
      //      _tablename = tablename;
      //   }
      //}
   }
}
