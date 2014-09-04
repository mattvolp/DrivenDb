using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fastlite.Framework.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.Framework.Tests.Collections
{
   [TestClass]
   public class EnumerableExtensionTests
   {
      [TestMethod]
      public void ToFixedList_WorksWithIReadOnlyLists()
      {
         var list = new List<string>();
         var sut = list.ToFixedList();

         Assert.IsNotNull(sut);
      }

      [TestMethod]
      public void ToFixedList_WorksWithIEnumerables()
      {
         IEnumerable<string> list = new List<string>();
         var sut = list.ToFixedList();

         Assert.IsNotNull(sut);
      }
   }
}
