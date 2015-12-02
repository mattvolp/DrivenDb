using System.IO;
using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DrivenDb.Scripting.Tests
{
   [TestClass]
   public class OptionWriterTests
   {
      [TestMethod]
      public void DeterminesTrueConditionProperly()
      {         
         using (var writer = new StringWriter())
         {
            const ScriptingOptions OPTIONS = ScriptingOptions.Serializable | ScriptingOptions.MinimizePropertyChanges;

            var sut = new ScriptTarget(OPTIONS, writer);
            
            sut.WriteLine("test", ScriptingOptions.Serializable);

            var actual = writer.ToString()
               .Trim();

            Assert.AreEqual("test", actual);
         }
      }

      [TestMethod]
      public void DeterminesFalseConditionProperly()
      {
         using (var writer = new StringWriter())
         {
            const ScriptingOptions OPTIONS = ScriptingOptions.Serializable | ScriptingOptions.MinimizePropertyChanges;

            var sut = new ScriptTarget(OPTIONS, writer);
            
            sut.WriteLine("test", ScriptingOptions.ImplementNotifyPropertyChanged);

            var actual = writer.ToString()
               .Trim();

            Assert.AreEqual("", actual);
         }
      }
   }
}
