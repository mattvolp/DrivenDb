using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DrivenDb.Core;
using DrivenDb.Data.Internal;
using DrivenDb.MsSql;
using DrivenDb.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DrivenDb.VisualStudio.Generator.Tests
{
   [TestClass]
   public class CsScriptWriterRuntimeTests
   {
      [TestMethod]
      public void ExplicitDefaultsForDateTimesHaveTimeTruncatedForDateFieldsScriptProperly()
      {
         using (var fixture = new EntityScripter()
               .SetOptions(ScriptingOptions.ImplementColumnDefaults | ScriptingOptions.TruncateTimeForDateColumns)
               .CreateTable("TestTable")
               .AffixColumn("Value", MsSqlType.Date, "N'08/02/1972 06:18:00'")
               .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var actual = proxy.GetProperty<DateTime>("Value");
            var expected = new DateTime(1972, 8, 2, 0, 0, 0);

            Assert.AreEqual(expected, actual);
         }
      }

      [TestMethod]
      public void UnicodeExplicitDefaultsForDateTimesScriptProperly()
      {
         using (var fixture = new EntityScripter()
               .SetOptions(ScriptingOptions.ImplementColumnDefaults)
               .CreateTable("TestTable")
               .AffixColumn("Value", MsSqlType.DateTime, "N'08/02/1972 06:18:00'")               
               .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var actual = proxy.GetProperty<DateTime>("Value");
            var expected = new DateTime(1972, 8, 2, 6, 18, 0);
            
            Assert.AreEqual(expected, actual);            
         }   
      }

      [TestMethod]
      public void GetDateDefaultValueDoesNotHasTimeTruncatedWhenAppliedToADateTimeColumn()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementColumnDefaults | ScriptingOptions.TruncateTimeForDateColumns)
            .CreateTable("TestTable")
            .AffixColumn("Id", MsSqlType.BigInt)
            .AffixColumn("Value", MsSqlType.DateTime, "(getdate())")
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var value = proxy.GetProperty<DateTime>("Value");

            Assert.AreNotEqual(0, value.Hour);
            Assert.AreNotEqual(0, value.Minute);
            Assert.AreNotEqual(0, value.Second);
            Assert.AreNotEqual(0, value.Millisecond);
         }
      }

      [TestMethod]
      public void NullStringsForNonNullableColumnsFailValidationWhenStateNew()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementValidationCheck | ScriptingOptions.ImplementStateTracking)
            .CreateTable("TestTable")
            .AffixColumn("Id", MsSqlType.BigInt, "(7)")
            .AffixColumn("Value", MsSqlType.Varchar)            
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");

            proxy.SetProperty("Value", default(string));

            var failures = proxy.ExecuteMethod<IEnumerable<RequirementFailure>>("GetRequirementsFailures");

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("Value", failures.First().Name);
         }
      }

      [TestMethod]
      public void NullStringsForNonNullableColumnsFailValidationWhenStateUpdated()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementValidationCheck | ScriptingOptions.ImplementStateTracking)
            .CreateTable("TestTable")
            .AffixColumn("Id", MsSqlType.BigInt, "(7)")
            .AffixColumn("Value", MsSqlType.Varchar)            
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var entity = proxy.GetProperty<IDbEntity>("Entity");

            entity.Reset();
            proxy.SetProperty("Value", default(string));

            var failures = proxy.ExecuteMethod<IEnumerable<RequirementFailure>>("GetRequirementsFailures");

            Assert.AreEqual(1, failures.Count());
            Assert.AreEqual("Value", failures.First().Name);
         }
      }

      [TestMethod]
      public void ValidationCheckReturnsNoErrorsWhenDeletedState()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementValidationCheck | ScriptingOptions.ImplementStateTracking)
            .CreateTable("TestTable")
            .AffixColumn("Id", MsSqlType.BigInt, "(7)")
            .AffixColumn("Value", MsSqlType.Date)            
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var entity = proxy.GetProperty<IDbEntity>("Entity");

            entity.Delete();

            var failures = proxy.ExecuteMethod<IEnumerable<RequirementFailure>>("GetRequirementsFailures");

            Assert.AreEqual(0, failures.Count());
         }
      }

      [TestMethod]
      public void ValidationCheckReturnsNoErrorsWhenCurrentState()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementValidationCheck | ScriptingOptions.ImplementStateTracking)
            .CreateTable("TestTable")
            .AffixColumn("Id", MsSqlType.BigInt, "(7)")
            .AffixColumn("Value", MsSqlType.Date)            
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var entity = proxy.GetProperty<IDbEntity>("Entity");

            entity.Reset();

            var failures = proxy.ExecuteMethod<IEnumerable<RequirementFailure>>("GetRequirementsFailures");

            Assert.AreEqual(0, failures.Count());
         }
      }

      [TestMethod]
      public void ImplementPrimaryKeyPropertyScriptsProperly()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementPrimaryKey | ScriptingOptions.ImplementColumnDefaults)
            .CreateTable("TestTable")
            .AffixKeyColumn("Id", MsSqlType.BigInt, "(7)")            
            .Build())
         {
            var table = fixture.CreateProxy("TestTable");
            var proxy = new EntityProxy(table.GetProperty<object>("PrimaryKey"));
            var equivalent = new Tuple<long>(7);

            Assert.AreEqual(7L, proxy.GetField<long>("Id"));
            Assert.AreEqual(equivalent, proxy.Instance);
         }
      }

      [TestMethod]
      public void ImplementPrimaryKeyClassScriptsProperly()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementPrimaryKey)
            .CreateTable("TestTable")
            .AffixKeyColumn("Id", MsSqlType.BigInt)            
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTableKey", 3L);

            Assert.IsNotNull(proxy.Instance);
            Assert.AreEqual(3L, proxy.GetField<long>("Id"));
         }
      }

      [TestMethod]
      public void LocalDatesScriptProperly()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.TruncateTimeForDateColumns)
            .CreateTable("TestTable")
            .AffixColumn("Value", MsSqlType.Date)            
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var datetime = DateTime.Now;

            Assert.AreEqual(DateTimeKind.Local, datetime.Kind);
            proxy.SetProperty("Value", datetime);

            var value = proxy.GetProperty<DateTime>("Value");

            Assert.AreEqual(DateTimeKind.Local, value.Kind);
            Assert.AreEqual(0, value.Hour);
            Assert.AreEqual(0, value.Minute);
            Assert.AreEqual(0, value.Second);
         }
      }

      [TestMethod]
      public void UnspecifiedDatesScriptProperly()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.UnspecifiedDateTimes | ScriptingOptions.TruncateTimeForDateColumns)
            .CreateTable("TestTable")            
            .AffixColumn("Value", MsSqlType.Date)
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var datetime = DateTime.Now;

            Assert.AreEqual(DateTimeKind.Local, datetime.Kind);

            proxy.SetProperty("Value", datetime);

            var value = proxy.GetProperty<DateTime>("Value");

            Assert.AreEqual(DateTimeKind.Unspecified, value.Kind);
            Assert.AreEqual(0, value.Hour);
            Assert.AreEqual(0, value.Minute);
            Assert.AreEqual(0, value.Second);
         }
      }

      [TestMethod]
      public void MakeSerializableWithStateMaintainsStateProperly()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.Serializable | ScriptingOptions.ImplementStateTracking)
            .CreateTable("TestTable")            
            .AffixColumn("Value", MsSqlType.Int)
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var entity = proxy.GetProperty<IDbEntity>("Entity");
            
            entity.Reset();
            proxy.SetProperty("Value", 1);

            var deserialized = fixture.SerializeAndDeserialize(proxy);
            var value = deserialized.GetProperty<IDbEntity>("Entity");

            Assert.AreEqual("Value", value.Changes.First().ColumnName);
            Assert.AreEqual(EntityState.Updated, value.State);
         }
      }

      [TestMethod]
      public void UnspecifiedDatetimesScriptProperly()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.UnspecifiedDateTimes)
            .CreateTable("TestTable")            
            .AffixColumn("Value", MsSqlType.DateTime)
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var datetime = DateTime.Now;

            Assert.AreEqual(DateTimeKind.Local, datetime.Kind);

            proxy.SetProperty("Value", datetime);

            var value = proxy.GetProperty<DateTime>("Value");

            Assert.AreEqual(DateTimeKind.Unspecified, value.Kind);
         }
      }

      [TestMethod]
      public void CustomTypeMappingsScriptProperly()
      {         
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementColumnDefaults)
            .CreateTable("TestTable")            
            .AffixColumn("Value", MsSqlType.Int, "(1)", typeof (DateTimeKind))
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var value = proxy.GetProperty<DateTimeKind>("Value");

            Assert.AreEqual(DateTimeKind.Utc, value);
            Assert.AreEqual(typeof (DateTimeKind), proxy.Instance.GetType().GetProperty("Value").PropertyType);
         }
      }

      [TestMethod]
      public void DateTimesForDateFieldsTruncateTheTimeProperlyWithConstructorDefaultsCaseInsensative()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementColumnDefaults | ScriptingOptions.TruncateTimeForDateColumns)
            .CreateTable("TestTable")
            .AffixColumn("Value", MsSqlType.Date, "(GETDATE())")
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var value = proxy.GetProperty<DateTime>("Value");

            Assert.AreEqual(0, value.Hour);
            Assert.AreEqual(0, value.Minute);
            Assert.AreEqual(0, value.Second);
            Assert.AreEqual(0, value.Millisecond);
         }
      }

      [TestMethod]
      public void DateTimesForDateFieldsTruncateTheTimeProperlyWithConstructorDefaults()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementColumnDefaults | ScriptingOptions.TruncateTimeForDateColumns)
            .CreateTable("TestTable")
            .AffixColumn("Value", MsSqlType.Date, "(getdate())")
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var value = proxy.GetProperty<DateTime>("Value");

            Assert.AreEqual(0, value.Hour);
            Assert.AreEqual(0, value.Minute);
            Assert.AreEqual(0, value.Second);
            Assert.AreEqual(0, value.Millisecond);
         }
      }

      [TestMethod]
      public void DateTimesForDateFieldsTruncateTheTimeProperly()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.TruncateTimeForDateColumns)
            .CreateTable("TestTable")
            .AffixColumn("Value", MsSqlType.Date)
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            
            proxy.SetProperty("Value", DateTime.Now);

            var value = proxy.GetProperty<DateTime>("Value");

            Assert.AreEqual(0, value.Hour);
            Assert.AreEqual(0, value.Minute);
            Assert.AreEqual(0, value.Second);
            Assert.AreEqual(0, value.Millisecond);
         }
      }

      [TestMethod]
      public void ImplementStateTrackingScriptsProperly()
      {         
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementStateTracking)
            .CreateTable("TestTable")
            .AffixColumn("Id", MsSqlType.BigInt)
            .AffixColumn("Value", MsSqlType.Int)
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var instance = (IDbEntityProvider) proxy.Instance;

            Assert.AreEqual(EntityState.New, instance.Entity.State);

            instance.Entity.Reset();

            Assert.AreEqual(EntityState.Current, instance.Entity.State);

            proxy.SetProperty("Id", 1);
            proxy.SetProperty("Value", 2);

            Assert.AreEqual(EntityState.Updated, instance.Entity.State);
            Assert.AreEqual("Id", instance.Entity.Changes.First().ColumnName);
            Assert.AreEqual(1L, instance.Entity.Changes.First().Value);
            Assert.AreEqual("Value", instance.Entity.Changes.Last().ColumnName);
            Assert.AreEqual(2, instance.Entity.Changes.Last().Value);

            instance.Entity.Delete();

            Assert.AreEqual(EntityState.Deleted, instance.Entity.State);
         }
      }

      [TestMethod]
      public void ImplementColumnDefaultsWithUnspecifiedDateTimesScriptsDateTimesProperly()
      {         
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementColumnDefaults | ScriptingOptions.UnspecifiedDateTimes)
            .CreateTable("TestTable")
            .AffixColumn("Value1", MsSqlType.DateTime, "'08/02/1972 06:18:00'")
            .AffixColumn("Value2", MsSqlType.DateTime, "(getdate())")
            .AffixColumn("Value3", MsSqlType.NullableDatetime, "('08/02/1972 06:18:00')")
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");

            var value1 = proxy.GetProperty<DateTime>("Value1");
            var value2 = proxy.GetProperty<DateTime>("Value2");
            var value3 = proxy.GetProperty<DateTime?>("Value3");

            Assert.AreEqual(DateTimeKind.Unspecified, value1.Kind);
            Assert.AreEqual(DateTimeKind.Unspecified, value2.Kind);
            Assert.AreEqual(DateTimeKind.Unspecified, value3.GetValueOrDefault().Kind);
         }
      }

      [TestMethod]
      public void ImplementColumnDefaultsScriptsDateTimesProperly()
      {         
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementColumnDefaults)
            .CreateTable("TestTable")
            .AffixColumn("Value1", MsSqlType.DateTime, "'08/02/1972 06:18:00'")
            .AffixColumn("Value2", MsSqlType.DateTime, "(getdate())")
            .AffixColumn("Value3", MsSqlType.NullableDatetime, "('08/02/1972 06:18:00')")
            .AffixColumn("Value4", MsSqlType.NullableDatetime, "(NULL)")
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var value1 = proxy.GetProperty<DateTime>("Value1");
            var value2 = proxy.GetProperty<DateTime>("Value2");
            var value3 = proxy.GetProperty<DateTime?>("Value3");
            var value4 = proxy.GetProperty<DateTime?>("Value4");

            var expected = new DateTime(1972, 8, 2, 6, 18, 0);

            Assert.AreEqual(expected, value1);
            Assert.AreEqual(DateTime.Now.Year, value2.Year);
            Assert.AreEqual(expected, value3.GetValueOrDefault());
            Assert.IsFalse(value4.HasValue);
         }
      }

      [TestMethod]
      public void ImplementColumnDefaultsScriptsDecimalProperly()
      {         
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementColumnDefaults)
            .CreateTable("TestTable")
            .AffixColumn("Value1", MsSqlType.Decimal, "1")
            .AffixColumn("Value2", MsSqlType.Decimal, "(1.2)")
            .AffixColumn("Value3", MsSqlType.NullableDecimal, "(1.3)")
            .AffixColumn("Value4", MsSqlType.NullableDecimal, "(NULL)")
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var value1 = proxy.GetProperty<decimal>("Value1");
            var value2 = proxy.GetProperty<decimal>("Value2");
            var value3 = proxy.GetProperty<decimal?>("Value3");
            var value4 = proxy.GetProperty<decimal?>("Value4");

            Assert.AreEqual(1, value1);
            Assert.AreEqual(1.2m, value2);
            Assert.AreEqual(1.3m, value3.GetValueOrDefault());
            Assert.IsFalse(value4.HasValue);
         }
      }

      [TestMethod]
      public void ImplementColumnDefaultsScriptsBoolProperly()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementColumnDefaults)
            .CreateTable("TestTable")
            .AffixColumn("Value1", MsSqlType.Bit, "1")
            .AffixColumn("Value2", MsSqlType.Bit, "(1)")
            .AffixColumn("Value3", MsSqlType.NullableBit, "((1))")
            .AffixColumn("Value4", MsSqlType.NullableBit, "(NULL)")
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var value1 = proxy.GetProperty<bool>("Value1");
            var value2 = proxy.GetProperty<bool>("Value2");
            var value3 = proxy.GetProperty<bool?>("Value3");
            var value4 = proxy.GetProperty<bool?>("Value4");

            Assert.IsTrue(value1);
            Assert.IsTrue(value2);
            Assert.IsTrue(value3.GetValueOrDefault());
            Assert.IsFalse(value4.HasValue);
         }
      }

      [TestMethod]
      public void ImplementColumnDefaultsScriptsIntProperly()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementColumnDefaults)
            .CreateTable("TestTable")
            .AffixColumn("Value1", MsSqlType.Int, "5")
            .AffixColumn("Value2", MsSqlType.Int, "(5)")
            .AffixColumn("Value3", MsSqlType.NullableInt, "((5))")
            .AffixColumn("Value4", MsSqlType.NullableInt, "(NULL)")
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var value1 = proxy.GetProperty<int>("Value1");
            var value2 = proxy.GetProperty<int>("Value2");
            var value3 = proxy.GetProperty<int?>("Value3");
            var value4 = proxy.GetProperty<int?>("Value4");

            Assert.AreEqual(5, value1);
            Assert.AreEqual(5, value2);
            Assert.AreEqual(5, value3.GetValueOrDefault());
            Assert.IsFalse(value4.HasValue);
         }
      }

      [TestMethod]
      public void ImplementColumnDefaultsScriptsLongsProperly()
      {         
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementColumnDefaults)
            .CreateTable("TestTable")
            .AffixColumn("Value1", MsSqlType.BigInt, "5")
            .AffixColumn("Value2", MsSqlType.BigInt, "(5)")
            .AffixColumn("Value3", MsSqlType.NullableBigInt, "((5))")
            .AffixColumn("Value4", MsSqlType.NullableBigInt, "(NULL)")
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var value1 = proxy.GetProperty<long>("Value1");
            var value2 = proxy.GetProperty<long>("Value2");
            var value3 = proxy.GetProperty<long?>("Value3");
            var value4 = proxy.GetProperty<long?>("Value4");

            Assert.AreEqual(5, value1);
            Assert.AreEqual(5, value2);
            Assert.AreEqual(5, value3.GetValueOrDefault());
            Assert.IsFalse(value4.HasValue);
         }
      }

      [TestMethod]
      public void WithoutUnspecifiedDateTimesScriptsProperly()
      {         
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.None)
            .CreateTable("TestTable")
            .AffixColumn("Id", MsSqlType.BigInt)
            .AffixColumn("Value", MsSqlType.DateTime)
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var datetime = DateTime.Now;

            Assert.AreEqual(DateTimeKind.Local, datetime.Kind);

            proxy.SetProperty("Value", datetime);

            var value = proxy.GetProperty<DateTime>("Value");

            Assert.AreEqual(DateTimeKind.Local, value.Kind);
         }
      }

      [TestMethod]
      public void UnspecifiedDateTimesScriptsProperly()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.UnspecifiedDateTimes)
            .CreateTable("TestTable")
            .AffixColumn("Id", MsSqlType.BigInt)
            .AffixColumn("Value", MsSqlType.DateTime)
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var datetime = DateTime.Now;

            Assert.AreEqual(DateTimeKind.Local, datetime.Kind);

            proxy.SetProperty("Value", datetime);

            var value = proxy.GetProperty<DateTime>("Value");

            Assert.AreEqual(DateTimeKind.Unspecified, value.Kind);
         }
      }

      [TestMethod]
      public void MakeSerializableScriptsProperly()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.Serializable)
            .CreateTable("TestTable")
            .AffixColumn("Id", MsSqlType.BigInt)
            .AffixColumn("Value", MsSqlType.Int)
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            
            proxy.SetProperty("Value", 1);

            var deserialized = fixture.SerializeAndDeserialize(proxy);
            var value = deserialized.GetProperty<int>("Value");

            Assert.AreEqual(1, value);
         }
      }

      [TestMethod]
      public void WithoutMinimizePropertyChangesScriptsProperly()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementNotifyPropertyChanged)
            .CreateTable("TestTable")
            .AffixColumn("Id", MsSqlType.BigInt)
            .AffixColumn("Value", MsSqlType.Int)
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var instance = (INotifyPropertyChanged) proxy.Instance;
            var changed = 0;

            instance.PropertyChanged += (s, e) => { changed += 1; };

            proxy.SetProperty("Value", 1);
            proxy.SetProperty("Value", 1);

            Assert.AreEqual(2, changed);
         }
      }

      [TestMethod]
      public void MinimizePropertyChangesScriptsProperly()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.MinimizePropertyChanges | ScriptingOptions.ImplementNotifyPropertyChanged)
            .CreateTable("TestTable")
            .AffixColumn("Id", MsSqlType.BigInt)
            .AffixColumn("Value", MsSqlType.Int)
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var instance = (INotifyPropertyChanged) proxy.Instance;
            var changed = 0;

            instance.PropertyChanged += (s, e) => { changed += 1; };

            proxy.SetProperty("Value", 1);
            proxy.SetProperty("Value", 1);

            Assert.AreEqual(1, changed);
         }
      }

      [TestMethod]
      public void ImplementLinqContextScriptsProperly()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementLinqContext)
            .CreateTable("TestTable")
            .AffixColumn("Id", MsSqlType.BigInt)
            .AffixColumn("Value", MsSqlType.Int)
            .Build())
         {
            var proxy = fixture.CreateProxy("TestContext");
            var table = proxy.GetProperty<object>("TestTable");

            Assert.IsTrue(table.GetType().FullName.Contains("Linq.Table"));
         }
      }

      [TestMethod]
      public void ImplementNotifyPropertyChangingScriptsProperly()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementNotifyPropertyChanging)
            .CreateTable("TestTable")
            .AffixColumn("Id", MsSqlType.BigInt)
            .AffixColumn("Value", MsSqlType.Int)
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var instance = (INotifyPropertyChanging) proxy.Instance;
            var changed = false;

            instance.PropertyChanging += (s, e) => { changed = true; };

            proxy.SetProperty("Value", 1);

            Assert.IsTrue(changed);
         }
      }

      [TestMethod]
      public void ImplementNotifyPropertyChangedScriptsProperly()
      {
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.ImplementNotifyPropertyChanged)
            .CreateTable("TestTable")
            .AffixColumn("Id", MsSqlType.BigInt)
            .AffixColumn("Value", MsSqlType.Int)
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");
            var instance = (INotifyPropertyChanged) proxy.Instance;
            var changed = false;

            instance.PropertyChanged += (s, e) => { changed = true; };

            proxy.SetProperty("Value", 1);

            Assert.IsTrue(changed);
         }
      }

      [TestMethod]
      public void CodeGeneratedForAllOptionsDoesCompileFireTest()
      {         
         using (var fixture = new EntityScripter()
            .SetOptions(ScriptingOptions.All)
            .CreateTable("TestTable")
            .AffixColumn("Id", MsSqlType.BigInt)
            .AffixColumn("Value", MsSqlType.Int)
            .Build())
         {
            var proxy = fixture.CreateProxy("TestTable");

            Assert.IsNotNull(proxy.Instance);
         }
      }
   }
}
