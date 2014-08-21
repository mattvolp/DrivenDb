using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.DrivenDb.Data.Tests.Base.Infrastructure
{
   public static class Asserts
   {
      public static void Throws<T>(Action action)
         where T : Exception
      {
         try
         {
            action();
         }
         catch (T)
         {
            Assert.IsTrue(true);
         }
         catch
         {
            Assert.Fail();
         }
      }

      public static void DoesNotThrow(Action action)
      {
         action();

         Assert.IsTrue(true);
      }
   }
}
