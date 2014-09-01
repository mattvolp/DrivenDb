using System.Collections;
using Fastlite.DrivenDb.Core._2._0.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.DrivenDb.Data.Tests._2._0.Framework
{
   [TestClass]
   public class DbResultTests
   {
      [TestMethod]
      public void Resultset_IsEmptyWhenNoValueIsPassed()
      {
         var sut = (new DbResultCollection<string>() as IEnumerable)
            .GetEnumerator();

         Assert.IsFalse(sut.MoveNext());
      }

      [TestMethod]
      public void Resultset_IsContainsPassedValueWhenValueIsPassed()
      {
         var values = new[] {"test"};
         var sut = (new DbResultCollection<string>(values) as IEnumerable)
            .GetEnumerator();

         Assert.IsTrue(sut.MoveNext());
         Assert.AreSame(values[0], sut.Current);
      }
   }
}
