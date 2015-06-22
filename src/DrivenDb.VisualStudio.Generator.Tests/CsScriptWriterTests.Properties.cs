using System.IO;
using DrivenDb.Data;
using DrivenDb.MsSql;
using DrivenDb.Data.Internal;
using DrivenDb.VisualStudio.Generator.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DrivenDb.VisualStudio.Generator.Tests
{
   [TestClass]
   public class CsScriptWriterPropertyTests
   {      
      [TestMethod]
      public void DoesntWriteLinqContextProperly()
      {
         var details = CreateColumnDetails(MsSqlType.BigInt, "Id");
         var actual = CreateScriptWriter(ScriptingOptions.None)
            .WriteProperty(details);

         Assert.IsFalse(actual.Contains("[Column]"));
      }

      [TestMethod]
      public void WritesLinqContextProperly()
      {
         var details = CreateColumnDetails(MsSqlType.BigInt, "Id");
         var actual = CreateScriptWriter(ScriptingOptions.ImplementLinqContext)
            .WriteProperty(details);        

         Assert.IsTrue(actual.Contains("[Column]"));
      }

      [TestMethod]
      public void DoesntWritePartialPropertyChangesProperly()
      {
         var details = CreateColumnDetails(MsSqlType.BigInt, "Id");
         var actual = CreateScriptWriter(ScriptingOptions.None)
            .WriteProperty(details);

         Assert.IsFalse(actual.Contains("OnIdChanging();"));
         Assert.IsFalse(actual.Contains("OnIdChanged();"));
      }

      [TestMethod]
      public void WritesPartialPropertyChangesProperly()
      {
         var details = CreateColumnDetails(MsSqlType.BigInt, "Id");
         var actual = CreateScriptWriter(ScriptingOptions.ImplementPartialPropertyChanges)
            .WriteProperty(details);

         Assert.IsTrue(actual.Contains("OnIdChanging(ref value);"));
         Assert.IsTrue(actual.Contains("OnIdChanged();"));
      }

      [TestMethod]
      public void DoesntWriteINotifyPropertyChangingProperly()
      {
         var details = CreateColumnDetails(MsSqlType.BigInt, "Id");
         var actual = CreateScriptWriter(ScriptingOptions.None)
            .WriteProperty(details);

         Assert.IsFalse(actual.Contains("PropertyChanging();"));
      }

      [TestMethod]
      public void WritesINotifyPropertyChangingProperly()
      {
         var details = CreateColumnDetails(MsSqlType.BigInt, "Id");
         var actual = CreateScriptWriter(ScriptingOptions.ImplementNotifyPropertyChanging)
            .WriteProperty(details);

         Assert.IsTrue(actual.Contains("PropertyChanging();"));
      }

      [TestMethod]
      public void DoesntWriteINotifyPropertyChangedProperly()
      {
         var details = CreateColumnDetails(MsSqlType.BigInt, "Id");
         var actual = CreateScriptWriter(ScriptingOptions.None)
            .WriteProperty(details);

         Assert.IsFalse(actual.Contains("PropertyChanged();"));
      }

      [TestMethod]
      public void WritesINotifyPropertyChangedProperly()
      {
         var details = CreateColumnDetails(MsSqlType.BigInt, "Id");
         var actual = CreateScriptWriter(ScriptingOptions.ImplementNotifyPropertyChanged)
            .WriteProperty(details);

         Assert.IsTrue(actual.Contains("PropertyChanged();"));
      }

      [TestMethod]
      public void WritesPropertyTypeProperly()
      {
         var details = CreateColumnDetails(MsSqlType.BigInt, "Id");
         var actual = CreateScriptWriter(ScriptingOptions.All)
            .WriteProperty(details);

         Assert.IsTrue(actual.Contains("public long Id"));
      }

      [TestMethod]
      public void WritesPropertyNameProperly()
      {
         var details = CreateColumnDetails(MsSqlType.BigInt, "Id");
         var actual = CreateScriptWriter(ScriptingOptions.All)
            .WriteProperty(details);

         Assert.IsTrue(actual.Contains("public long Id"));
         Assert.IsTrue(actual.Contains("return _Id"));
         Assert.IsTrue(actual.Contains("_Id != value"));
         Assert.IsTrue(actual.Contains("OnIdChanging"));
         Assert.IsTrue(actual.Contains("_Id = value"));
         Assert.IsTrue(actual.Contains("OnIdChanged"));
      }

      private static ColumnDetail CreateColumnDetails(DbType sqlType, string name)
      {
         var details = new ColumnDetail(sqlType, name, false, false, false, false, false, null, 0);

         return details;
      }

      private static CsGeneratorLikeness CreateScriptWriter(ScriptingOptions options)
      {
         var stringWriter = new StringWriter();
         var scriptWriter = new CsGeneratorLikeness(options, stringWriter);

         return scriptWriter;
      }

      private class CsGeneratorLikeness
         : CsGenerator
      {
         private readonly StringWriter _writer;

         public CsGeneratorLikeness(ScriptingOptions options, StringWriter writer)
            : base(options, writer)
         {
            _writer = writer;
         }

         public new string WriteProperty(ColumnDetail details)
         {
            var mapping = new ColumnMap(details, null);

            base.WriteProperty(mapping);

            return _writer.ToString();
         }
      }
   }
}
