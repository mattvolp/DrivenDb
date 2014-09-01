using System.Collections.Generic;
using Fastlite.DrivenDb.Data._2._0;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.DrivenDb.Data.Tests._2._0.Framework
{
   [TestClass]
   public class DBRecordCollectionTests
   {
      [TestMethod]
      public void DBRecordCollection_IteratesSuppliedValues()
      {
         var values = new List<DbRecord2<string>>()
            {
               new DbRecord2<string>(new[] {"n"}, new object[] {"a"}),
               new DbRecord2<string>(new[] {"n"}, new object[] {"b"}),
               new DbRecord2<string>(new[] {"n"}, new object[] {"c"}),
            };

         var sut = new DbRecordCollection<string>(values);

         Assert.AreEqual(3, sut.Count);         
      }
   }
}
