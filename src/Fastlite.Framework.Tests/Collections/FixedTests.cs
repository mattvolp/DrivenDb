using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fastlite.Framework.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.Framework.Tests.Collections
{
   [TestClass]
   public class FixedTests
   {
      [TestMethod]
      public void Fixed_IsEmptyWhenNoValueIsPassed()
      {
         var sut = new Fixed<string>();

         Assert.IsFalse(sut.Any());
      }

      [TestMethod]
      public void Fixed_IsNotEmptyWhenNoValueIsPassed()
      {
         var list = new List<string>()
            {
               "test",
            };

         var sut = new Fixed<string>(list);

         Assert.AreEqual("test", sut.Single());
      }

      [TestMethod]
      public void Fixed_CountIsCorrectWhenNoValueIsPassed()
      {         
         var sut = new Fixed<string>();

         Assert.AreEqual(0, sut.Count);
      }

      [TestMethod]
      public void Fixed_CountIsCorrectValueIsPassed()
      {
         var list = new List<string>()
            {
               "1", "2", "3"
            };

         var sut = new Fixed<string>(list);

         Assert.AreEqual(3, sut.Count);
      }

      [TestMethod]
      public void Fixed_IndexReturnsCorrectResultWhenNotEmpty()
      {
         var list = new List<string>()
            {
               "1", "2", "3"
            };

         var sut = new Fixed<string>(list);

         Assert.AreEqual("2", sut[1]);
      }

      [TestMethod]
      [ExpectedException(typeof(ArgumentOutOfRangeException))]
      public void Fixed_IndexThrowsWhenOutOfRangeAnNotEmpty()
      {
         var list = new List<string>()
            {
               "1", "2", "3"
            };

         var sut = new Fixed<string>(list);

         sut[3].Ignore();
      }

      [TestMethod]
      [ExpectedException(typeof(ArgumentOutOfRangeException))]
      public void Fixed_IndexThrowsWhenOutOfRangeAndEmpty()
      {         
         var sut = new Fixed<string>();

         sut[3].Ignore();
      }

      [TestMethod]
      public void Fixed_AcceptsEnumerables()
      {
         IEnumerable<string> list = new List<string>()
            {
               "1", "2", "3"
            };

         var sut = new Fixed<string>(list);

         Assert.AreEqual("2", sut[1]);
      }

      [TestMethod]
      public void Fixed_UbiquitousIEnumerableTest()
      {
         var list = new List<string>()
            {
               "1", "2", "3"
            };

         var sut = (new Fixed<string>(list) as IEnumerable);

         Assert.IsNotNull(sut.GetEnumerator());
      }
   }
}
