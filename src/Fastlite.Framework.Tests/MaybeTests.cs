using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.Framework.Tests
{
   [TestClass]
   public class MaybeTests
   {
      [TestMethod]
      public void Maybe_HasNoValueWhenNoValueIsPassed()
      {
         var sut = new Maybe<string>();

         Assert.IsFalse(sut.HasValue);         
      }

      [TestMethod]
      [ExpectedException(typeof(InvalidOperationException))]
      public void Maybe_ValueThrowsThenNoValueIsPassed()
      {
         var sut = new Maybe<string>();

         sut.Value.Ignore();
      }

      [TestMethod]
      public void Maybe_HasValueWhenValueIsPassed()
      {
         var sut = new Maybe<string>("test");

         Assert.IsTrue(sut.HasValue);
         Assert.IsNotNull(sut.Value);
      }

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

      [TestMethod]
      public void Maybe_ReturnsZeroWhenNoValueIsPassed()
      {
         var sut = (new Maybe<string>() as IReadOnlyCollection<string>);

         Assert.AreEqual(0, sut.Count);
      }

      [TestMethod]
      public void Maybe_ReturnsOneWhenNoValueIsPassed()
      {
         var sut = (new Maybe<string>("test") as IReadOnlyCollection<string>);

         Assert.AreEqual(1, sut.Count);
      }
   }
}
