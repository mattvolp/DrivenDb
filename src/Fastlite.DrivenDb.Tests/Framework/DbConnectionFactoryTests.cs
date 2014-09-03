using System;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.DrivenDb.Tests.Framework
{
   [TestClass]
   public class DbConnectionFactoryTests
   {
      [TestMethod]
      public void DbConnectionFactory_DoesNotAcceptNullFuncs()
      {
         try
         {
            var sut = new DbConnectionFactory(null);

            Assert.Fail();
         }
         catch (Exception e)
         {
            Assert.IsInstanceOfType(e, typeof (ArgumentNullException));
         }
      }

      [TestMethod]
      public void DbConnectionFactory_CreatesConnectionsProperly()
      {
         var sut = new DbConnectionFactory(() => new SqlConnection(""));
         var actual = sut.Create();

         Assert.IsNotNull(actual);
      }
   }
}
