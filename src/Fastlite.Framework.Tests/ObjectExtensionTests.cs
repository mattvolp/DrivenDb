using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.Framework.Tests
{
   [TestClass]
   public class ObjectExtensionTests
   {
      [TestMethod]
      public void Ignore_Ignores()
      {
         1.Ignore();
      }
   }
}
