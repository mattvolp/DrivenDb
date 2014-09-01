using System.Collections;
using Fastlite.DrivenDb.Core._2._0.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.DrivenDb.Data.Tests._2._0.Framework
{
   [TestClass]
   public class MaybeTests
   {
      [TestMethod]
      public void Maybe_IsEmptyWhenNoValueIsPassed()
      {
         var sut = (new Maybe<string>() as IEnumerable)
            .GetEnumerator();
         
         Assert.IsFalse(sut.MoveNext());         
      }

      [TestMethod]
      public void Maybe_ContainsValueWhenValueIsPassed()
      {
         var value = "test";
         var sut = (new Maybe<string>(value) as IEnumerable)
            .GetEnumerator();
         
         Assert.IsTrue(sut.MoveNext());
         Assert.AreSame(value, sut.Current);
      }
   }
}
