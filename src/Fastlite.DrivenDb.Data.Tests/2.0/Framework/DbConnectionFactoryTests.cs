using System;
using System.Data.SqlClient;
using Fastlite.DrivenDb.Data._2._0;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.DrivenDb.Data.Tests._2._0.Framework
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
