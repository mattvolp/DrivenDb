using System;
using DrivenDb.Core.Extensions;
using DrivenDb.Data;
using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsValidationWriter
      : IClassContentScripter
   {
      public TableTarget Write(TableTarget target)
      {
         return target 
            .Chain(GuardAgainstInvalidOptionSelection)
            .Chain(WriteValidation);
      }

      private static void GuardAgainstInvalidOptionSelection(TableTarget target)
      {
         if (!target.Target.Options.HasFlag(ScriptingOptions.ImplementStateTracking))
         {
            throw new Exception("Cannot implement validation check without state tracking enabled");
         }
      }

      private static void WriteValidation(TableTarget target) 
      {         
         /*
          * state == current -> true
          * state == deleted -> true
          * state == updated -> if all non-nullable/non-generated string columns have a value then true
          * state == new -> if all non-nullable/non-generated columns have a change recorded
          */
         target.Writer
            .WriteLines(new ScriptLines()
               {
                  {"        partial void HasExtendedRequirementsMet(IList<RequirementFailure> failures);                            "},
                  {"                                                                                                                "},
                  {"        public IEnumerable<RequirementFailure> GetRequirementsFailures()                                        "},
                  {"        {                                                                                                       "},
                  {"            var failures = new List<RequirementFailure>();                                                      "},
                  {"                                                                                                                "},
                  {"            if (_entity.State == EntityState.Current || _entity.State == EntityState.Deleted)                   "},
                  {"            {                                                                                                   "},
                  {"                return failures;                                                                                "},
                  {"            }                                                                                                   "},
                  {"                                                                                                                "},
                  {"            if (_entity.State == EntityState.Updated || _entity.State == EntityState.New)                       "},
                  {"            {                                                                                                   "},
               })

            .WriteTemplate(target.Table.GetRequiredStringColumns(), new ScriptLines()
               {
                  {"                if (_$0 == default(string))                                                                     "},
                  {"                {                                                                                               "},
                  {"                    failures.Add(new RequirementFailure(\"$0\", \"Null value not allowed\", default(string)));  "},
                  {"                }                                                                                               "},
               }, ColumnExtractor)

            .WriteLines(new ScriptLines()
               {
                  {"            }                                                                                                   "},
                  {"                                                                                                                "},
                  {"            if (_entity.State == EntityState.New)                                                               "},
                  {"            {                                                                                                   "},
               })

            .WriteTemplate(target.Table.GetRequiredColumns(), new ScriptLines()
               {
                  {"                if (!_entity.Changes.Any(c => c.ColumnName == \"$0\"))                                          "},
                  {"                {                                                                                               "},
                  {"                    failures.Add(new RequirementFailure(\"$0\", \"Required value not set\", default(string)));  "},
                  {"                }                                                                                               "},
               }, ColumnExtractor)

            .WriteLines(new ScriptLines()
               {
                  {"            }                                                                                                   "},
                  {"                                                                                                                "},
                  {"            HasExtendedRequirementsMet(failures);                                                               "},
                  {"                                                                                                                "},
                  {"            return failures;                                                                                    "},
                  {"        }                                                                                                       "},
               })
            .Ignore();
      }

      private static string[] ColumnExtractor(ColumnMap column)
      {
         return new[] { column.Detail.Name };
      }      
   }
}
