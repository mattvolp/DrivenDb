using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fastlite.Framework.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.Framework.Tests.Collections
{
   [TestClass]
   public class FixedListTests
   {
      [TestMethod]
      public void FixedList_IsEmptyWhenNoValueIsPassed()
      {
         var sut = new FixedList<string>();

         Assert.IsFalse(sut.Any());
      }

      [TestMethod]
      public void FixedList_IsNotEmptyWhenNoValueIsPassed()
      {
         var list = new List<string>()
            {
               "test",
            };

         var sut = new FixedList<string>(list);

         Assert.AreEqual("test", sut.Single());
      }

      [TestMethod]
      public void FixedList_CountIsCorrectWhenNoValueIsPassed()
      {         
         var sut = new FixedList<string>();

         Assert.AreEqual(0, sut.Count);
      }

      [TestMethod]
      public void FixedList_CountIsCorrectValueIsPassed()
      {
         var list = new List<string>()
            {
               "1", "2", "3"
            };

         var sut = new FixedList<string>(list);

         Assert.AreEqual(3, sut.Count);
      }

      [TestMethod]
      public void FixedList_IndexReturnsCorrectResultWhenNotEmpty()
      {
         var list = new List<string>()
            {
               "1", "2", "3"
            };

         var sut = new FixedList<string>(list);

         Assert.AreEqual("2", sut[1]);
      }

      [TestMethod]
      [ExpectedException(typeof(ArgumentOutOfRangeException))]
      public void FixedList_IndexThrowsWhenOutOfRangeAnNotEmpty()
      {
         var list = new List<string>()
            {
               "1", "2", "3"
            };

         var sut = new FixedList<string>(list);

         sut[3].Ignore();
      }

      [TestMethod]
      [ExpectedException(typeof(ArgumentOutOfRangeException))]
      public void FixedList_IndexThrowsWhenOutOfRangeAndEmpty()
      {         
         var sut = new FixedList<string>();

         sut[3].Ignore();
      }

      [TestMethod]
      public void FixedList_AcceptsEnumerables()
      {
         IEnumerable<string> list = new List<string>()
            {
               "1", "2", "3"
            };

         var sut = new FixedList<string>(list);

         Assert.AreEqual("2", sut[1]);
      }

      [TestMethod]
      public void FixedList_UbiquitousIEnumerableTest()
      {
         var list = new List<string>()
            {
               "1", "2", "3"
            };

         var sut = (new FixedList<string>(list) as IEnumerable);

         Assert.IsNotNull(sut.GetEnumerator());
      }
   }
}
