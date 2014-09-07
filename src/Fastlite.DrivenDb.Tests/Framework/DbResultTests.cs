using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.DrivenDb.Tests.Framework
{
   [TestClass]
   public class DbResultTests
   {
      //[TestMethod]
      //public void Resultset_IsEmptyWhenNoValueIsPassed()
      //{
      //   var sut = (new DbResultCollection<string>() as IEnumerable)
      //      .GetEnumerator();

      //   Assert.IsFalse(sut.MoveNext());
      //}

      [TestMethod]
      public void Resultset_IsContainsPassedValueWhenValueIsPassed()
      {
         var values = new[] {"test"};
         var sut = (new DbResultList<string>(values) as IEnumerable)
            .GetEnumerator();

         Assert.IsTrue(sut.MoveNext());
         Assert.AreSame(values[0], sut.Current);
      }
   }
}
