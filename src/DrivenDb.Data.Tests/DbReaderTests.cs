using DrivenDb.Core;
using DrivenDb.MsSql;
using DrivenDb.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DrivenDb.Data.Tests
{
   [TestClass]
   public class DbReaderTests
   {
      [TestMethod]
      public void DbReaderReadsSuccessfullyFireTest()
      {
         //var entities = new IDbEntityProvider[]
         //   {

         //   };

         ////using (var publisher = new SqLitePublisher())
         //using (var fixture = new EntityScripter()
         //   .AllScriptingOptions()
         //   .CreateTable("TestTable")
         //   .AffixColumn("Id", MsSqlType.Int)
         //   .AffixColumn("Value", MsSqlType.Nvarchar)
         //   .Publish(entities)
         //   .Build())
         //{
         //   var type = fixture.CreateProxy("TestTable");
         //   var sut = fixture.CreateReader();
         //   var actual = () sut.Read(type, "SELECT * FROM BULLCRAP");

         //   Assert.AreEqual("IS CRAP", actual);
         //}
      }
   }
}
