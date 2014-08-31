using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fastlite.DrivenDb.Data.Tests._2._0
{
   [TestClass]
   public class DrivenDb_BehaviorTests
   {
      [TestMethod]
      public void DrivenDb_CanPopulateClassesFromAResultset()
      {
         /*
          * use static MySchema.Table.Column names instead of strings, have those values not be strings so
          * that no one will pass strings to those methods, they will have to use the static values.
          * 
          accessor.RegisterResultTransformationFor(DbType).As<bit, bool>()
            .RegisterResultTransformationFor("Table").OnColumn("Column").As<bit?, bool>(value => Bool.Parse(value))
            .RegisterParameterTransformationFor(typeof(IEnumerable)).AsScript(e => String.Join(",", e))
            .RegisterParameterTransformationFor(typeof(DateTime)).AsIntercept(d => SqlMinDateTime)
            .RegisterParameterTransformationFor(typeof(DateTime)).AsTransformation<string>(d => SqlMinDateTime.ToString())
          
           accessor.Query("SELECT")
            .IngoreResultTransformationsFor("TABLE")     // no string?
            .IngoreAllParameterTransformations()
            .IgnoreAllResultTransformations()
            .ResultTransform<bit,bool>("Column",)     // temporary overriding rules
            .ParameterTransform<...>(...)
            .ParameterIntercept
            ..
            .As<T>();
         
           accessor.Command("INSERT CRAP");
          */
      }

      [TestMethod]
      public void DrivenDb_CanReadMultipleEntityTypesFromMultipleResultsets()
      {

      }

      [TestMethod]
      public void DrivenDb_CanReadMultipleContractsFromMultipleResultsets()
      {

      }
   }
}
